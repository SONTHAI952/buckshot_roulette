using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors
{
    [CustomPropertyDrawer(typeof(EnableFieldIfAttribute))]
    public class EnableFieldIfAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool canEnable = CanEnable(property);
            if (canEnable)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                bool oldEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = oldEnabled;
            }
        }

        private bool CanEnable(SerializedProperty property)
        {
            var castedAttribute = attribute as EnableFieldIfAttribute;
            if (castedAttribute == null)
                return true;
            
            if (FieldAttributeEditorUtility.TryCheckCondition(property, castedAttribute.conditionFieldName, castedAttribute.conditionValue, out bool isMatchConditionValue))
                return isMatchConditionValue;

            return true;
        }
    }
}