using System;
using System.Reflection;
using UnityEditor;
using ZeroX.Extensions;

namespace ZeroX.Editors
{
    public static class FieldAttributeEditorUtility
    {
        private static SerializedProperty FindConditionSp(SerializedProperty property, string conditionFieldName)
        {
            string parentPropertyPath = property.GetParentPropertyPath();
            
            if (string.IsNullOrEmpty(parentPropertyPath) == false)
            {
                var parentProperty = property.serializedObject.FindProperty(parentPropertyPath);
                return parentProperty.FindPropertyRelative(conditionFieldName);
            }
            else
            {
                return property.serializedObject.FindProperty(conditionFieldName);
            }
        }

        private static bool TryFindConditionValueViaReflection(SerializedProperty property, string conditionFieldName, out object value)
        {
            string parentPropertyPath = property.GetParentPropertyPath();
            object parentObject;
            
            if (string.IsNullOrEmpty(parentPropertyPath) == false)
            {
                var parentProperty = property.serializedObject.FindProperty(parentPropertyPath);
                parentObject = parentProperty.GetObject();
            }
            else
            {
                parentObject = property.serializedObject.targetObject;
            }


            if (parentObject == null)
            {
                value = null;
                return false;
            }
                

            
            var propertyInfo = parentObject.GetType().GetPropertyDeep(conditionFieldName);
            if (propertyInfo == null)
            {
                value = null;
                return false;
            }
            
            
            value = propertyInfo.GetValue(parentObject);
            return true;
        }

        private static bool TryFindConditionValue(SerializedProperty property, string conditionFieldName, out object value)
        {
            var conditionSp = FindConditionSp(property, conditionFieldName);
            if (conditionSp != null)
            {
                value = conditionSp.boxedValue;
                return true;
            }

            
            
            if (TryFindConditionValueViaReflection(property, conditionFieldName, out value))
            {
                return true;
            }

            
            
            return false;
        }

        private static bool EqualConditionValue(object value1, object value2)
        {
            if (value1 == null && value2 == null)
                return true;
            
            if (value1 == null)
                return false;
            
            if (value2 == null)
                return false;
            
            
            
            if (value1 is Enum)
            {
                value1 = Convert.ToInt32(value1);
            }
            
            if (value2 is Enum)
            {
                value2 = Convert.ToInt32(value2);
            }
            
            
            
            return value1.Equals(value2);
        }
        
        public static bool TryCheckCondition(SerializedProperty property, string conditionFieldName, object conditionValue, out bool isMatchConditionValue)
        {
            if (TryFindConditionValue(property, conditionFieldName, out var conditionValueCurrent) == false)
            {
                isMatchConditionValue = false;
                return false;
            }
            
            
            isMatchConditionValue = EqualConditionValue(conditionValueCurrent, conditionValue);
            return true;
        }
    }
}