using System;
using UnityEngine;

namespace ZeroX
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DisableFieldIfAttribute : PropertyAttribute
    {
        public string conditionFieldName;
        public object conditionValue;
        
        
        public DisableFieldIfAttribute(string condition, object conditionValue)
        {
            this.conditionFieldName = condition;
            this.conditionValue = conditionValue;
        }
        
        public DisableFieldIfAttribute(string condition)
        {
            this.conditionFieldName = condition;
            this.conditionValue = true;
        }
    }
}