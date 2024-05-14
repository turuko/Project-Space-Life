using Model.Factions;
using UnityEngine;

namespace Battle.Entity_Components
{

    public enum DamageType
    {
        Laser,
        Melee,
        Psychic
    }
    
    [RequireComponent(typeof(CombatArmor))]
    public class CombatHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 60f;
        [SerializeField] private float currentHealth;

        private CombatArmor armor;
        public float MaxHealth
        {
            get => maxHealth;
            private set => maxHealth = value;
        }

        public float CurrentHealth
        {
            get => currentHealth;
            private set => currentHealth = value;
        }

        public void Initialize(Entity entity)
        {
            MaxHealth = entity.GetMaxHealth();
            CurrentHealth = entity.GetCurrentHealth();
            armor = GetComponent<CombatArmor>();
        }

        public void TakeDamage(float damage, DamageType damageType)
        {
            //Armor reduction
            float finalDamage = Mathf.Max(armor.ReduceDamage(damage, damageType), 0f);

            currentHealth -= finalDamage;

            if (currentHealth >= 0)
            {
                //Die / knock out
                
            }
        }
    }
}