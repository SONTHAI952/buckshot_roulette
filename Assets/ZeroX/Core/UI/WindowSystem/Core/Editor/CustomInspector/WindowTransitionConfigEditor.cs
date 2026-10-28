using UnityEditor;
using UnityEngine;

namespace ZeroX.WindowSystem.Editors
{
    [CustomPropertyDrawer(typeof(WindowTransitionConfig))]
    public class WindowTransitionConfigEditor : PropertyDrawer
    {
        private const int indentWidth = 15;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            if (property.isExpanded == false)
                return lineHeight;

            
            if (property.FindPropertyRelative("animType").intValue == (int)WindowAnimType.None)
                return lineHeight * 2;
            
            
            return lineHeight * 7;
        }

        public override void OnGUI(Rect masterPosition, SerializedProperty property, GUIContent label)
        {
            masterPosition = EditorGUI.IndentedRect(masterPosition);
            
            DrawToggle(masterPosition, property, label);
            if (property.isExpanded)
                DrawFields(masterPosition, property);
        }

        private void DrawToggle(Rect masterPosition, SerializedProperty property, GUIContent label)
        {
            Rect position = masterPosition;
            position.height = EditorGUIUtility.singleLineHeight;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
        }
        
        private void DrawChildProperty(Rect masterPosition, SerializedProperty sp, int fieldIndex)
        {
            Rect position = masterPosition;
            position.x += indentWidth;
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (fieldIndex + 1);
            position.width -= indentWidth;
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(position, sp);
        }

        private void DrawFields(Rect masterPosition, SerializedProperty property)
        {
            //Anim Type
            SerializedProperty animTypeSp = property.FindPropertyRelative("animType");
            DrawChildProperty(masterPosition, animTypeSp, 0);
            WindowAnimType windowAnimType = (WindowAnimType)animTypeSp.intValue;

            if (windowAnimType == WindowAnimType.None)
            {
            }
            else if(windowAnimType == WindowAnimType.Pure)
                DrawChildProperty(masterPosition, property.FindPropertyRelative("animPureId"), 1);
            
            else if(windowAnimType == WindowAnimType.MonoBehaviour)
                DrawChildProperty(masterPosition, property.FindPropertyRelative("animMono"), 1);
            
            else if(windowAnimType == WindowAnimType.ScriptableObject)
                DrawChildProperty(masterPosition, property.FindPropertyRelative("animSO"), 1);
            
            else
                Debug.LogError("Not code for WindowAnimType: " + windowAnimType);

            
            
            if (windowAnimType != WindowAnimType.None)
            {
                DrawChildProperty(masterPosition, property.FindPropertyRelative("openDuration"), 3);
                DrawChildProperty(masterPosition, property.FindPropertyRelative("closeDuration"), 4);
                DrawChildProperty(masterPosition, property.FindPropertyRelative("ignoreTimeScale"), 5);
            }
        }
    }
}