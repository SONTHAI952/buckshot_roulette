using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ZeroX.JsonSystem
{
    public class ZReader
    {
        public string json;
        public int index;

        public ZReader(string json)
        {
            this.json = json;
        }
        
        
        /// <summary>
        /// Di chuyển index tới sau dấu 2 chấm
        /// </summary>
        public void MoveToAfterTwoDot()
        {
            while (json[index] != ':')
            {
                index++;
            }
            
            index++;
        }

        /// <summary>
        /// Di chuyển index tới ký tự đầu tiên không phải space
        /// </summary>
        public void MoveToNonSpace()
        {
            while (true)
            {
                char c = json[index];
                if (c != ' ' && c != '\t' && c != '\n' && c != '\r')
                    return;

                index++;
            }
        }

        /// <summary>
        /// Dừng sau dấu , hoặc tại dấu ]
        /// </summary>
        public void MoveTo_AfterComma_Or_RightBracket()
        {
            while (true)
            {
                char c = json[index];

                if (c == ',')
                {
                    index++;
                    return;
                }
                
                if(c == ']')
                    return;
                
                index++;
            }
        }
        
        /// <summary>
        /// Bắt đầu tìm từ index hiện tại. Cơ chế là tìm dấu " để bắt đầu ghi key và dừng tại dấu " kết thúc.
        /// <para>Nếu không tìm thấy key thì sẽ dừng tại index của }</para>
        /// <para>Các trường hợp khác thì là sai format</para>
        /// </summary>
        public string ReadNextKey()
        {
            //Tìm dấu " bắt đầu
            while (true)
            {
                char c = json[index];
                
                if (c == '}') //Nghĩa là hết key rồi
                    return null;
                
                if (c == '"') //Tìm thấy dấu " bắt đầu
                    break;
                
                if (c != ' ' && c != '\t' && c != '\n' && c != '\r' && c != ',')
                    throw new FormatException(ZJsonConst.EM_IncorrectJsonStringFormat);
                
                index++;
            }
            
            
            
            //Đọc key (có unescape nếu key chứa ký tự đặc biệt), kết thúc tại dấu " đóng
            return ReadNextScalarValue_String();
        }

        /// <summary>
        /// Giống ReadNextKey nhưng không cắt chuỗi: xuất vùng key raw [keyStart, keyLength] trên json (chưa unescape).
        /// <para>Trả về false nếu hết key (index dừng tại }). True thì index dừng tại dấu " đóng key</para>
        /// </summary>
        public bool ReadNextKeyBounds(out int keyStart, out int keyLength)
        {
            //Tìm dấu " bắt đầu
            while (true)
            {
                char c = json[index];

                if (c == '}') //Nghĩa là hết key rồi
                {
                    keyStart = 0;
                    keyLength = 0;
                    return false;
                }

                if (c == '"') //Tìm thấy dấu " bắt đầu
                    break;

                if (c != ' ' && c != '\t' && c != '\n' && c != '\r' && c != ',')
                    throw new FormatException(ZJsonConst.EM_IncorrectJsonStringFormat);

                index++;
            }


            index++; //Dịch chuyển ra sau ký tự "
            keyStart = index;

            bool isEscaped = false;
            while (true)
            {
                char c = json[index];

                if (isEscaped)
                {
                    isEscaped = false;
                }
                else
                {
                    if (c == '\\')
                        isEscaped = true;
                    else if (c == '"')
                        break;
                }

                index++;
            }

            keyLength = index - keyStart;
            return true;
        }


        /// <summary>
        /// Khi gọi hàm này, index phải đang ở vị trí của ký tự { bắt đầu.
        /// <para>Kết thúc hàm này, index nằm ở vị trí của ký tự } cùng cấp với ký tự { bắt đầu</para>
        /// </summary>
        public string ReadNextObjectValue()
        {
            int startIndex = index;
            int number = 0;
            bool isInStringValue = false;
            bool isEscaped = false;

            while (true)
            {
                char c = json[index];

                if (isInStringValue)
                {
                    if (isEscaped)
                    {
                        isEscaped = false;
                    }
                    else
                    {
                        if (c == '\\')
                        {
                            isEscaped = true;
                        }
                        else if (c == '"')
                        {
                            isInStringValue = false;
                        }
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        isInStringValue = true;
                    }
                    else if (c == '{')
                    {
                        number++;
                    }
                    else if (c == '}')
                    {
                        number--;
                        if (number == 0)
                            break;
                    }
                }

                
                index++;
            }
            
            return json.Substring(startIndex, index - startIndex + 1);
        }

        public string ReadNextListValue()
        {
            int startIndex = index;
            int number = 0;
            bool isInStringValue = false;
            bool isEscaped = false;

            while (true)
            {
                char c = json[index];

                if (isInStringValue)
                {
                    if (isEscaped)
                    {
                        isEscaped = false;
                    }
                    else
                    {
                        if (c == '\\')
                        {
                            isEscaped = true;
                        }
                        else if (c == '"')
                        {
                            isInStringValue = false;
                        }
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        isInStringValue = true;
                    }
                    else if (c == '[')
                    {
                        number++;
                    }
                    else if (c == ']')
                    {
                        number--;
                        if (number == 0)
                            break;
                    }
                }

                
                index++;
            }
            
            return json.Substring(startIndex, index - startIndex + 1);
        }

        /// <summary>
        /// Trả về true nếu index hiện tại đang ở vị trí bắt đầu của "null"
        /// </summary>
        public bool IsCurrentAtNull()
        {
            if (json[index] == 'n')
            {
                if (index + 1 < json.Length)
                {
                    if(json[index + 1] == 'u')
                        return true;
                }
            }

            return false;
        }
        
        #region Read Next Scalar Value

        public void ReadNextScalarValue(out ZJsonScalarType scalarType, out string scalarValue)
        {
            char firstChar = json[index];


            if (firstChar == '\"')
            {
                scalarType = ZJsonScalarType.String;
                scalarValue = ReadNextScalarValue_String();
                return;
            }


            if ((firstChar >= 48 && firstChar <= 57) ||
                firstChar == '-' ||
                firstChar == 'n' || //nan
                firstChar == 'N' || //NaN
                firstChar == 'i' || //infinity
                firstChar == 'I') //Infinity
            {
                scalarType = ZJsonScalarType.Number;
                scalarValue = ReadNextScalarValue_Number();
                return;
            }


            if (firstChar == 't' || firstChar == 'f')
            {
                scalarType = ZJsonScalarType.Bool;
                scalarValue = ReadNextScalarValue_Bool();
                return;
            }


            throw new FormatException(ZJsonConst.EM_IncorrectJsonStringFormat);
        }

        /// <summary>
        /// Giống ReadNextScalarValue nhưng number thường sẽ không cắt chuỗi: value == null và dùng bounds [numberStart, numberLength].
        /// <para>Number đặc biệt (NaN, Infinity), String, Bool vẫn trả chuỗi qua value</para>
        /// </summary>
        public void ReadNextScalarValueBounds(out ZJsonScalarType scalarType, out string value, out int numberStart, out int numberLength)
        {
            numberStart = 0;
            numberLength = 0;
            char firstChar = json[index];


            if (firstChar == '\"')
            {
                scalarType = ZJsonScalarType.String;
                value = ReadNextScalarValue_String();
                return;
            }


            if ((firstChar >= 48 && firstChar <= 57) ||
                firstChar == '-' ||
                firstChar == 'n' || //nan
                firstChar == 'N' || //NaN
                firstChar == 'i' || //infinity
                firstChar == 'I') //Infinity
            {
                scalarType = ZJsonScalarType.Number;
                value = ReadNextScalarValue_Number_Bounds(out numberStart, out numberLength);
                return;
            }


            if (firstChar == 't' || firstChar == 'f')
            {
                scalarType = ZJsonScalarType.Bool;
                value = ReadNextScalarValue_Bool();
                return;
            }


            throw new FormatException(ZJsonConst.EM_IncorrectJsonStringFormat);
        }
        
        /// <summary>
        /// Dừng tại ký tự " cuối
        /// </summary>
        private string ReadNextScalarValue_String()
        {
            StringBuilder builder = null; //Để tối ưu tốc độ, chỉ dùng StringBuilder khi có ký tự đặc biệt

            index++; //Đi qua ký tự " đầu tiên
            int startSegmentIndex = index;
            
            while (true)
            {
                char curChar = json[index];

                if (curChar == '\"')
                {
                    //Append segment hiện tại nếu sử dụng builder
                    if (builder != null)
                    {
                        string segment = json.Substring(startSegmentIndex, index - startSegmentIndex);
                        builder.Append(segment);
                    }
                    
                    break;
                }
                
                if (curChar == '\\') //Special Char
                {
                    //Append segment hiện tại
                    string segment = json.Substring(startSegmentIndex, index - startSegmentIndex);
                    if(builder == null)
                        builder = new StringBuilder();
                    builder.Append(segment);
                    
                    
                    //Special Char
                    char specialChar = ReadNextScalarValue_String_SpecialChar();
                    builder.Append(specialChar);

                    
                    //startSegmentIndex tiếp theo
                    startSegmentIndex = index + 1;
                }
                
                index++;
            }
            
            
            if(builder == null)
                return json.Substring(startSegmentIndex, index - startSegmentIndex);
            else
                return builder.ToString();
        }

        /// <summary>
        /// Index phải đang ở vị trí của ký tự \ bắt đầu special.
        /// <para>Khi kết thúc sẽ ở vị trí ký tự cuối của đoạn special</para>
        /// </summary>
        private char ReadNextScalarValue_String_SpecialChar()
        {
            index++; //Move qua ký tự \
            char c = json[index];

            switch (c)
            {
                case '"':
                    return '"';
                
                case '\\':
                    return '\\';
                
                case 't':
                    return '\t';
                
                case 'n':
                    return '\n';
                
                case 'r':
                    return '\r';
                
                case 'b':
                    return '\b';
                
                case 'f':
                    return '\f';

                case 'u':
                {
                    
                    string unicode = new string(new[] {
                        json[index + 1],
                        json[index + 2],
                        json[index + 3],
                        json[index + 4] });
                    
                    index += 4;

                    return (char)Convert.ToInt32(unicode, 16);
                }

                default:
                {
                    Debug.LogException(new Exception("Not code for special char: " + c));
                    return c;
                }
            }
        }
        
        /// <summary>
        /// Dừng tại ký tự cuối của number.
        /// <para>Number thường trả về null và xuất bounds; number đặc biệt (NaN, Infinity) trả về chuỗi chuẩn</para>
        /// </summary>
        private string ReadNextScalarValue_Number_Bounds(out int start, out int length)
        {
            int startIndex = index;
            int numberStringLength = -1;

            char firstChar = json[index];
            char secondChar = index + 1 < json.Length ? json[index + 1] : ' ';


            while (true)
            {
                char curChar = json[index];

                if (curChar == ',' || curChar == '}' || curChar == ']')
                {
                    if(numberStringLength == -1)
                        numberStringLength = index - startIndex;

                    index--; //Lùi lại để ở tại vị trí của ký tự cuối
                    break;
                }

                if (curChar == ' ' || curChar == '\t' || curChar == '\n' || curChar == '\r') //Vì Json format chấp nhận các ký tự này phía sau ký tự số cuối cùng
                {
                    if(numberStringLength == -1)
                        numberStringLength = index - startIndex;
                }

                index++;
            }

            start = startIndex;
            length = numberStringLength;


            //Case số đặc biệt
            if (firstChar == '-')
            {
                if (secondChar == 'i' || secondChar == 'I')
                    return "-Infinity";
            }
            else
            {
                if (firstChar == 'n' || firstChar == 'N')
                    return "NaN";

                if (firstChar == 'i' || firstChar == 'I')
                    return "Infinity";
            }

            return null;
        }

        private string ReadNextScalarValue_Number()
        {
            string special = ReadNextScalarValue_Number_Bounds(out int start, out int length);
            if (special != null)
                return special;

            return json.Substring(start, length);
        }
        
        //Dừng tại ký tự cuối của true hoặc false
        private string ReadNextScalarValue_Bool()
        {
            if (json[index] == 't')
            {
                index += 3;
                return "true";
            }
            else
            {
                index += 4;
                return "false";
            }
        }

        #endregion
        
        
        
        #region Skip Key Value

        /// <summary>
        /// Khi gọi hàm này, index phải đang ở vị trí của ký tự đầu tiên của value.
        /// <para>Khi kết thúc hàm này, index nằm ở vị trí ký tự cuối của đoạn value. Nếu là string thì ký tự cuối là ", object là }, array là ], number là số cuối cùng</para>
        /// </summary>
        public void SkipValue()
        {
            char c = json[index];

            if (c == '{')
            {
                SkipValue_Object();
                return;
            }

            if (c == '[')
            {
                SkipValue_List();
                return;
            }

            if (IsCurrentAtNull()) //null
            {
                index += 3;
                return;
            }

            if (c == 't') //true
            {
                index += 3;
                return;
            }
            
            if (c == 'f') //false
            {
                index += 4;
                return;
            }
            
            if ((c >= 48 && c <= 57) ||
                c == '-' ||
                c == 'n' || //nan
                c == 'N' || //NaN
                c == 'i' || //infinity
                c == 'I') //Infinity
            {
                SkipValue_Number();
                return;
            }

            if (c == '\"')
            {
                SkipValue_String();
                return;
            }
            
            throw new FormatException(ZJsonConst.EM_IncorrectJsonStringFormat);
        }
        
        private void SkipValue_Object()
        {
            int number = 0;
            bool isInStringValue = false;
            bool isEscaped = false;

            while (true)
            {
                char c = json[index];

                if (isInStringValue)
                {
                    if (isEscaped)
                    {
                        isEscaped = false;
                    }
                    else
                    {
                        if (c == '\\')
                        {
                            isEscaped = true;
                        }
                        else if (c == '"')
                        {
                            isInStringValue = false;
                        }
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        isInStringValue = true;
                    }
                    else if (c == '{')
                    {
                        number++;
                    }
                    else if (c == '}')
                    {
                        number--;
                        if (number == 0)
                            break;
                    }
                }

                
                index++;
            }
        }

        private void SkipValue_List()
        {
            int number = 0;
            bool isInStringValue = false;
            bool isEscaped = false;

            while (true)
            {
                char c = json[index];

                if (isInStringValue)
                {
                    if (isEscaped)
                    {
                        isEscaped = false;
                    }
                    else
                    {
                        if (c == '\\')
                        {
                            isEscaped = true;
                        }
                        else if (c == '"')
                        {
                            isInStringValue = false;
                        }
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        isInStringValue = true;
                    }
                    else if (c == '[')
                    {
                        number++;
                    }
                    else if (c == ']')
                    {
                        number--;
                        if (number == 0)
                            break;
                    }
                }

                
                index++;
            }
        }

        private void SkipValue_Number()
        {
            while (true)
            {
                char curChar = json[index];
                
                if (curChar == ',' || curChar == '}' || curChar == ']')
                {
                    index--; //Lùi lại để ở tại vị trí của ký tự cuối
                    break;
                }
                
                index++;
            }
        }
        
        /// <summary>
        /// Index phải đang ở ký tự " bắt đầu chuỗi. Khi kết thúc, index sẽ ở vị trí dấu " kết thúc chuỗi
        /// </summary>
        private void SkipValue_String()
        {
            index++; //Move qua dấu " bắt đầu chuỗi
            bool isEscaped = false;
            
            while (true)
            {
                char c = json[index];

                if (isEscaped)
                {
                    isEscaped = false;
                }
                else
                {
                    if (c == '\\')
                    {
                        isEscaped = true;
                    }
                    else if (c == '\"')
                    {
                        break;
                    }
                }
                
                index++;
            }
        }

        #endregion
    }
}