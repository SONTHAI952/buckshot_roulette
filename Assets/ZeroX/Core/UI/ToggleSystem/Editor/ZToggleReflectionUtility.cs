using System.Reflection;

namespace ZeroX.ToggleSystem.Editors
{
    public class ZToggleReflectionUtility
    {
        private static BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public static void Internal_SetIsOn(ZToggle toggle, bool newValue)
        {
            var type = typeof(ZToggle);
            var methodInfo = type.GetMethod("Internal_SetIsOn", bindingFlags);
            methodInfo.Invoke(toggle, new object[]{newValue});
        }
    }
}