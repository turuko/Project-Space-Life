using Model.Factions;
using Model.Universe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Menus
{
    public class PartyMemberButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI NameLabel;
        [SerializeField] private TextMeshProUGUI AmountLabel;
        [SerializeField] private Image MemberImage;
        [SerializeField] private Button SelectButton;
        [SerializeField] public Button DismissButton;


        public void Init(Unit unit, Character character, int amount)
        {

            if (unit != null)
            {
                NameLabel.text = unit.Name;
                AmountLabel.text = "x" + amount;
                Debug.Log("Init Member Button: " + unit.Name);
                //set member Image
                //set selectbutton onClick
            }
            else if (character != null)
            {
                NameLabel.text = character.Name;
                AmountLabel.text = (character.CurrentHealth / character.MaxHealth * 100) + "%";
                
                Debug.Log("Init Member Button: " + character.Name);
                //set member Image
                //set selectbutton onClick
            }
            else
            {
                Debug.LogError("No unit or character set");
            }
        }

    }
}