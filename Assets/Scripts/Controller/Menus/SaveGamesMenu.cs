using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Controller.Menus;
using UnityEngine;

public class SaveGamesMenu : MonoBehaviour
{
    [SerializeField] private SaveGameButton saveGameButtonPrefab;
    [SerializeField] private GameObject buttonGroup;
    private List<SaveGameButton> saveGameButtons;

    private void Awake()
    {
        saveGameButtons = new List<SaveGameButton>();
    }

    private void OnSaveGameButtonClicked(SaveGameButton button)
    {
        DataPersistenceController.Instance.ChangeSelectedSaveGame(button.GetSaveGameName());
        GameManager.Instance.SetGameLoaded();
        GameManager.Instance.LoadGalaxyScene();
    }

    public void OnBackButtonClicked(GameObject previousMenu)
    {
        for (int i = 0; i < buttonGroup.transform.childCount; i++)
        {
            Destroy(buttonGroup.transform.GetChild(i).gameObject);
        }
        
        
        Debug.Log("GameObject: " + gameObject.name);
        previousMenu.SetActive(true);
        if(GetComponentInParent<GamesMenu>())
            GetComponentInParent<GamesMenu>().PopStack();
        
        gameObject.SetActive(false);
    }

    public void ActivateMenu()
    {
        Debug.Log("SaveGamesMenu::Activate");
        gameObject.SetActive(true);
        var saveGames = DataPersistenceController.Instance.GetAllSaveGameNames();

        foreach (var saveGame in saveGames)
        {
            Debug.Log("Save name: " + saveGame);
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
