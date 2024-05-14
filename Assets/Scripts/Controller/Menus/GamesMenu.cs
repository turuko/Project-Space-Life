using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Menus
{
    public class GamesMenu : MonoBehaviour
    {
        
        [SerializeField] private Button saveGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitToMainMenuButton;
        [SerializeField] private Button exitGameButton;
        [SerializeField] private GameObject buttonGroup;
        
        [SerializeField] private SaveGamesMenu saveGamesMenu;
        [SerializeField] private SaveNewGameMenu saveNewGameMenu;

        private Stack<string> menuStack;
        
        
        public void Open()
        {
            Debug.Log("Game menu::Open");
            gameObject.SetActive(true);

            menuStack = new Stack<string>();
            
            if(saveGameButton.onClick.GetPersistentEventCount() <= 0)
            {
                Debug.Log("Setting save button onclick");
                saveGameButton.onClick.AddListener(() =>
                {
                    OpenMenuPage("SaveGame");
                });
            }
            
            //load game menu
            if (loadGameButton.onClick.GetPersistentEventCount() <= 0)
            {
                Debug.Log("Setting load button onclick");
                loadGameButton.onClick.AddListener(() =>
                {
                    OpenMenuPage("LoadGame");
                });
            }
            
            //settings menu
            
            exitToMainMenuButton.onClick.AddListener(GameManager.Instance.LoadMainMenuScene);
            
            //TODO: implement popup to confirm exiting, showing how long ago game was saved.
            exitGameButton.onClick.AddListener(Application.Quit);
        }

        private void OpenMenuPage(string page)
        {
            buttonGroup.SetActive(false);
            if (page.Equals("SaveGame"))
            {
                saveNewGameMenu.ActivateMenu();
            }
            else if (page.Equals("LoadGame"))
            {
                saveGamesMenu.ActivateMenu();
            }
            else
            {
                Debug.LogWarning("No such page");
            }
            menuStack.Push(page);
        }

        public void CloseMenuPage()
        {
            Debug.Log("menuStack: " + menuStack.Count);
            if (menuStack.Count > 0)
            {
                var openPage = menuStack.Peek();
                if (openPage.Equals("SaveGame"))
                {
                    saveNewGameMenu.OnBackButtonClicked(buttonGroup);
                }
                else if (openPage.Equals("LoadGame"))
                {
                    saveGamesMenu.OnBackButtonClicked(buttonGroup);
                }
            }
            else
            {
                Close();
            }
        }

        public void PopStack()
        {
            menuStack.Pop();
        }

        private void Close()
        {
            StartCoroutine(WaitForFrames(1, () => 
            {
                gameObject.SetActive(false);
                GameManager.Instance.gameMenuIsOpen = false;
                
            }));
            Debug.Log("Game menu::Close");
            saveGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            GameManager.Instance.gameMenuIsOpen = false;
        }

        private IEnumerator WaitForFrames(int frames, Action cb)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }

            cb();
        }
    }
}