using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.DashboardSystem
{
    public static class ZDBGUILayout
    {
        private const string SelectableLabelControlName = "ZeroX.Dashboard.SelectableLabel";
        
        
        public static void DrawHorizontalLine()
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(2), GUILayout.ExpandWidth(true));
            GUI.DrawTexture(rect, ZDBMedia.SystemLine, ScaleMode.StretchToFill);
        }
        
        public static void HorizontalSpacingWithLine(float spacing = 30)
        {
            GUILayout.Space(spacing/2);
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(2), GUILayout.ExpandWidth(true));
            GUI.DrawTexture(rect, ZDBMedia.SystemLine, ScaleMode.StretchToFill);
            GUILayout.Space(spacing/2);
        }

        public static void DrawIcon(Texture2D texture, float width, float height)
        {
            GUILayout.Box(texture, ZDBStyle.Icon, GUILayout.Height(height), GUILayout.Width(width));
        }
        
        public static void DrawIcon(Texture2D texture, float width, float height, float paddingTop)
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
            GUILayout.Space(paddingTop);
            GUILayout.Box(texture, ZDBStyle.Icon, GUILayout.Height(height), GUILayout.Width(width));
            GUILayout.EndVertical();
        }

        public static bool DrawButtonWithIcon(Texture2D texture, float width, float height)
        {
            return GUILayout.Button(texture, ZDBStyle.ButtonWithIcon, GUILayout.Height(height), GUILayout.Width(width));
        }
        
        public static bool DrawButtonWithIcon(Texture2D texture, float width, float height, float paddingTop)
        {
            GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.ExpandHeight(false));
            GUILayout.Space(paddingTop);
            bool isClicked = GUILayout.Button(texture, ZDBStyle.ButtonWithIcon, GUILayout.Height(height), GUILayout.Width(width));
            GUILayout.EndVertical();
            return isClicked;
        }

        public static void Label(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, EditorStyles.wordWrappedLabel, options);
        }

        public static void SelectableLabel(string text, params GUILayoutOption[] options)
        {
            GUI.SetNextControlName(SelectableLabelControlName);
            string newText = EditorGUILayout.TextArea(text, ZDBStyle.Label.RichText, options);
            
             if (GUI.GetNameOfFocusedControl() == SelectableLabelControlName)
             {
                 if(newText != text)
                    GUI.FocusControl(null);
             }
        }

        public static void SelectableLabelWithBG(string text, float padding, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Space(padding);
            
            GUILayout.BeginVertical();
            GUILayout.Space(padding);
            
            SelectableLabel(text);
            
            GUILayout.Space(padding);
            GUILayout.EndVertical();
            
            GUILayout.Space(padding);
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Thường cần chỉnh label height và label expand width
        /// </summary>
        /// <returns></returns>
        public static bool DrawToggle(string text, bool value, GUIStyle labelStyle, float iconSize, params GUILayoutOption[] labelOptions)
        {
            GUILayout.BeginHorizontal();
            
            if(labelOptions == null || labelOptions.Length == 0)
                GUILayout.Label(text, labelStyle, GUILayout.Height(15), GUILayout.ExpandWidth(true));
            else
                GUILayout.Label(text, labelStyle, labelOptions);
            
            //Draw Before Button Toggle
            GUILayout.Space(5);
            
            //Button Toggle
            var toggleIcon = value ? ZDBMedia.ToggleOn : ZDBMedia.ToggleOff;
            bool isClicked = DrawButtonWithIcon(toggleIcon, iconSize, iconSize);
            
            GUILayout.EndHorizontal();
            GUILayout.Space(-9);

            if(isClicked)
                value = !value;
            
            return value;
        }

        public static bool DrawToggleRequiredSdk(string text, bool value, Action drawBeforeButtonToggle = null)
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(text, ZDBStyle.Label.MiddleLeft_Bold_20, GUILayout.Height(40), GUILayout.ExpandWidth(true));
            
            //Draw Before Button Toggle
            drawBeforeButtonToggle?.Invoke();
            GUILayout.Space(5);
            
            //Button Toggle
            var toggleIcon = value ? ZDBMedia.ToggleOn : ZDBMedia.ToggleOff;
            bool isClicked = DrawButtonWithIcon(toggleIcon, 45, 45);
            
            GUILayout.EndHorizontal();
            GUILayout.Space(-9);

            if(isClicked)
                value = !value;
            
            return value;
        }
        
        public static bool DrawToggleOptionalSdk(string text, bool value, Action drawBeforeButtonToggle = null)
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(text, ZDBStyle.Label.MiddleLeft_15, GUILayout.Height(30), GUILayout.ExpandWidth(true));
            
            //Draw Before Button Toggle
            drawBeforeButtonToggle?.Invoke();
            GUILayout.Space(5);
            
            //Button Toggle
            var toggleIcon = value ? ZDBMedia.ToggleOn : ZDBMedia.ToggleOff;
            bool isClicked = DrawButtonWithIcon(toggleIcon, 35, 35);
            
            GUILayout.EndHorizontal();
            GUILayout.Space(-9);

            if(isClicked)
                value = !value;
            
            return value;
        }
        
        
        public static bool Button(string title, float height = 30)
        {
            return GUILayout.Button(title, GUILayout.Height(height));
        }

        public static void ButtonOpenUrl(string title, string url, float height = 30)
        {
            if (GUILayout.Button(title, GUILayout.Height(height)))
            {
                Application.OpenURL(url);
            }
        }

        public static void ReadOnlyObjectField(string label, Object obj, params GUILayoutOption[] options)
        {
            EditorGUILayout.ObjectField(label, obj, typeof(Object), false, options);
        }
        
        public static void ReadOnlyObjectFieldWithHelpBox(string label, Object obj, params GUILayoutOption[] options)
        {
            EditorGUILayout.ObjectField(label, obj, typeof(Object), false, options);
            
            if (obj == null)
                EditorGUILayout.HelpBox(label + " not exist.", MessageType.Error);
        }
    }
}