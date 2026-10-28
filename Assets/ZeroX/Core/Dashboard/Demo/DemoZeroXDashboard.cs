using UnityEngine;


namespace ZeroX.DashboardSystem.Demo
{
    public class DemoZeroXDashboard : MonoBehaviour
    {
        [TextArea(3, 5)]
        public string output;


        [ContextMenu("Log Type")]
        public void LogType()
        {
            //output = typeof(TTPCore).AssemblyQualifiedName;
        }
    }
}