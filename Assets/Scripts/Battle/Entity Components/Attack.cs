using System;
using System.Collections;
using Battle.Weapons;
using Model.Factions;
using Model.Items;
using UnityEngine;
using Utility.Object_Pooling;

namespace Battle.Entity_Components
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private Weapon selectedWeapon;

        private float timeSinceLastAttack = 0f;
        private float ammo; // Only used for ranged weapons
        private float reloadTime; // Only used for ranged weapons
        private bool currentlyReloading = false;
        private float timeSinceReloadStart = 0f;

        [SerializeField] private Transform rangedAttackPoint;
        
        public Weapon SelectedWeapon
        {
            get => selectedWeapon;
            private set => selectedWeapon = value;
        }

        public void Initialize(Entity entity)
        {
            // Get the initially selected weapon of the entity
            SelectedWeapon = entity.GetInitialWeapon();
            if (SelectedWeapon is RangedWeapon)
            {
                ammo = ((RangedWeapon)SelectedWeapon).MagSize;
                reloadTime = ((RangedWeapon)SelectedWeapon).ReloadTime;
            }
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (currentlyReloading)
            {
                timeSinceReloadStart += Time.deltaTime;

                if (timeSinceReloadStart >= reloadTime)
                {
                    currentlyReloading = false;
                    timeSinceReloadStart = 0f;
                    ammo = ((RangedWeapon)SelectedWeapon).MagSize;
                }
            }
        }

        public void PerformAttack()
        {
            if(timeSinceLastAttack >= SelectedWeapon.AttackSpeed)
            {
                switch (SelectedWeapon.Type)
                {
                    case WeaponType.Rifle:
                    case WeaponType.Pistol:
                    case WeaponType.Sniper:
                    case WeaponType.HeavyGun:
                        var rangedWeapon = (RangedWeapon)SelectedWeapon;

                        if (ammo > 0 && !currentlyReloading)
                        {
                            var projectile = SimplePool.Spawn(rangedWeapon.ProjectilePrefab, rangedAttackPoint.position, Quaternion.LookRotation(rangedAttackPoint.forward));
                            projectile.GetComponent<Projectile>().Initialize(rangedWeapon);

                            ammo--;
                            timeSinceLastAttack = 0f;
                        }
                        else
                        {
                            currentlyReloading = true;
                        }
                        break;
                    
                    case WeaponType.OneHand:
                    case WeaponType.TwoHand:
                        var meleeWeapon = (MeleeWeapon)SelectedWeapon;
                        
                        
                        
                        timeSinceLastAttack = 0f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}