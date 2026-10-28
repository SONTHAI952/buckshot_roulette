using System.Collections.Generic;
using UnityEngine;
using ZeroX.DataTableSystem.SoTableSystem;

namespace ZeroX.DataTableSystem.Demo
{
    [CreateAssetMenu(menuName = "ZeroX/Data Table System/Demo/Test Combine")]
    public class TestTableCombine : SoTableOneId<TestTableCombine, TestRowCombine, int>
    {
        protected override string ShortMainPath => "ZeroX/DataTableSystem Demo/Test Combine";
        protected override int GetRowId(TestRowCombine row)
        {
            return row.scoreThreshold;
        }


        public override void TransformCsv(List<string> listFieldNameInput, List<List<string>> listCsvRowInput, out List<string> listFieldNameOutput, out List<List<string>> listCsvRowOutput)
        {
            //Tạo parameters
            CsvDataTransformer_CombineColumn.CombineParameters parameters = new CsvDataTransformer_CombineColumn.CombineParameters();
            parameters.inputListFieldName = listFieldNameInput;
            parameters.inputListCsvRow = listCsvRowInput;
            
            parameters.alternateFieldName = "listShapeRate";
            parameters.jsonKeyForColumnName = "shapeId";
            parameters.jsonKeyForCellValue = "rate";

            parameters.fromColumnIndex = 1;
            parameters.toColumnIndex = 16;
            
            //Combine
            CsvDataTransformer_CombineColumn.Combine(parameters);

            //Set output
            listFieldNameOutput = parameters.outputListFieldName;
            listCsvRowOutput = parameters.outputListCsvRow;
        }
    }
}