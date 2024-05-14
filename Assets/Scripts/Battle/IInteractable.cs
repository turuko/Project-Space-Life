using UnityEngine;

namespace Battle
{
    public abstract class IInteractable : MonoBehaviour
    {
        public abstract void Interact(BattlePlayer player);
    }
}