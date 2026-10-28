using System;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    [System.Serializable]
    public class WindowTransitionConfig
    {
        public float openDuration = 0.35f;
        public float closeDuration = 0.35f;
        public bool ignoreTimeScale = true;
        
        public WindowAnimType animType = WindowAnimType.Pure;

        [NonSerialized] private WindowAnimNone animNone = new WindowAnimNone();
        public WindowAnimPureId animPureId = WindowAnimPureId.Fade;
        public WindowAnimMono animMono;
        public WindowAnimSO animSO;



        public IWindowAnim Anim
        {
            get
            {
                if (animType == WindowAnimType.None)
                    return animNone;
                
                if (animType == WindowAnimType.Pure)
                    return WindowAnimPureTable.GetAnim(animPureId);

                if (animType == WindowAnimType.MonoBehaviour)
                    return animMono;

                if (animType == WindowAnimType.ScriptableObject)
                    return animSO;
                
                Debug.LogError("Not code for anim type: " + animType);
                return null;
            }
        }


        
        public static WindowTransitionConfig CreateDefault()
        {
            return new WindowTransitionConfig();
        }
    }
}