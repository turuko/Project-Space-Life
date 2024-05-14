using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Menus
{
    public class NewSavePopup : MonoBehaviour
    {
        [SerializeField] private TMP_InputField saveNameInput;
        [SerializeField] private SaveNewGameMenu saveNewGameMenu;

        public void OnConfirmButtonClicked()
        {
            DataPersistenceController.Instance.ChangeSelectedSaveGame(saveNameInput.text);
            DataPersistenceController.Instance.SaveGame();
            
            gameObject.SetActive(false);
            saveNewGameMenu.RefreshMenu();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            saveNameInput.text = "";
        }
    }
}