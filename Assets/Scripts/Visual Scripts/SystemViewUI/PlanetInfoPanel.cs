using Controller;
using Model.Universe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Visual_Scripts.SystemViewUI
{
    public class PlanetInfoPanel : MonoBehaviour
    {
        private Planet Planet;

        [SerializeField] private TextMeshProUGUI Title;
        [SerializeField] private TextMeshProUGUI SettlementSlots;
        [SerializeField] private TextMeshProUGUI Size;
        
        [SerializeField] private Image TypeIcon;
        [SerializeField] private TextMeshProUGUI TypeText;
        
        [SerializeField] private RectTransform SettlementInfoContent;

        [SerializeField] private SettlementScrollPanel SettlementScrollPanelPrefab;

        [SerializeField] private Button CloseButton;

        public void Open(Planet planet)
        {
            gameObject.SetActive(true);
            Planet = planet;
            Title.text = Planet.Name;
            SettlementSlots.text = Planet.GetAvailableSettlementSlots() + " / " + Planet.maxSettlementSlots;
            Size.text = Planet.planetSize.ToString();
            TypeText.text = Planet.planetType.ToString();
            CloseButton.onClick.AddListener(GameManager.Instance.ClosePlanetInfo);
            
            foreach (var settlement in Planet.GetSettlements())
            {
                var scrollPanel = Instantiate(SettlementScrollPanelPrefab, SettlementInfoContent);
                scrollPanel.Open(settlement);
            }
        }

        public void Close()
        {
            CloseButton.onClick.RemoveAllListeners();
            foreach (Transform child in SettlementInfoContent.transform)
            {
                Destroy(child.gameObject);
            }
            
            gameObject.SetActive(false);
        }
    }
}