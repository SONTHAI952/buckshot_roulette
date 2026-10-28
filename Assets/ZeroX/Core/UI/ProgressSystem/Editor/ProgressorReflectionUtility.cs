using System.Reflection;

namespace ZeroX.ProgressSystem.EditorNS
{
    public static class ProgressorReflectionUtility
    {
        private static BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public static void SetValue(Progressor progressor, float newValue)
        {
            var type = typeof(Progressor);
            var methodInfo = type.GetMethod("SetValue_Editor", bindingFlags);
            methodInfo.Invoke(progressor, new object[]{newValue});
        }
        
        public static void SetAnimateValue(Progressor progressor, float newAnimateValue)
        {
            var type = typeof(Progressor);
            var methodInfo = type.GetMethod("SetAnimateValue_Editor", bindingFlags);
            methodInfo.Invoke(progressor, new object[]{newAnimateValue});
        }
    }
}