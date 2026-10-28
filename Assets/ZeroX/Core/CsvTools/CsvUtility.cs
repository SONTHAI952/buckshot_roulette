using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ZeroX.CsvTools
{
    public static class CsvUtility
    {
        private static readonly char lineSeparator = '\n';
        
        public static List<List<string>> ConvertCsvToListCsvRow(string csvText, char fieldSeparator, char quoteCharacter, bool trimField)
        {
            csvText = csvText.Replace("\r\n", "\n");
            List<List<string>> listRow = new List<List<string>>();
            int startIndex = 0;
            while (true)
            {
                List<string> listCell = new List<string>();
                while (true)
                {
                    if(startIndex >= csvText.Length)
                        break;
                    int endIndex;
                    char startChar = csvText[startIndex];
                    string cellValue = null;
                    if (startChar != quoteCharacter)
                    {
                        endIndex = FindEndIndexNormal(ref csvText, startIndex, fieldSeparator);
                        cellValue = csvText.Substring(startIndex, endIndex - startIndex);
                        cellValue = cellValue.Replace("\"\"", "\"");
                        //cellValue = RemoveEndOfLine(cellValue);
                        
                        if (trimField)
                            cellValue = cellValue.Trim();
                    }
                    else
                    {
                        startIndex++;
                        endIndex = FindEndIndexQuote(ref csvText, startIndex, fieldSeparator, quoteCharacter);
                        cellValue = csvText.Substring(startIndex, endIndex - startIndex);
                        cellValue = cellValue.Replace("\"\"", "\"");
                        //cellValue = RemoveEndOfLine(cellValue);
                        //cellValue = cellValue.Remove(cellValue.Length - 1); //Remove end quote character
                        if (trimField)
                            cellValue = cellValue.Trim();

                        endIndex++; //Để cho qua quoteCharacter. Vì trong trường hợp startChar thì quoteCharacter thì ký tự cuối chắc chắn cùng phải là quoteCharacter
                    }

                    listCell.Add(cellValue);

                    if(endIndex == csvText.Length)
                    {
                        startIndex = endIndex;
                        break;
                    }
                    
                    if(csvText[endIndex] == lineSeparator)
                    {
                        startIndex = endIndex + 1;
                        break;
                    }

                    startIndex = endIndex + 1;
                }
                
                listRow.Add(listCell);
                
                if(startIndex >= csvText.Length)
                    break;
            }
            
            if (listRow.Count > 0)
            {
                //Đây là trường hợp đặc biệt khi cell cuối trống, lại không có ký tự endOfLine để xác định kết thúc nên sẽ gây ra việc thiếu 1 cell cuối cùng
                if(csvText[csvText.Length - 1] == fieldSeparator)
                    listRow[listRow.Count - 1].Add("");
            }

            return listRow;
        }

        static int FindEndIndexNormal(ref string csvText, int startIndex, char fieldSeparator)
        {
            for (int i = startIndex; i < csvText.Length; i++)
            {
                char c = csvText[i];
                if (c == fieldSeparator || c == lineSeparator)
                {
                    return i;
                }
            }

            return csvText.Length;
        }

        static int FindEndIndexQuote(ref string csvText, int startIndex, char fieldSeparator, char quoteCharacter)
        {
            for (int i = startIndex; i < csvText.Length; i++)
            {
                char c = csvText[i];
                if (c == quoteCharacter)
                {
                    int numberQuoteCharacter = CountPreviousQuoteCharacter(ref csvText, i, quoteCharacter, startIndex);
                    if (numberQuoteCharacter % 2 == 0)
                        return i;
                }
            }

            return csvText.Length;
        }

        /// <summary>
        /// Đếm từ i-1 đổ lại. Ko tính char tại i
        /// </summary>
        static int CountPreviousQuoteCharacter(ref string csvText, int index, char quoteCharacter, int previousLimitIndex)
        {
            if (index < csvText.Length - 1 && csvText[index + 1] == quoteCharacter) //Nếu ký tự tiếp theo mà là quoteCharacter thì ko cần đếm 
                return -1;
            
            int total = 0;
            for (int i = index - 1; i >= previousLimitIndex; i--)
            {
                if (csvText[i] == quoteCharacter)
                    total++;
                else
                    break;
            }
            
            return total;
        }

        static string RemoveEndOfLine(string text)
        {
            if (text == null)
                return null;

            if (text.Length == 1)
            {
                if (text[0] == '\n' || text[0] == '\r')
                    return "";
            }

            if (text[text.Length - 1] == '\n')
            {
                if (text[text.Length - 2] == '\r')
                    text = text.Substring(0, text.Length - 2);
                else
                    text = text.Substring(0, text.Length - 1);

                return text;
            }

            if (text[text.Length - 1] == '\r')
            {
                if (text[text.Length - 2] == '\n')
                    text = text.Substring(0, text.Length - 2);
                else
                    text = text.Substring(0, text.Length - 1);

                return text;
            }

            return text;
        }


        public static List<string> ConvertListColumnNameToListFieldName(List<string> listColumnName, bool upperCaseAfterWordSeparator, bool removeWordSeparator)
        {
            var listFieldName = new List<string>();
            
            
            if(listColumnName == null || listColumnName.Count == 0)
                return listFieldName;

            
            foreach (var col in listColumnName)
            {
                StringBuilder sb = new StringBuilder();
                bool needUpperCase = false;
                for (int i = 0; i < col.Length; i++)
                {
                    char c = col[i];
                    if (c == '_' || c == ' ')
                    {
                        if (upperCaseAfterWordSeparator)
                            needUpperCase = true;

                        if (removeWordSeparator == false)
                            sb.Append(c);
                        
                        continue;
                    }

                    
                    if (needUpperCase)
                    {
                        sb.Append(c.ToString().ToUpper());
                        needUpperCase = false;
                    }
                    else
                        sb.Append(c);
                }
                
                listFieldName.Add(sb.ToString());
            }

            return listFieldName;
        }






        #region Export To Clipboard
        
        private static void AddOneCellToStringBuilder(StringBuilder sb, string cell)
        {
            sb.Append("\"");
                    
            for (int i = 0; i < cell.Length; i++)
            {
                char c = cell[i];
                if (c == '\"')
                    sb.Append('\"'); //Nếu là ký tự " thì cần thêm 1 ký tự " nữa liền kề để trở thành ""

                sb.Append(c);
            }
            
            sb.Append("\"");
            sb.Append("\t"); //Ký tự kết thúc một cell
        }
        
        public static void ExportToClipboard(List<List<string>> listRow)
        {
            StringBuilder sb = new StringBuilder();
            
            for (int rowIndex = 0; rowIndex < listRow.Count; rowIndex++)
            {
                var row = listRow[rowIndex];
                
                for (int cellIndex = 0; cellIndex < row.Count; cellIndex++)
                {
                    string cell = row[cellIndex];
                    AddOneCellToStringBuilder(sb, cell);
                }

                sb.Append("\r\n"); //Ký tự kết thức một dòng
            }

            GUIUtility.systemCopyBuffer = sb.ToString();
        }

        public static void ExportOneColumnToClipboard(List<string> column)
        {
            StringBuilder sb = new StringBuilder();
            
            for (int rowIndex = 0; rowIndex < column.Count; rowIndex++)
            {
                var cell = column[rowIndex];
                AddOneCellToStringBuilder(sb, cell);
                
                sb.Append("\r\n"); //Ký tự kết thức một dòng
            }

            GUIUtility.systemCopyBuffer = sb.ToString();
        }
        
        #endregion
    }
}