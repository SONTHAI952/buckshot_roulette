using UnityEngine;
using ZeroX.DataTableSystem.SoTableSystem;

namespace ZeroX.DataTableSystem.Demo
{
    [CreateAssetMenu(menuName = "ZeroX/Data Table System/Demo/Test Table So")]
    public class TestTableSo : SoTableOneId<TestTableSo, TestRowSoBase, string>
    {
        protected override string ShortMainPath => "ZeroX/DataTableSystem Demo/Test So";

        protected override string GetRowId(TestRowSoBase row)
        {
            return row.id;
        }
    }
}