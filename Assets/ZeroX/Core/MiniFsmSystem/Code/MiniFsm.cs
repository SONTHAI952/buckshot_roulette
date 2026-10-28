using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.MiniFsmSystem
{
    [System.Serializable]
    public class MiniFsm<TStateId>
    {
        private readonly Dictionary<TStateId, MiniState> dictState = new Dictionary<TStateId, MiniState>();
        
        
        [SerializeField] private TStateId entryStateId = default;
        
        
        //Trace State
        private readonly MiniStateId<TStateId> prevStateId = new MiniStateId<TStateId>();
        private readonly MiniStateId<TStateId> currentStateId = new MiniStateId<TStateId>();
        private readonly MiniStateId<TStateId> nextStateId = new MiniStateId<TStateId>();
        
        
        //Cache Current State
        private MiniState currentStateCached;
        private bool needReCacheCurrentState;

        
        //Status
        private MiniFsmStatus status = MiniFsmStatus.Stop;
        private MiniFsmProcess currentFsmProcess = MiniFsmProcess.None;

        
        //Queue
        private bool isHandlingQueueAction;
        private Queue<MiniFsmQueueCommand> queueAction = new Queue<MiniFsmQueueCommand>();
        private Queue<TStateId> queueStateId = new Queue<TStateId>(); //Data tương ứng cho action
        
        
        //Event
        public Action onFsmStart;
        public Action onFsmStop;
        public Action onFsmPause;
        public Action onFsmResume;
        



        //Properties
        public TStateId EntryStateId
        {
            get => entryStateId;
            set => entryStateId = value;
        }
        
        
        public bool HasPrevStateId => prevStateId.hasValue;
        public TStateId PrevStateId => prevStateId.value;

        public bool HasCurrentStateId => currentStateId.hasValue;
        public TStateId CurrentStateId => currentStateId.value;

        public bool HasNextStateId => nextStateId.hasValue;
        public TStateId NextStateId => nextStateId.value;
        
        
        
        public MiniFsmStatus Status => status;
        public bool IsRunning => status == MiniFsmStatus.Running;
        public bool IsRunningOrPausing => status == MiniFsmStatus.Pause;
        public bool IsPausing => status == MiniFsmStatus.Pause;

        

        //Constructor
        // protected MiniFsm(TStateId noneStateId)
        // {
        //     this.noneStateId = noneStateId;
        // }


        
        #region Utility

        private static void TryInvokeAction(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion
        


        #region Internal Modify State

        protected void InternalSetState(TStateId id, MiniState state)
        {
            dictState[id] = state;
        }

        protected bool InternalRemoveState(TStateId id)
        {
            return dictState.Remove(id);
        }

        protected void InternalRemoveAllState()
        {
            dictState.Clear();
        }

        protected bool InternalGetState(TStateId id, out MiniState state)
        {
            if (dictState.TryGetValue(id, out var result))
            {
                state = result;
                return true;
            }
            else
            {
                state = null;
                return false;
            }
        }

        #endregion
        
        

        #region Control Fsm


        private void InternalReset()
        {
            prevStateId.SetNone();
            currentStateId.SetNone();
            nextStateId.SetNone();
            
            ClearQueueProcess();
            
            needReCacheCurrentState = true;
            currentStateCached = null;
        }
        
        public void ClearFsm()
        {
            if (status != MiniFsmStatus.Stop)
            {
                Debug.LogError("Can only clear fsm while stopping");
                return;
            }
            
            if (currentFsmProcess != MiniFsmProcess.None)
            {
                Debug.LogError("Cannot Clear FSM from inside FSM");
                return;
            }
            
            InternalReset();
            dictState.Clear();
        }
        
        public void StartFsm()
        {
            if (currentFsmProcess != MiniFsmProcess.None)
            {
                Debug.LogError("Cannot Start FSM from inside FSM");
                return;
            }
            
            if (status != MiniFsmStatus.Stop)
            {
                Debug.LogError("Unable to start FSM because it has already been running previously");
                return;
            }

            
            //Reset
            InternalReset();
            
            
            //OnFsmStart
            currentFsmProcess = MiniFsmProcess.StartFsm;
            status = MiniFsmStatus.Running;
            TryInvokeAction(onFsmStart);
            currentFsmProcess = MiniFsmProcess.None;
            
            
            //Enter Entry State
            SwitchState(entryStateId);
        }
        
        
        
        public void StopFsm()
        {
            if (currentFsmProcess != MiniFsmProcess.None)
            {
                EnqueueAction(MiniFsmQueueCommand.StopFsm);
                return;
            }

            if (status == MiniFsmStatus.Stop)
            {
                Debug.LogError("Unable to stop FSM because it has already been stop previously");
                return;
            }
            
            
            //Status
            status = MiniFsmStatus.Stop;
            nextStateId.SetNone(); //Exit current state Mà không có nextStateId thì có nghĩa là StopFSM
            
            
            //Exit Current State
            if (currentStateId.hasValue)
            {
                if (InternalGetState(currentStateId.value, out var stateToExit))
                {
                    currentFsmProcess = MiniFsmProcess.ExitState;
                    stateToExit.OnStateExit();
                    currentFsmProcess = MiniFsmProcess.None;
                }
                
                prevStateId.SetValue(currentStateId.value);
            }
            else
            {
                prevStateId.SetNone();
            }
            
            
            //Reset Somethings
            currentStateId.SetNone();
            needReCacheCurrentState = true;
            currentStateCached = null;
            
            
            //OnFsmStop
            currentFsmProcess = MiniFsmProcess.StopFsm;
            TryInvokeAction(onFsmStop);
            currentFsmProcess = MiniFsmProcess.None;
            //Không handle queue action sau khi stop vì sau khi stop sẽ clear queue và Fsm đã stop rồi thì ko thể thực hiện action gì khác cho đến khi start lại
            

            
            //Clear QueueAction
            ClearQueueProcess();
        }

        
        public void PauseFsm()
        {
            if (currentFsmProcess != MiniFsmProcess.None)
            {
                EnqueueAction(MiniFsmQueueCommand.PauseFsm);
                return;
            }
            
            if (status != MiniFsmStatus.Running)
            {
                if(status == MiniFsmStatus.Pause)
                    Debug.LogError("Unable to pause FSM because it has already been pause previously");
                else
                    Debug.LogError("Unable to pause FSM because it is not running");
                
                return;
            }

            status = MiniFsmStatus.Pause;
            
            
            
            //OnFsmPause
            currentFsmProcess = MiniFsmProcess.PauseFsm;
            TryInvokeAction(onFsmPause);
            currentFsmProcess = MiniFsmProcess.None;
            HandleQueueAction();
        }
        

        public void ResumeFsm()
        {
            if (currentFsmProcess != MiniFsmProcess.None)
            {
                EnqueueAction(MiniFsmQueueCommand.ResumeFsm);
                return;
            }
            
            if (status != MiniFsmStatus.Pause)
            {
                Debug.LogError("Unable to resume FSM because it is not pausing");
                return;
            }

            status = MiniFsmStatus.Running;
            
            
            
            //OnFsmResume
            currentFsmProcess = MiniFsmProcess.ResumeFsm;
            TryInvokeAction(onFsmResume);
            currentFsmProcess = MiniFsmProcess.None;
            HandleQueueAction();
        }

        
        
        public void UpdateFsm()
        {
            if(status != MiniFsmStatus.Running)
                return;

            
            if (needReCacheCurrentState)
            {
                needReCacheCurrentState = false;
                
                if (currentStateId.hasValue)
                    currentStateCached = GetState(currentStateId.value);
                else
                    currentStateCached = null;
            }
            
            
            if (currentStateCached != null)
            {
                currentStateCached.OnStateUpdate();
            }
        }
        
        public void LateUpdateFsm()
        {
            if(status != MiniFsmStatus.Running)
                return;
            
            
            if (needReCacheCurrentState)
            {
                needReCacheCurrentState = false;

                if (currentStateId.hasValue)
                    currentStateCached = GetState(currentStateId.value);
                else
                    currentStateCached = null;
            }
            
            
            if (currentStateCached != null)
            {
                currentStateCached.OnStateLateUpdate();
            }
        }
        
        public void FixedUpdateFsm()
        {
            if(status != MiniFsmStatus.Running)
                return;
            
            
            if (needReCacheCurrentState)
            {
                needReCacheCurrentState = false;
                
                if (currentStateId.hasValue)
                    currentStateCached = GetState(currentStateId.value);
                else
                    currentStateCached = null;
            }
            
            
            if (currentStateCached != null)
            {
                currentStateCached.OnStateFixedUpdate();
            }
        }
        

        #endregion


        
        #region Switch State

        public bool SwitchState(TStateId id)
        {
            if (currentFsmProcess != MiniFsmProcess.None)
            {
                EnqueueAction(MiniFsmQueueCommand.SwitchState, id);
                return false;
            }

            if (status != MiniFsmStatus.Running)
            {
                if (status == MiniFsmStatus.Stop)
                    Debug.LogError("Cannot SwitchState when FSM stop");

                if (status == MiniFsmStatus.Pause)
                    Debug.LogError("Cannot SwitchState when FSM pause");

                return false;
            }

            
            //Kiểm tra xem có state để chuyển không đã
            if (InternalGetState(id, out var stateToEnter) == false)
            {
                Debug.LogErrorFormat("State with id '{0}' not exist", id);
                return false;
            }
            

            //Exit Current State
            if (currentStateId.hasValue)
            {
                if (InternalGetState(currentStateId.value, out var stateToExit))
                {
                    nextStateId.SetValue(id);
                    
                    currentFsmProcess = MiniFsmProcess.ExitState;
                    stateToExit.OnStateExit();
                    currentFsmProcess = MiniFsmProcess.None;
                    
                    nextStateId.SetNone();
                }
                
                prevStateId.SetValue(currentStateId.value);
            }
            else
            {
                prevStateId.SetNone();
            }
            
            
            
            //Enter Next State
            currentStateId.SetValue(id);
            currentFsmProcess = MiniFsmProcess.EnterState;
            stateToEnter.OnStateEnter();
            currentFsmProcess = MiniFsmProcess.None;
            
            //Cache
            needReCacheCurrentState = true;

            
            //Handle Queue Action
            HandleQueueAction();

            return true;
        }

        /// <summary>
        /// Để chuyển được state thì state hiện tại phải là fromStateId.
        /// Dùng trong các trường hợp như chuyển sang state win nếu đang ở state playing
        /// </summary>
        public bool SwitchStateFromTo(TStateId fromStateId, TStateId toStateId)
        {
            if (status != MiniFsmStatus.Running)
            {
                if (status == MiniFsmStatus.Stop)
                    Debug.LogError("Cannot SwitchState when FSM stop");

                if (status == MiniFsmStatus.Pause)
                    Debug.LogError("Cannot SwitchState when FSM pause");

                return false;
            }

            if (currentStateId.hasValue == false)
            {
                Debug.LogErrorFormat("Cannot SwitchState from '{0}' to '{1}' because currentState is empty", fromStateId, toStateId);
                return false;
            }
            
            if (currentStateId.value.Equals(fromStateId) == false)
            {
                Debug.LogErrorFormat("Cannot SwitchState to '{1}' because currentState is not '{0}'", fromStateId, toStateId);
                return false;
            }
            
            return SwitchState(toStateId);
        }

        #endregion
        
        

        #region Queue Action

        private void ClearQueueProcess()
        {
            queueAction.Clear();
            queueStateId.Clear();
        }

        private void EnqueueAction(MiniFsmQueueCommand commandToEnqueue, TStateId id = default)
        {
            //Không cần thiết vì sau khi StopFsm thì queue sẽ bị hủy
            // if (hasQueueStopFsm)
            // {
            //     Debug.LogError("Cannot execute other action because StopFsm will call");
            //     return;
            // }
            
            if (currentFsmProcess == MiniFsmProcess.StopFsm)
            {
                Debug.LogErrorFormat("Cannot {0} in OnFsmStop", commandToEnqueue);
                return;
            }
            
            if (currentFsmProcess == MiniFsmProcess.ExitState)
            {
                Debug.LogErrorFormat("Cannot {0} in OnStateExit", commandToEnqueue);
                return;
            }
            
            queueAction.Enqueue(commandToEnqueue);
            queueStateId.Enqueue(id);
        }

        private void HandleQueueAction()
        {
            if(isHandlingQueueAction)
                return;

            
            
            isHandlingQueueAction = true;
            
            while (queueAction.Count > 0)
            {
                var action = queueAction.Dequeue();
                var stateId = queueStateId.Dequeue();

                switch (action)
                {
                    case MiniFsmQueueCommand.SwitchState:
                    {
                        SwitchState(stateId);
                        break;
                    }
                    case MiniFsmQueueCommand.PauseFsm:
                    {
                        PauseFsm();
                        break;
                    }
                    case MiniFsmQueueCommand.ResumeFsm:
                    {
                        ResumeFsm();
                        break;
                    }
                    case MiniFsmQueueCommand.StopFsm:
                    {
                        StopFsm();
                        break;
                    }
                    default:
                    {
                        Debug.LogError("Not code for action: " + action);
                        break;
                    }
                }
            }
            
            isHandlingQueueAction = false;
        }

        #endregion


        
        #region Modify State

        public void SetState(TStateId id, MiniState state)
        {
            dictState[id] = state;
            
            //Cache
            needReCacheCurrentState = true;
        }

        public bool RemoveState(TStateId id)
        {
            //Cache
            needReCacheCurrentState = true;
            
            return dictState.Remove(id);
        }

        public void RemoveAllState()
        {
            dictState.Clear();
            
            //Cache
            needReCacheCurrentState = true;
        }

        public MiniState GetState(TStateId id)
        {
            if(dictState.TryGetValue(id, out var state))
            {
                return state;
            }
            else
            {
                return null;
            }
        }

        #endregion
        
        
        
        #region Set State Command

        public void SetOnStateEnter(TStateId id, IStateCommand stateCommand)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateEnter = stateCommand;
        }
        
        public void SetOnStateExit(TStateId id, IStateCommand stateCommand)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateExit = stateCommand;
        }
        
        public void SetOnStateUpdate(TStateId id, IStateCommand stateCommand)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateUpdate = stateCommand;
        }
        
        public void SetOnStateLateUpdate(TStateId id, IStateCommand stateCommand)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateLateUpdate = stateCommand;
        }
        
        public void SetOnStateFixedUpdate(TStateId id, IStateCommand stateCommand)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateFixedUpdate = stateCommand;
        }

        #endregion
        
        

        #region Set State Command - Action

        public void SetOnStateEnter(TStateId id, Action action)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateEnter = new ActionStateCommand(action);
        }
        
        public void SetOnStateExit(TStateId id, Action action)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateExit = new ActionStateCommand(action);
        }
        
        public void SetOnStateUpdate(TStateId id, Action action)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateUpdate = new ActionStateCommand(action);
        }
        
        public void SetOnStateLateUpdate(TStateId id, Action action)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateLateUpdate = new ActionStateCommand(action);
        }
        
        public void SetOnStateFixedUpdate(TStateId id, Action action)
        {
            if (InternalGetState(id, out var state) == false)
            {
                state = new MiniState();
                InternalSetState(id, state);
            }

            state.onStateFixedUpdate = new ActionStateCommand(action);
        }

        #endregion
    }
}