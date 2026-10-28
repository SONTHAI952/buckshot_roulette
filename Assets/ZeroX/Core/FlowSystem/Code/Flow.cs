using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.FlowSystem
{
    [System.Serializable]
    public class Flow
    {
        [SerializeField] [ReadOnlyField] private FlowAction waitingFlowAction;
        [SerializeField] private List<FlowAction> listFlowAction = new List<FlowAction>();

        
        //Property
        public bool IsRunning { get; private set; }
        
        
        //Temp
        private bool isInitialized = false;
        private MonoBehaviour owner;
        private Coroutine crRunner;
        private WaitToken waitToken;
        


        
        
        private void InitIfNot()
        {
            if(isInitialized)
                return;
            
            isInitialized = true;
            foreach (var flowAction in listFlowAction)
            {
                if (flowAction == null)
                {
                    Debug.LogError("Has flow action null in list");
                    continue;
                }
                
                flowAction.Inject(this);
            }
        }
        
        public WaitToken Run(MonoBehaviour owner)
        {
            InitIfNot();

            if (IsRunning)
            {
                Debug.LogError("Flow is already running");
                return WaitToken.CreateHasResulted();
            }

            if (owner == null)
            {
                Debug.LogError("Owner can't be null");
                return WaitToken.CreateHasResulted();
            }
            
            
            this.owner = owner;
            waitToken = new WaitToken();
            crRunner = owner.StartCoroutine(Runner());
            return waitToken;
        }


        IEnumerator Runner()
        {
            IsRunning = true;
            waitingFlowAction = null;

            
            foreach (var flowAction in listFlowAction)
            {
                if (flowAction == null)
                {
                    Debug.LogError("Has flow action null in list");
                    continue;
                }
                
                waitingFlowAction = flowAction;
                
                flowAction.Run();
                if (flowAction.WaitingType == WaitingType.WaitToRunNextAction)
                    yield return new WaitWhile(() => flowAction.IsRunning);

                waitingFlowAction = null;
            }


            foreach (var flowAction in listFlowAction)
            {
                if (flowAction == null)
                    continue;
                
                if (flowAction.WaitingType == WaitingType.WaitToRunNextFlow)
                {
                    waitingFlowAction = flowAction;
                    yield return new WaitWhile(() => flowAction.IsRunning);
                }
            }
            

            waitingFlowAction = null;
            owner = null;
            crRunner = null;
            IsRunning = false;
            SetResultForWaitToken();
        }

        public void EndFlowImmediate()
        {
            if(IsRunning == false)
                return;

            if (owner != null)
            {
                owner.StopCoroutine(crRunner);
            }

            foreach (var flowAction in listFlowAction)
            {
                if(flowAction != null)
                    flowAction.EndFlowActionImmediate();
            }
            
            waitingFlowAction = null;
            owner = null;
            crRunner = null;
            IsRunning = false;
            SetResultForWaitToken();
        }

        private void SetResultForWaitToken()
        {
            WaitToken wtTemp = waitToken;
            waitToken = null;
            wtTemp.SetResult();
        }
    }
}