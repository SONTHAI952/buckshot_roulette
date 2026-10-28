using UnityEditor;
using UnityEngine;

namespace ZeroX.ZGridEditors
{
    public static class GridEditorStyle
    {
        public const int BgPadding = 3;
            
            
        public static GUIStyle BigTitleMiddle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
        };
            
        public static GUIStyle Background = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(BgPadding, BgPadding, BgPadding, BgPadding),
            margin = new RectOffset(0, 0, 0, 0),
        };
            
        public static GUIStyle Button = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(0, 0, 0, 0)
        };
        
        public static GUIStyle Box = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0)
        };

        public static GUIStyle MiddleLabel = new GUIStyle(EditorStyles.label)
        {
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            alignment = TextAnchor.MiddleCenter,
        };
        
        public static GUIStyle BoxLabelSelectedToOrder = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 15
        };
    }
}