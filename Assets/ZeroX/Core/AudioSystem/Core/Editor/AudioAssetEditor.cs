using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.AudioSystem.Editors
{
    [CustomEditor(typeof(AudioAsset))]
    public class AudioAssetEditor : Editor
    {
        private SerializedObject audioAssetSo;
        private SerializedProperty audioClipSp;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            audioAssetSo = serializedObject;
            audioClipSp = audioAssetSo.FindProperty(AudioAsset.fn_audioClip);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Play"))
            {
                StopAllAudioClips();
                PlayAudioClip();
            }

            if (GUILayout.Button("Stop"))
            {
                StopAllAudioClips();
            }
            
            GUILayout.EndHorizontal();
        }

        void PlayAudioClip()
        {
            PlayAudioClip((AudioClip)audioClipSp.objectReferenceValue, 0, false);
        }

        void PlayAudioClip(AudioClip audioClip, int startSample, bool loop)
        {
            System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var bindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var listMethod = audioUtilClass.GetMethods(bindingFlag);
            // foreach (var methodInfo in listMethod)
            // {
            //     Debug.Log(methodInfo.Name);
            // }
            
            System.Reflection.MethodInfo method = audioUtilClass.GetMethod("PlayPreviewClip",
                bindingFlag,
                null,
                new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );
            method.Invoke(
                null,
                new object[] { audioClip, startSample, loop }
            );
        }
        
        public static void StopAllAudioClips() 
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass =
                unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[]{},
                null
            );
            method.Invoke(
                null,
                new object[] {}
            );
        }
    }
}