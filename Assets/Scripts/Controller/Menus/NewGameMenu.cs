using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Universe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Controller.Menus
{
    public class NewGameMenu : MonoBehaviour
    {
        [SerializeField] private Button ConfirmButton;
        [SerializeField] private TMP_InputField SeedInput;
        [SerializeField] private TMP_Dropdown SizeDropdown;

        private GalaxySize size;

        public void Open()
        {
            gameObject.SetActive(true);
            SeedInput.text = GalaxySettings.seed.ToString();
            SizeDropdown.ClearOptions();

            var enumValues = Enum.GetValues(typeof(GalaxySize));

            var options = new List<string>();
            foreach (var value in enumValues)
            {
                options.Add(value.ToString());
            }
            SizeDropdown.AddOptions(options);
            SizeDropdown.value = (int)GalaxySize.Small;
            size = GalaxySize.Small;
            SizeDropdown.onValueChanged.AddListener(OnSizeChanged);
            
            ConfirmButton.onClick.AddListener(async () =>
            {
                if (string.IsNullOrEmpty(SeedInput.text))
                {
                    Debug.Log("No seed");
                    return;
                }
                
                GameManager.Instance.SetSeed(SeedInput.text);

                GameManager.Instance.SetGalaxySize(size);

                await NewGame();
            });
        }
        
        public void OnBackButtonClicked(GameObject previousMenu)
        {
            previousMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        
        private async Task NewGame()
        {
            DataPersistenceController.Instance.NewGame();

            await GameManager.Instance.GalaxyInit();
            GameManager.Instance.LoadCreationScene();
        }

        private void OnSizeChanged(int index)
        {
            size = (GalaxySize)index;
        }
    }
}