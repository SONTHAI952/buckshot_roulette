using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZeroX.AudioSystem
{
    public class AudioSourceControllerPool
    {
        private static List<AudioSourceController> listAudioSourceController = new List<AudioSourceController>();

        public static int maxPool = 20;
        public static float updateInterval = 10;

        private static float lastTimeUpdate = 0;
        
        public static AudioSourceController GetFromPool()
        {
            if (listAudioSourceController.Count > 0)
            {
                for (int i = listAudioSourceController.Count - 1; i >= 0; i--)
                {
                    var audioSourceController = listAudioSourceController[i];
                    if (audioSourceController == null || audioSourceController.InUse)
                    {
                        listAudioSourceController.RemoveAt(i);
                        continue;
                    }
                    
                    //Lấy ra khỏi pool
                    listAudioSourceController.RemoveAt(i);
                    if (audioSourceController.gameObject.scene != SceneManager.GetActiveScene())
                        SceneManager.MoveGameObjectToScene(audioSourceController.gameObject, SceneManager.GetActiveScene());

                    AudioSystemUtility.ResetAudioSource(audioSourceController.AudioSource);
                    audioSourceController.gameObject.SetActive(true);
                    return audioSourceController;
                }
            }

            GameObject ascGameObject = new GameObject();
            var audioSource = ascGameObject.AddComponent<AudioSource>();
            AudioSourceController asc = ascGameObject.AddComponent<AudioSourceController>();
            AudioSourceController.SetAudioSource(asc, audioSource);
            return asc;
        }

        public static void PutInPool(AudioSourceController audioSourceController)
        {
            if(audioSourceController == null || audioSourceController.InUse)
                return;
            
            listAudioSourceController.Add(audioSourceController);
            audioSourceController.gameObject.SetActive(false);
            AudioSourceController.ResetAudioSource(audioSourceController);
        }

        public static void RemoveFromPool(AudioSourceController audioSourceController)
        {
            listAudioSourceController.Remove(audioSourceController);
        }

        public static void ClearPoolIfMaximum()
        {
            if (listAudioSourceController.Count <= maxPool)
                return;

            for (int i = listAudioSourceController.Count - 1; i >= 0; i--)
            {
                var audioSourceController = listAudioSourceController[i];
                if (audioSourceController == null || audioSourceController.InUse == false)
                {
                    listAudioSourceController.RemoveAt(i);
                    if (audioSourceController != null)
                        Object.Destroy(audioSourceController.gameObject);

                    if(listAudioSourceController.Count <= maxPool)
                        return;
                }
            }
        }
        
        public static void Update()
        {
            if(Time.unscaledTime - lastTimeUpdate < updateInterval)
                return;

            lastTimeUpdate = Time.unscaledTime;
            ClearPoolIfMaximum();
        }
    }
}