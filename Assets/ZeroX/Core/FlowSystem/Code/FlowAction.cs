using System.Collections;
using UnityEngine;

namespace ZeroX.FlowSystem
{
    public abstract class FlowAction : MonoBehaviour
    {
        [SerializeField] protected WaitingType waitingType = WaitingType.None;

        
        //Property
        public WaitingType WaitingType => waitingType;
        protected virtual WaitingType DefaultWaitingType => WaitingType.WaitToRunNextAction;
        public Flow Flow { get; private set; }
        public bool IsRunning { get; private set; }
        

        //Temp
        private Coroutine crRunner;
        private Coroutine crFlowActionTimeline;

        
        
        
        internal void Inject(Flow flow)
        {
            this.Flow = flow;
        }


        protected virtual void Reset()
        {
            waitingType = DefaultWaitingType;
        }


        internal void Run()
        {
            if (IsRunning)
            {
                Debug.LogError($"Flow Action {this.name} is already running");
                return;
            }
            
            crRunner = StartCoroutine(Runner());
        }

        private IEnumerator Runner()
        {
            IsRunning = true;
            
            crFlowActionTimeline = StartCoroutine(FlowActionTimeline());
            yield return crFlowActionTimeline;
            
            crRunner = null;
            crFlowActionTimeline = null;
            IsRunning = false;
        }

        protected abstract IEnumerator FlowActionTimeline();

        /// <summary>
        /// Dừng flow action này ngay lập tức. Nhưng Flow vẫn sẽ chạy tiếp tới action tiếp theo
        /// </summary>
        public void EndFlowActionImmediate()
        {
            if(IsRunning == false)
                return;
            
            StopCoroutine(crRunner);
            StopCoroutine(crFlowActionTimeline);
            
            crRunner = null;
            crFlowActionTimeline = null;
            IsRunning = false;
        }
    }
}