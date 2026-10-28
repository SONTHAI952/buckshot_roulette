using UnityEngine;
using ZeroX.AudioSystem;

namespace ZeroX.ButtonSystem
{
    [CreateAssetMenu(menuName = "ZeroX/Button System/Setting")]
    public class ButtonSystemSetting : ScriptableObject
    {
        [SerializeField] private AudioAsset clickSfx_Normal;
        [SerializeField] private AudioAsset clickSfx_Important;
        
        
        
        private static ButtonSystemSetting instance;

        public static ButtonSystemSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    string path = "ZeroX/ButtonSystem/Button System Setting";
                    instance = Resources.Load<ButtonSystemSetting>(path);
                    if (instance == null)
                    {
                        Debug.LogError("Button Setting not exist at path: " + path);
                    }
                }

                return instance;
            }
        }


        public AudioAsset GetClickSfx(ButtonClickSfxType clickSfxType)
        {
            switch (clickSfxType)
            {
                case ButtonClickSfxType.None:
                    return null;
                
                case ButtonClickSfxType.Normal:
                    return clickSfx_Normal;
                
                case ButtonClickSfxType.Important:
                    return clickSfx_Important;

                default:
                {
                    Debug.LogError("Not code for clickSfx type: " + clickSfxType);
                    return null;
                }
            }
        }
    }
}