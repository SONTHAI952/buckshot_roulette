using UnityEngine;

namespace ZeroX
{
    public enum ActiveMode
    {
        Off, EditorOnly, All
    }
    
    public static class ActiveModeExtension
    {
        public static bool IsActive(this ActiveMode mode)
        {
            if (mode == ActiveMode.Off)
                return false;

            
            if (mode == ActiveMode.All)
                return true;

            
            if (mode == ActiveMode.EditorOnly)
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
            
            
            Debug.LogError("Not code for active mode: " + mode);
            return false;
        }
        
        public static bool IsInactive(this ActiveMode mode)
        {
            if (mode == ActiveMode.Off)
                return true;

            
            if (mode == ActiveMode.All)
                return false;

            
            if (mode == ActiveMode.EditorOnly)
            {
#if UNITY_EDITOR
                return false;
#else
                return true;
#endif
            }
            
            
            Debug.LogError("Not code for active mode: " + mode);
            return false;
        }
    }
}