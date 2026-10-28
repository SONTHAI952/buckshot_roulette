using TMPro;
using UnityEngine;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressTargets + "Progress Target TextMeshPro")]
    public class ProgressTargetTextMeshPro : ProgressTarget
    {
        [SerializeField] protected TMP_Text textMeshPro;
        public TargetVariable targetVariable = TargetVariable.AnimateProgress;
        public string textFormat = "{0:0}%";
        
        [Header("Modify")]
        public float multiplier = 100f;
        public RoundType roundType = RoundType.Nearest;
        public int decimalPlacesLength = 1;

        
        
        public override void UpdateTarget(Progressor progressor)
        {
            float value = ProgressUtility.GetTargetVariableValue(progressor, targetVariable);
            value *= multiplier;
            value = ProgressUtility.Round(value, roundType, decimalPlacesLength);
            
            textMeshPro.text = string.Format(textFormat, value);
        }

        protected override void Reset()
        {
            base.Reset();
            textMeshPro = GetComponent<TMP_Text>();
        }
    }
}