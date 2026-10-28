using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors
{
    [CustomPropertyDrawer(typeof(ShowFieldIfRunningAttribute))]
    public class ShowFieldIfRunningAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool canShow = CanShow(property);

            if (canShow)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool canShow = CanShow(property);
            if (canShow)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private bool CanShow(SerializedProperty property)
        {
            return EditorApplication.isPlaying;
        }
    }
}