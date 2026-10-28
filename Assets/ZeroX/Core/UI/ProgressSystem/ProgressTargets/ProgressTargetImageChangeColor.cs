using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressTargets + "Progress Target Image Change Color")]
    public class ProgressTargetImageChangeColor : ProgressTarget
    {
        public TargetProgress targetProgress = TargetProgress.AnimateProgress;
        public Color emptyColor = Color.gray;
        public Color fillColor = Color.green;
        [SerializeField] private List<Image> listImage;
        
        public override void UpdateTarget(Progressor progressor)
        {
            if(listImage.Count == 0)
                return;
            
            float progress = ProgressUtility.GetTargetProgressValue(progressor, targetProgress);
            if (Mathf.Approximately(progress, 0))
                progress = 0;
            else if (Mathf.Approximately(progress, 1))
                progress = 1;
            
            int numberFillSprite = Mathf.FloorToInt(progress * listImage.Count);
            if (numberFillSprite == 0 && progress > 0)
                numberFillSprite = 1;

            for (int i = 0; i < listImage.Count; i++)
            {
                listImage[i].color = i < numberFillSprite ? fillColor : emptyColor;
            }
        }
    }
}