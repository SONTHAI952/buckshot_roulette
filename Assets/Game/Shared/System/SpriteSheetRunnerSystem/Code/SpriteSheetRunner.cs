using System.Collections.Generic;
using UnityEngine;

namespace SpriteSheetRunnerSystem
{
    public abstract class SpriteSheetRunner : MonoBehaviour
    {
        [Min(1)]
        [SerializeField] private int sampleRate = 30;
        
        [Min(0)]
        [SerializeField] private float speed = 1;
        
        [Min(-1)]
        [Tooltip("LoopCount = -1 là loop vô hạn")]
        [SerializeField] private int loopCount = -1;
        
        [SerializeField] private ActionOnEnable actionOnEnable = ActionOnEnable.Restart;
        [SerializeField] private bool ignoreTimeScale = true;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private List<Sprite> listSprite;

        
        
        private int currentFrame = 0;
        private float lastTimeChangeSprite = 0;
        public bool IsPlaying { get; protected set; }
        private int loop = 0;
        
        

        public float Speed
        {
            get => speed;
            set
            {
                speed = value;
                if (speed < 0)
                    speed = 0;
            }
        }

        private void OnEnable()
        {
            if (actionOnEnable == ActionOnEnable.None)
            {
                
            }
            else if (actionOnEnable == ActionOnEnable.Play)
            {
                Play();
            }
            else if (actionOnEnable == ActionOnEnable.Restart)
            {
                Restart();
            }
            else
            {
                Debug.LogError("Not code for actionOnEnable: " + actionOnEnable);
            }
            
        }

        private void Update()
        {
            if(IsPlaying == false)
                return;

            if (loopCount > 0)
            {
                if(loop >= loopCount)
                    return;
            }
            
            if(sampleRate <= 0)
                return;
            
            if(speed <= 0)
                return;
            
            if(listSprite.Count <= 0)
                return;

            if (listSprite.Count == 1)
            {
                SetSprite(listSprite[0]);
                return;
            }
                

            float currentTime = 0;
            if (ignoreTimeScale)
                currentTime = Time.unscaledTime;
            else
                currentTime = Time.time;
            
            float clipDuration = listSprite.Count / (float)sampleRate;
            float deltaTime = clipDuration / listSprite.Count;
            deltaTime /= speed;
            if(currentTime - lastTimeChangeSprite < deltaTime)
                return;
            
            float progress = currentFrame / (listSprite.Count - 1f);
            int frame = Mathf.FloorToInt(curve.Evaluate(progress) * (listSprite.Count - 1f));
            SetSprite(listSprite[frame]);

            lastTimeChangeSprite = currentTime;
            
            currentFrame++;
            if (currentFrame >= listSprite.Count)
            {
                currentFrame = 0;
                
                loop++;
                if (loopCount > 0 && loop >= loopCount)
                {
                    IsPlaying = false;
                }
            }
        }

        public abstract void SetSprite(Sprite sprite);


        private void ResetState()
        {
            currentFrame = 0;
            lastTimeChangeSprite = -999;
            loop = 0;
        }

        /// <summary>
        /// Chạy tiếp từ trạng thái hiện tại
        /// </summary>
        public void Play()
        {
            IsPlaying = true;
        }

        /// <summary>
        /// Reset và chạy lại từ đầu
        /// </summary>
        public void Restart()
        {
            ResetState();
            IsPlaying = true;
        }

        /// <summary>
        /// Tạm dừng, có thể tiếp tục bằng Play
        /// </summary>
        public void Pause()
        {
            IsPlaying = false;
        }

        /// <summary>
        /// Dừng và reset trạng thái
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
            ResetState();
        }
    }
}