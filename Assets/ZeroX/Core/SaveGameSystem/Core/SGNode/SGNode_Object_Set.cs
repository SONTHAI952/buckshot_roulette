using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace ZeroX.SaveGameSystem
{
    public partial class SGNode
    {
        public void SetObject(object value)
        {
            if (value == null)
            {
                Debug.LogError("Cannot set object because value is null");
                return;
            }

            lock (Database)
            {
                Internal_SetObject(value);
            }
        }
        
        private void Internal_SetObject(object value)
        {
            Type valueType = value.GetType();
            
            
            
            if (valueType.IsArray)
            {
                SetObject_Array(value, valueType);
                return;
            }
            
            if (valueType.IsGenericType)
            {
                var valueTypeGenericDefinition = valueType.GetGenericTypeDefinition();
                if (valueTypeGenericDefinition == ReflectionType.LIST)
                {
                    SetObject_List(value, valueType);
                    return;
                }
                
                if (valueTypeGenericDefinition == ReflectionType.DICTIONARY)
                {
                    SetObject_Dictionary(value, valueType);
                    return;
                }
                
                Debug.LogError("SGNode.SetObject - Not code for value type: " + valueType.Name);
                return;
            }
            
            
            
            #region PrimitiveLike

            if (valueType.IsPrimitive)
            {
                SetObject_DotNetPrimitive(value, valueType);
                return;
            }
            
            if (valueType == ReflectionType.STRING)
            {
                SetString((string)value);
                return;
            }
            
            if (valueType == ReflectionType.DECIMAL)
            {
                SetDecimal((decimal)value);
                return;
            }
            
            if (valueType == ReflectionType.BIG_INTEGER)
            {
                SetBigInt((BigInteger)value);
                return;
            }

            #endregion
            
            
            
            #region Bonus

            if (valueType == ReflectionType.DateTime)
            {
                SetDateTime((DateTime)value);
                return;
            }
            
            if (valueType == ReflectionType.Color)
            {
                SetColor((Color)value);
                return;
            }
            
            if (valueType == ReflectionType.VECTOR2)
            {
                SetVector2((Vector2)value);
                return;
            }
            
            if (valueType == ReflectionType.VECTOR2_INT)
            {
                SetVector2Int((Vector2Int)value);
                return;
            }
            
            if (valueType == ReflectionType.VECTOR3)
            {
                SetVector3((Vector3)value);
                return;
            }
            
            if (valueType == ReflectionType.VECTOR3_INT)
            {
                SetVector3Int((Vector3Int)value);
                return;
            }

            #endregion


            
            SetObject_Obj(value, valueType);
        }
        
        
        
        private void SetObject_DotNetPrimitive(object value, Type valueType)
        {
            if (valueType == ReflectionType.INT)
            {
                SetInt((int)value);
                return;
            }
            
            if (valueType == ReflectionType.FLOAT)
            {
                SetFloat((float)value);
                return;
            }
            
            if (valueType == ReflectionType.LONG)
            {
                SetLong((long)value);
                return;
            }
            
            if (valueType == ReflectionType.DOUBLE)
            {
                SetDouble((double)value);
                return;
            }
            
            if (valueType == ReflectionType.BOOL)
            {
                SetBool((bool)value);
                return;
            }
        }
        
        private void SetObject_Array(object value, Type valueType)
        {
            Array array = (Array)value;
            int length = array.Length;
            DeleteLastChilds(ChildCount - length); //Nếu mảng cũ dài hơn mảng mới thì xóa bớt mảng cũ đi

            int oldChildCount = ChildCount;
            int index = 0;
            
            //Lặp lần đầu: ghi đè các node cũ để tái sử dụng
            for (; index < oldChildCount; index++)
            {
                var element = array.GetValue(index);
                if (element == null)
                    GetChildAt(index).SetUndefined();
                else
                    GetChildAt(index).Internal_SetObject(element);
            }

            //Lặp lần 2: thêm các node mới
            for (; index < length; index++)
            {
                var element = array.GetValue(index);
                if (element == null)
                    AddChild();
                else
                    AddChild().Internal_SetObject(element);
            }
        }
        
        private void SetObject_List(object value, Type valueType)
        {
            IList list = (IList)value;
            int length = list.Count;
            DeleteLastChilds(ChildCount - length); //Nếu mảng cũ dài hơn mảng mới thì xóa bớt mảng cũ đi

            int oldChildCount = ChildCount;
            int index = 0;
            
            //Lặp lần đầu: ghi đè các node cũ để tái sử dụng
            for (; index < oldChildCount; index++)
            {
                var element = list[index];
                if (element == null)
                    GetChildAt(index).SetUndefined();
                else
                    GetChildAt(index).Internal_SetObject(element);
            }

            //Lặp lần 2: thêm các node mới
            for (; index < length; index++)
            {
                var element = list[index];
                if (element == null)
                    AddChild();
                else
                    AddChild().Internal_SetObject(element);
            }
        }
        
        private void SetObject_Dictionary(object value, Type valueType)
        {
            IDictionary dictionary = (IDictionary)value;
            HashSet<string> listNoDelete = new HashSet<string>();
            
            foreach (DictionaryEntry entry in dictionary)
            {
                string key = entry.Key.ToString();
                var element = entry.Value;
                
                if (element == null)
                    this[key].SetUndefined();
                else
                    this[key].Internal_SetObject(element);

                listNoDelete.Add(key);
            }
            
            //Delete các phần tử không thuộc dictionary
            for (int i = 0; i < ChildCount; i++)
            {
                string childKey = GetChildAt(i).Key;
                if(listNoDelete.Contains(childKey))
                    continue;
                
                DeleteChild(childKey);
                i--;
            }
        }
        
        private void SetObject_Obj(object value, Type valueType)
        {
            var childFieldInfos = ReflectionUtility.GetAllFieldInfoCanSave(valueType);
            foreach (var childFieldInfo in childFieldInfos)
            {
                var childValue = childFieldInfo.GetValue(value);
                if (childValue == null)
                    this[childFieldInfo.Name].SetUndefined();
                else
                    this[childFieldInfo.Name].Internal_SetObject(childValue);
            }
        }
    }
}