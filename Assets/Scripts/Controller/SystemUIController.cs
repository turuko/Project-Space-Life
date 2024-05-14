using Model.Universe;
using UnityEngine;
using Visual_Scripts.SystemViewUI;

namespace Controller
{
    public class SystemUIController : UIController
    {
        [SerializeField] private PlanetInfoPanel PlanetInfoPanel;
        [SerializeField] private SettlementInfoPanel SettlementInfoPanel;

        public void OpenPlanetInfo(Planet planet)
        {
            PlanetInfoPanel.Open(planet);
        }
        
        public void ClosePlanetInfo()
        {
            PlanetInfoPanel.Close();
        }
        
        public void OpenSettlementInfo(Settlement settlement)
        {
            PlanetInfoPanel.Close();
            SettlementInfoPanel.Open(settlement);
            settlement.DetermineRecruits(GameManager.Instance.GetPlayerCharacter());
        }
        
        public void CloseSettlementInfo(Planet planet)
        {
            SettlementInfoPanel.Close();
            PlanetInfoPanel.Open(planet);
        }
    }
}