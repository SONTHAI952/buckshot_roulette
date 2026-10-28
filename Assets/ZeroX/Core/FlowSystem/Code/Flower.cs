using System;
using UnityEngine;

namespace ZeroX.FlowSystem
{
    public class Flower : MonoBehaviour
    {
        public enum WhenType
        {
            Awake, FirstEnable, Start
        }
        
        
        public bool autoRun = false;
        public WhenType autoRunWhen = WhenType.Start;
        [SerializeField] private Flow flow = new Flow();
        
        
        //Temp
        private bool isFirstEnable = true;


        private void Awake()
        {
            if (autoRun && autoRunWhen == WhenType.Awake)
            {
                Run();
            }
        }

        private void OnEnable()
        {
            if (isFirstEnable)
            {
                isFirstEnable = false;
                
                if (autoRun && autoRunWhen == WhenType.FirstEnable)
                {
                    Run();
                }
            }
        }

        private void Start()
        {
            if (autoRun && autoRunWhen == WhenType.Start)
            {
                Run();
            }
        }

        public WaitToken Run()
        {
            return flow.Run(this);
        }

        public void EndFlowImmediate()
        {
            flow.EndFlowImmediate();
        }
    }
}