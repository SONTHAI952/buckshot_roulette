using UnityEngine;
#if ZEROX_SDK_NICE_VIBRATION
using Lofelt.NiceVibrations;
#endif

namespace ZeroX.HapticSystem
{
    public static class HapticManager
    {
        //Const
        private const float DefaultAvoidResonanceTime = 0.035f;
        
        //Temp
        private static float lastTimePlayHaptic = -999f;
        
        
        
        
        
        public static bool EnableHaptic
        {
            get => HapticSettingDI.EnableHaptic;
            set => HapticSettingDI.EnableHaptic = value;
        }
        
        
        
        public static void PlayLightImpact(float avoidResonanceTime = DefaultAvoidResonanceTime)
        {
#if ZEROX_SDK_NICE_VIBRATION
            if(HapticSettingDI.EnableHaptic == false)
                return;
            
            if(Time.unscaledTime - lastTimePlayHaptic < avoidResonanceTime)
                return;
            
            lastTimePlayHaptic = Time.unscaledTime;
            HapticController.fallbackPreset = HapticPatterns.PresetType.LightImpact;
            HapticPatterns.PlayConstant(0.15f, 1, 0.035f);
#endif
        }

        public static void PlayMediumImpact(float avoidResonanceTime = DefaultAvoidResonanceTime)
        {
#if ZEROX_SDK_NICE_VIBRATION
            if(HapticSettingDI.EnableHaptic == false)
                return;
            
            if(Time.unscaledTime - lastTimePlayHaptic < avoidResonanceTime)
                return;
            
            lastTimePlayHaptic = Time.unscaledTime;
            HapticController.fallbackPreset = HapticPatterns.PresetType.LightImpact;
            HapticPatterns.PlayConstant(0.5f, 1, 0.0525f);
#endif
        }
        
        public static void PlayHeavyImpact(float avoidResonanceTime = DefaultAvoidResonanceTime)
        {
#if ZEROX_SDK_NICE_VIBRATION
            if(HapticSettingDI.EnableHaptic == false)
                return;
            
            if(Time.unscaledTime - lastTimePlayHaptic < avoidResonanceTime)
                return;
            
            lastTimePlayHaptic = Time.unscaledTime;
            HapticController.fallbackPreset = HapticPatterns.PresetType.LightImpact;
            HapticPatterns.PlayConstant(1, 1, 0.07f);
#endif
        }
    }
}