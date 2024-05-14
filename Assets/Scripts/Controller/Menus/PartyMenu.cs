using System;
using System.Collections.Generic;
using System.Linq;
using Model.Databases;
using Model.Factions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Menus
{
    public class PartyMenu : InfoMenu
    {
        [SerializeField] private PartyMemberButton buttonPrefab;

        private List<Unit> dismissedUnits;
        private List<Unit> partyUnits;

        [SerializeField] private Button CancelButton;
        [SerializeField] private Button ConfirmButton;
        
        public override void Open()
        {
            dismissedUnits = new List<Unit>();
            partyUnits = new List<Unit>(UnitDatabase.GetPartyUnits(GameManager.Instance.GetPlayerParty().UnitsId));

            RightSideTitle.text = GameManager.Instance.GetPlayerCharacter().Name + "'s Party";
            
            ConfirmButton.onClick.AddListener(Confirm);
            CancelButton.onClick.AddListener(Cancel);
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            for (int i = 0; i < RightSide.transform.childCount; i++)
            {
                Destroy(RightSide.transform.GetChild(i).gameObject);
            }
            
            for (int i = 0; i < LeftSide.transform.childCount; i++)
            {
                Destroy(LeftSide.transform.GetChild(i).gameObject);
            }
            
            var leaderButton = Instantiate(buttonPrefab, RightSide.transform);
            leaderButton.Init(null, GameManager.Instance.GetPlayerCharacter(), 1);
            leaderButton.DismissButton.gameObject.SetActive(false);

            foreach (var charKVP in GameManager.Instance.GetPlayerParty().CompanionsId.GroupBy(item => item)
                         .ToDictionary(group => group.Key, group => group.Count()))
            {
                var button = Instantiate(buttonPrefab, RightSide.transform);
                button.DismissButton.gameObject.SetActive(false);
                button.Init(null, CharacterDatabase.GetCharacter(charKVP.Key), charKVP.Value);
            }

            foreach (var unitKVP in partyUnits.GroupBy(item => item)
                         .ToDictionary(group => group.Key, group => group.Count()))
            {
                var button = Instantiate(buttonPrefab, RightSide.transform);
                button.Init(unitKVP.Key, null, unitKVP.Value);
                button.DismissButton.onClick.AddListener(() => DismissUnit(partyUnits.First(u => u.Name == unitKVP.Key.Name)));
            }

            foreach (var unitKVP in dismissedUnits.GroupBy(item => item).ToDictionary(group => group.Key, group => group.Count()))
            {
                var button = Instantiate(buttonPrefab, LeftSide.transform);
                button.Init(unitKVP.Key, null, unitKVP.Value);
                button.DismissButton.GetComponentInChildren<TextMeshProUGUI>().text = ">";
                button.DismissButton.onClick.AddListener(() => UndismissUnit(dismissedUnits.First(u => u.Name == unitKVP.Key.Name)));
            }
        }

        private void DismissUnit(Unit unit)
        {
            dismissedUnits.Add(unit);
            partyUnits.Remove(unit);
            UpdateUI();
        }

        private void UndismissUnit(Unit unit)
        {
            dismissedUnits.Remove(unit);
            partyUnits.Add(unit);
            UpdateUI();
        }

        private void Confirm()
        {
            if (partyUnits.Count != GameManager.Instance.GetPlayerParty().UnitsId.Sum(u => u.Value))
            {
                GameManager.Instance.GetPlayerParty().UnitsId = partyUnits.GroupBy(u => u).ToDictionary(group => group.Key.Id, group => group.Count());
            }

            Cancel();
        }

        private void Cancel()
        {
            switch (GameManager.Instance.GetPreviousState())
            {
                case GameManager.GameState.MainMenu:
                    break;
                case GameManager.GameState.InfoMenu:
                    break;
                case GameManager.GameState.Galaxy:
                    //needs to go back to where the player was when menu was opened. not where the player spawns on galaxy creation.
                    GameManager.Instance.LoadGalaxyScene();
                    break;
                case GameManager.GameState.System:
                    //needs to go back to where the player was when menu was opened. not where the player spawns on system creation.
                    GameManager.Instance.LoadSystemScene(GameManager.Instance.GetSystemInFocus());
                    break;
                case GameManager.GameState.InfoPanelOpen:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}