using System;
using Controller;
using Model.Factions;
using Model.Stat_System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Visual_Scripts
{
    public class StatChangeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Label;
        [SerializeField] private TextMeshProUGUI Value;
        
        [SerializeField] private Button Add;
        [SerializeField] private Button Subtract;

        private string StatName;

        public void Open(string statName)
        {
            StatName = statName;
            Label.text = statName;
            Value.text = GameManager.Instance.GetPlayerCharacter().GetStat(statName).ToString();
            GameManager.Instance.GetPlayerCharacter().RegisterStatsChangedCB(UpdateValues);
            
            Add.onClick.AddListener(() =>
            {
                GameManager.Instance.GetPlayerCharacter().IncreaseStat(statName, 1f);
            });
            
            Subtract.onClick.AddListener(() =>
            {
                if(GameManager.Instance.GetPlayerCharacter().GetStat(statName) <= GameManager.Instance.GetPlayerCharacter().GetStatBeforeLevel(statName))
                    return;
                GameManager.Instance.GetPlayerCharacter().DecreaseStat(statName, 1f);
            });
            
            Subtract.gameObject.SetActive(false);
        }

        private void UpdateValues(Character playerChar)
        {
            if (playerChar.GetStat(StatName) >
                playerChar.GetStatBeforeLevel(StatName) && !Subtract.gameObject.activeSelf)
            {
                Subtract.gameObject.SetActive(true);
            }
            
            if (playerChar.GetStat(StatName) <=
                playerChar.GetStatBeforeLevel(StatName) && Subtract.gameObject.activeSelf)
            {
                Subtract.gameObject.SetActive(false);
            }

            switch (playerChar.GetStatType(StatName))
            {
                case StatType.Attribute:
                    Add.interactable = playerChar.AttributePoints != 0;
                    break;
                case StatType.Combat:
                    Add.interactable = playerChar.CombatSkillPoints != 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Value.text = playerChar.GetStat(StatName).ToString();
        }
    }
}