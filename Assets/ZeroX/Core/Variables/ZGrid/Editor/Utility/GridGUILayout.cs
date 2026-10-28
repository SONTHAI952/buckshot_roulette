using UnityEditor;
using UnityEngine;

namespace ZeroX.ZGridEditors
{
    public static class GridGUILayout
    {
        public static void Box(string label, params GUILayoutOption[] options)
        { 
            GUILayout.Box(label, GridEditorStyle.Box, options);
        }
        
        public static bool Button(string label, params GUILayoutOption[] options)
        {
            return GUILayout.Button(label, GridEditorStyle.Button, options);
        }
        
        public static bool Button(GUIContent label, params GUILayoutOption[] options)
        {
            return GUILayout.Button(label, GridEditorStyle.Button, options);
        }
        
        public static bool Button(string label, Color bgColor, params GUILayoutOption[] options)
        {
            Color oldBgColor = GUI.color;
            GUI.color = bgColor;
            bool isClicked = GUILayout.Button(label, GridEditorStyle.Button, options);
            GUI.color = oldBgColor;
            return isClicked;
        }

        public static void BeginHorizontalBG(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(GridEditorStyle.Background, options);
        }

        public static void EndHorizontalBG()
        {
            GUILayout.EndHorizontal();
        }
        
        public static void BeginVerticalBG(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(GridEditorStyle.Background, options);
        }

        public static void EndVerticalBG()
        {
            GUILayout.EndVertical();
        }

        public static Vector2 BeginScrollViewNoScrollBar(Vector2 scrollPos, params GUILayoutOption[] options)
        {
            return EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUIStyle.none, options);
        }

        public static void EndScrollViewNoScrollBar()
        {
            EditorGUILayout.EndScrollView();
        }
    }
}