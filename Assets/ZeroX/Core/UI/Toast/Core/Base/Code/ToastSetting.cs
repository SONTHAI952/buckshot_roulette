using UnityEngine;

namespace ZeroX.ToastSystem
{
    //[CreateAssetMenu(fileName = "ToastSetting", menuName = "ZeroX/Toast Setting", order = 0)]
    public class ToastSetting : ScriptableObject
    {
        public ToastSingle toastSingle;
        public ToastMultiple toastMultiple;
        public ToastFlyUp toastFlyUp;
        
        private static ToastSetting instance;

        public static ToastSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ToastSetting>(ToastConst.SETTING_RESOURCES_FILE_PATH);
                    if (instance == null)
                        Debug.LogError("Toast Setting not exist at Resources path: " + ToastConst.SETTING_RESOURCES_FILE_PATH);
                }
                
                return instance;
            }
        }
    }
}