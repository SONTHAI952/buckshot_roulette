using System.Threading;
using UnityEngine;

namespace ZeroX.LogSystem.Demo
{
    public class DemoZLogger : MonoBehaviour
    {
        [ContextMenu("Test Debug")]
        private void TestDebug()
        {
            Thread thread = new Thread(DoWork);
            thread.Start();
            
            ZLogger.Log("Xin chào");
            //ZDebug.LogErrorFormat(this, Color.magenta, "Xin chào {0}", "Lượng");
        }

        private void DoWork()
        {
            Thread.Sleep(1000);
            ZLogger.Log("Xin chào từ task khác");
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
                ZLogger.Log("Xin chào");
        }
    }
}