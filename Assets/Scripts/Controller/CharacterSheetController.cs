using Model.Factions;
using Model.Stat_System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Visual_Scripts;

namespace Controller
{
    public class CharacterSheetController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField CharacterNameInput;
        [SerializeField] private TextMeshProUGUI NameWarning;

        [SerializeField] private RectTransform CharacterStatContainer;
        [SerializeField] private RectTransform CombatStatContainer;

        [SerializeField] private StatChangeUI StatUIPrefab;

        [SerializeField] private TextMeshProUGUI AttributePointsLeft;
        [SerializeField] private TextMeshProUGUI CombatSkillPointsLeft;

        [SerializeField] private Button ResetButton;
        [SerializeField] private Button ContinueButton;

        private void Start()
        {
            Open();
        }

        private void UpdateValues(Character playerChar)
        {
            AttributePointsLeft.text = playerChar.AttributePoints.ToString();
            CombatSkillPointsLeft.text = playerChar.CombatSkillPoints.ToString();
        }

        public void Open()
        {
            GameManager.Instance.GetPlayerCharacter().RegisterStatsChangedCB(UpdateValues);
            GameManager.Instance.GetPlayerCharacter().SetStatsBeforeLevel();
            AttributePointsLeft.text = GameManager.Instance.GetPlayerCharacter().AttributePoints.ToString();
            CombatSkillPointsLeft.text = GameManager.Instance.GetPlayerCharacter().CombatSkillPoints.ToString();

            foreach (var statName in GameManager.Instance.GetPlayerCharacter().Attributes.GetDictionary().Keys)
            {
                Instantiate(StatUIPrefab, CharacterStatContainer).Open(statName);
            }
            
            foreach (var statName in GameManager.Instance.GetPlayerCharacter().CombatStats.GetDictionary().Keys)
            {
                Instantiate(StatUIPrefab, CombatStatContainer).Open(statName);
            }
            
            ResetButton.onClick.AddListener(GameManager.Instance.GetPlayerCharacter().ResetToStatsBeforeLevel);
            ContinueButton.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(CharacterNameInput.text))
                {
                    NameWarning.gameObject.SetActive(true);
                    return;
                }

                GameManager.Instance.GetPlayerCharacter().SetName(CharacterNameInput.text);
                GameManager.Instance.LoadGalaxyScene();
            });
        }

    }
}
