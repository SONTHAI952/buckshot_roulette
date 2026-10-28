using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ZeroX.AudioSystem;
using ZeroX.Extensions;
using ZeroX.SingletonSystem;
using Random = UnityEngine.Random;

namespace ZeroX.EmissionAndFlyEffectSystem
{
    public class EmissionAndFlyEffect : Singleton_AutoSpawn_Persistent<EmissionAndFlyEffect>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform cellContainer;
        [SerializeField] protected Image cellPrefab;
        
        [Header("Default Value")]
        [SerializeField] private int defaultMaxCell = 15;
        [SerializeField] private Vector2 defaultCellSize = new Vector2(150, 150);
        [SerializeField] private float defaultStartCellScale = 1;
        [SerializeField] private float defaultEndCellScale = 0.65f;
        
        
        [Header("Emission")]
        [SerializeField] private float minEmissionRadius = 50;
        [SerializeField] private float maxEmissionRadius = 200;
        [SerializeField] private float emissionDuration = 0.5f;
        [SerializeField] private AnimationCurve emissionEase = AnimationCurveExtension.OutSine(0, 0, 1, 1);
        [SerializeField] private AudioAsset emissionSfx;

        [Header("Fly")]
        [SerializeField] private float delayFlyFromEmission = 0f;
        [SerializeField] private float totalDelayFlyBetweenCell = 0.2f;
        [SerializeField] private float flyDuration = 0.5f;
        [SerializeField] private AnimationCurve flyEase = AnimationCurveExtension.InSine(0, 0, 1, 1);
        [SerializeField] private AudioAsset cellFlyFinishedSfx;
        [SerializeField] private float cellFlyFinishedSfxRestTime = 0.075f;


        public int PlayingCount { get; private set; }
        private float lastTimePlayCellFlyFinishedSfx = -999;
        
        


        protected override void Awake()
        {
            base.Awake();
            canvas.gameObject.SetActive(false);
        }
        
        protected override EmissionAndFlyEffect CreateInstance()
        {
            string path = "ZeroX/EmissionAndFlyEffect/Emission And Fly Effect";
            var prefab = Resources.Load<EmissionAndFlyEffect>(path);
            return Instantiate(prefab);
        }
        
        class RefInt
        {
            public int value;
        }
        
        
        
        

        protected void Reset()
        {
            cellContainer = transform as RectTransform;
        }


        #region Utility

        public Camera GetCanvasCamera(GameObject gObj)
        {
            Canvas canvas = gObj.GetComponentInParent<Canvas>().rootCanvas;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.worldCamera;
        }
        
        Vector2 RandomEmissionOffset()
        {
            float distance = Random.Range(minEmissionRadius, maxEmissionRadius);
            return Random.insideUnitCircle.normalized * distance;
        }

        Vector3 ConvertWorldPointToLocalPointInCellContainer(Camera camera, Vector3 worldPoint)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPoint);
            Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cellContainer, screenPoint, canvasCamera, out Vector2 localPoint);
            return localPoint;
        }

        #endregion
        
        
        public virtual Image CreateCell(EmissionAndFlyEffectData data)
        {
            var cell = Instantiate(cellPrefab);
            cell.sprite = data.icon;
            RectTransform rectTransform = (RectTransform)cell.transform;
            rectTransform.sizeDelta = data.cellSize ?? defaultCellSize;
            rectTransform.localScale = Vector3.one * (data.startCellScale ?? defaultStartCellScale);

            return cell;
        }
        
        private Tween DoTweenCell(Transform cell, int cellCount, int cellIndex, long cellValue, Vector3 endLocalPoint, float endLocalScale, Action<long> onCellFinished, RefInt totalCellTweening)
        {
            totalCellTweening.value++;
            Vector2 localDestEmissionPoint = (Vector2)cell.localPosition + RandomEmissionOffset();
            float delayFlyBetweenCell = cellCount <= 1 ? 0 : totalDelayFlyBetweenCell / (cellCount - 1);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(cell.DOLocalMove(localDestEmissionPoint, emissionDuration).SetEase(emissionEase)); //Emission
            sequence.AppendInterval(delayFlyFromEmission + cellIndex * delayFlyBetweenCell);
            
            float flyTime = sequence.Duration();
            sequence.Append(cell.DOLocalMove(endLocalPoint, flyDuration).SetEase(flyEase)); //Fly
            sequence.Insert(flyTime, cell.DOScale(endLocalScale, flyDuration).SetEase(flyEase)); //Scale

            sequence.OnComplete(() =>
            {
                Destroy(cell.gameObject);
                
                try
                {
                    onCellFinished?.Invoke(cellValue);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                
                totalCellTweening.value--;
                
                //Sfx
                if (Time.realtimeSinceStartup - lastTimePlayCellFlyFinishedSfx >= cellFlyFinishedSfxRestTime)
                {
                    lastTimePlayCellFlyFinishedSfx = Time.realtimeSinceStartup;
                    if (cellFlyFinishedSfx != null)
                        cellFlyFinishedSfx.Play();
                }
            });

            return sequence;
        }

        public WaitToken Play(EmissionAndFlyEffectData data)
        {
            if (data.maxCell == 0 || data.totalCellValue == 0)
            {
                data.onFinished?.Invoke();
                return WaitToken.CreateHasResulted();
            }

            WaitToken waitToken = new WaitToken();
            StartCoroutine(Timeline());
            return waitToken;

            
            
            IEnumerator Timeline()
            {
                PlayingCount++;
                canvas.gameObject.SetActive(true);
                
                //Calculate data for cell
                int maxCell = data.maxCell ?? defaultMaxCell;
                int cellCount = data.totalCellValue < maxCell ? (int)data.totalCellValue : maxCell;
                long cellValueNormal = data.totalCellValue / cellCount;
                long cellValueRemainder = data.totalCellValue - (cellValueNormal * cellCount);
                float endCellScale = data.endCellScale ?? defaultEndCellScale;

                //Calculate Point
                Vector3 startLocalPos = ConvertWorldPointToLocalPointInCellContainer(data.startCamera, data.startWorldPos);
                Vector3 endLocalPos = ConvertWorldPointToLocalPointInCellContainer(data.endCamera, data.endWorldPos);

                //Play Effect
                RefInt totalCellTweening = new RefInt();
                for (int i = 0; i < cellCount; i++)
                {
                    //Create Cell
                    var cell = CreateCell(data).transform;
                    cell.SetParent(cellContainer);
                    cell.transform.localPosition = startLocalPos;
                    cell.transform.localScale = Vector3.one;
                    cell.gameObject.SetActive(true);

                    //Tween Cell
                    long cellValue = i < cellCount - 1 ? cellValueNormal : cellValueNormal + cellValueRemainder; //Nếu là cell cuối thì cộng thêm giá trị dư
                    DoTweenCell(cell, cellCount, i, cellValue, endLocalPos, endCellScale, data.onCellFinished, totalCellTweening);
                }

                //Sfx
                if (emissionSfx != null)
                    emissionSfx.Play();

                //Wait effect finish
                yield return new WaitWhile(() => totalCellTweening.value > 0);
                PlayingCount--;
                if(PlayingCount <= 0)
                    canvas.gameObject.SetActive(false);
                
                data.onFinished?.Invoke();
                waitToken.SetResult();
            }
        }
        
        /*public void Play(RectTransform startPoint, Transform endPoint, long totalCellValue, int maxCell, Action<long> onCellFinished = null, Action onFinished = null)
        {
            EmissionAndFlyEffectData data = new EmissionAndFlyEffectData();
            data.startCamera = GetCanvasCamera(startPoint.gameObject);
            data.endCamera = GetCanvasCamera(endPoint.gameObject);
            
            
            Play(cameraOfEmissionPoint, startPoint.position, cameraOfDestFlyPoint, endPoint.position, totalCellValue, maxCell, onCellFinished, onFinished);
        }
        
        public void Play(RectTransform startPoint, Transform endPoint, long totalCellValue, Action<long> onCellFinished = null, Action onFinished = null)
        {
            Camera cameraOfEmissionPoint = GetCanvasCamera(startPoint.gameObject);
            Camera cameraOfDestFlyPoint = GetCanvasCamera(endPoint.gameObject);
            
            Play(cameraOfEmissionPoint, startPoint.position, cameraOfDestFlyPoint, endPoint.position, totalCellValue, defaultMaxCell, onCellFinished, onFinished);
        }
        
        
        public void Play(Camera cameraOfEmissionPoint, Vector3 emissionWorldPoint, Transform destFlyPoint, long totalCellValue, int maxCell, Action<long> onCellFinished = null, Action onFinished = null)
        {
            Camera cameraOfDestFlyPoint = GetCanvasCamera(destFlyPoint.gameObject);
            
            Play(cameraOfEmissionPoint, emissionWorldPoint, cameraOfDestFlyPoint, destFlyPoint.position, totalCellValue, maxCell, onCellFinished, onFinished);
        }
        
        public void Play(Camera cameraOfEmissionPoint, Vector3 emissionWorldPoint, Transform destFlyPoint, long totalCellValue, Action<long> onCellFinished = null, Action onFinished = null)
        {
            Camera cameraOfDestFlyPoint = GetCanvasCamera(destFlyPoint.gameObject);
            
            Play(cameraOfEmissionPoint, emissionWorldPoint, cameraOfDestFlyPoint, destFlyPoint.position, totalCellValue, defaultMaxCell, onCellFinished, onFinished);
        }
        
        
        //Bonus
        public void Play(RectTransform startPoint, Transform endPoint, long totalCellValue, TextMeshProUGUI textValue, long fromValue, Action<long> onCellFinished = null, Action onFinished = null)
        {
            Camera cameraOfEmissionPoint = GetCanvasCamera(startPoint.gameObject);
            Camera cameraOfDestFlyPoint = GetCanvasCamera(endPoint.gameObject);

            long toValue = fromValue + totalCellValue;
            long value = fromValue;
            Play(cameraOfEmissionPoint, startPoint.position, cameraOfDestFlyPoint, endPoint.position, totalCellValue, defaultMaxCell,
                cellValue =>
                {
                    value += cellValue;
                    textValue.text = value.ToString();
                    onCellFinished?.Invoke(cellValue);
                },
                () =>
                {
                    textValue.text = toValue.ToString();
                    onFinished?.Invoke();
                });
        }
        
        public void Play(RectTransform startPoint, Transform endPoint, long totalCellValue, TextMeshProUGUI textValue, long fromValue)
        {
            Camera cameraOfEmissionPoint = GetCanvasCamera(startPoint.gameObject);
            Camera cameraOfDestFlyPoint = GetCanvasCamera(endPoint.gameObject);

            long toValue = fromValue + totalCellValue;
            long value = fromValue;
            Play(cameraOfEmissionPoint, startPoint.position, cameraOfDestFlyPoint, endPoint.position, totalCellValue, defaultMaxCell,
                cellValue =>
                {
                    value += cellValue;
                    textValue.text = value.ToString();
                },
                () =>
                {
                    textValue.text = toValue.ToString();
                });
        }*/
    }
}