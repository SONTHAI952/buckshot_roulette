using DG.Tweening;
using UnityEngine;

namespace ZeroX.CameraMatcherSystem
{
    public class CameraMatcher : MonoBehaviour
    {
        public Vector3 planeRight = Vector3.right;
        public Vector3 planeUp = Vector3.up;
        public Camera cameraOfWorldObject;
        public RectTransform rectTransform;

        
        //Temp
        private Tween tweenMove;
        
        
        
        
        
        public void StopDoMoveCamera()
        {
            if (tweenMove != null)
            {
                tweenMove.Kill();
                tweenMove = null;
            }
        }

        private CameraMatcherParameter CreateParameter(Vector3 worldObjectCenter, Vector2 worldObjectSize)
        {
            CameraMatcherParameter parameter = new CameraMatcherParameter();
            parameter.planeRight = planeRight;
            parameter.planeUp = planeUp;

            parameter.rectTransform = rectTransform;
            parameter.cameraOfWorldObject = cameraOfWorldObject;

            parameter.worldObjectCenter = worldObjectCenter;
            parameter.worldObjectSize = worldObjectSize;

            return parameter;
        }
        
        public void MoveCamera_To_WorldObject_Inside_BoardArea(Vector3 worldObjectCenter, Vector2 worldObjectSize)
        {
            StopDoMoveCamera();
            
            var parameter = CreateParameter(worldObjectCenter, worldObjectSize);

            if (cameraOfWorldObject.orthographic)
            {
                CameraMatcherOrthographic.CalculateCameraPos_To_WorldObject_Inside_RectTransform(parameter, out Vector3 destPos, out float destOrthoSize);
                transform.position = destPos;
                cameraOfWorldObject.orthographicSize = destOrthoSize;
            }
            else
            {
                Vector3 destPos = CameraMatcherPerspective.CalculateCameraPos_To_WorldObject_Inside_RectTransform(parameter);
                transform.position = destPos;
            }
        }
        
        public Tween DoMoveCamera_To_WorldObject_Inside_BoardArea(Vector3 worldObjectCenter, Vector2 worldObjectSize, float duration)
        {
            StopDoMoveCamera();
            
            
            var parameter = CreateParameter(worldObjectCenter, worldObjectSize);

            if (cameraOfWorldObject.orthographic)
            {
                CameraMatcherOrthographic.CalculateCameraPos_To_WorldObject_Inside_RectTransform(parameter, out Vector3 destPos, out float destOrthoSize);
                
                Sequence sequence = DOTween.Sequence(this);
                sequence.Append(transform.DOMove(destPos, duration));
                sequence.Insert(0, cameraOfWorldObject.DOOrthoSize(destOrthoSize, duration));
                return sequence;
            }
            else
            {
                Vector3 destPos = CameraMatcherPerspective.CalculateCameraPos_To_WorldObject_Inside_RectTransform(parameter);
                return transform.DOMove(destPos, 0.5f);
            }
        }
    }
}