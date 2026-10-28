using UnityEngine;

namespace ZeroX.CameraMatcherSystem
{
    public static class CameraMatcherPerspective
    {
        private class Context
        {
            public Camera cameraOfWorldObject;
            public Transform cameraOfWorldObjectTransform; //Cache
            public Vector3 worldObjectCenter;
            public Vector2 worldObjectSize;
            
            public RectTransform rectTransform;
            public Camera cameraOfRectTransform; //Cache
            
            
            public Vector3 planeNormal = Vector3.up;
            public Vector3 planeInPoint = Vector3.zero;
            public Vector3 planeRight = Vector3.right; //Trục bên phải của plane hướng về phía nào
            public Vector3 planeUp = Vector3.forward; //Trục hướng lên của plane hướng về phía nào
            
            

            public Context(CameraMatcherParameter parameter)
            {
                cameraOfWorldObject = parameter.cameraOfWorldObject;
                cameraOfWorldObjectTransform = cameraOfWorldObject.transform;

                worldObjectCenter = parameter.worldObjectCenter;
                worldObjectSize = parameter.worldObjectSize;

                rectTransform = parameter.rectTransform;
                cameraOfRectTransform = CameraMatcherUtility.GetRenderCameraOfRectTransform(rectTransform);

                planeRight = parameter.planeRight;
                planeUp = parameter.planeUp;
                planeNormal = Vector3.Cross(parameter.planeRight, parameter.planeUp);
                planeInPoint = parameter.worldObjectCenter;
            }
        }
        
        
        
        
        
        
        
        
        
        
        public static Vector3 CalculateCameraPos_To_WorldObject_Inside_RectTransform(CameraMatcherParameter parameter)
        {
            Context context = new Context(parameter);
            
            
            Vector3 cameraOfWorldObjectOldPos = context.cameraOfWorldObject.transform.position;
            
            MoveCamera_To_WorldObject_MatchInside_RectTransform(context);
            Vector3 destPos = context.cameraOfWorldObject.transform.position;
            
            context.cameraOfWorldObject.transform.position = cameraOfWorldObjectOldPos;
            
            return destPos;
        }

        //Match Center Camera với Center của WorldObject
        private static void MoveCamera_To_WorldObjectCenter_Match_RectTransformCenter(Context context)
        {
            Vector2 boardAreaScreenPoint = RectTransformUtility.WorldToScreenPoint(context.cameraOfRectTransform, context.rectTransform.position);
            var boardAreaWorldPoint = CameraMatcherUtility.ScreenToWorldPoint(context.cameraOfWorldObject, boardAreaScreenPoint, context.planeNormal, context.planeInPoint);

            Vector3 offset = context.worldObjectCenter - boardAreaWorldPoint;
            context.cameraOfWorldObjectTransform.position += offset;
        }
        
        private static Vector3 CalculateCameraPos_To_WorldObjectCenter_Match_RectTransformCenter(Context context)
        {
            Vector2 boardAreaScreenPoint = RectTransformUtility.WorldToScreenPoint(context.cameraOfRectTransform, context.rectTransform.position);
            var boardAreaWorldPoint = CameraMatcherUtility.ScreenToWorldPoint(context.cameraOfWorldObject, boardAreaScreenPoint, context.planeNormal, context.planeInPoint);

            Vector3 offset = context.worldObjectCenter - boardAreaWorldPoint;
            return context.cameraOfWorldObject.transform.position + offset;
        }
        


       



        private static void MoveCamera_To_WorldObject_MatchInside_RectTransform(Context context)
        {
            FindCameraDestPos_To_WorldObjectHeight_Match_RectTransformHeight(context);
            
            float rectTransformWidth = context.rectTransform.rect.width;
            float width = Calculate_WidthInRectTransform_Of_WorldObject(context);

            if (width > rectTransformWidth)
                FindCameraDestPos_To_WorldObjectWidth_Match_RectTransformWidth(context);
        }
        
        private static void FindCameraDestPos_To_WorldObjectHeight_Match_RectTransformHeight(Context context)
        {
            float delta = 1;
            float rectTransformHeight = context.rectTransform.rect.height;
            
            
            MoveCamera_To_WorldObjectCenter_Match_RectTransformCenter(context);
            float height = Calculate_HeightInRectTransform_Of_WorldObject(context);
            
            int sign = height > rectTransformHeight ? -1 : 1; //-1 có nghĩa là lùi camera lại. Nếu height > rectTransformHeight thì tức là cần lùi camera lại

            int safe = 0;
            while (Mathf.Abs(height - rectTransformHeight) > 10 && delta > 0.01f)
            {
                int newSign = height > rectTransformHeight ? -1 : 1;
                if (sign != newSign) //Nếu đảo chiều thì cần giảm delta để mức dịch chuyển camera ít dần -> dò vs khoảng nhỏ dần để tránh height và rectTransformHeight ko bao giờ match
                {
                    delta *= 0.75f;
                    sign = newSign;
                }

                context.cameraOfWorldObjectTransform.position += context.cameraOfWorldObjectTransform.forward * sign * delta;
                MoveCamera_To_WorldObjectCenter_Match_RectTransformCenter(context);
                height = Calculate_HeightInRectTransform_Of_WorldObject(context);

                safe++;
                if (safe > 10000)
                {
                    Debug.LogError("Loop vô hạn rồi");
                    break;
                }
            }
            
            //Debug.Log("Safe height: " + safe);
        }
        
        private static void FindCameraDestPos_To_WorldObjectWidth_Match_RectTransformWidth(Context context)
        {
            float delta = 1;
            float rectTransformWidth = context.rectTransform.rect.width;
            
            
            MoveCamera_To_WorldObjectCenter_Match_RectTransformCenter(context);
            float width = Calculate_WidthInRectTransform_Of_WorldObject(context);
            
            int sign = width > rectTransformWidth ? -1 : 1; //-1 có nghĩa là lùi camera lại. Nếu height > rectTransformHeight thì tức là cần lùi camera lại

            int safe = 0;
            while (Mathf.Abs(width - rectTransformWidth) > 10 && delta > 0.01f)
            {
                int newSign = width > rectTransformWidth ? -1 : 1;
                if (sign != newSign) //Nếu đảo chiều thì cần giảm delta để mức dịch chuyển camera ít dần -> dò vs khoảng nhỏ dần để tránh width và rectTransformWidth ko bao giờ match
                {
                    delta *= 0.75f;
                    sign = newSign;
                }

                context.cameraOfWorldObjectTransform.position += context.cameraOfWorldObjectTransform.forward * sign * delta;
                MoveCamera_To_WorldObjectCenter_Match_RectTransformCenter(context);
                width = Calculate_WidthInRectTransform_Of_WorldObject(context);

                safe++;
                if (safe > 10000)
                {
                    Debug.LogError("Loop vô hạn rồi");
                    break;
                }
            }
            
            //Debug.Log("Safe width: " + safe);
        }
        
        
        
        
        
        
        private static float Calculate_WidthInRectTransform_Of_WorldObject(Context context)
        {
            Vector3 leftWorldPos = context.worldObjectCenter - context.planeRight * context.worldObjectSize.x * 0.5f;
            Vector3 leftScreenPoint = RectTransformUtility.WorldToScreenPoint(context.cameraOfWorldObject, leftWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(context.rectTransform, leftScreenPoint, context.cameraOfRectTransform, out var leftLocalPointInRectTransform);
            
            
            Vector3 rightWorldPos = context.worldObjectCenter + context.planeRight * context.worldObjectSize.x * 0.5f;
            Vector3 rightScreenPoint = RectTransformUtility.WorldToScreenPoint(context.cameraOfWorldObject, rightWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(context.rectTransform, rightScreenPoint, context.cameraOfRectTransform, out var rightLocalPointInRectTransform);


            return Mathf.Abs(rightLocalPointInRectTransform.x - leftLocalPointInRectTransform.x);
        }
        
        private static float Calculate_HeightInRectTransform_Of_WorldObject(Context context)
        {
            Vector3 topWorldPos = context.worldObjectCenter + context.planeUp * context.worldObjectSize.y * 0.5f;
            Vector3 topScreenPoint = RectTransformUtility.WorldToScreenPoint(context.cameraOfWorldObject, topWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(context.rectTransform, topScreenPoint, context.cameraOfRectTransform, out var topLocalPointInRectTransform);
            
            
            Vector3 bottomWorldPos = context.worldObjectCenter - context.planeUp * context.worldObjectSize.y * 0.5f;
            Vector3 bottomScreenPoint = RectTransformUtility.WorldToScreenPoint(context.cameraOfWorldObject, bottomWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(context.rectTransform, bottomScreenPoint, context.cameraOfRectTransform, out var bottomLocalPointInRectTransform);


            return Mathf.Abs(topLocalPointInRectTransform.y - bottomLocalPointInRectTransform.y);
        }
    }
}