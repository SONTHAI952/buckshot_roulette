using System;
using System.Globalization;
using System.Text;

namespace ZeroX.JsonSystem
{
    public static class ZJsonHelper
    {
        #region Append Number Invariant

        //Buffer theo thread để format số không alloc chuỗi trung gian
        [ThreadStatic] private static char[] numberBuffer;

        private static char[] GetNumberBuffer()
        {
            return numberBuffer ?? (numberBuffer = new char[36]);
        }

        public static void AppendInvariant(StringBuilder builder, int value)
        {
            var buffer = GetNumberBuffer();
            value.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture);
            builder.Append(buffer, 0, written);
        }

        public static void AppendInvariant(StringBuilder builder, uint value)
        {
            var buffer = GetNumberBuffer();
            value.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture);
            builder.Append(buffer, 0, written);
        }

        public static void AppendInvariant(StringBuilder builder, long value)
        {
            var buffer = GetNumberBuffer();
            value.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture);
            builder.Append(buffer, 0, written);
        }

        public static void AppendInvariant(StringBuilder builder, ulong value)
        {
            var buffer = GetNumberBuffer();
            value.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture);
            builder.Append(buffer, 0, written);
        }

        public static void AppendInvariant(StringBuilder builder, float value)
        {
            var buffer = GetNumberBuffer();
            value.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture);
            builder.Append(buffer, 0, written);
        }

        public static void AppendInvariant(StringBuilder builder, double value)
        {
            var buffer = GetNumberBuffer();
            value.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture);
            builder.Append(buffer, 0, written);
        }

        public static void AppendInvariant(StringBuilder builder, decimal value)
        {
            var buffer = GetNumberBuffer();
            value.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture);
            builder.Append(buffer, 0, written);
        }

        #endregion



        /// <summary>
        /// Thực hiện escape string. Raw truyền vào không được null
        /// </summary>
        public static string FixStringForJson(string raw)
        {
            StringBuilder builder = null;
            int startSegmentIndex = 0;
            
            
            
            for (int i = 0; i < raw.Length; i++)
            {
                char curChar = raw[i];

                
                if (IsCharNeedEscape(curChar))
                {
                    //Append Segment hiện tại
                    string segment = raw.Substring(startSegmentIndex, i - startSegmentIndex);
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                        builder.Append('"');
                    }
                    builder.Append(segment);
                    
                    
                    //Escape Char
                    string escapeString = EscapeChar(curChar);
                    builder.Append(escapeString);
                    
                    
                    //startSegmentIndex tiếp theo
                    startSegmentIndex = i + 1;
                }
            }

            if (builder != null)
            {
                string segment = raw.Substring(startSegmentIndex, raw.Length - startSegmentIndex);
                builder.Append(segment);
            }
            
            

            if (builder == null)
            {
                return $"\"{raw}\"";
            }
            else
            {
                builder.Append('"');
                return builder.ToString();
            }
        }
        
        /// <summary>
        /// Escape string rồi append thẳng vào builder (kèm 2 dấu " bao quanh), không tạo chuỗi trung gian. Raw truyền vào không được null
        /// </summary>
        public static void AppendStringForJson(StringBuilder builder, string raw)
        {
            builder.Append('"');

            int startSegmentIndex = 0;
            for (int i = 0; i < raw.Length; i++)
            {
                char curChar = raw[i];

                if (IsCharNeedEscape(curChar))
                {
                    //Append Segment hiện tại
                    builder.Append(raw, startSegmentIndex, i - startSegmentIndex);

                    //Escape Char
                    builder.Append(EscapeChar(curChar));

                    //startSegmentIndex tiếp theo
                    startSegmentIndex = i + 1;
                }
            }

            if (startSegmentIndex == 0)
                builder.Append(raw); //Không có ký tự cần escape
            else
                builder.Append(raw, startSegmentIndex, raw.Length - startSegmentIndex);

            builder.Append('"');
        }

        private static bool IsCharNeedEscape(char c)
        {
            return c == '"' ||
                   c == '\\' ||
                   c == '\b' ||
                   c == '\f' ||
                   c == '\n' ||
                   c == '\r' ||
                   c == '\t' ||
                   c < 32;
        }

        private static string EscapeChar(char c)
        {
            switch (c)
            {
                case '"':
                    return "\\\"";
                
                case '\\':
                    return "\\\\";
                
                case '\t':
                    return "\\t";
                
                case '\n':
                    return "\\n";
                
                case '\r':
                    return "\\r";
                
                case '\b':
                    return "\\b";
                
                case '\f':
                    return "\\f";
            }

            if (c < 32) //Các ký tự điều khiển
                return $"\\u{(int)c:x4}";
            
            throw new Exception("Char is not need escape: " + c);
        }
    }
}