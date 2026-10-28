using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroX.AudioSystem.Editors
{
    [CustomEditor(typeof(AudioDatabase))]
    public class AudioDatabaseEditor : Editor
    {
        private AudioDatabase audioDatabase;
        private SerializedObject audioDatabaseSo;
        private SerializedProperty listAudioGroupSp;
        private SerializedProperty audioGroupIdSeqSp;
        
        
        //New Group
        [System.NonSerialized] private bool addingNewGroup = false;
        [System.NonSerialized] private string newGroupName;
        
        //AudioGroup Editor Setting
        private AudioGroupEditorSettingsCollection audioGroupEditorSettings = new AudioGroupEditorSettingsCollection();
        

        public override void OnInspectorGUI()
        {
            audioDatabase = (AudioDatabase) target;
            audioDatabaseSo = serializedObject;
            listAudioGroupSp = audioDatabaseSo.FindProperty(AudioDatabase.fn_listAudioGroup);
            audioGroupIdSeqSp = audioDatabaseSo.FindProperty(AudioDatabase.fn_audioGroupIdSeq);
            
            audioDatabaseSo.Update();
            DrawTitle();
            DrawTopBar();
            DrawAreaAddNewAudioGroup();
            DrawListAudioGroup();

            audioDatabaseSo.ApplyModifiedProperties();
        }


        #region Layout Utility
        
        bool DrawButtonWithBgColor(string text, Color backgroundColor, params GUILayoutOption[] options)
        {
            Color oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            bool click = GUILayout.Button(text, options);
            GUI.backgroundColor = oldBgColor;

            return click;
        }
        
        bool DrawButtonWithBgColor(string text, Color backgroundColor, int width, int height)
        {
            return DrawButtonWithBgColor(text, backgroundColor, GUILayout.Width(width), GUILayout.Height(height));
        }
        
        bool DrawButtonWithContentColor(string text, Color contentColor, params GUILayoutOption[] options)
        {
            Color oldContentColor = GUI.contentColor;
            GUI.contentColor = contentColor;
            bool click = GUILayout.Button(text, options);
            GUI.contentColor = oldContentColor;

            return click;
        }

        bool DrawButtonWithContentColor(string text, Color contentColor, int width, int height)
        {
            return DrawButtonWithContentColor(text, contentColor, GUILayout.Width(width), GUILayout.Height(height));
        }


        #endregion

        void DrawTitle()
        {
            GUILayout.Space(10);
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 20;
            GUILayout.Label("Audio Database", style);
            GUILayout.Space(15);
        }

        void DrawTopBar()
        {
            var topBarStyle = new GUIStyle(EditorStyles.helpBox);
            topBarStyle.padding = new RectOffset(5, 5, 5, 5);
            GUILayout.BeginHorizontal(topBarStyle, GUILayout.Height(30));
            
            DrawButtonAddNewAudioGroup();
            DrawButtonSaveDatabase();
            
            GUILayout.EndHorizontal();
        }

        void DrawButtonAddNewAudioGroup()
        {
            if (addingNewGroup == false)
            {
                if(GUILayout.Button("New Group", GUILayout.Width(100), GUILayout.ExpandHeight(true)))
                {
                    addingNewGroup = true;
                    newGroupName = "";
                }
            }
            else
            {
                if(DrawButtonWithBgColor("New Group", new Color(0f, 0.78f, 0.1f), GUILayout.Width(100), GUILayout.ExpandHeight(true)))
                {
                    addingNewGroup = false;
                    newGroupName = "";
                }
            }
        }

        void DrawButtonSaveDatabase()
        {
            if(GUILayout.Button("Save Database", GUILayout.Width(100), GUILayout.ExpandHeight(true)))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.Refresh();
            }
        }

        void DrawAreaAddNewAudioGroup()
        {
            if(addingNewGroup == false)
                return;
            
            GUILayout.BeginHorizontal();
            
            GUILayout.Label("New Group Name");
            newGroupName = EditorGUILayout.TextField(newGroupName);

            bool oldEnable = GUI.enabled;
            GUI.enabled = string.IsNullOrWhiteSpace(newGroupName) == false && CheckExistAudioGroupName(newGroupName) == false;
            bool clickAdd = DrawButtonWithContentColor("✓", Color.green, 20, 19);
            GUI.enabled = oldEnable;
                
            bool clickCancel = DrawButtonWithContentColor("✕", Color.red, 20, 19);
            
            GUILayout.EndHorizontal();

            if (clickCancel)
            {
                addingNewGroup = false;
            }

            if (clickAdd)
            {
                audioGroupIdSeqSp.longValue++;
                while (CheckExistAudioGroupId(audioGroupIdSeqSp.longValue))
                {
                    audioGroupIdSeqSp.longValue++;
                }
                long newAudioGroupId = audioGroupIdSeqSp.longValue;
                
                listAudioGroupSp.InsertArrayElementAtIndex(listAudioGroupSp.arraySize);
                var audioGroupSp = listAudioGroupSp.GetArrayElementAtIndex(listAudioGroupSp.arraySize - 1);

                audioGroupSp.FindPropertyRelative(AudioGroup.fn_id).longValue = newAudioGroupId;
                audioGroupSp.FindPropertyRelative(AudioGroup.fn_name).stringValue = newGroupName;

                addingNewGroup = false;
            }
        }

        bool CheckExistAudioGroupName(string audioGroupName)
        {
            for (int i = 0; i < listAudioGroupSp.arraySize; i++)
            {
                var audioGroupSp = listAudioGroupSp.GetArrayElementAtIndex(i);
                var audioGroupNameSp = audioGroupSp.FindPropertyRelative(AudioGroup.fn_name);
                if (audioGroupNameSp.stringValue.ToLower() == audioGroupName.ToLower())
                    return true;
            }

            return false;
        }

        bool CheckExistAudioGroupId(long audioGroupId)
        {
            for (int i = 0; i < listAudioGroupSp.arraySize; i++)
            {
                var audioGroupSp = listAudioGroupSp.GetArrayElementAtIndex(i);
                var audioGroupIdSp = audioGroupSp.FindPropertyRelative(AudioGroup.fn_id);
                if (audioGroupIdSp.longValue == audioGroupId)
                    return true;
            }

            return false;
        }
        
        void DrawListAudioGroup()
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontSize = 12;
            GUILayout.Label("Groups", labelStyle);
            GUILayout.BeginVertical();

            int indexWantDelete = -1;
            for (int i = 0; i < listAudioGroupSp.arraySize; i++)
            {
                var audioGroupSp = listAudioGroupSp.GetArrayElementAtIndex(i);

                bool wantDelete = false;
                DrawAudioGroup(audioGroupSp, ref wantDelete);
                if (wantDelete)
                    indexWantDelete = i;
            }
            GUILayout.EndVertical();

            if (indexWantDelete >= 0)
            {
                listAudioGroupSp.DeleteArrayElementAtIndex(indexWantDelete);
            }
        }

        public static float StandardVerticalSpacing
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
                return 2f;
#else
					return EditorGUIUtility.standardVerticalSpacing;
#endif
            }
        }

        void DrawAudioGroup(SerializedProperty audioGroupSp, ref bool wantDelete)
        {
            var idSp = audioGroupSp.FindPropertyRelative(AudioGroup.fn_id);
            var nameSp = audioGroupSp.FindPropertyRelative(AudioGroup.fn_name);
            var audioMixerGroupSp = audioGroupSp.FindPropertyRelative(AudioGroup.fn_audioMixerGroup);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleLeft;
            GUIStyle marginZero = new GUIStyle(GUI.skin.label);
            marginZero.margin = new RectOffset(0, 0, 3, 0);
            
            bool foldout = audioGroupEditorSettings.GetFoldout(idSp.longValue);
            string arrow = foldout ? "▼" : "►";
            float buttonHeight = 23;
            Color buttonBgColor = foldout ? new Color(1f, 0.59f, 0f) : GUI.backgroundColor;
            
            GUILayout.BeginHorizontal();
            bool clicked = DrawButtonWithBgColor("", buttonBgColor, GUILayout.Height(buttonHeight));
            foldout = clicked ? !foldout : foldout;
            audioGroupEditorSettings.SetFoldout(idSp.longValue, foldout);

            if (DrawButtonWithContentColor("✕", Color.red, GUILayout.Width(20), GUILayout.Height(buttonHeight)))
            {
                if (EditorUtility.DisplayDialog("Delete Audio Group?", "Do you want to delete this group?", "Yes", "No"))
                {
                    wantDelete = true;
                }
            }
            GUILayout.EndHorizontal();


            //Draw Content Button
            GUILayout.Space(-buttonHeight - StandardVerticalSpacing);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(arrow, labelStyle, GUILayout.Width(15), GUILayout.Height(buttonHeight));
            GUILayout.Label(nameSp.stringValue, labelStyle, GUILayout.Height(buttonHeight));
            GUILayout.EndHorizontal();


            if (foldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.BeginVertical();

                DrawAudioGroupId(idSp);
                EditorGUILayout.PropertyField(idSp, new GUIContent("Group Id"));
                
                EditorGUILayout.PropertyField(nameSp, new GUIContent("Group Name"));
                EditorGUILayout.PropertyField(audioMixerGroupSp, new GUIContent("Audio Mixer Group"));
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
        
        void DrawAudioGroupId(SerializedProperty idSp)
        {
            GUILayout.BeginHorizontal();
            bool oldEnable = GUI.enabled;
            bool editId = audioGroupEditorSettings.GetEditId(idSp.longValue);
            GUI.enabled = editId;
            EditorGUILayout.PropertyField(idSp);
            GUI.enabled = oldEnable;

            if (editId == false)
            {
                if (GUILayout.Button("✎", GUILayout.Width(19), GUILayout.Height(19)))
                {
                    audioGroupEditorSettings.SetEditId(idSp.longValue, true);
                }
            }
            else
            {
                if (DrawButtonWithContentColor("✓", Color.green, 19, 19))
                {
                    audioGroupEditorSettings.SetEditId(idSp.longValue, false);
                }
            }
            
            GUILayout.EndHorizontal();
        }
    }
}