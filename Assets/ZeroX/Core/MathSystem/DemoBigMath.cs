using System.Numerics;
using UnityEngine;

namespace ZeroX.MathSystem
{
    public class DemoBigMath : MonoBehaviour
    {
        [SerializeField] private int numberBuy = 3;



        [ContextMenu("Calculate")]
        public void Calculate()
        {
            //Debug.Log(MathBig.Round("3.91629422", 0));
            //Debug.Log(MathBig.Divide("11.7487744948", "3"));
            //Debug.Log(BigInteger.Ma);
            Debug.Log("Raw: " + GetBuyCharacterPrice(numberBuy));
            Debug.Log("-----");
            Debug.Log("Pro: " + CalculateBuyPrice(numberBuy));
            //Debug.Log(MathBig.Multiply("2.5", "3"));
            //BigInteger price = CalculateBuyPrice(numberBuy);
            //Debug.Log(price);
        }

        private string CalculateBuyPrice(int numberBuy)
        {
            //result = a * b / 3
            //50 * Round(result)
            
            string a = (numberBuy + 1).ToString();
            string b = MathBig.Pow("1.08", (numberBuy + 11).ToString());

            string result = MathBig.Multiply(a, b);
            result = MathBig.Divide(result, "3");
            result = MathBig.Round(result, 0);
            Debug.Log("ProDebug: " + result);
            result = MathBig.Multiply("50", result);

            return result;
        }
        
        public float GetBuyCharacterPrice(int numberBuy)
        {
            float a = numberBuy + 1;
            float b = Mathf.Pow(1.08f, numberBuy + 11);

            float result = a * b;
            result = result / 3;
            result = Mathf.Round(result);
            Debug.Log("RawDebug: " + result);
            result = 50 * result;

            return result;

            // float x = (numberBuy + 1) * Mathf.Pow(1.08f, numberBuy + 11) / 3;
            // return 50 * Mathf.Round(x);
        }
    }
}