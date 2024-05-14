using Battle.Weapons;
using Newtonsoft.Json;
using UnityEngine;

namespace Model.Items
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Custom/Weapon/RangedWeapon")]
    public class RangedWeapon : Weapon
    {

        [SerializeField] private float range;
        [SerializeField] private float reloadTime;
        [SerializeField] private int magSize;
        [SerializeField] private float projectileSpeed;
        
        public float Range
        {
            get => range;
            set => range = value;
        }

        public float ReloadTime
        {
            get => reloadTime;
            set => reloadTime = value;
        }

        public int MagSize
        {
            get => magSize;
            set => magSize = value;
        }

        public float ProjectileSpeed
        {
            get => projectileSpeed;
            set => projectileSpeed = value;
        }

        [SerializeField][JsonIgnore]
        private GameObject projectilePrefab;

        [JsonIgnore]
        public GameObject ProjectilePrefab
        {
            get => projectilePrefab;
            set => projectilePrefab = value;
        }
    }
}