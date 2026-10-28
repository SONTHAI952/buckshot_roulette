using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Shared.CampaignModeSystem
{
    public class LevelDifficultDisplayer : MonoBehaviour
    {
        [SerializeField] private GameObject normalUI;
        [SerializeField] private GameObject hardUI;
        [SerializeField] private GameObject superHardUI;
        
        [SerializeField] private List<TMP_Text> listTextLevel;


        private void OnEnable()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            int level = CampaignModeDI.SelectedLevel;
            var difficult = CampaignLevelTable.GetRowByIdWithLog(level).Difficult;
            
            normalUI.SetActive(difficult == LevelDifficult.Normal);
            hardUI.SetActive(difficult == LevelDifficult.Hard);
            superHardUI.SetActive(difficult == LevelDifficult.SuperHard);
            
            //Text Level
            foreach (var textLevel in listTextLevel)
            {
                textLevel.text = "Level " + level;
            }
        }
    }
}