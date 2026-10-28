using UnityEngine;

namespace ZeroX.LogSystem
{
    public static class ZLogger
    {
        public static bool EnableLog = true;
        public static bool EnableLogError = true;
        public static bool EnableColor = true;



        private const string FORMAT_COLOR = "<color=#{0}>{1}</color>";
        
        

        #region Log

        public static void Log(object message)
        {
            if (!EnableLog)
                return;
            
            if(ZeroRx.IsMainThread)
                Debug.Log(message);
            else
                ZeroRx.RunAction(() => Debug.Log(message));
        }
        
        public static void Log(object message, Object context)
        {
            if (!EnableLog)
                return;
            
            if(ZeroRx.IsMainThread)
                Debug.Log(message, context);
            else
                ZeroRx.RunAction(() => Debug.Log(message, context));
        }
        
        
        
        public static void Log(object message, Color color)
        {
            if (!EnableLog)
                return;

            if (ZeroRx.IsMainThread)
            {
                if(EnableColor)
                    Debug.LogFormat(FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), message);
                else
                    Debug.Log(message);
            }
            else
            {
                ZeroRx.RunAction(() =>
                {
                    if(EnableColor)
                        Debug.LogFormat(FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), message);
                    else
                        Debug.Log(message);
                });
            }
        }
        
        public static void Log(object message, Object context, Color color)
        {
            if (!EnableLog)
                return;

            if (ZeroRx.IsMainThread)
            {
                if(EnableColor)
                    Debug.LogFormat(context, FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), message);
                else
                    Debug.Log(message, context);
            }
            else
            {
                ZeroRx.RunAction(() =>
                {
                    if(EnableColor)
                        Debug.LogFormat(context, FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), message);
                    else
                        Debug.Log(message, context);
                });
            }
        }

        #endregion



        #region Log Format

        public static void LogFormat(string format, params object[] args)
        {
            if (!EnableLog)
                return;

            if (ZeroRx.IsMainThread)
                Debug.LogFormat(format, args);
            else
                ZeroRx.RunAction(() => Debug.LogFormat(format, args));
            
        }
        
        public static void LogFormat(Object context, string format, params object[] args)
        {
            if (!EnableLog)
                return;
            
            if(ZeroRx.IsMainThread)
                Debug.LogFormat(context, format, args);
            else
                ZeroRx.RunAction(() => Debug.LogFormat(context, format, args));
        }
        
        
        
        public static void LogFormat(Color color, string format, params object[] args)
        {
            if (!EnableLog)
                return;

            if (ZeroRx.IsMainThread)
            {
                if(EnableColor)
                    Debug.LogFormat(FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), string.Format(format, args));
                else
                    Debug.LogFormat(format, args);
            }
            else
            {
                ZeroRx.RunAction(() =>
                {
                    if(EnableColor)
                        Debug.LogFormat(FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), string.Format(format, args));
                    else
                        Debug.LogFormat(format, args);
                });
            }
        }
        
        public static void LogFormat(Object context, Color color, string format, params object[] args)
        {
            if (!EnableLog)
                return;

            if (ZeroRx.IsMainThread)
            {
                if(EnableColor)
                    Debug.LogFormat(context, FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), string.Format(format, args));
                else
                    Debug.LogFormat(context, format, args);
            }
            else
            {
                ZeroRx.RunAction(() =>
                {
                    if(EnableColor)
                        Debug.LogFormat(context, FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), string.Format(format, args));
                    else
                        Debug.LogFormat(context, format, args);
                });
            }
        }

        #endregion
        
        
        
        #region Log Error

        public static void LogError(object message)
        {
            if(!EnableLogError)
                return;
            
            if(ZeroRx.IsMainThread)
                Debug.LogError(message);
            else
                ZeroRx.RunAction(() => Debug.LogError(message));
        }
        
        public static void LogError(object message, Object context)
        {
            if(!EnableLogError)
                return;
            
            if(ZeroRx.IsMainThread)
                Debug.LogError(message, context);
            else
                ZeroRx.RunAction(() => Debug.LogError(message, context));
        }
        
        
        
        public static void LogError(object message, Color color)
        {
            if(!EnableLogError)
                return;

            if (ZeroRx.IsMainThread)
            {
                if(EnableColor)
                    Debug.LogErrorFormat(FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), message);
                else
                    Debug.LogError(message);
            }
            else
            {
                ZeroRx.RunAction(() =>
                {
                    if(EnableColor)
                        Debug.LogErrorFormat(FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), message);
                    else
                        Debug.LogError(message);
                });
            }
        }
        
        public static void LogError(object message, Object context, Color color)
        {
            if(!EnableLogError)
                return;

            if (ZeroRx.IsMainThread)
            {
                if(EnableColor)
                    Debug.LogErrorFormat(context, FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), message);
                else
                    Debug.LogError(message, context);
            }
            else
            {
                ZeroRx.RunAction(() =>
                {
                    if(EnableColor)
                        Debug.LogErrorFormat(context, FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), message);
                    else
                        Debug.LogError(message, context);
                });
            }
        }
        
        #endregion
        
        
        
        #region Log Error Format

        public static void LogErrorFormat(string format, params object[] args)
        {
            if(!EnableLogError)
                return;
            
            if(ZeroRx.IsMainThread)
                Debug.LogErrorFormat(format, args);
            else
                ZeroRx.RunAction(() => Debug.LogErrorFormat(format, args));
        }
        
        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            if(!EnableLogError)
                return;
            
            if(ZeroRx.IsMainThread)
                Debug.LogErrorFormat(context, format, args);
            else
                ZeroRx.RunAction(() => Debug.LogErrorFormat(context, format, args));
        }
        
        
        
        public static void LogErrorFormat(Color color, string format, params object[] args)
        {
            if(!EnableLogError)
                return;

            if (ZeroRx.IsMainThread)
            {
                if(EnableColor)
                    Debug.LogErrorFormat(FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), string.Format(format, args));
                else
                    Debug.LogErrorFormat(format, args);
            }
            else
            {
                ZeroRx.RunAction(() =>
                {
                    if(EnableColor)
                        Debug.LogErrorFormat(FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), string.Format(format, args));
                    else
                        Debug.LogErrorFormat(format, args);
                });
            }
        }
        
        public static void LogErrorFormat(Object context, Color color, string format, params object[] args)
        {
            if(!EnableLogError)
                return;

            if (ZeroRx.IsMainThread)
            {
                if(EnableColor)
                    Debug.LogErrorFormat(context, FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), string.Format(format, args));
                else
                    Debug.LogErrorFormat(context, format, args);
            }
            else
            {
                ZeroRx.RunAction(() =>
                {
                    if(EnableColor)
                        Debug.LogErrorFormat(context, FORMAT_COLOR, ColorUtility.ToHtmlStringRGB(color), string.Format(format, args));
                    else
                        Debug.LogErrorFormat(context, format, args);
                });
            }
        }

        #endregion


        #region Draw
        
        public static void DrawBox(Vector3 center, Vector3 size, Color color, float duration = 0.016f)
        {
            #if !UNITY_EDITOR
            return;
            #endif
            
            
            Vector3 half = size * 0.5f;

            // 8 đỉnh của box
            Vector3 p0 = center + new Vector3(-half.x, -half.y, -half.z);
            Vector3 p1 = center + new Vector3(-half.x, -half.y,  half.z);
            Vector3 p2 = center + new Vector3( half.x, -half.y,  half.z);
            Vector3 p3 = center + new Vector3( half.x, -half.y, -half.z);

            Vector3 p4 = center + new Vector3(-half.x,  half.y, -half.z);
            Vector3 p5 = center + new Vector3(-half.x,  half.y,  half.z);
            Vector3 p6 = center + new Vector3( half.x,  half.y,  half.z);
            Vector3 p7 = center + new Vector3( half.x,  half.y, -half.z);

            // đáy
            Debug.DrawLine(p0, p1, color, duration);
            Debug.DrawLine(p1, p2, color, duration);
            Debug.DrawLine(p2, p3, color, duration);
            Debug.DrawLine(p3, p0, color, duration);

            // đỉnh
            Debug.DrawLine(p4, p5, color, duration);
            Debug.DrawLine(p5, p6, color, duration);
            Debug.DrawLine(p6, p7, color, duration);
            Debug.DrawLine(p7, p4, color, duration);

            // cạnh đứng
            Debug.DrawLine(p0, p4, color, duration);
            Debug.DrawLine(p1, p5, color, duration);
            Debug.DrawLine(p2, p6, color, duration);
            Debug.DrawLine(p3, p7, color, duration);
        }


        public static void DrawPoint(Vector3 center, float size, Color color, float duration = 0.016f)
        {
#if !UNITY_EDITOR
            return;
#endif

            int numberDraw = Mathf.CeilToInt(size / 0.05f);
            float stepSize = size / numberDraw;
            for (int i = 0; i < numberDraw; i++)
            {
                float sizeSmooth = stepSize * (i + 1);
                DrawBox(center, new Vector3(sizeSmooth, sizeSmooth, sizeSmooth), color, duration);
            }
        }
        
        #endregion
    }
}