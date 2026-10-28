using System;
using UnityEngine;

namespace ZeroX
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HideFieldIfAttribute : PropertyAttribute
    {
        public string conditionFieldName;
        public object conditionValue;
        
        
        public HideFieldIfAttribute(string condition, object conditionValue)
        {
            this.conditionFieldName = condition;
            this.conditionValue = conditionValue;
        }
        
        public HideFieldIfAttribute(string condition)
        {
            this.conditionFieldName = condition;
            this.conditionValue = true;
        }
    }
}