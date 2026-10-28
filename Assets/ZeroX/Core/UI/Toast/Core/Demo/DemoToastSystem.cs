using System;
using UnityEngine;

namespace ZeroX.ToastSystem.Demo
{
    public class DemoToastSystem : MonoBehaviour
    {
        [SerializeField] private RectTransform target;
        
        
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToastMultiple.Show("Hello, This is Toast Multiple");
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ToastFlyUp.Show("Hello, This is Toast Fly Up", target);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ToastSingle.Show("Hello, This is Toast Single");
            }
        }
    }
}