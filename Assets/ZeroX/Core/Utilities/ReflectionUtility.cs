using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ZeroX
{
    public static class ReflectionUtility
    {
        public const BindingFlags bindingFlags_ForGetAllField = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        
        public static readonly Type TYPE_OBJECT = typeof(object);
        public static readonly Type TYPE_VALUE_TYPE = typeof(ValueType);
        
        private static readonly Type TYPE_SERIALIZE_FIELD = typeof(SerializeField);
        private static readonly Type TYPE_NON_SERIALIZED = typeof(NonSerializedAttribute);


        //Temp
        private static Dictionary<FieldInfo, bool> dictSerializeField = new Dictionary<FieldInfo, bool>();
        private static Dictionary<FieldInfo, bool> dictNonSerialized = new Dictionary<FieldInfo, bool>();
        
        
        
        
        
        public static bool IsDefineAttribute_SerializeField(FieldInfo fieldInfo)
        {
            if (dictSerializeField.TryGetValue(fieldInfo, out bool isDefine))
                return isDefine;

            isDefine = Attribute.IsDefined(fieldInfo, TYPE_SERIALIZE_FIELD);
            dictSerializeField.Add(fieldInfo, isDefine);
            return isDefine;
        }
        
        public static bool IsDefineAttribute_NonSerialized(FieldInfo fieldInfo)
        {
            if (dictNonSerialized.TryGetValue(fieldInfo, out bool isDefine))
                return isDefine;

            isDefine = Attribute.IsDefined(fieldInfo, TYPE_NON_SERIALIZED);
            dictNonSerialized.Add(fieldInfo, isDefine);
            return isDefine;
        }
        
        

        public static IEnumerable<FieldInfo> GetAllFieldInfoCanSave(Type type)
        {
            while (type != null && type != TYPE_OBJECT && type != TYPE_VALUE_TYPE)
            {
                var fieldInfos = type.GetFields(bindingFlags_ForGetAllField);
                foreach (var fieldInfo in fieldInfos)
                {
                    if (fieldInfo.IsPublic)
                    {
                        if(IsDefineAttribute_NonSerialized(fieldInfo))
                            continue;
                    }
                    else
                    {
                        if(IsDefineAttribute_SerializeField(fieldInfo) == false)
                            continue;
                    }
                    
                    yield return fieldInfo;
                }

                type = type.BaseType;
            }
        }
    }
}