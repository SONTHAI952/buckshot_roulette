using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace ZeroX.DataTableSystem
{
    public static class CsvDataTransformer_CombineColumn
    {
        public class CombineParameters
        {
            public List<string> inputListFieldName;
            public List<List<string>> inputListCsvRow;
            
            public List<string> outputListFieldName;
            public List<List<string>> outputListCsvRow;
            
            public int fromColumnIndex;
            public int toColumnIndex;
            
            public string alternateFieldName;
            public string jsonKeyForColumnName;
            public string jsonKeyForCellValue;
        }
        
        public static void Combine(CombineParameters parameters)
        {
            if (parameters.toColumnIndex < parameters.fromColumnIndex)
            {
                Debug.LogError("ToColumnIndex cannot smaller fromColumnIndex");
                parameters.outputListFieldName = parameters.inputListFieldName;
                parameters.outputListCsvRow = parameters.inputListCsvRow;
                return;
            }



            parameters.outputListFieldName = CombineFieldNames(parameters.inputListFieldName, parameters.fromColumnIndex, parameters.toColumnIndex, parameters.alternateFieldName);
            parameters.outputListCsvRow = new List<List<string>>();


            //Combine cellValue
            foreach (var inputListCell in parameters.inputListCsvRow)
            {
                List<string> outputListCell = CombineCells(parameters.inputListFieldName, inputListCell, parameters.fromColumnIndex, parameters.toColumnIndex, parameters.jsonKeyForColumnName, parameters.jsonKeyForCellValue);
                parameters.outputListCsvRow.Add(outputListCell);
            }
        }

        private static List<string> CombineFieldNames(List<string> inputListFieldName, int fromColumnIndex, int toColumnIndex, string alternateFieldName)
        {
            List<string> outputListFieldName = new List<string>();
            
            for (int i = 0; i < fromColumnIndex; i++)
            {
                outputListFieldName.Add(inputListFieldName[i]);
            }
            
            outputListFieldName.Add(alternateFieldName);
            
            for (int i = toColumnIndex + 1; i < inputListFieldName.Count; i++)
            {
                outputListFieldName.Add(inputListFieldName[i]);
            }

            return outputListFieldName;
        }

        private static List<string> CombineCells(List<string> inputListFieldName, List<string> inputListCell, int fromColumnIndex, int toColumnIndex, string jsonKeyForColumnName, string jsonKeyForCellValue)
        {
            List<string> outputListCell = new List<string>();
            
            for (int i = 0; i < fromColumnIndex; i++)
            {
                outputListCell.Add(inputListCell[i]);
            }
            
            outputListCell.Add(null); //giữ chỗ
            
            for (int i = toColumnIndex + 1; i < inputListFieldName.Count; i++)
            {
                outputListCell.Add(inputListCell[i]);
            }


            JSONArray jsonArray = new JSONArray();
            for (int i = fromColumnIndex; i <= toColumnIndex; i++)
            {
                JSONObject jsonObject = new JSONObject();
                
                jsonObject[jsonKeyForColumnName] = inputListFieldName[i];
                jsonObject[jsonKeyForCellValue] = inputListCell[i];
                
                jsonArray.Add(jsonObject);
            }

            outputListCell[fromColumnIndex] = jsonArray.ToString();


            return outputListCell;
        }
    }
}