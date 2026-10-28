using System;
using UnityEditor;
using UnityEditor.UI;


namespace ZeroX.ButtonSystem.Editors
{
    [CustomEditor(typeof(ZButton), true)]
    public class ZButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            //Original Button
            base.OnInspectorGUI();
            
            
            //ZButton
            serializedObject.Update();


            //Draws
            SerializedProperty allSp = serializedObject.FindProperty("clickVfxType");
            EditorGUILayout.PropertyField(allSp);
            
            while (allSp.NextVisible(false))
            {
                EditorGUILayout.PropertyField(allSp);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}