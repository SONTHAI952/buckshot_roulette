using System;
using UnityEngine;

namespace ZeroX.MiniFsmSystem.Demo
{
    public class DemoMiniFsm : MonoBehaviour
    {
        //private MiniFsm<TestFsmId> miniFsm = new MiniFsm<TestFsmId>();
        [SerializeField] public TestFsm miniFsm = new TestFsm();
        [SerializeField] public string abc;
        [SerializeField] public string def;
        

        private void Awake()
        {
            InitFsm();
        }

        private void InitFsm()
        {
            Debug.Log("Init Fsm");
            
            miniFsm.onFsmStart = () => Debug.Log("On Fsm Start");
            miniFsm.onFsmStop = () => Debug.Log("On Fsm Stop");
            
            miniFsm.SetOnStateEnter(TestFsmId.Idle, () => Debug.Log("On State Enter: " + miniFsm.CurrentStateId));
            miniFsm.SetOnStateExit(TestFsmId.Idle, () => Debug.Log("On State Exit: " + miniFsm.CurrentStateId));
            
            miniFsm.SetOnStateEnter(TestFsmId.Attack, () =>
            {
                //miniFsm.SwitchState(TestFsmId.Run);
                Debug.Log("On State Enter: " + miniFsm.CurrentStateId);
            });
            miniFsm.SetOnStateExit(TestFsmId.Attack, () => Debug.Log("On State Exit: " + miniFsm.CurrentStateId));
            //miniFsm.SetOnStateUpdate(TestFsmId.Attack, () => Debug.Log("On State Update: " + miniFsm.CurrentStateId));
            
            miniFsm.SetOnStateEnter(TestFsmId.Run, () => Debug.Log("On State Enter: " + miniFsm.CurrentStateId));
            
            miniFsm.EntryStateId = TestFsmId.Attack;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
                miniFsm.SwitchState(TestFsmId.Idle);
            
            if(Input.GetKeyDown(KeyCode.Alpha2))
                miniFsm.SwitchState(TestFsmId.Attack);
            
            if(Input.GetKeyDown(KeyCode.Alpha3))
                miniFsm.SwitchState(TestFsmId.Run);
            
            if(Input.GetKeyDown(KeyCode.Alpha4))
                miniFsm.SwitchStateFromTo(TestFsmId.Run, TestFsmId.Attack);
            
            if(Input.GetKeyDown(KeyCode.Alpha5))
                miniFsm.SwitchStateFromTo(TestFsmId.Run, TestFsmId.Idle);
            
            if(Input.GetKeyDown(KeyCode.Q))
                miniFsm.StartFsm();
            
            if(Input.GetKeyDown(KeyCode.W))
                miniFsm.StopFsm();
            
            if(Input.GetKeyDown(KeyCode.E))
                miniFsm.ClearFsm();
            
            if(Input.GetKeyDown(KeyCode.A))
                InitFsm();
            
            miniFsm.UpdateFsm();
        }
    }
}