using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZeroX.AudioSystem.Editors
{
    [CustomPropertyDrawer(typeof(AudioGroupIdFieldAttribute))]
    public class AudioGroupIdFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var audioManager = AudioSystemEditorUtility.FindAudioManagerInProject();
            if (audioManager == null)
            {
                EditorGUI.HelpBox(position, "AudioManager not found in project!", MessageType.Error);
                return;
            }

            var listAudioGroup = audioManager.AudioDatabase.GetIEnumerableAudioGroup().ToList();
            var listAudioGroupName = listAudioGroup.Select(a => a.Name).ToArray();
            var listAudioGroupId = listAudioGroup.Select(a => a.Id).ToList();

            long oldId = property.longValue;
            int oldIndex = listAudioGroupId.IndexOf(oldId);

            Color oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = oldIndex >= 0 ? oldBgColor : Color.red;
            int newIndex = EditorGUI.Popup(position, property.displayName, oldIndex, listAudioGroupName);
            GUI.backgroundColor = oldBgColor;

            if (newIndex >= 0)
            {
                long newId = listAudioGroupId[newIndex];
                property.longValue = newId;
            }
        }
    }
}