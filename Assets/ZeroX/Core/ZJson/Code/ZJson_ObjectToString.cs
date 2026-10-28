using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace ZeroX.JsonSystem
{
    public static class ZJson_ObjectToString
    {
        /// <summary>
        /// Hàm này không nhận obj null, vì các thành phần con đã tự xử lý case obj null rồi
        /// </summary>
        public static void ParseMaster(StringBuilder builder, Type objType, object obj)
        {
            if (obj is IZJsonCustomSerializable customSerializable)
            {
                var jsonNode = customSerializable.ToJsonNode();
                builder.Append(jsonNode == null ? "null" : jsonNode.ToJson());
                return;
            }
            
            
            if (objType.IsGenericType)
            {
                var genericTypeDefinition = objType.GetGenericTypeDefinition();

                if (genericTypeDefinition == ZJsonReflection.TYPE_DICTIONARY)
                {
                    ParseDict(builder, obj);
                    return;
                }


                if (genericTypeDefinition == ZJsonReflection.TYPE_LIST)
                {
                    ParseList(builder, obj);
                    return;
                }
                    

                Debug.LogError("Not support generic type: " + genericTypeDefinition.Name);
                builder.Append("null");
                return;
            }


            if (objType.IsArray)
            {
                ParseArray(builder, obj);
                return;
            }



            if (ZJsonReflection.IsBuiltInObjectType(objType))
            {
                if (ZJsonBuiltInHandler.IsHandledType(objType))
                {
                    ZJsonBuiltInHandler.AppendToJson(builder, objType, obj);
                    return;
                }

                builder.Append(JsonUtility.ToJson(obj));
                return;
            }



            if (ZJsonReflection.IsScalarType(objType))
            {
                ParseScalar(builder, objType, obj);
                return;
            }


            if (objType.IsEnum)
            {
                builder.Append(((Enum)obj).ToString("D"));
                return;
            }

            
#if UNITY_EDITOR
            if (ZJsonReflection.IsParseToGUID(objType))
            {
                ParseToGUID(builder, obj);
                return;
            }
#endif
                
            

            ParseObject(builder, objType, obj);
        }

        private static void ParseDict(StringBuilder builder, object dict)
        {
            IDictionary iDict = (IDictionary)dict;
            Type keyType = dict.GetType().GenericTypeArguments[0];
            bool keyIsString = keyType == ZJsonReflection.TYPE_STRING;
            bool keyIsEnum = keyType.IsEnum;

            builder.Append("{");

            bool isFirstChild = true;
            foreach (DictionaryEntry entry in iDict)
            {
                if(isFirstChild == false)
                    builder.Append(",");
                else
                    isFirstChild = false;


                //Key: chỉ key string mới có thể chứa ký tự cần escape
                if (keyIsString)
                {
                    ZJsonHelper.AppendStringForJson(builder, (string)entry.Key);
                }
                else
                {
                    builder.Append('"');
                    if (keyIsEnum)
                        builder.Append(((Enum)entry.Key).ToString("D"));
                    else
                        builder.Append(Convert.ToString(entry.Key, CultureInfo.InvariantCulture));
                    builder.Append('"');
                }
                builder.Append(':');
                
                
                //Value
                var value = entry.Value;
                if (value == null)
                {
                    builder.Append("null");
                    continue;
                }
                
                var valueType = value.GetType();
                if (ZJsonReflection.CanTypeSerializable(valueType) == false)
                {
                    builder.Append("null");
                    continue;
                }

                ParseMaster(builder, valueType, value);
            }
            
            builder.Append("}");
        }
        
        private static void ParseList(StringBuilder builder, object list)
        {
            IList iList = (IList)list;
            
            builder.Append('[');
            
            //First Phase
            int firstPhaseCount = iList.Count - 1;
            for (int i = 0; i < firstPhaseCount; i++)
            {
                var value = iList[i];
                if (value == null)
                {
                    builder.Append("null");
                }
                else
                {
                    var valueType = value.GetType();
                    if (ZJsonReflection.CanTypeSerializable(valueType) == false)
                    {
                        builder.Append("null");
                    }
                    else
                    {
                        ParseMaster(builder, valueType, value);
                    }
                }
                
                builder.Append(",");
            }
            
            
            //Last Phase
            //Phần tử cuối
            if (iList.Count > 0)
            {
                var value = iList[iList.Count - 1];
                if (value == null)
                {
                    builder.Append("null");
                }
                else
                {
                    var valueType = value.GetType();
                    if (ZJsonReflection.CanTypeSerializable(valueType) == false)
                    {
                        builder.Append("null");
                    }
                    else
                    {
                        ParseMaster(builder, valueType, value);
                    }
                }
            }
            
            
            
            builder.Append(']');
        }
        
        private static void ParseArray(StringBuilder builder, object array)
        {
            Array iArray = (Array)array;
            
            builder.Append('[');
            
            
            bool isFirstChild = true;
            foreach (var value in iArray)
            {
                if(isFirstChild == false)
                    builder.Append(",");
                else
                    isFirstChild = false;
                
                
                
                if (value == null)
                {
                    builder.Append("null");
                }
                else
                {
                    var valueType = value.GetType();
                    if (ZJsonReflection.CanTypeSerializable(valueType) == false)
                    {
                        builder.Append("null");
                    }
                    else
                    {
                        ParseMaster(builder, valueType, value);
                    }
                }
            }
            
            
            builder.Append(']');
        }
        
        private static void ParseScalar(StringBuilder builder, Type objType, object scalar)
        {
            if (objType == ZJsonReflection.TYPE_BOOL)
            {
                builder.Append((bool)scalar ? "true" : "false");
                return;
            }



            if (objType == ZJsonReflection.TYPE_STRING)
            {
                ZJsonHelper.AppendStringForJson(builder, (string)scalar);
                return;
            }

            if (objType == ZJsonReflection.TYPE_CHAR)
            {
                ZJsonHelper.AppendStringForJson(builder, ((char)scalar).ToString());
                return;
            }


            //Number: append trực tiếp qua TryFormat, không alloc chuỗi trung gian
            switch (Type.GetTypeCode(objType))
            {
                case TypeCode.Int32: ZJsonHelper.AppendInvariant(builder, (int)scalar); return;
                case TypeCode.Single: ZJsonHelper.AppendInvariant(builder, (float)scalar); return;
                case TypeCode.Int64: ZJsonHelper.AppendInvariant(builder, (long)scalar); return;
                case TypeCode.Double: ZJsonHelper.AppendInvariant(builder, (double)scalar); return;
                case TypeCode.UInt32: ZJsonHelper.AppendInvariant(builder, (uint)scalar); return;
                case TypeCode.UInt64: ZJsonHelper.AppendInvariant(builder, (ulong)scalar); return;
                case TypeCode.Int16: ZJsonHelper.AppendInvariant(builder, (short)scalar); return;
                case TypeCode.UInt16: ZJsonHelper.AppendInvariant(builder, (ushort)scalar); return;
                case TypeCode.Byte: ZJsonHelper.AppendInvariant(builder, (byte)scalar); return;
                case TypeCode.SByte: ZJsonHelper.AppendInvariant(builder, (sbyte)scalar); return;
                case TypeCode.Decimal: ZJsonHelper.AppendInvariant(builder, (decimal)scalar); return;
            }

            //nint/nuint (TypeCode.Object)
            if (objType == ZJsonReflection.TYPE_NINT)
            {
                ZJsonHelper.AppendInvariant(builder, (long)(nint)scalar);
                return;
            }

            if (objType == ZJsonReflection.TYPE_NUINT)
            {
                ZJsonHelper.AppendInvariant(builder, (ulong)(nuint)scalar);
                return;
            }

            //Fallback
            builder.Append(Convert.ToString(scalar, CultureInfo.InvariantCulture));
        }

#if UNITY_EDITOR
        private static void ParseToGUID(StringBuilder builder, object obj)
        {
            UnityEngine.Object unityObject = (UnityEngine.Object)obj;
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(unityObject);
            if (string.IsNullOrEmpty(assetPath))
            {
                builder.Append("null");
                return;
            }
                
            string guid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
            builder.Append('"');
            builder.Append(guid);
            builder.Append('"');
        }
#endif
        
        private static void ParseObject(StringBuilder builder, Type objType, object obj)
        {
            var accessors = ZJsonReflection.GetAllFieldAccessor(objType);

            builder.Append("{");

            for (int i = 0; i < accessors.Length; i++)
            {
                if(i != 0)
                    builder.Append(",");

                var accessor = accessors[i];


                //Key: tên field là identifier nên không cần escape
                builder.Append('"');
                builder.Append(accessor.name);
                builder.Append("\":");


                //Value
                var fieldValue = accessor.getter(obj);
                if (fieldValue == null)
                {
                    builder.Append("null");
                }
                else
                {
                    ParseMaster(builder, accessor.fieldType, fieldValue);
                }
            }


            builder.Append("}");
        }
    }
}