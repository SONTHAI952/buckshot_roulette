using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.Editors.NoteEditors
{
    [CustomEditor(typeof(Note))]
    public class NoteEditor : Editor
    {
        private SerializedObject noteSo;
        private SerializedProperty contentSp;
        
        
        private void OnEnable()
        {
            noteSo = serializedObject;
            contentSp = noteSo.FindProperty("content");
        }

        public override void OnInspectorGUI()
        {
            noteSo.Update();
            
            
            Note note = (Note) target;

            if (note.editing == false)
            {
                DrawContent();
                DrawButtonEdit();
            }
            else
            {
                DrawContentEditing();
                DrawButtonEndEdit();
            }

            
            noteSo.ApplyModifiedProperties();
        }

        private void DrawButtonEdit()
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Edit", GUILayout.Width(35)))
            {
                Note note = (Note) target;
                note.editing = true;
            }
            
            GUILayout.EndHorizontal();
        }
        
        private void DrawButtonEndEdit()
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OK", GUILayout.Width(35)))
            {
                Note note = (Note) target;
                note.editing = false;
            }
            
            GUILayout.EndHorizontal();
        }

        private void DrawContent()
        {
            Color oldColor = GUI.contentColor;
            GUI.contentColor = Color.green;
            
            GUIStyle textStyle = new GUIStyle(EditorStyles.label);
            textStyle.wordWrap = true;
            
            EditorGUILayout.LabelField(contentSp.stringValue, textStyle);

            GUI.contentColor = oldColor;
        }

        protected void DrawContentEditing()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textArea);
            textStyle.wordWrap = true;
            
            contentSp.stringValue = EditorGUILayout.TextArea(contentSp.stringValue, textStyle, GUILayout.MinHeight(100));
        }
    }
}