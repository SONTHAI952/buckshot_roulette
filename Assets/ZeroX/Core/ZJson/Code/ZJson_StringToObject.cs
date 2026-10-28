using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ZeroX.JsonSystem
{
    public static class ZJson_StringToObject
    {
        /// <summary>
        /// Nếu json null, "", "null" thì sẽ trả về null
        /// </summary>
        public static object ParseStringOverwriteObject(string json, Type objType, object objToOverwrite)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            
            try
            {
                ZReader reader = new ZReader(json);
                reader.MoveToNonSpace();
                return ParseMaster(reader, objType, objToOverwrite);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception(ZJsonConst.EM_IncorrectJsonStringFormat);
            }
        }


        private static object ParseMaster(ZReader reader, Type objType, object objToOverwrite)
        {
            char c = reader.json[reader.index];
            
            if(c == '{')
                return ParseObject(reader, objType, objToOverwrite);

            if (c == '[')
                return ParseList(reader, objType, objToOverwrite);

            if (reader.IsCurrentAtNull())
                return ParseNull(reader);
                
            
#if UNITY_EDITOR
            if (ZJsonReflection.IsParseToGUID(objType))
                return ParseGUID(reader, objType, objToOverwrite);
#endif
            
            //Một trường hợp đặc biệt
            if (c == '"')
            {
                if (objType != ZJsonReflection.TYPE_STRING && objType != ZJsonReflection.TYPE_CHAR)
                    return ParseEmptyStringToObjectNull(reader);
            }

            return ParseScalar(reader, objType, objToOverwrite);
        }


        #region Object

        /// <summary>
        /// Index phải đang ở vị trí của {
        ///<para>Khi kết thúc hàm thì index đang ở vị trí của }</para>
        /// </summary>
        private static object ParseObject(ZReader reader, Type objType, object objToOverwrite)
        {
            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == ZJsonReflection.TYPE_DICTIONARY)
                return ParseObject_Dictionary(reader, objType, objToOverwrite);
                
            if(ZJsonReflection.IsBuiltInObjectType(objType))
                return ParseObject_BuiltInObject(reader, objType, objToOverwrite);

            //Shape mismatch: json object không thể vào scalar/enum/list/array -> bỏ qua an toàn, trả null
            if (ZJsonReflection.IsScalarType(objType) || objType.IsEnum || objType.IsArray ||
                (objType.IsGenericType && objType.GetGenericTypeDefinition() == ZJsonReflection.TYPE_LIST))
            {
                reader.SkipValue();
                return null;
            }

            //Sau này có thể kiểm tra custom parse tại đây

            return ParseObject_LiteralObject(reader, objType, objToOverwrite);
        }
        
        private static object ParseObject_Dictionary(ZReader reader, Type dictType, object dict)
        {
            if (dict == null)
                dict = ZJsonReflection.CreateInstance(dictType);

            Type keyType = dictType.GenericTypeArguments[0];
            bool keyIsString = keyType == ZJsonReflection.TYPE_STRING;
            bool keyIsEnum = keyType.IsEnum;
            Type elementType = dictType.GenericTypeArguments[1];
            bool elementTypeCanSerializable = ZJsonReflection.CanTypeSerializable(elementType);
            bool elementIsValueType = elementType.IsValueType;
            IDictionary iDict = (IDictionary)dict;
            iDict.Clear();
            
            
            
            reader.index++; //Move qua {
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;
                
                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot(); //Move tới sau dấu :
                reader.MoveToNonSpace(); //Move tới ký tự đầu tiên không space



                
                
                #region Xử lý child

                if (elementTypeCanSerializable)
                {
                    var element = ParseMaster(reader, elementType, null);

                    if (element == null && elementIsValueType)
                        throw new Exception("Json null for value type " + elementType.Name + ", key: " + key);

                    if (keyIsString)
                        iDict.Add(key, element);
                    else if (keyIsEnum)
                        iDict.Add(Enum.Parse(keyType, key), element); //Enum.Parse nhận cả tên lẫn số
                    else
                        iDict.Add(Convert.ChangeType(key, keyType, CultureInfo.InvariantCulture), element);
                }
                else
                {
                    reader.SkipValue();
                }
                
                #endregion
                
                
                
                
                
                reader.index++; //Move qua ký tự cuối của đoạn data của childNode.
            }

            return dict;
        }

        private static object ParseObject_BuiltInObject(ZReader reader, Type objType, object obj)
        {
            //Lưu ý rằng có một số type như AnimationCurve có thể Serializable bởi Unity để hiển thị trong Inspector nhưng không thể Convert sang json nên kết quả là object đó sẽ bị trống

            if (ZJsonBuiltInHandler.IsHandledType(objType))
                return ZJsonBuiltInHandler.FromReader(objType, reader); //Đọc trực tiếp, không cắt substring và không qua node trung gian

            string json = reader.ReadNextObjectValue();

            if (obj == null)
                return JsonUtility.FromJson(json, objType);

            JsonUtility.FromJsonOverwrite(json, obj);
            return obj;
        }


        /// <summary>
        /// Index phải đang ở vị trí của {
        ///<para>Khi kết thúc hàm thì index đang ở vị trí của }</para>
        /// </summary>
        private static object ParseObject_LiteralObject(ZReader reader, Type objType, object obj)
        {
            if (obj == null)
                obj = ZJsonReflection.CreateInstance(objType);



            if (obj is IZJsonCustomSerializable customSerializable)
            {
                string objectDataJson = reader.ReadNextObjectValue();
                ZJsonNode objectNode = ZJson_StringToNode.Parse(objectDataJson);
                customSerializable.FromJsonNode(objectNode);
                return obj;
            }



            reader.index++; //Move qua {
            while (true)
            {
                //Đọc key theo bounds và so tên field trực tiếp trên json, không cắt chuỗi
                if (reader.ReadNextKeyBounds(out int keyStart, out int keyLength) == false)
                    break;

                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot(); //Move tới sau dấu :
                reader.MoveToNonSpace(); //Move tới ký tự đầu tiên không space





                #region Xử lý child

                var accessor = ZJsonReflection.MatchFieldAccessor(objType, reader.json, keyStart, keyLength);
                if (accessor != null)
                {
                    var childObject = accessor.getter(obj);
                    var childObjectNew = ParseMaster(reader, accessor.fieldType, childObject);

                    //Là kiểu giá trị hoặc không giống nhau thì cần set lại value
                    if(accessor.fieldIsValueType || ReferenceEquals(childObject, childObjectNew) == false)
                        accessor.setter(obj, childObjectNew);
                }
                else
                {
                    reader.SkipValue();
                }

                #endregion





                reader.index++; //Move qua ký tự cuối của đoạn data của childNode.
            }

            return obj;
        }
        
        

        #endregion
        
        
        
        #region List

        /// <summary>
        /// Index phải đang ở vị trí của [
        ///<para>Khi kết thúc hàm thì index đang ở vị trí của ]</para>
        /// </summary>
        private static object ParseList(ZReader reader, Type objType, object objToOverwrite)
        {
            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == ZJsonReflection.TYPE_LIST)
                return ParseList_List(reader, objType, objToOverwrite);
            
            if (objType.IsArray)
                return ParseList_Array(reader, objType, objToOverwrite);

            //Shape mismatch: json list chỉ vào được List/Array -> bỏ qua an toàn, trả null
            reader.SkipValue();
            return null;
        }

        private static object ParseList_List(ZReader reader, Type listType, object list)
        {
             if (list == null)
                 list = ZJsonReflection.CreateInstance(listType);
             
             
             Type elementType = listType.GenericTypeArguments[0];
             bool elementTypeCanSerializable = ZJsonReflection.CanTypeSerializable(elementType);
             bool elementIsValueType = elementType.IsValueType;
             IList iList = (IList)list;
             iList.Clear();
             

             reader.index++; //Move qua [
             while (true)
             {
                 reader.MoveToNonSpace();
                 if(reader.json[reader.index] == ']')
                     break;
                 
                 
                 
                 #region Xử lý child
                 if (elementTypeCanSerializable)
                 {
                     var element = ParseMaster(reader, elementType, null);

                     if (element == null && elementIsValueType)
                         throw new Exception("Json null for value type " + elementType.Name + ", index: " + iList.Count);

                     iList.Add(element);
                 }
                 else
                 {
                     reader.SkipValue();
                 }
                 #endregion
                 
                 
                 
                 reader.index++; //Move qua ký tự cuối của đoạn data của childNode.
                 reader.MoveTo_AfterComma_Or_RightBracket();
             }

             return list;
        }
        
        private static object ParseList_Array(ZReader reader, Type arrayType, object array)
        {
            var elementType = arrayType.GetElementType();
            bool elementTypeCanSerializable = ZJsonReflection.CanTypeSerializable(elementType);
            bool elementIsValueType = elementType.IsValueType;
            List<object> listTemp = new List<object>();
             

            reader.index++; //Move qua [
            while (true)
            {
                reader.MoveToNonSpace();
                if(reader.json[reader.index] == ']')
                    break;
                 
                 
                 
                #region Xử lý child
                if (elementTypeCanSerializable)
                {
                    var element = ParseMaster(reader, elementType, null);

                    if (element == null && elementIsValueType)
                        throw new Exception("Json null for value type " + elementType.Name + ", index: " + listTemp.Count);

                    listTemp.Add(element);
                }
                else
                {
                    reader.SkipValue();
                }
                #endregion
                 
                 
                 
                reader.index++; //Move qua ký tự cuối của đoạn data của childNode.
                reader.MoveTo_AfterComma_Or_RightBracket();
            }

            
            //Chuyển đổi sang array
            Array iArray = array == null ? null : (Array)array;
            if(iArray == null || iArray.Length != listTemp.Count)
                iArray = Array.CreateInstance(elementType, listTemp.Count);
            
            for (int i = 0; i < listTemp.Count; i++)
            {
                iArray.SetValue(listTemp[i], i);
            }
            return iArray;
        }
        #endregion



        #region Scalar

        /// <summary>
        /// Index phải đang ở vị trí đầu tiên non-space.
        /// <para>Dừng tại ký tự cuối của đoạn scalar</para>
        /// </summary>
        private static object ParseScalar(ZReader reader, Type objType, object objToOverwrite)
        {
            //Shape mismatch: value dạng scalar nhưng type đích không phải scalar/enum -> bỏ qua an toàn, trả null
            if (ZJsonReflection.IsScalarType(objType) == false && objType.IsEnum == false)
            {
                reader.SkipValue();
                return null;
            }

            //Number thường trả bounds (value == null), không cắt chuỗi
            reader.ReadNextScalarValueBounds(out ZJsonScalarType scalarType, out string value, out int numberStart, out int numberLength);


            if (objType.IsEnum)
                return ParseScalar_Enum(reader.json, objType, value, numberStart, numberLength);


            if (value == null)
                return ParseScalar_NumberBounds(reader.json, objType, numberStart, numberLength);


            //Còn lại: String, Bool, hoặc number đặc biệt (NaN, Infinity)
            if (objType == ZJsonReflection.TYPE_STRING)
                return value;

            if (objType == ZJsonReflection.TYPE_BOOL)
                return bool.Parse(value);

            if (objType == ZJsonReflection.TYPE_CHAR)
            {
                if (value.Length != 1)
                    throw new FormatException(ZJsonConst.EM_IncorrectJsonStringFormat);
                return value[0];
            }

            if (objType == ZJsonReflection.TYPE_FLOAT)
                return float.Parse(value, CultureInfo.InvariantCulture); //NaN, Infinity

            if (objType == ZJsonReflection.TYPE_DOUBLE)
                return double.Parse(value, CultureInfo.InvariantCulture); //NaN, Infinity

            return Convert.ChangeType(value, objType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse number trực tiếp trên vùng json bằng Span, không alloc chuỗi
        /// </summary>
        private static object ParseScalar_NumberBounds(string json, Type objType, int start, int length)
        {
            var span = json.AsSpan(start, length);

            switch (Type.GetTypeCode(objType))
            {
                case TypeCode.Int32: return int.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
                case TypeCode.Single: return float.Parse(span, NumberStyles.Float, CultureInfo.InvariantCulture);
                case TypeCode.Int64: return long.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
                case TypeCode.Double: return double.Parse(span, NumberStyles.Float, CultureInfo.InvariantCulture);
                case TypeCode.UInt32: return uint.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
                case TypeCode.UInt64: return ulong.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
                case TypeCode.Int16: return short.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
                case TypeCode.UInt16: return ushort.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
                case TypeCode.Byte: return byte.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
                case TypeCode.SByte: return sbyte.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
                case TypeCode.Decimal: return decimal.Parse(span, NumberStyles.Number, CultureInfo.InvariantCulture);

                case TypeCode.String: return json.Substring(start, length); //Number vào field string: coercion như cũ

                case TypeCode.Char:
                {
                    if (length != 1)
                        throw new FormatException(ZJsonConst.EM_IncorrectJsonStringFormat);
                    return json[start];
                }
            }

            //nint/nuint (TypeCode.Object): Convert.ChangeType không hỗ trợ IntPtr
            if (objType == ZJsonReflection.TYPE_NINT)
            {
                if (IntPtr.Size == 4)
                    return (nint)int.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);

                return (nint)long.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            if (objType == ZJsonReflection.TYPE_NUINT)
            {
                if (IntPtr.Size == 4)
                    return (nuint)uint.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);

                return (nuint)ulong.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            //Fallback (bool nhận number...): giữ hành vi throw của ChangeType
            return Convert.ChangeType(json.Substring(start, length), objType, CultureInfo.InvariantCulture);
        }

        private static object ParseScalar_Enum(string json, Type objType, string value, int numberStart, int numberLength)
        {
            var underlyingType = Enum.GetUnderlyingType(objType);

            if (value == null)
            {
                var span = json.AsSpan(numberStart, numberLength);

                if (underlyingType == typeof(ulong))
                    return Enum.ToObject(objType, ulong.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture));

                return Enum.ToObject(objType, long.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture));
            }

            var underlyingValue = Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
            return Enum.ToObject(objType, underlyingValue);
        }

        #endregion


        
        #region GUID

#if UNITY_EDITOR
        private static object ParseGUID(ZReader reader, Type objType, object objToOverwrite)
        {
            reader.ReadNextScalarValue(out ZJsonScalarType scalarType, out string scalarValue);

            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(scalarValue);
            return UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, objType);
        }
#endif

        #endregion
        
        
        
        /// <summary>
        /// Dừng tại ký tự cuối của null
        /// </summary>
        private static object ParseNull(ZReader reader)
        {
            reader.index += 3;
            return null;
        }

        /// <summary>
        /// Skip trọn chuỗi quoted (escape-aware) và trả về null. Dùng khi value là chuỗi nhưng type đích không phải string/char
        /// <para>Khi kết thúc, index ở vị trí dấu " đóng chuỗi</para>
        /// </summary>
        private static object ParseEmptyStringToObjectNull(ZReader reader)
        {
            reader.SkipValue();
            return null;
        }
    }
}