using System.Threading.Tasks;
using Model.Factions;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button NewGameButton;
        [SerializeField] private Button ContinueButton;
        [SerializeField] private Button LoadButton;
        [SerializeField] private Button SettingsButton;
        [SerializeField] private Button ExitButton;

        [SerializeField] private SaveGamesMenu saveGamesMenu;
        [SerializeField] private NewGameMenu newGameMenu;

        private void Start()
        {
            Open();
            if (!DataPersistenceController.Instance.HasGameData())
                ContinueButton.interactable = false;
        }

        private void Open()
        {
            NewGameButton.onClick.AddListener(NewGame);
            ContinueButton.onClick.AddListener(() =>
            {
                GameManager.Instance.SetGameLoaded();
                GameManager.Instance.LoadGalaxyScene();
            });
            LoadButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                saveGamesMenu.ActivateMenu();
            });
            ExitButton.onClick.AddListener(Application.Quit);
        }

        private void NewGame()
        {
            gameObject.SetActive(false);
            newGameMenu.Open();
        }
    }
}