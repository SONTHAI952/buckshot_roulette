using System;
using UnityEngine;
using UnityEngine.UI;
using ZeroX.SceneTransitionerSystem;

namespace ZeroX.UI.SceneTransitionerSystem.Demo
{
    public class DemoSceneTransitioner : MonoBehaviour
    {
        [SerializeField] private Button buttonLoadScene;
        [SerializeField] private Button buttonAllowSceneActivation;
        [SerializeField] private SceneTransitioner sceneTransitioner1;
        [SerializeField] private SceneTransitioner sceneTransitioner2;
        [SerializeField] private string sceneName;
        [SerializeField] private GameObject fakeScene;


        private void Awake()
        {
            buttonLoadScene.onClick.AddListener(ButtonLoader_OnClick);
            buttonAllowSceneActivation.onClick.AddListener(ButtonAllowSceneActivation_OnClick);
        }

        private void ButtonLoader_OnClick()
        {
            //SceneTransitioner.Default.LoadAction(() => fakeScene.gameObject.SetActive(true), "Fake Scene");
            //sceneTransitioner.LoadAction(() => fakeScene.gameObject.SetActive(true), "Fake Scene");
            Debug.Log("Has Current 1: " + (SceneTransitioner.Current != null));
            sceneTransitioner1.LoadSceneAsync(sceneName);
            sceneTransitioner1.AllowSceneActivationAfterLoadDone = false;
            Debug.Log("Set False");
            Debug.Log("Has Current 2: " + (SceneTransitioner.Current != null));
        }

        private void ButtonAllowSceneActivation_OnClick()
        {
            sceneTransitioner1.AllowSceneActivationAfterLoadDone = true;
        }
    }
}