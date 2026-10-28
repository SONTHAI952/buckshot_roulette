using System.Collections;
using UnityEngine;

namespace ZeroX
{
    public class AspectRatioFitterUseScale : MonoBehaviour
    {
        public enum AspectMode
        {
            FitInParent,
            EnvelopeInParent
        }
        
        
        public AspectMode aspectMode = AspectMode.FitInParent;
        public float aspectRatio = 1;
        
        
        private void Awake()
        {
            ApplyScale();
        }

        private void Start()
        {
            ApplyScale();
            StartCoroutine(Timeline());
        }


        IEnumerator Timeline()
        {
            yield return null;
            ApplyScale();
        }


        private void ApplyScale()
        {
            RectTransform parent = (RectTransform)transform.parent;
            Vector2 parentSize = parent.rect.size;

            float sizeY = 1;
            float sizeX = aspectRatio;
            
            float ratioX = parentSize.x / sizeX;
            float ratioY = parentSize.y / sizeY;

            //Chọn ratio theo mode
            float ratio = 1;
            if(aspectMode == AspectMode.FitInParent)
                ratio = Mathf.Min(ratioX, ratioY);
            else if(aspectMode == AspectMode.EnvelopeInParent)
                ratio = Mathf.Max(ratioX, ratioY);
            
            //Scale
            transform.localScale = new Vector3(sizeX * ratio, sizeY * ratio, transform.localScale.z);
        }
    }
}