using Model.Items;
using UnityEngine;

namespace Model.Factions
{
    public interface Entity
    {
        public Equipment GetEquipment();
        public float GetMaxHealth();
        public float GetCurrentHealth();
        public string GetId();

        public Weapon GetInitialWeapon();
    }
}