using UnityEditor;
using UnityEngine;

namespace ZeroX.DashboardSystem
{
    public static class ZDBStyle
    {
        public static class Label
        {
            public static GUIStyle MiddleCenter_20 = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
            };
            
            public static GUIStyle MiddleCenter_Bold_20 = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            
        
            public static GUIStyle MiddleLeft_20 = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleLeft,
            };
            
            public static GUIStyle MiddleLeft_Bold_20 = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
            };
        
            public static GUIStyle MiddleLeft_15 = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleLeft,
            };
            
            public static GUIStyle MiddleLeft_Bold_15 = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
            };
            
            public static GUIStyle MiddleLeft_12 = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
            };

            public static GUIStyle RichText = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                richText = true
            };
        }


        public static GUIStyle SystemLine = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(0, 0, 0, 0)
        };


        public static GUIStyle Icon = new GUIStyle(GUIStyle.none)
        {
        };
        
        
        
        public static GUIStyle ButtonWithIcon = new GUIStyle(GUIStyle.none)
        {
        };
    }
}