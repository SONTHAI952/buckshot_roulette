using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using ZeroX.DataTableSystem.Editors;
#endif

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract partial class SoTable<TTable, TRow>
    {
        private static readonly HashSet<string> hashSetErrorMessage = new HashSet<string>();
        
        
        
#if UNITY_EDITOR
        private void Editor_ImportFromCsv(List<string> listField, List<List<string>> listCsvRow)
        {
            TransformCsv(listField, listCsvRow, out var listFieldOutput, out var listCsvRowOutput);


            Editor_StartRecordErrorSeason();
            
            try
            {
                var rowType = typeof(TRow);
                if (rowType.IsSubclassOf(typeof(ScriptableObject)) == false)
                {
                    EditorNonSO_ImportFromCsv(listField, listCsvRow);
                }
                else
                {
                    EditorSO_ImportFromCsv(listField, listCsvRow);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            Editor_EndRecordErrorSeason();
        }
#endif



        #region Record Error

        
#if UNITY_EDITOR
        private static void Editor_StartRecordErrorSeason()
        {
            hashSetErrorMessage.Clear();
        }
        
        
        private static void Editor_EndRecordErrorSeason()
        {
            foreach (var message in hashSetErrorMessage)
            {
                Debug.LogError(message);
            }
            
            hashSetErrorMessage.Clear();
        }
#endif
        
        private static void RecordError(string message)
        {
#if UNITY_EDITOR
            hashSetErrorMessage.Add(message);
#endif
        }

        #endregion
    }
}