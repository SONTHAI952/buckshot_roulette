using UnityEngine;

namespace ZeroX.ProgressSystem
{
    public static class ProgressUtility
    {
        public static float GetTargetProgressValue(Progressor progressor, TargetProgress targetProgress)
        {
            switch (targetProgress)
            {
                case TargetProgress.Progress:
                    return progressor.Progress;
                case TargetProgress.AnimateProgress:
                    return progressor.AnimateProgress;
                case TargetProgress.InverseProgress:
                    return progressor.InverseProgress;
                case TargetProgress.InverseAnimateProgress:
                    return progressor.InverseAnimateProgress;
                default:
                {
                    Debug.LogError("Not code for targetProgress: " + targetProgress);
                    return progressor.Progress;
                }
            }
        }
        
        public static float GetTargetVariableValue(Progressor progressor, TargetVariable targetVariable)
        {
            float value = 0;
            switch (targetVariable)
            {
                case TargetVariable.MinValue:
                {
                    value = progressor.MinValue;
                    break;
                }
                case TargetVariable.MaxValue:
                {
                    value = progressor.MaxValue;
                    break;
                }
                case TargetVariable.Value:
                {
                    value = progressor.Value;
                    break;
                }
                case TargetVariable.AnimateValue:
                {
                    value = progressor.AnimateValue;
                    break;
                }
                case TargetVariable.Progress:
                {
                    value = progressor.Progress;
                    break;
                }
                case TargetVariable.AnimateProgress:
                {
                    value = progressor.AnimateProgress;
                    break;
                }
                case TargetVariable.InverseProgress:
                {
                    value = progressor.InverseProgress;
                    break;
                }
                case TargetVariable.InverseAnimateProgress:
                {
                    value = progressor.InverseAnimateProgress;
                    break;
                }
                default:
                {
                    Debug.LogError("Not code for targetVariable: " + targetVariable);
                    value = progressor.Progress;
                    break;
                }
            }

            return value;
        }

        public static float Round(float value, RoundType roundType, int decimalPlacesLength)
        {
            long exponent = 1;
            for (int i = 0; i < decimalPlacesLength; i++)
            {
                exponent *= 10;
            }
            
            if (roundType == RoundType.Nearest)
                return Mathf.Round(value * exponent) / exponent;
            
            if (roundType == RoundType.Floor)
                return Mathf.Floor(value * exponent) / exponent;
            
            if (roundType == RoundType.Ceil)
                return Mathf.Ceil(value * exponent) / exponent;

            Debug.LogError("Not support round type: " + roundType);
            return value;
        }
    }
}