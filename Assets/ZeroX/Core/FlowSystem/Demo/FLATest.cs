using System;
using System.Collections;
using UnityEngine;

namespace ZeroX.FlowSystem.Demo
{
    public class FLATest : FlowAction
    {
        [SerializeField] private float waitingTime = 1f;
        
        protected override IEnumerator FlowActionTimeline()
        {
            Debug.Log("Start Action " + name);
            yield return new WaitForSeconds(waitingTime);
            Debug.Log("End Action " + name);
        }

        private void Update()
        {
            if (IsRunning && Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Key3 " + name);
                EndFlowActionImmediate();
            }
        }
    }
}