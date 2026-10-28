using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors
{
    [InitializeOnLoad]
    public static class TimeScaleToolbar
    {
        private const string Save_Key_MinScale = "ZeroX.TimeScaleToolbar.MinScale";
        private const string Save_Key_MaxScale = "ZeroX.TimeScaleToolbar.MaxScale";
        private const float Element_Height = 21;
        
        private static GUIStyle NumberFieldAlignMiddle;
        private static GUIStyle LabelFieldAlignMiddle;
        
        
        
        
        //Property
        private static float MinScale
        {
            get => EditorPrefs.GetFloat(Save_Key_MinScale, 0);
            set => EditorPrefs.SetFloat(Save_Key_MinScale, value);
        }
        
        private static float MaxScale
        {
            get => EditorPrefs.GetFloat(Save_Key_MaxScale, 2);
            set => EditorPrefs.SetFloat(Save_Key_MaxScale, value);
        }
        
        
        
        static TimeScaleToolbar()
        {
            ToolbarGUILayout.RightToolbarGUI_AlignRight.Add(OnGUI);
        }

        private static void OnGUI()
        {
            if(EditorApplication.isPlaying == false)
                return;
            
            InitGUIStyleIfNot();
            
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(Element_Height));
            GUILayout.BeginVertical();
            GUILayout.Space(-1);
            
            GUILayout.BeginHorizontal();
            DrawControl();
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(15);
        }

        private static void DrawControl()
        {
            GUILayout.Label("TimeScale", GUILayout.Height(Element_Height));
            MinScale = EditorGUILayout.FloatField(MinScale, NumberFieldAlignMiddle, GUILayout.Width(25), GUILayout.Height(Element_Height));
            
            
            float newScale = EditorGUILayout.Slider(Time.timeScale, MinScale, MaxScale, GUILayout.Width(150), GUILayout.Height(Element_Height));
            
            //Snap
            if (Mathf.Approximately(newScale, Time.timeScale) == false && Event.current.control)
            {
                newScale = Mathf.RoundToInt(newScale / 0.1f) * 0.1f;
            }
            Time.timeScale = newScale;
            
            
            
            MaxScale = EditorGUILayout.FloatField(MaxScale, NumberFieldAlignMiddle, GUILayout.Width(25), GUILayout.Height(Element_Height));
        }

        private static void InitGUIStyleIfNot()
        {
            if (NumberFieldAlignMiddle == null)
            {
                NumberFieldAlignMiddle = new GUIStyle(EditorStyles.numberField)
                {
                    alignment = TextAnchor.MiddleCenter,
                };
            }

            if (LabelFieldAlignMiddle == null)
            {
                LabelFieldAlignMiddle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                };
            }
        }
    }
}