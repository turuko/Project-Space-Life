using System;
using Model.Factions;
using UnityEngine;

namespace Battle.Entity_Components
{
    public class CombatArmor : MonoBehaviour
    {
        private float laserArmor;
        private float meleeArmor;
        private float psychicResistance;

        #region Constants

        private const float LASER_ARMOR_SOAK_FACTOR = 0.5f;
        private const float MElEE_ARMOR_SOAK_FACTOR = 0.65f;
        private const float PSYCHIC_ARMOR_SOAK_FACTOR = 0.8f;
        
        private const float LASER_ARMOR_REDUCTION_FACTOR = 0.65f;
        private const float MELEE_ARMOR_REDUCTION_FACTOR = 0.75f;
        private const float PSYCHIC_ARMOR_REDUCTION_FACTOR = 1f;

        #endregion

        public float LaserArmor
        {
            get => laserArmor;
            private set => laserArmor = value;
        }

        public float MeleeArmor
        {
            get => meleeArmor;
            private set => meleeArmor = value;
        }

        public float PsychicResistance
        {
            get => psychicResistance;
            private set => psychicResistance = value;
        }

        public void Initialize(Entity entity)
        {
            var equipment = entity.GetEquipment();

            laserArmor = equipment.LaserArmor;
            meleeArmor = equipment.MeleeArmor;
            psychicResistance = equipment.PsychicResistance;
        }

        public float ReduceDamage(float initialDamage, DamageType damageType)
        {
            float reducedDamage;
            float finalDamage;
            switch (damageType)
            {
                case DamageType.Laser:
                    reducedDamage = initialDamage - (LaserArmor * LASER_ARMOR_SOAK_FACTOR);
                    finalDamage = reducedDamage * (1f - (LaserArmor / 100f * LASER_ARMOR_REDUCTION_FACTOR));
                    return finalDamage;
                case DamageType.Melee:
                    reducedDamage = initialDamage - (MeleeArmor * MElEE_ARMOR_SOAK_FACTOR);
                    finalDamage = reducedDamage * (1f - (MeleeArmor / 100f * MELEE_ARMOR_REDUCTION_FACTOR));
                    return finalDamage;
                case DamageType.Psychic:
                    reducedDamage = initialDamage - (PsychicResistance * PSYCHIC_ARMOR_SOAK_FACTOR);
                    finalDamage = reducedDamage * (1f - (PsychicResistance / 100f * PSYCHIC_ARMOR_REDUCTION_FACTOR));
                    return finalDamage;
                default:
                    throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null);
            }
        }
    }
}