using System.Globalization;
using UnityEngine;

namespace ZeroX.ZNumberDemo
{
    public class DemoZNumber : MonoBehaviour
    {
        public double baseValue;
        public long exponent;

        [TextArea(5, 10)] [SerializeField] private string output;

        [ContextMenu("Test")]
        public void Test()
        {
            ZNumber numberA = new ZNumber(2.2m, 1);
            ZNumber numberB = new ZNumber(2m, 2);
            
            var numberC = numberA / numberB;
            
            output = ZNumber.CalculateMagnitude(000.00456m).ToString();
        }
    }
}