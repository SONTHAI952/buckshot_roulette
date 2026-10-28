using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors
{
    [CustomPropertyDrawer(typeof(HideFieldIfAttribute))]
    public class HideFieldIfAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool canHide = CanHide(property);

            if (canHide)
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool canHide = CanHide(property);
            if (canHide == false)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private bool CanHide(SerializedProperty property)
        {
            var castedAttribute = attribute as HideFieldIfAttribute;
            if (castedAttribute == null)
                return false;
            
            if (FieldAttributeEditorUtility.TryCheckCondition(property, castedAttribute.conditionFieldName, castedAttribute.conditionValue, out bool isMatchConditionValue))
                return isMatchConditionValue;

            return false;
        }
    }
}