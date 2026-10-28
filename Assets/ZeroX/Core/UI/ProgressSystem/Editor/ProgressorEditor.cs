using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.ProgressSystem.EditorNS
{
    [CustomEditor(typeof(Progressor))]
    public class ProgressorEditor : Editor
    {
        private Progressor progressor;
        private SerializedObject progressorSo;
        private SerializedProperty minValueSp;
        private SerializedProperty maxValueSp;
        private SerializedProperty valueSp;
        private SerializedProperty animateValueModeSp;
        private SerializedProperty animateValueSp;
        private SerializedProperty animateValueSpeedSp;
        private SerializedProperty wholeNumbersSp;
        private SerializedProperty listProgressTargetSp;
        
        private SerializedProperty onValueChangedSp;
        private SerializedProperty onProgressChangedSp;
        private SerializedProperty onAnimateValueChangedSp;
        private SerializedProperty onAnimateProgressChangedSp;

        private const float labelWidth = 140;
        private static bool foldoutEvents = false;

        void SaveChanges()
        {
            var gameObject = ((MonoBehaviour) target).gameObject;
            if(string.IsNullOrEmpty(gameObject.scene.name) == false)
                EditorUtility.SetDirty(target);
                
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            try
            {
                PrefabUtility.SavePrefabAsset(gameObject);
            }
            catch
            { }
        }
        
        void RecordUndo(string commandName)
        {
            Undo.RecordObject(target, commandName);
        }
        
        
        
        public override void OnInspectorGUI()
        {
            progressor = (Progressor) target;
            progressorSo = serializedObject;
            minValueSp = progressorSo.FindProperty("minValue");
            maxValueSp = progressorSo.FindProperty("maxValue");
            valueSp = progressorSo.FindProperty("value");
            animateValueModeSp = progressorSo.FindProperty("animateValueMode");
            animateValueSp = progressorSo.FindProperty("animateValue");
            animateValueSpeedSp = progressorSo.FindProperty("animateValueSpeed");
            wholeNumbersSp = progressorSo.FindProperty("wholeNumbers");
            listProgressTargetSp = progressorSo.FindProperty("listProgressTarget");
            
            onValueChangedSp = progressorSo.FindProperty("onValueChanged");
            onProgressChangedSp = progressorSo.FindProperty("onProgressChanged");
            onAnimateValueChangedSp = progressorSo.FindProperty("onAnimateValueChanged");
            onAnimateProgressChangedSp = progressorSo.FindProperty("onAnimateProgressChanged");
            
            
            
            
            serializedObject.Update();
            
            
            DrawValueGroup();
            GUILayout.Space(10);
            DrawAnimateGroup();
            GUILayout.Space(10);
            DrawWholeNumbers();
            DrawListProgressTarget();
            DrawEvents();
            
            serializedObject.ApplyModifiedProperties();
        }

        #region Layout Utility

        static void DrawLabel(SerializedProperty sp, float width)
        {
            var labelStyle = sp.prefabOverride ? EditorStyles.boldLabel : EditorStyles.label;
            GUILayout.Label(sp.displayName, labelStyle, GUILayout.Width(width));
        }

        #endregion

        void DrawValueGroup()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            DrawMinMax();
            DrawValue();
            GUILayout.EndVertical();
        }

        void DrawMinMax()
        {
            GUILayout.BeginHorizontal();
            DrawMinValue();
            DrawProgressInfo();
            DrawMaxValue();
            GUILayout.EndHorizontal();
        }

        void DrawMinValue()
        {
            DrawLabel(minValueSp, labelWidth);

            float newMinValue = EditorGUILayout.FloatField(minValueSp.floatValue, GUILayout.Width(40));
            if (newMinValue != progressor.MinValue)
            {
                RecordUndo("Change Min Value");
                progressor.MinValue = newMinValue;
                SaveChanges();
            }
        }
        
        void DrawMaxValue()
        {
            DrawLabel(maxValueSp, 62);
            
            float newMaxValue = EditorGUILayout.FloatField(maxValueSp.floatValue, GUILayout.Width(50));
            if (newMaxValue != progressor.MaxValue)
            {
                RecordUndo("Change Max Value");
                progressor.MaxValue = newMaxValue;
                SaveChanges();
            }
        }

        void DrawProgressInfo()
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.MiddleCenter;
            
            float value = valueSp.floatValue;
            float percent = progressor.Progress * 100f;
            GUILayout.Label(string.Format("{0} ({1:0.##}%)", value, percent), style);
        }

        void DrawValue()
        {
            GUILayout.BeginHorizontal();
            DrawLabel(valueSp, labelWidth);
            
            float newValue = EditorGUILayout.Slider(valueSp.floatValue, minValueSp.floatValue, maxValueSp.floatValue);
            
            if (newValue != valueSp.floatValue)
            {
                RecordUndo("Change Value");

                if (EditorApplication.isPlaying)
                    progressor.Value = newValue;
                else
                    ProgressorReflectionUtility.SetValue(progressor, newValue);
                
                SaveChanges();
            }

            GUILayout.EndHorizontal();
        }

        #region Animate Value

        void DrawAnimateGroup()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            
            DrawAnimateValueMode();
            if (progressor.AnimateValueMode != AnimateValueMode.Instant)
            {
                DrawAnimateValue();
                DrawAnimateValueSpeed();
            }
            
            GUILayout.EndVertical();
        }
        
        void DrawAnimateValue()
        {
            GUILayout.BeginHorizontal();
            DrawLabel(animateValueSp, labelWidth);
            
            float newAnimateValue = EditorGUILayout.Slider(animateValueSp.floatValue, minValueSp.floatValue, maxValueSp.floatValue);
            
            if (newAnimateValue != animateValueSp.floatValue)
            {
                RecordUndo("Change Animate Value");
                
                if (EditorApplication.isPlaying)
                    progressor.AnimateValue = newAnimateValue;
                else
                    ProgressorReflectionUtility.SetAnimateValue(progressor, newAnimateValue);
                
                SaveChanges();
            }

            GUILayout.EndHorizontal();
        }

        void DrawAnimateValueMode()
        {
            GUILayout.BeginHorizontal();
            DrawLabel(animateValueModeSp, labelWidth);
            
            var newAnimateMode = (AnimateValueMode)EditorGUILayout.EnumPopup(progressor.AnimateValueMode);
            if (newAnimateMode != progressor.AnimateValueMode)
            {
                RecordUndo("Change Animate Value Mode");
                progressor.AnimateValueMode = newAnimateMode;
                SaveChanges();
            }
            GUILayout.EndHorizontal();
        }

        void DrawAnimateValueSpeed()
        {
            GUILayout.BeginHorizontal();
            DrawLabel(animateValueSpeedSp, labelWidth);
            
            var newAnimateValueSpeed = EditorGUILayout.FloatField(animateValueSpeedSp.floatValue);
            if (newAnimateValueSpeed != animateValueSpeedSp.floatValue)
            {
                RecordUndo("Change Animate Value Speed");
                progressor.AnimateValueSpeed = newAnimateValueSpeed;
                SaveChanges();
            }
            GUILayout.EndHorizontal();
        }
        
        #endregion

        void DrawWholeNumbers()
        {
            GUILayout.BeginHorizontal();
            DrawLabel(wholeNumbersSp, labelWidth);
            
            bool newWholeNumbers = EditorGUILayout.Toggle(wholeNumbersSp.boolValue);
            if (newWholeNumbers != wholeNumbersSp.boolValue)
            {
                RecordUndo("Change Whole Numbers");
                progressor.WholeNumbers = newWholeNumbers;
                SaveChanges();
            }
            GUILayout.EndHorizontal();
        }

        void DrawListProgressTarget()
        {
            EditorGUILayout.PropertyField(listProgressTargetSp);
        }

        void DrawEvents()
        {
            var foldoutStyle = new GUIStyle(EditorStyles.foldout);
            bool prefabOverride = onValueChangedSp.prefabOverride || onProgressChangedSp.prefabOverride || onAnimateValueChangedSp.prefabOverride || onAnimateProgressChangedSp.prefabOverride;
            foldoutStyle.fontStyle = prefabOverride ? FontStyle.Bold : FontStyle.Normal;
            foldoutEvents = EditorGUILayout.Foldout(foldoutEvents, "Events", foldoutStyle);
            if(foldoutEvents == false)
                return;
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginVertical();

            EditorGUILayout.PropertyField(onValueChangedSp);
            EditorGUILayout.PropertyField(onProgressChangedSp);
            EditorGUILayout.PropertyField(onAnimateValueChangedSp);
            EditorGUILayout.PropertyField(onAnimateProgressChangedSp);
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}