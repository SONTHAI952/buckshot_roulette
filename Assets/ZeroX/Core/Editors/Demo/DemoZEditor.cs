using UnityEngine;

namespace ZeroX.Editors.Demo
{
    public enum DemoOption
    {
        A, B, C, D, E, F, G ,H
    }
    
    public class DemoZEditor : MonoBehaviour
    {
        public DemoOption option = DemoOption.A;
        
        [ShowFieldIf(nameof(IsOptionA))]
        public string aValue;
        
        [ShowFieldIf("OptionProperty", DemoOption.B)]
        public string bValue;
        //
        // [EnableFieldIf("option", DemoOption.C)]
        // public string cValue;
        //
        // [DisableFieldIf("option", DemoOption.D)]
        // public string dValue;
        
        [ShowFieldIf("option", DemoOption.E)]
        public string eValue;
        //
        // [HideFieldIf("option", DemoOption.F)]
        // public string fValue; 
        //
        // [ShowFieldIf("option", DemoOption.G)]
        // public string gValue; 
        //
        // [ShowFieldIf("option", DemoOption.H)]
        // public string hValue; 
        
        [Header("----------")]
        public bool useRigidbody = false;
        
        [ShowFieldIf("useRigidbody")]
        public Rigidbody rigidbody;

        [HideFieldIf(nameof(rigidbody), null)]
        public int rigidbodyConfig;
        
        public int t2;
        
        public int t3;

        
        private bool IsOptionA => option == DemoOption.A;
        private DemoOption OptionProperty => option;

        [ContextMenu("Test")]
        public void Test()
        {
           
            
        }

    }
}