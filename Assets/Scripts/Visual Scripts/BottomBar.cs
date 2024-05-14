using System;
using Controller;
using Controller.Menus;
using Model;
using Model.Factions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Visual_Scripts
{
    public class BottomBar : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI GoldLabel;
        [SerializeField] public TextMeshProUGUI FoodLabel;
        [SerializeField] public TextMeshProUGUI HealthLabel;
        [SerializeField] public TextMeshProUGUI MoraleLabel;
        [SerializeField] public TextMeshProUGUI PartyLabel;

        [SerializeField] public Image SpeedIndicator1;
        [SerializeField] public Image SpeedIndicator2;
        [SerializeField] public Image SpeedIndicator3;

        [SerializeField] public TextMeshProUGUI RotationsIndicator;
        [SerializeField] public Image RotationProgress;

        [SerializeField] public Button PartyButton;
        [SerializeField] public Button InventoryButton;

        public void Init()
        {
            PartyButton.onClick.AddListener(() => {GameManager.Instance.LoadInfoMenu(InfoMenuType.PartyMenu);});
            
            InventoryButton.onClick.AddListener(() => {GameManager.Instance.LoadInfoMenu(InfoMenuType.InventoryMenu);});
            
            GameManager.Instance.GetPlayerCharacter().RegisterHealthChangedCB(UpdatePlayerHealth);
            GameManager.Instance.GetPlayerParty().RegisterPartyChangedCB(UpdatePlayerPartyValues);
            GameManager.Instance.GetRotationController().RegisterChangedCB(UpdateRotationIndicator);
        }
        
        public void UpdateRotationIndicator(StandardRotationController rotationController)
        {
            RotationsIndicator.text = "Rotations: " + rotationController.GetRotations();
        }

        public void UpdateRotationProgress(StandardRotationController rotationController)
        {
            RotationProgress.fillAmount = 1 - (rotationController.GetTimeSinceLastRotation() /
                                                    rotationController.GetStandardRotationLength());
        }
        
        public void UpdateGameSpeedIndicator(int gameSpeed)
        {
            if (gameSpeed == 0)
            {
                SpeedIndicator1.gameObject.SetActive(false);
                SpeedIndicator2.gameObject.SetActive(false);
                SpeedIndicator3.gameObject.SetActive(false);
            }
            else if (gameSpeed == 1)
            {
                SpeedIndicator1.gameObject.SetActive(true);
                SpeedIndicator2.gameObject.SetActive(false);
                SpeedIndicator3.gameObject.SetActive(false);
            }
            else if (gameSpeed == 2)
            {
                SpeedIndicator1.gameObject.SetActive(true);
                SpeedIndicator2.gameObject.SetActive(true);
                SpeedIndicator3.gameObject.SetActive(false);
            }
            else if (gameSpeed == 3)
            {
                SpeedIndicator1.gameObject.SetActive(true);
                SpeedIndicator2.gameObject.SetActive(true);
                SpeedIndicator3.gameObject.SetActive(true);
            }
        }

        public void UpdatePlayerHealth(Character character)
        {
            HealthLabel.text = (character.CurrentHealth / character.MaxHealth * 100) + "%";
        }

        public void UpdatePlayerPartyValues(Party party)
        {
            GoldLabel.text = party.Gold.ToString();
            FoodLabel.text = party.Food.ToString();
            MoraleLabel.text = party.Morale.ToString();
            PartyLabel.text = party.GetNumberInParty().ToString();

        }
    }
}