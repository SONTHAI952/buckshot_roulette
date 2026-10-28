using UnityEngine;
using ZeroX.DataTableSystem.SoTableSystem;

namespace ZeroX.DataTableSystem.Demo
{
    [CreateAssetMenu(menuName = "ZeroX/Data Table System/Demo/Test Table So Two Id")]
    public class TestTableSoTwoId : SoTableTwoId<TestTableSoTwoId, TestRowSoTwoId, TestEnemyType, string>
    {
        protected override string ShortMainPath => "ZeroX/DataTableSystem Demo/Test So Two Id";
        protected override void GetRowId(TestRowSoTwoId row, out TestEnemyType id1, out string id2)
        {
            id1 = row.type;
            id2 = row.id;
        }
    }
}