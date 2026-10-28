using System;
using UnityEngine;

namespace ZeroX.MiniFsmSystem
{
    public class MiniState
    {
        public IStateCommand onStateEnter;
        public IStateCommand onStateExit;
        
        public IStateCommand onStateUpdate;
        public IStateCommand onStateLateUpdate;
        public IStateCommand onStateFixedUpdate;

        
        
        public void OnStateEnter()
        {
            try
            {
                onStateEnter?.Execute();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void OnStateExit()
        {
            try
            {
                onStateExit?.Execute();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void OnStateUpdate()
        {
            try
            {
                onStateUpdate?.Execute();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void OnStateLateUpdate()
        {
            try
            {
                onStateLateUpdate?.Execute();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void OnStateFixedUpdate()
        {
            try
            {
                onStateFixedUpdate?.Execute();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}