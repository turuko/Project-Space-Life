using UnityEngine;

namespace Model.Items
{
    public enum WeaponType
    {
        Rifle,
        Pistol,
        OneHand,
        TwoHand,
        Sniper,
        HeavyGun
    }
    
    
    public class Weapon : Item
    {
        [SerializeField] private WeaponType type;

        public WeaponType Type
        {
            get => type;
            set => type = value;
        }

        [SerializeField] private float damage;

        public float Damage
        {
            get => damage;
            set => damage = value;
        }

        [SerializeField] private float attackSpeed;

        public float AttackSpeed
        {
            get => attackSpeed;
            set => attackSpeed = value;
        }
    }
}