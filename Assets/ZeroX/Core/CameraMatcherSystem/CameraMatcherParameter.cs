using UnityEngine;

namespace ZeroX.CameraMatcherSystem
{
    [System.Serializable]
    public class CameraMatcherParameter
    {
        public Camera cameraOfWorldObject;
        public Vector3 worldObjectCenter;
        public Vector2 worldObjectSize;
        public RectTransform rectTransform;
        
        
        public Vector3 planeRight = Vector3.right; //Trục bên phải của plane hướng về phía nào
        public Vector3 planeUp = Vector3.forward; //Trục hướng lên của plane hướng về phía nào


        public static CameraMatcherParameter CreateForPlaneXY()
        { 
            CameraMatcherParameter parameter = new CameraMatcherParameter();
            parameter.planeRight = Vector3.right;
            parameter.planeUp = Vector3.up;

            return parameter;
        }
        
        public static CameraMatcherParameter CreateForPlaneXZ()
        { 
         CameraMatcherParameter parameter = new CameraMatcherParameter();
         parameter.planeRight = Vector3.right;
         parameter.planeUp = Vector3.forward;

         return parameter;
        }
        
        
        
        
        // Giải thích về planeRight và planeUp
        /****************************************************
         *                    *                             *
         *                   ***                            *
         *                  * * *                           *
         *                    *                             *
         *                    *                             *
         *                    *                             *
         *                    * Đây là planeUp              *
         *                    *                             *
         *                    *                             *
         *                    *                             *
         *                    *                      *      *
         *                    *                       *     *
         *                    ***************************   *  Đây là planeRight
         *                                            *     *
         *                                           *      *
         *                                                  *
         *                                                  *
         *                                                  *
         *                                                  *
         *                                                  *
         *                                                  *
         *                                                  *
         ****************************************************
         */
        
        
        
        
        // Giải thích về planeRight và planeUp
        /*
                               ****************************************************
                              *                    *                             *
                             *                   ***                            *
                            *                  * * *                           *
                           *                 *  *  *                          *
                          *                    *                             *
                         *                    *                             *
                        *                    * Đây là planeUp              *
                       *                    *                             *
                      *                    *                             *
                     *                    *                             *
                    *                    *                      *      *
                   *                    *                       *     *
                  *                    ***************************   *  Đây là planeRight
                 *                                            *     *
                *                                           *      *
               *                                                  *
              *                                                  *
             *                                                  *
            *                                                  *
           *                                                  *
          *                                                  *
         *                                                  *
        ****************************************************
        */
    }
}