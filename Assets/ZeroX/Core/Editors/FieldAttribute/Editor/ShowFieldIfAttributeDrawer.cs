using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors
{
    [CustomPropertyDrawer(typeof(ShowFieldIfAttribute))]
    public class ShowFieldIfAttributeDrawer : PropertyDrawer
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
            var castedAttribute = attribute as ShowFieldIfAttribute;
            if (castedAttribute == null)
                return true;

            if (FieldAttributeEditorUtility.TryCheckCondition(property, castedAttribute.conditionFieldName, castedAttribute.conditionValue, out bool isMatchConditionValue))
                return isMatchConditionValue;

            return true;
        }
    }
}