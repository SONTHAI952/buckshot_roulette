using System;
using UnityEngine;

namespace ZeroX
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowFieldIfAttribute : PropertyAttribute
    {
        public string conditionFieldName;
        public object conditionValue;
        
        
        public ShowFieldIfAttribute(string condition, object conditionValue)
        {
            this.conditionFieldName = condition;
            this.conditionValue = conditionValue;
        }
        
        public ShowFieldIfAttribute(string condition)
        {
            this.conditionFieldName = condition;
            this.conditionValue = true;
        }
    }
}