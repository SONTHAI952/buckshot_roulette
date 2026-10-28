using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ZeroX.JsonSystem
{
    public static class ZJson_ObjectToNode
    {
        /// <summary>
        /// Hàm này sẽ không check serializable với objectType truyền vào, chỉ check với các childField của object truyền vào
        /// </summary>
        public static ZJsonNode ParseMaster(Type objType, object obj)
        {
            if (obj == null) //Được phép null vì các field của object có thể có giá trị null và json cũng chấp nhận giá trị null
                return null;

            
            if(obj is IZJsonCustomSerializable customSerializable)
                return customSerializable.ToJsonNode();
            
            
            if (objType.IsGenericType)
            {
                var genericTypeDefinition = objType.GetGenericTypeDefinition();
                
                if(genericTypeDefinition == ZJsonReflection.TYPE_DICTIONARY)
                    return ParseDict(objType, obj);
                
                if(genericTypeDefinition == ZJsonReflection.TYPE_LIST)
                    return ParseList(objType, obj);

                Debug.LogError("Not support generic type: " + genericTypeDefinition.Name);
                return null;
            }
            
            
            if(objType.IsArray)
                return ParseArray(objType, obj);
            
            
            if(ZJsonReflection.IsBuiltInObjectType(objType))
                return ParseBuiltInObject(objType, obj);
            
            
            if(ZJsonReflection.IsScalarType(objType))
                return ParseScalar(objType, obj);

            if (objType.IsEnum)
                return ParseScalarEnum(objType, obj);
            

            return ParseObject(objType, obj);
        }

        private static ZJsonObject ParseDict(Type objType, object dict)
        {
            ZJsonObject jsonObject = new ZJsonObject();
            bool keyIsEnum = objType.GenericTypeArguments[0].IsEnum;

            IDictionary iDict = (IDictionary)dict;
            foreach (DictionaryEntry entry in iDict)
            {
                string key = keyIsEnum
                    ? ((Enum)entry.Key).ToString("D")
                    : Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
                var value = entry.Value;
                if (value == null)
                {
                    jsonObject.AddNull(key);
                    continue;
                }
                
                var valueType = value.GetType();
                if (ZJsonReflection.CanTypeSerializable(valueType) == false)
                {
                    jsonObject.AddNull(key);
                    continue;
                }
                
                ZJsonNode valueNode = ParseMaster(valueType, value);
                jsonObject.Add(key, valueNode);
            }
            
            return jsonObject;
        }

        private static ZJsonList ParseList(Type objType, object list)
        {
            ZJsonList jsonList = new ZJsonList();
            
            IList iList = (IList)list;
            for (int i = 0; i < iList.Count; i++)
            {
                var value = iList[i];
                if (value == null)
                {
                    jsonList.AddNull();
                    continue;
                }
                
                var valueType = value.GetType();
                if (ZJsonReflection.CanTypeSerializable(valueType) == false)
                {
                    jsonList.AddNull();
                    continue;
                }
                
                ZJsonNode valueNode = ParseMaster(valueType, value);
                jsonList.Add(valueNode);
            }
            
            return jsonList;
        }
        
        private static ZJsonList ParseArray(Type objType, object array)
        {
            ZJsonList jsonList = new ZJsonList();
            
            Array iArray = (Array)array;
            foreach (object value in iArray)
            {
                if (value == null)
                {
                    jsonList.AddNull();
                    continue;
                }
                
                var valueType = value.GetType();
                if (ZJsonReflection.CanTypeSerializable(valueType) == false)
                {
                    jsonList.AddNull();
                    continue;
                }
                
                ZJsonNode valueNode = ParseMaster(valueType, value);
                jsonList.Add(valueNode);
            }
            
            return jsonList;
        }

        private static ZJsonScalar ParseScalar(Type objType, object scalar)
        {
            if(objType == ZJsonReflection.TYPE_BOOL)
                return new ZJsonScalar(ZJsonScalarType.Bool, (bool)scalar ? "true" : "false");
            
            
            if(objType == ZJsonReflection.TYPE_STRING)
                return new ZJsonScalar(ZJsonScalarType.String, scalar == null ? "" : (string)scalar);
            
            if(objType == ZJsonReflection.TYPE_CHAR)
                return new ZJsonScalar(ZJsonScalarType.String, scalar == null ? "" : scalar.ToString());
            
            //Number
            string s = Convert.ToString(scalar, CultureInfo.InvariantCulture);
            return new ZJsonScalar(ZJsonScalarType.Number, s);
        }

        private static ZJsonScalar ParseScalarEnum(Type objType, object scalar)
        {
            string s = ((Enum)scalar).ToString("D"); //"D" xử lý đúng mọi underlying type, kể cả ulong lớn
            return new ZJsonScalar(ZJsonScalarType.Number, s);
        }

        private static ZJsonObject ParseObject(Type objType, object obj)
        {
            ZJsonObject jsonObject = new ZJsonObject();


            var accessors = ZJsonReflection.GetAllFieldAccessor(objType);
            for (int i = 0; i < accessors.Length; i++)
            {
                var accessor = accessors[i];
                var fieldValue = accessor.getter(obj);
                var childNode = ParseMaster(accessor.fieldType, fieldValue);
                jsonObject.Add(accessor.name, childNode);
            }


            return jsonObject;
        }
        
        private static ZJsonObject ParseBuiltInObject(Type objType, object obj)
        {
            if (ZJsonBuiltInHandler.IsHandledType(objType))
                return ZJsonBuiltInHandler.ToNode(objType, obj);

            string json = JsonUtility.ToJson(obj);
            return (ZJsonObject)ZJson_StringToNode.Parse(json);
        }
    }
}