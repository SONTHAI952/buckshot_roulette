using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroX.AudioSystem.Editors
{
    public static class AudioSystemEditorUtility
    {
        public static AudioManager FindAudioManagerInProject()
        {
            var listAsset = AssetDatabase.FindAssets("Audio Manager t:GameObject");
            AudioManager audioManager = null;
            foreach (var guid in listAsset)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var newAudioManager = AssetDatabase.LoadAssetAtPath<AudioManager>(assetPath);
                if (audioManager == null)
                {
                    audioManager = newAudioManager;
                }
                else
                {
                    Debug.LogError("There is more than one Audio Manager in the project");
                }
            }

            return audioManager;
        }
    }
}