using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.ZGridDemo
{
    public enum TestEnemyType
    {
        EnemyA, EnemyB, EnemyC
    }
    
    [System.Serializable]
    public class TestCellA
    {
        public string name = "";
        public int age = 15;
    }
    
    [System.Serializable]
    public class TestCellB
    {
        public string name = "";
        public TestEnemyType enemyType;
        public int hp;
        public int power;
        public Color color;
        public Material material;
    }
    
    public class DemoZGrid : MonoBehaviour
    {
        public ZGrid<TestCellA> testGridA = new ZGrid<TestCellA>();
        public ZGrid<TestCellB> testGridB = new ZGrid<TestCellB>();
        public ZGrid<string> testGridString = new ZGrid<string>();
        public List<ZGrid<string>> listGrid = new List<ZGrid<string>>();
    }
}