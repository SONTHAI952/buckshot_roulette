using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZeroX.JsonSystem
{
    public sealed class ZJsonList : ZJsonNode, IEnumerable<ZJsonNode>
    {
        private List<ZJsonNode> listChild = new List<ZJsonNode>();


        public override ZJsonNodeType NodeType => ZJsonNodeType.List;
        public override int ChildCount => listChild.Count;


        
        #region Access
        
        public override ZJsonNode this[string key]
        {
            get => throw new Exception(ZJsonConst.EM_JsonNodeIsNotObject);
            set => throw new Exception(ZJsonConst.EM_JsonNodeIsNotObject);
        }

        public override ZJsonNode this[int index]
        {
            get => listChild[index];
            set => listChild[index] = value;
        }

        #endregion
        


        public bool Contains(ZJsonNode childNode)
        {
            return listChild.Contains(childNode);
        }

        public void Add(ZJsonNode childNode)
        {
            listChild.Add(childNode);
        }

        public void RemoveAt(int index)
        {
            listChild.RemoveAt(index);
        }

        public bool Remove(ZJsonNode childNode)
        {
            return listChild.Remove(childNode);
        }

        public int RemoveAll(Predicate<ZJsonNode> predicate)
        {
            return listChild.RemoveAll(predicate);
        }

        public void Clear()
        {
            listChild.Clear();
        }


        public IEnumerator<ZJsonNode> GetEnumerator()
        {
            return listChild.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        #region To Json String

        public override void ToJson(StringBuilder builder)
        {
            builder.Append('[');

            int firstPhaseCount = listChild.Count - 1;
            for (int i = 0; i < firstPhaseCount; i++)
            {
                var childNode = listChild[i];
                
                if(childNode == null)
                    builder.Append("null");
                else
                    childNode.ToJson(builder);
                
                builder.Append(",");
            }
            
       
            //Phần tử cuối
            if (listChild.Count > 0)
            {
                var childNode = listChild[listChild.Count - 1];
                if(childNode == null)
                    builder.Append("null");
                else
                    childNode.ToJson(builder);
            }


            builder.Append(']');
        }

        #endregion



        #region To Object

        public override object OverwriteObject(Type objType, object objToOverwrite)
        {
            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == ZJsonReflection.TYPE_LIST)
                return OverwriteList(objType, objToOverwrite);

            if (objType.IsArray)
                return OverwriteArray(objType, objToOverwrite);

            //Shape mismatch: json list chỉ vào được List/Array -> trả null (field sẽ về null/default)
            return null;
        }
        
        private object OverwriteList(Type listType, object list)
        {
            if (list == null)
                list = ZJsonReflection.CreateInstance(listType);
            
            int childCount = listChild.Count;
            Type elementType = listType.GenericTypeArguments[0];
            bool elementIsValueType = elementType.IsValueType;
            IList iList = (IList)list;
            iList.Clear();

            for (int i = 0; i < childCount; i++)
            {
                var childNode = listChild[i];
                if (childNode == null)
                {
                    if (elementIsValueType)
                        throw new Exception("Json null for value type " + elementType.Name + ", index: " + i);

                    iList.Add(null);
                }
                else
                {
                    var childObj = childNode.OverwriteObject(elementType, null);

                    if (childObj == null && elementIsValueType)
                        throw new Exception("Json null for value type " + elementType.Name + ", index: " + i);

                    iList.Add(childObj);
                }
            }

            return list;
        }

        private object OverwriteArray(Type arrayType, object array)
        {
            int childCount = listChild.Count;
            var elementType = arrayType.GetElementType();
            bool elementIsValueType = elementType.IsValueType;

            Array iArray = array == null ? null : (Array)array;
            if(iArray == null || iArray.Length != childCount)
                iArray = Array.CreateInstance(elementType, childCount);

            for (int i = 0; i < childCount; i++)
            {
                var childNode = listChild[i];
                if (childNode == null)
                {
                    if (elementIsValueType)
                        throw new Exception("Json null for value type " + elementType.Name + ", index: " + i);

                    iArray.SetValue(null, i);
                }
                else
                {
                    var childObj = childNode.OverwriteObject(elementType, null);

                    if (childObj == null && elementIsValueType)
                        throw new Exception("Json null for value type " + elementType.Name + ", index: " + i);

                    iArray.SetValue(childObj, i);
                }
            }
            
            return iArray;
        }

        #endregion
        
        
        
        #region Add Bonus

        public void AddNull()
        {
            listChild.Add(null);
        }

        public void Add(string value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsString = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(bool value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsBool = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(byte value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsByte = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(sbyte value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsSByte = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(char value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsChar = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(double value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsDouble = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(float value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsFloat = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(int value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsInt = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(uint value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsUInt = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(nint value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsNInt = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(nuint value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsNUInt = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(long value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsLong = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(ulong value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsULong = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(short value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsShort = value;
            listChild.Add(jsonScalar);
        }
        
        public void Add(ushort value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsUShort = value;
            listChild.Add(jsonScalar);
        }

        public void Add(decimal value)
        {
            var jsonScalar = new ZJsonScalar();
            jsonScalar.AsDecimal = value;
            listChild.Add(jsonScalar);
        }
        
        #endregion
    }
}