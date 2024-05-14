using System;
using Battle.Entity_Components;
using Model.Items;
using UnityEngine;
using Utility.Object_Pooling;

namespace Battle.Weapons
{
    public class Projectile : MonoBehaviour
    {
        //Speed?, range? more?

        private float speed;
        private float range;
        private float damage;

        private bool isInitialized;
        
        private float distanceTraveled; // Keep track of the distance traveled by the projectile


        private void Update()
        {
            if (!isInitialized)
                return;

            float distance = speed * Time.deltaTime;
            distanceTraveled += distance;
            
            transform.position += transform.forward.normalized * (speed * Time.deltaTime);

            if (distanceTraveled >= range)
            {
                isInitialized = false;
                SimplePool.Despawn(gameObject);
            }
        }

        public void OnCollisionEnter(Collision other)
        {
            Debug.Log("Collided with: " + other.transform.name);
            if (other.transform.GetComponent<CombatHealth>() != null)
            {
                //Do damage
                other.transform.GetComponent<CombatHealth>().TakeDamage(damage, DamageType.Laser);
            }
            isInitialized = false;
            SimplePool.Despawn(gameObject);
        }

        public void Initialize(RangedWeapon weapon)
        {
            speed = weapon.ProjectileSpeed;// projectile speed?
            range = weapon.Range;
            damage = weapon.Damage;
            distanceTraveled = 0f;
            isInitialized = true;
        }
    }
}