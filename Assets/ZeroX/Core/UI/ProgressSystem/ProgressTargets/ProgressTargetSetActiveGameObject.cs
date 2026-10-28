using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressTargets + "Progress Target Set Active GameObject")]
    public class ProgressTargetSetActiveGameObject : ProgressTarget
    {
        public TargetProgress targetProgress = TargetProgress.AnimateProgress;
        public List<GameObject> listGameObject;
        
        public override void UpdateTarget(Progressor progressor)
        {
            if(listGameObject.Count == 0)
                return;

            float progress = ProgressUtility.GetTargetProgressValue(progressor, targetProgress);
            if (Mathf.Approximately(progress, 0))
                progress = 0;
            else if (Mathf.Approximately(progress, 1))
                progress = 1;
            
            int numberGameObjectActive = Mathf.FloorToInt(progress * listGameObject.Count);
            if (numberGameObjectActive == 0 && progress > 0)
                numberGameObjectActive = 1;

            for (int i = 0; i < listGameObject.Count; i++)
            {
                listGameObject[i].SetActive(i < numberGameObjectActive);
            }
        }
    }
}