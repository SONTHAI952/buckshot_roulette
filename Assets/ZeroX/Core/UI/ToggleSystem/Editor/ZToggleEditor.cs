/*using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.UI.ToggleSystem.Editors
{
    [CustomEditor(typeof(ZToggle))]
    public class ZToggleEditor : Editor
    {
        private ZToggle toggle;
        private SerializedObject toggleSo;
        private SerializedProperty isOnSp;
        private SerializedProperty buttonSp;
        private SerializedProperty listToggleTargetSp;
        private SerializedProperty onValueChangedSp;
        
        
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
            toggle = (ZToggle) target;
            toggleSo = serializedObject;
            isOnSp = toggleSo.FindProperty("isOn");
            buttonSp = toggleSo.FindProperty("button");
            listToggleTargetSp = toggleSo.FindProperty("listToggleTarget");
            onValueChangedSp = toggleSo.FindProperty("onValueChanged");
            
            
            
            serializedObject.Update();
            
            DrawIsOn();
            DrawButton();
            DrawListToggleTarget();
            DrawOnValueChanged();
            
            serializedObject.ApplyModifiedProperties();
        }

        void DrawIsOn()
        {
            bool oldValue = isOnSp.boolValue;
            bool newValue = EditorGUILayout.Toggle(isOnSp.displayName, oldValue);
            if (newValue != oldValue)
            {
                RecordUndo("Change IsOn");
                isOnSp.boolValue = newValue;
                SaveChanges();
            
                if (EditorApplication.isPlaying)
                {
                    ZToggleReflectionUtility.Internal_SetIsOn(toggle, newValue);
                }
            }
        }

        void DrawButton()
        {
            EditorGUILayout.PropertyField(buttonSp);
        }

        void DrawListToggleTarget()
        {
            EditorGUILayout.PropertyField(listToggleTargetSp);
        }

        void DrawOnValueChanged()
        {
            EditorGUILayout.PropertyField(onValueChangedSp);
        }
    }
}*/