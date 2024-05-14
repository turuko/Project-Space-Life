using Controller;
using Model.Universe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Visual_Scripts.SystemViewUI
{
    public class SettlementScrollPanel : MonoBehaviour
    {
        private Settlement Settlement;

        [SerializeField] private TextMeshProUGUI Title;
        [SerializeField] private TextMeshProUGUI Type;
        [SerializeField] private TextMeshProUGUI Specialization;
        
        [SerializeField] private Image FactionIcon;
        
        [SerializeField] private Button GoToButton;

        public void Open(Settlement settlement)
        {
            Settlement = settlement;
            Title.text = Settlement.Name;
            Type.text = Settlement.SettlementType.ToString();
            
            
            if (Settlement.SettlementType == SettlementType.SpecializedColony)
            {
                Colony colony = Settlement as Colony;
                if (colony != null)
                {
                    Specialization.text = colony.ColonyType.ToString();
                }
                else
                {
                    // Handle the case where the cast to Colony was not successful.
                    Specialization.text = "Invalid Colony"; // Or another appropriate message.
                }
            }
            else
            {
                Specialization.text = "";
            }
            GoToButton.onClick.AddListener(() =>
            {
                GameManager.Instance.OpenSettlementInfo(Settlement);
            });
        }
    }
}