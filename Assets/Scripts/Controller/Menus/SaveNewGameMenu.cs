using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Menus
{
    public class SaveNewGameMenu : MonoBehaviour
    {
        [SerializeField] private SaveGameButton saveGameButtonPrefab;
        [SerializeField] private GameObject buttonGroup;
        private List<SaveGameButton> saveGameButtons;
        [SerializeField] private NewSavePopup newSavePopup;
        private void Awake()
        {
            saveGameButtons = new List<SaveGameButton>();
        }

        public void OnNewGameButtonClicked()
        {
            newSavePopup.Open();
        }

        private void OnSaveGameButtonClicked(SaveGameButton button)
        {
            DataPersistenceController.Instance.ChangeSelectedSaveGame(button.GetSaveGameName());
            DataPersistenceController.Instance.SaveGame();
        }

        public void OnBackButtonClicked(GameObject previousMenu)
        {
            for (int i = 0; i < buttonGroup.transform.childCount; i++)
            {
                Destroy(buttonGroup.transform.GetChild(i).gameObject);
            }
            
            Debug.Log("GameObject: " + gameObject.name);
            previousMenu.SetActive(true);
            GetComponentInParent<GamesMenu>().PopStack();
            gameObject.SetActive(false);
        }

        public void ActivateMenu()
        {
            Debug.Log("SaveNewGameMenu::Activate");
            gameObject.SetActive(true);
            var saveGames = DataPersistenceController.Instance.GetAllSaveGameNames();

            foreach (var saveGame in saveGames)
            {
                var button = Instantiate(saveGameButtonPrefab, buttonGroup.transform);
                
                var timeSinceSaved = DateTime.Now - DataPersistenceController.Instance.GetLastWriteTime(saveGame);

                button.SetData(saveGame, FormatTimeSpan(timeSinceSaved));
                button.SetOnClick(() => OnSaveGameButtonClicked(button));
                saveGameButtons.Add(button);
            }
        }

        public void RefreshMenu()
        {
            for (int i = 0; i < buttonGroup.transform.childCount; i++)
            {
                if (buttonGroup.transform.GetChild(i).name == "New Save Button")
                    continue;
                Destroy(buttonGroup.transform.GetChild(i).gameObject);
            }
            
            var saveGames = DataPersistenceController.Instance.GetAllSaveGameNames();
            Debug.Log("saveGames count: " + saveGames.Count);
            
            foreach (var saveGame in saveGames)
            {
                var button = Instantiate(saveGameButtonPrefab, buttonGroup.transform);
                
                var timeSinceSaved = DateTime.Now - DataPersistenceController.Instance.GetLastWriteTime(saveGame);

                button.SetData(saveGame, FormatTimeSpan(timeSinceSaved));
                button.SetOnClick(() => OnSaveGameButtonClicked(button));
                saveGameButtons.Add(button);
            }
        }
        
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            string formattedTime = "";

            if (timeSpan.Days > 0)
            {
                formattedTime += $"{timeSpan.Days} day{(timeSpan.Days > 1 ? "s" : "")} ";
            }

            if (timeSpan.Hours > 0)
            {
                formattedTime += $"{timeSpan.Hours} hour{(timeSpan.Hours > 1 ? "s" : "")} ";
            }

            if (timeSpan.Minutes > 0)
            {
                formattedTime += $"{timeSpan.Minutes} minute{(timeSpan.Minutes > 1 ? "s" : "")} ";
            }

            if (timeSpan.Seconds > 0 && timeSpan.Minutes < 1)
            {
                formattedTime += $"{timeSpan.Seconds} second{(timeSpan.Seconds > 1 ? "s" : "")} ";
            }

            if (string.IsNullOrEmpty(formattedTime))
            {
                formattedTime = "less than a second";
            }

            return formattedTime.Trim();
        }
    }
}