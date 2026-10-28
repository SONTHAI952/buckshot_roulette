using System;
using System.Text;
using UnityEngine;

namespace ZeroX.JsonSystem
{
    public static class ZJson_StringToNode
    {
        /// <summary>
        /// Nếu json null hoặc empty thì sẽ trả về null
        /// </summary>
        public static ZJsonNode Parse(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                ZReader reader = new ZReader(json);
                reader.MoveToNonSpace();
                return ParseNode(reader);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception(ZJsonConst.EM_IncorrectJsonStringFormat);
            }
        }
        

        
        /// <summary>
        /// Parse object, list, scalar. Cơ chế là xác định xem chuỗi tiếp theo là kiểu object, list, hay scalar.
        /// <para>Khi kết thúc thì reader phải ở ký tự cuối của đoạn data</para>
        /// </summary>
        private static ZJsonNode ParseNode(ZReader reader)
        {
            char c = reader.json[reader.index];
            
            if(c == '{')
                return ParseObject(reader);

            if (c == '[')
                return ParseList(reader);

            if (reader.IsCurrentAtNull())
                return ParseNull(reader);

            
            //Scalar
            reader.ReadNextScalarValue(out ZJsonScalarType scalarType, out string scalarValue);
            return new ZJsonScalar(scalarType, scalarValue);
        }
        

        
        
        #region Object

        /// <summary>
        /// Index phải đang ở vị trí của {
        ///<para>Khi kết thúc hàm thì index đang ở vị trí của }</para>
        /// </summary>
        private static ZJsonObject ParseObject(ZReader reader)
        {
            ZJsonObject jsonObject = new ZJsonObject();
            
            reader.index++; //Move qua {
            
            while (true)
            {
                string key = reader.ReadNextKey();
                if (key == null)
                    break;
                
                reader.index++; //Move qua "
                reader.MoveToAfterTwoDot(); //Move tới sau dấu :
                reader.MoveToNonSpace(); //Move tới ký tự đầu tiên không space
                
                ZJsonNode childNode = ParseNode(reader);
                jsonObject.Add(key, childNode);
                
                reader.index++; //Move qua ký tự cuối của đoạn data của childNode.
            }

            return jsonObject;
        }

        #endregion



        #region List

        /// <summary>
        /// Index phải đang ở vị trí của [
        ///<para>Khi kết thúc hàm thì index đang ở vị trí của ]</para>
        /// </summary>
        private static ZJsonList ParseList(ZReader reader)
        {
            ZJsonList jsonList = new ZJsonList();

            reader.index++; //Move qua [
            
            while (true)
            {
                reader.MoveToNonSpace();
                if(reader.json[reader.index] == ']')
                    break;
                
                ZJsonNode childNode = ParseNode(reader);
                jsonList.Add(childNode);
                
                reader.index++; //Move qua ký tự cuối của đoạn data của childNode.
                
                reader.MoveTo_AfterComma_Or_RightBracket();
            }

            return jsonList;
        }

        #endregion

        

        /// <summary>
        /// Dừng tại ký tự cuối của null
        /// </summary>
        private static ZJsonNode ParseNull(ZReader reader)
        {
            reader.index += 3;
            return null;
        }
    }
}