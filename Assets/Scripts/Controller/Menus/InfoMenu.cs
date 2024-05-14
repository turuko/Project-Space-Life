using TMPro;
using UnityEngine;

namespace Controller.Menus
{

    public enum InfoMenuType
    {
        PartyMenu,
        InventoryMenu,
        //... etc
    }
    public abstract class InfoMenu : MonoBehaviour
    {
        public GameObject RightSide;
        public TextMeshProUGUI RightSideTitle;
        
        
        public GameObject LeftSide;
        public TextMeshProUGUI LeftSideTitle;

        public abstract void Open();
    }
}