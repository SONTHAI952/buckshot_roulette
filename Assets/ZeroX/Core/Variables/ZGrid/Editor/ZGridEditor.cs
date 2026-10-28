using UnityEditor;
using UnityEngine;

namespace ZeroX.ZGridEditors
{
    [CustomPropertyDrawer(typeof(ZGridBase), true)]
    public class ZGridEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Label
            EditorGUI.BeginProperty(position, label, property);
            Rect buttonEditRect = EditorGUI.PrefixLabel(position, label, property.prefabOverride ? EditorStyles.boldLabel : EditorStyles.label);
            EditorGUI.EndProperty();
            
            
            //Button Edit
            if (GUI.Button(buttonEditRect, "Edit Grid"))
            {
                ZGridEditorWindow.GetInstance().Open(property);
            }
        }
    }
}