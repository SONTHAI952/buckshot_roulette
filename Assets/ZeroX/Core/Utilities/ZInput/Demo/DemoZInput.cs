using UnityEngine;

namespace ZeroX.ZInputDemo
{
    public class DemoZInput : MonoBehaviour
    {
        private Vector2 prevTouchPos;
        
        private void Update()
        {
            if (ZInput.IsTouchDown)
            {
                Debug.Log("Is Touch Down");
            }
            
            if (ZInput.IsTouchUp)
            {
                Debug.Log("Is Touch Up");
                Debug.Log("TouchMoveDelta: " + ZInput.TouchMoveDelta);
            }

            if (ZInput.IsTouchStay)
            {
                Vector2 delta = (Vector2) Input.mousePosition - prevTouchPos;
                prevTouchPos = Input.mousePosition;
                if(delta.magnitude > 0.1f)
                    Debug.Log("Normal: " + delta.magnitude);
            }

            if(ZInput.TouchMoveDelta.magnitude > 0.1f)
                Debug.Log("ZInput: " + ZInput.TouchMoveDelta.magnitude);
        }
    }
}