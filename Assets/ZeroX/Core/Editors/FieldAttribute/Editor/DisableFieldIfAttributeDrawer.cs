using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors
{
    [CustomPropertyDrawer(typeof(DisableFieldIfAttribute))]
    public class DisableFieldIfAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool canDisable = CanDisable(property);
            if (canDisable)
            {
                bool oldEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = oldEnabled;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private bool CanDisable(SerializedProperty property)
        {
            var castedAttribute = attribute as DisableFieldIfAttribute;
            if (castedAttribute == null)
                return false;
            
            if (FieldAttributeEditorUtility.TryCheckCondition(property, castedAttribute.conditionFieldName, castedAttribute.conditionValue, out bool isMatchConditionValue))
                return isMatchConditionValue;

            return false;
        }
    }
}