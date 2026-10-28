using System;
using UnityEngine;

namespace ZeroX
{
    public class AutoRotateZ : MonoBehaviour
    {
        public float rotateSpeed = -360;
        public bool ignoreTimeScale = false;


        private void Update()
        {
            if (ignoreTimeScale)
            {
                transform.Rotate(0, 0, rotateSpeed * Time.unscaledDeltaTime);
            }
            else
            {
                transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
            }
        }
    }
}