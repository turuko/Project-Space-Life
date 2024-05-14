using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Controller.Menus
{
    public class SaveGameButton : MonoBehaviour
    {
        private string saveGameName = "";

        [SerializeField] private TextMeshProUGUI saveGameNameText;
        [SerializeField] private TextMeshProUGUI timeSinceSavedText;
        [SerializeField] private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void SetData(string saveName, string timeSinceSaved)
        {
            saveGameName = saveName;
            saveGameNameText.text = saveGameName;
            timeSinceSavedText.text = timeSinceSaved;
        }
        
        public void SetOnClick(UnityAction action)
        {
            button.onClick.AddListener(action);
        }
        
        public string GetSaveGameName()
        {
            return saveGameName;
        }
    }
}
