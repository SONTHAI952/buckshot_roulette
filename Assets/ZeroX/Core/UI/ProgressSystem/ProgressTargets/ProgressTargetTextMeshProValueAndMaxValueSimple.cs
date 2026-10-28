using TMPro;
using UnityEngine;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressTargets + "Progress Target TextMeshPro Value And Max Value Simple")]
    public class ProgressTargetTextMeshProValueAndMaxValueSimple : ProgressTarget
    {
        [SerializeField] protected TMP_Text textMeshPro;
        public TargetValue targetValue = TargetValue.AnimateValue;
        [Tooltip("0 is value, 1 is max value")]
        public string textFormat = "{0:0}/{1:0}";
        
        [Header("Modify")]
        public RoundType roundType = RoundType.Nearest;
        public int decimalPlacesLength = 1;
        
        
        
        
        public override void UpdateTarget(Progressor progressor)
        {
            float value = targetValue == TargetValue.Value ? progressor.Value : progressor.AnimateValue;
            value = ProgressUtility.Round(value, roundType, decimalPlacesLength);
            
            float maxValue = progressor.MaxValue;
            textMeshPro.text = string.Format(textFormat, value, maxValue);
        }

        protected override void Reset()
        {
            base.Reset();
            textMeshPro = GetComponent<TMP_Text>();
        }
    }
}