using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ZeroX.DataTableSystem.Demo
{
    public class TestDataTableSystem : MonoBehaviour
    {
        [SerializeField] private TestTable testTable;
        [SerializeField] private List<TestRowSoBase> listRowSo;
        
        [TextArea]
        [SerializeField] private string csv;
        private void Start()
        {
            // TestTable.SetMain(testTable);
            // Debug.Log("Lan 1: " + TestTable.GetRowByIdWithLog("enemy_10"));
            // TestTable.Rows.RemoveAt(0);
            // Debug.Log("Lan 2: " + TestTable.GetRowByIdWithLog("enemy_10"));
            // TestTable.UpdateAfterListRowChange();
            // Debug.Log("Lan 3: " + TestTable.GetRowByIdWithLog("enemy_10"));

            
        }


        class ABase<T1, T2>
        {
            [SerializeField] private string b1;
            [SerializeField] private string b2;
            public string b3;
        }

        class A : ABase<string, int>
        {
            public string c1;
            public string c2;
            public string c3;
        }

        [ContextMenu("Test Row Type")]
        private void TestRowType()
        {
            var typeA = typeof(A);
            var fields = typeA.BaseType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fi in fields)
            {
                Debug.Log(fi);
            }
        }

        [ContextMenu("Test Equal")]
        private void TestEqual()
        {
            object a = null;
            Debug.Log(a.Equals(null));
        }

        [ContextMenu("Test Get Path")]
        public void TestGetPath()
        {
            string path = "Assets/Folder1/Folder2/abc.txt";
            string s = Path.GetFileNameWithoutExtension(path);
            Debug.Log(s);
        }

        [ContextMenu("TestImportFromCsv")]
        public void TestImportFromCsv()
        {
            //Debug.Log("Power: " + TestTableSo.GetRowByIdWithLog("enemy_2").powerLevel);
            //Debug.Log("Power: " + TestTableSo.GetRowByIdWithLog("enemy_2").powerLevel);
        }
    }
}