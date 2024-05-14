using System;
using Controller;
using Model.Databases;
using Model.Universe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Visual_Scripts.SystemViewUI
{
    public class SettlementInfoPanel : MonoBehaviour
    {

        private enum State
        {
            OverviewState,
            RecruitState,
            TradeState,
            BarState
        }
        
        private Settlement Settlement;

        [SerializeField] private TextMeshProUGUI Title;
        [SerializeField] private TextMeshProUGUI SettlementType;

        [SerializeField] private RectTransform ContentContainer;
        
        [SerializeField] private Button RecruitButton;
        [SerializeField] private Button BarButton;
        [SerializeField] private Button TradeButton;

        [SerializeField] private Button CloseButton;

        [SerializeField] private UnitButton UnitButtonPrefab;

        private State state;
        
        public void Open(Settlement settlement)
        {
            gameObject.SetActive(true);
            Settlement = settlement;
            Title.text = Settlement.Name;
            SettlementType.text = Settlement.SettlementType.ToString();

            state = State.OverviewState;
            
            
            //TODO Implement buttons.
            
            RecruitButton.onClick.AddListener(() =>
            {
                state = State.RecruitState;
                for (int i = 0; i < ContentContainer.childCount; i++)
                {
                    ContentContainer.GetChild(i).gameObject.SetActive(false);
                }

                foreach (var recruit in settlement.Recruits)
                {
                    var button = Instantiate(UnitButtonPrefab, ContentContainer);
                    button.Init(Settlement, recruit);
                }
            });
            
            CloseButton.onClick.AddListener(() =>
            {
                switch (state)
                {
                    case State.OverviewState:
                        GameManager.Instance.CloseSettlementInfo(PlanetDatabase.GetPlanet(Settlement.PlanetId));
                        break;
                    case State.RecruitState:
                        for (int i = 0; i < ContentContainer.childCount; i++)
                        {
                            if (ContentContainer.GetChild(i).TryGetComponent<UnitButton>(out _)) 
                                Destroy(ContentContainer.GetChild(i).gameObject);
                        }

                        for (int i = 0; i < ContentContainer.childCount; i++)
                        {
                            ContentContainer.GetChild(i).gameObject.SetActive(true);
                        }

                        state = State.OverviewState;
                        break;
                    case State.TradeState:
                        break;
                    case State.BarState:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            });
        }

        public void Close()
        {
            CloseButton.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
        }
    }
}
