using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors
{
    [CustomPropertyDrawer(typeof(ZListDrawerAttribute))]
    public class ZListDrawerAttributeEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = label.text.Replace("Element ", "");
            EditorGUI.BeginProperty(position, label, property);
            
            Rect labelPos = DrawLabel(position, property, label);
            
            Rect fieldPos = position;
            fieldPos.x += labelPos.width + 2;
            fieldPos.width -= labelPos.width + 2;
            EditorGUI.PropertyField(fieldPos, property, GUIContent.none, true);
            
            EditorGUI.EndProperty();
        }

        private Rect DrawLabel(Rect totalPos, SerializedProperty property, GUIContent label)
        {
            Rect labelPos = totalPos;
            labelPos.width = ((ZListDrawerAttribute)attribute).labelWidth;
            labelPos.y += 1.5f;
           
            var style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.UpperLeft;
            EditorGUI.LabelField(labelPos, label, style);

            return labelPos;
        }
    }
}