using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using ZeroX.Extensions;

namespace ZeroX.JsonSystem
{
    public sealed class ZJsonObject : ZJsonNode, IEnumerable<ZJsonNode>
    {
        private Dictionary<string, ZJsonNode> dictChild = new Dictionary<string, ZJsonNode>();
        
        
        
        
        public override ZJsonNodeType NodeType => ZJsonNodeType.Object;
        
        public override int ChildCount => dictChild.Count;
        
        public IEnumerable<string> Keys => dictChild.Keys;
        public IEnumerable<KeyValuePair<string, ZJsonNode>> KeyValues => dictChild;
        
        
        
        #region Constructor

        public ZJsonObject()
        {
        }
        
        #endregion



        #region Access

        public override ZJsonNode this[string key]
        {
            get => dictChild[key];
            set => dictChild[key] = value;
        }
        
        public override ZJsonNode this[int index]
        {
            get => throw new Exception(ZJsonConst.EM_JsonNodeIsNotList);
            set => throw new Exception(ZJsonConst.EM_JsonNodeIsNotList);
        }

        #endregion
        
        

        public bool HasKey(string key)
        {
            return dictChild.ContainsKey(key);
        }

        public bool TryGetChild(string key, out ZJsonNode childNode)
        {
            if (dictChild.TryGetValue(key, out childNode))
                return true;
            
            return false;
        }
        
        public void Add(string key, ZJsonNode childNode)
        {
            dictChild.Add(key, childNode);
        }

        public void Remove(string key)
        {
            dictChild.Remove(key);
        }

        public void Clear()
        {
            dictChild.Clear();
        }

        public IEnumerator<ZJsonNode> GetEnumerator()
        {
            return dictChild.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        
        
        #region To Json String

        public override void ToJson(StringBuilder builder)
        {
            builder.Append("{");

            
            bool isFirstChild = true;
            foreach (var kv in dictChild)
            {
                if(isFirstChild == false)
                    builder.Append(",");
                else
                    isFirstChild = false;
                
                //Key
                ZJsonHelper.AppendStringForJson(builder, kv.Key);
                builder.Append(':');
                
                //Value
                var value = kv.Value;
                if (value == null)
                    builder.Append("null");
                else
                    value.ToJson(builder);
            }
            

            builder.Append("}");
        }

        #endregion


        #region To Object

        public override object OverwriteObject(Type objType, object objToOverwrite)
        {
            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == ZJsonReflection.TYPE_DICTIONARY)
                return OverwriteDictionary(objType, objToOverwrite);

            if(ZJsonReflection.IsBuiltInObjectType(objType))
                return OverwriteBuiltInObject(objType, objToOverwrite);

            //Shape mismatch: json object không thể vào scalar/enum/list/array -> trả null (field sẽ về null/default)
            if (ZJsonReflection.IsScalarType(objType) || objType.IsEnum || objType.IsArray ||
                (objType.IsGenericType && objType.GetGenericTypeDefinition() == ZJsonReflection.TYPE_LIST))
                return null;

            return OverwriteLiteralObject(objType, objToOverwrite);
        }
        
        private object OverwriteDictionary(Type dictType, object dict)
        {
            if (dict == null)
                dict = ZJsonReflection.CreateInstance(dictType);

            Type keyType = dictType.GenericTypeArguments[0];
            bool keyIsString = keyType == ZJsonReflection.TYPE_STRING;
            bool keyIsEnum = keyType.IsEnum;
            Type elementType = dictType.GenericTypeArguments[1];
            bool elementIsValueType = elementType.IsValueType;
            IDictionary iDict = (IDictionary)dict;
            iDict.Clear();

            foreach (var kv in dictChild)
            {
                object key;
                if (keyIsString)
                    key = kv.Key;
                else if (keyIsEnum)
                    key = Enum.Parse(keyType, kv.Key); //Enum.Parse nhận cả tên lẫn số
                else
                    key = Convert.ChangeType(kv.Key, keyType, CultureInfo.InvariantCulture);

                if (kv.Value == null)
                {
                    if (elementIsValueType)
                        throw new Exception("Json null for value type " + elementType.Name + ", key: " + kv.Key);

                    iDict.Add(key, null);
                }
                else
                {
                    var element = kv.Value.OverwriteObject(elementType, null);

                    if (element == null && elementIsValueType)
                        throw new Exception("Json null for value type " + elementType.Name + ", key: " + kv.Key);

                    iDict.Add(key, element);
                }
            }

            return iDict;
        }
        
        private object OverwriteLiteralObject(Type objType, object obj)
        {
            if (obj == null)
                obj = ZJsonReflection.CreateInstance(objType);


            if (obj is IZJsonCustomSerializable customSerializable)
            {
                customSerializable.FromJsonNode(this);
                return obj;
            }


            foreach (var kv in dictChild)
            {
                var accessor = ZJsonReflection.GetFieldAccessor(objType, kv.Key);
                if(accessor == null)
                    continue;

                var value = kv.Value;
                if (value == null)
                {
                    accessor.setter(obj, null);
                }
                else
                {
                    object fieldValue = accessor.getter(obj);
                    object fieldValueNew = value.OverwriteObject(accessor.fieldType, fieldValue);

                    //Là kiểu giá trị hoặc không giống nhau thì cần set lại value
                    if(accessor.fieldIsValueType || ReferenceEquals(fieldValue, fieldValueNew) == false)
                        accessor.setter(obj, fieldValueNew);
                }
            }


            return obj;
        }
        
        private object OverwriteBuiltInObject(Type objType, object obj)
        {
            //Lưu ý rằng có một số type như AnimationCurve có thể Serializable bởi Unity để hiển thị trong Inspector nhưng không thể Convert sang json nên kết quả là object đó sẽ bị trống

            if (ZJsonBuiltInHandler.IsHandledType(objType))
                return ZJsonBuiltInHandler.FromNode(objType, this);

            string json = ToJson();

            if (obj == null)
                return JsonUtility.FromJson(json, objType);

            JsonUtility.FromJsonOverwrite(json, obj);
            return obj;
        }

        #endregion



        #region Add Bonus

        public void AddNull(string key)
        {
            dictChild.Add(key, null);
        }

        public void Add(string key, string value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsString = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, bool value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsBool = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, byte value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsByte = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, sbyte value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsSByte = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, char value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsChar = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, double value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsDouble = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, float value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsFloat = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, int value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsInt = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, uint value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsUInt = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, nint value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsNInt = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, nuint value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsNUInt = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, long value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsLong = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, ulong value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsULong = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, short value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsShort = value;
            dictChild.Add(key, jsonScalar);
        }
        
        public void Add(string key, ushort value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsUShort = value;
            dictChild.Add(key, jsonScalar);
        }

        public void Add(string key, decimal value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsDecimal = value;
            dictChild.Add(key, jsonScalar);
        }
        
        #endregion
    }
}