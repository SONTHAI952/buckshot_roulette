using System;
using System.Collections;
using UnityEngine;

namespace ZeroX.FlowSystem.Demo
{
    public class DemoFlowSystem : MonoBehaviour
    {
        [SerializeField] private Flow flow = new Flow();

        

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Key 1");
                flow.Run(this).OnFinish(wt =>
                {
                    Debug.Log("Flow finished");
                });
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Key 2");
                flow.EndFlowImmediate();
            }
        }
    }
}