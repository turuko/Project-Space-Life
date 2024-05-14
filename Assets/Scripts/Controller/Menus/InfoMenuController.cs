using UnityEngine;

namespace Controller.Menus
{
    public class InfoMenuController : UIController
    {
        private InfoMenu activeMenu;

        public void Initialize(InfoMenu menu)
        {
            if (menu == null)
            {
                Debug.LogError("Trying to open null info menu");
                return;
            }
            
            activeMenu = menu;
            activeMenu.Open();
        }
    }
}