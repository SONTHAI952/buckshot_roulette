using System;
using UnityEngine;
using UnityEngine.UI;
using ZeroX.SaveGameSystem;

namespace ZeroX.AudioSystem.Demo
{
    public class TestAudioSystem : MonoBehaviour
    {
        [SerializeField] private Button buttonLoginSaveGame;

        private void Awake()
        {
           buttonLoginSaveGame.onClick.AddListener(ButtonLoginSaveGame_OnClick);
        }

        private void Update()
        {
           
        }

        private void ButtonLoginSaveGame_OnClick()
        {
            SaveGame.Login();
        }
    }
}