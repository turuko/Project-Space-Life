using UnityEngine;

namespace Battle.Vehicles
{
    public class Mount : IInteractable
    {
        protected bool mountEnabled;
        
        public override void Interact(BattlePlayer player)
        {
            Debug.Log("Interacted with: " + gameObject.name);
            player.gameObject.SetActive(false);
            mountEnabled = true;
        }
    }
}