using System;
using System.Collections.Generic;
using Model.Databases;
using Model.Items;
using Model.Stat_System;
using Model.Universe;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Factions
{ 
    [CreateAssetMenu(fileName = "New Unit", menuName = "Custom/Unit")]
    public class Unit : ScriptableObject, Entity
    {
        [SerializeField] private string id;

        public string Id
        {
            get => id;
            set => id = value;
        }
        
        [SerializeField] private new string name;
        public string Name
        {
            get => name;
            set => name = value;
        }
        
        [FormerlySerializedAs("species")] [SerializeField] private string speciesId;
        public string SpeciesId
        {
            get => speciesId;
            set => speciesId = value;
        }

        [SerializeField] private string upgradesFrom;

        public string UpgradesFrom
        {
            get => upgradesFrom;

            set => upgradesFrom = value;
        }

        [SerializeField] private List<string> upgradesTo;

        public List<string> UpgradesTo
        {
            get => upgradesTo;
            set => upgradesTo = value;
        }
        
        [SerializeField] private float recruitmentCost;
        public float RecruitmentCost 
        {
            get => recruitmentCost;

            set => recruitmentCost = value;
        }

        [SerializeField] private float foodCost;
        public float FoodCost 
        {
            get => foodCost;

            set => foodCost = value;
        }

        [SerializeField] private float goldCost;

        public float GoldCost
        {
            get => goldCost;

            set => goldCost = value;
        }

        [SerializeField] private float maxHealth;

        public float MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
        }

        [SerializeField] private Equipment equipment;

        public Equipment Equipment
        {
            get => equipment;
            set => equipment = value;
        }
        
        [SerializeField] private StatCollection combatStats;
        public StatCollection CombatStats
        {
            get => combatStats;
            set => combatStats = value;
        }


        public void SetTroopTree(string _upgradesFrom, List<string> _upgradesTo)
        {
            UpgradesFrom = _upgradesFrom;
            UpgradesTo = _upgradesTo;
        }
        
        public Weapon GetInitialWeapon()
        {
            return (Weapon)ItemDatabase.GetItem(Equipment.EquipmentSlots[ItemSlot.Weapon][0]);
        }

        public string GetId() => Id;

        public Equipment GetEquipment() =>  Equipment;

        public float GetMaxHealth() =>  MaxHealth;

        public float GetCurrentHealth() => MaxHealth;
    }
}