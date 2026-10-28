using System;
using System.Collections;
using System.Globalization;

namespace ZeroX.SaveGameSystem
{
    public partial class SGNode
    {
        public T GetObject<T>()
        {
            var result = Internal_GetObject(typeof(T), null);
            if (result == null)
                return default;

            return (T)result;
        }
        
        /// <summary>
        /// Nếu là objToOverwrite là array thì sẽ trả về object mới, vì array không thể thay đổi size
        /// </summary>
        public object GetObjectOverwrite(object objToOverwrite)
        {
            if (objToOverwrite == null)
                throw new NullReferenceException("Object to overwrite cannot null");
            
            return Internal_GetObject(objToOverwrite.GetType(), objToOverwrite);
        }
        
        
        
        private object Internal_GetObject(Type objType, object objToOverwrite)
        {
            if (NodeType == NodeType.Undefined)
            {
                if (objType == ReflectionType.STRING)
                    return "";
                
                if (objType.IsValueType)
                    return Activator.CreateInstance(objType);
                
                return null;
            }
            
            if (objType.IsArray)
                return GetObject_Array(objType);

            if (objType.IsGenericType)
            {
                var objTypeGenericDefinition = objType.GetGenericTypeDefinition();
                if (objTypeGenericDefinition == ReflectionType.LIST)
                    return GetObject_List(objType, objToOverwrite);

                if (objTypeGenericDefinition == ReflectionType.DICTIONARY)
                    return GetObject_Dictionary(objType, objToOverwrite);

                throw new Exception("SGNode.GetObject - Not code for obj type: " + objType.Name);
            }
            
            
            #region Primitive-Like

            if (objType.IsPrimitive)
                return GetObject_DotNetPrimitive(objType);

            if (objType == ReflectionType.STRING)
                return GetString();

            if (objType == ReflectionType.DECIMAL)
                return GetDecimal();

            if (objType == ReflectionType.BIG_INTEGER)
                return GetBigInt();

            #endregion

            
            
            #region Bonus

            if (objType == ReflectionType.DateTime)
                return GetDateTime();

            if (objType == ReflectionType.Color)
                return GetColor();

            if (objType == ReflectionType.VECTOR2)
                return GetVector2();

            if (objType == ReflectionType.VECTOR2_INT)
                return GetVector2Int();

            if (objType == ReflectionType.VECTOR3)
                return GetVector3();

            if (objType == ReflectionType.VECTOR3_INT)
                return GetVector3Int();

            #endregion



            return GetObject_Object(objType, objToOverwrite);
        }

        private object GetObject_DotNetPrimitive(Type valueType)
        {
            if (valueType == ReflectionType.INT)
                return GetInt();
            
            if (valueType == ReflectionType.FLOAT)
                return GetFloat();
            
            if (valueType == ReflectionType.LONG)
                return GetLong();

            if (valueType == ReflectionType.DOUBLE)
                return GetDouble();

            if (valueType == ReflectionType.BOOL)
                return GetBool();

            throw new Exception("Not code for dot net primitive type: " + valueType.Name);
        }
        
        
        
        private object GetObject_Array(Type arrayType)
        {
            Type elementType = arrayType.GetElementType();
            int length = ChildCount;
            var array = Array.CreateInstance(elementType, length);
            
            for (int i = 0; i < length; i++)
            {
                var element = GetChildAt(i).Internal_GetObject(elementType, null);
                array.SetValue(element, i);
            }

            return array;
        }
        
        /// <summary>
        /// Nếu list truyền vào không null thì clear và thêm phần tử vào đó. Còn null thì tạo mới
        /// </summary>
        private object GetObject_List(Type listType, object listObjToOverwrite)
        {
            if (listObjToOverwrite == null)
                listObjToOverwrite = Activator.CreateInstance(listType);
            
            Type elementType = listType.GenericTypeArguments[0];
            IList list = (IList)listObjToOverwrite;
            list.Clear();

            int length = ChildCount;
            for (int i = 0; i < length; i++)
            {
                var element = GetChildAt(i).Internal_GetObject(elementType, null);
                list.Add(element);
            }

            return list;
        }
        
        /// <summary>
        /// Nếu list truyền vào không null thì clear và thêm phần tử vào đó. Còn null thì tạo mới
        /// </summary>
        private object GetObject_Dictionary(Type dictionaryType, object dictObjToOverwrite)
        {
            if (dictObjToOverwrite == null)
                dictObjToOverwrite = Activator.CreateInstance(dictionaryType);
            
            Type keyType = dictionaryType.GenericTypeArguments[0];
            Type elementType = dictionaryType.GenericTypeArguments[1];
            IDictionary dictionary = (IDictionary)dictObjToOverwrite;
            dictionary.Clear();

            int length = ChildCount;
            for (int i = 0; i < length; i++)
            {
                SGNode childNode = GetChildAt(i);
                var key = ParseSGNodeKeyToDictionaryKey(childNode.Key, keyType);
                var element = childNode.Internal_GetObject(elementType, null);
                dictionary.Add(key, element);
            }

            return dictionary;
        }

        private object ParseSGNodeKeyToDictionaryKey(string key, Type keyType)
        {
            if (keyType == ReflectionType.STRING)
                return key;

            if (keyType == ReflectionType.INT)
                return ParseInt(key);

            if (keyType == ReflectionType.LONG)
                return ParseLong(key);
            
            if (keyType == ReflectionType.BIG_INTEGER)
                return ParseBigInteger(key);
            
            

            if (keyType == ReflectionType.FLOAT)
                return float.Parse(key, CultureInfo.InvariantCulture);
            
            if (keyType == ReflectionType.DOUBLE)
                return double.Parse(key, CultureInfo.InvariantCulture);
            
            if (keyType == ReflectionType.DECIMAL)
                return decimal.Parse(key, CultureInfo.InvariantCulture);


            throw new Exception("Not support dictionary key type: " + keyType);
        }

        //Hàm này chỉ dùng cho object ko phải primitive-like, ko phải array, list, dictionary
        private object GetObject_Object(Type objType, object objToOverwrite)
        {
            if (objToOverwrite == null)
                objToOverwrite = Activator.CreateInstance(objType);
            
            
            var listFieldInfo = ReflectionUtility.GetAllFieldInfoCanSave(objType);
            foreach (var fieldInfo in listFieldInfo)
            {
                if (TryGetChild(fieldInfo.Name, out SGNode childNode) == false)
                    continue;
                
                if(childNode.NodeType == NodeType.Undefined)
                    continue;


                var childObjToOverwrite = fieldInfo.GetValue(objToOverwrite);
                childObjToOverwrite = childNode.Internal_GetObject(fieldInfo.FieldType, childObjToOverwrite);
                fieldInfo.SetValue(objToOverwrite, childObjToOverwrite);
            }

            
            return objToOverwrite;
        }
    }
}