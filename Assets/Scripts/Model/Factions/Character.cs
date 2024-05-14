using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Model.Databases;
using Model.Items;
using Model.Stat_System;
using UnityEngine;

namespace Model.Factions
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Custom/Character")]
    public class Character : ScriptableObject, Entity
    {

        #region Fields

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
        
        [SerializeField] private string speciesId;
        public string SpeciesId
        {
            get => speciesId;
            set => speciesId = value;
        }

        [SerializeField] private StatCollection attributesBeforeLevel;
        public StatCollection AttributesBeforeLevel
        {
            get => attributesBeforeLevel;
            set => attributesBeforeLevel = value;
        }
        
        
        [SerializeField] private StatCollection attributes;
        public StatCollection Attributes
        {
            get => attributes;
            set => attributes = value;
        }

        
        [SerializeField] private StatCollection combatStatsBeforeLevel;
        public StatCollection CombatStatsBeforeLevel
        {
            get => combatStatsBeforeLevel;
            set => combatStatsBeforeLevel = value;
        }


        [SerializeField] private StatCollection combatStats;
        public StatCollection CombatStats
        {
            get => combatStats;
            set => combatStats = value;
        }


        [SerializeField] private string factionId;
        public string FactionId 
        { 
            get => factionId;
            set => factionId = value;
        }
        
        [SerializeField] private Equipment equipment;

        public Equipment Equipment
        {
            get => equipment;
            set => equipment = value;
        }

        [SerializeField] private float foodCost;
        public float FoodCost
        {
            get => foodCost;
            set => foodCost = value;
        }
        
        [SerializeField] private float goodCost;
        public float GoldCost
        {
            get => goodCost;
            set => goodCost = value;
        }
        
        [SerializeField] private float maxHealth;
        public float MaxHealth
        {
            get => maxHealth; 
            set => maxHealth = value;
        }
        
        [SerializeField] private float currentHealth;
        public float CurrentHealth
        {
            get => currentHealth; 
            set => currentHealth = value;
        }

        [SerializeField] private int level;
        public int Level
        {
            get => level;
            set => level = value;
        }

        [SerializeField] private int attributePointsBeforeLevel;
        public int AttributePointsBeforeLevel
        {
            get => attributePointsBeforeLevel;
            set => attributePointsBeforeLevel = value;
        }

        [SerializeField] private int attributePoints;
        public int AttributePoints
        {
            get => attributePoints; 
            set => attributePoints = value;
        }

        [SerializeField] private int combatSkillPointsBeforeLevel;
        public int CombatSkillPointsBeforeLevel
        {
            get => combatSkillPointsBeforeLevel;
            set => combatSkillPointsBeforeLevel = value;
        }

        [SerializeField] private int combatSkillPoints;

        public int CombatSkillPoints
        {
            get => combatSkillPoints; 
            set => combatSkillPoints = value;
        }

        private Action<Character> StatsChanged;
        
        private Action<Character> HealthChanged;
        
        #endregion

        public static Character CreateCharacter(Dictionary<string, CharacterStat> combatStats,
            Dictionary<string, CharacterStat> charStats, int attributePoints, int combatSkillPoints, float foodCost,
            float maxHealth, int level)
        {
            Character character = ScriptableObject.CreateInstance<Character>();
            character.Id = Guid.NewGuid().ToString();
            character.Attributes = ScriptableObject.CreateInstance<StatCollection>();
            character.Attributes.Stats = new SerializedDictionary<string, CharacterStat>(combatStats);
            character.CombatStats = ScriptableObject.CreateInstance<StatCollection>();
            character.CombatStats.Stats = new SerializedDictionary<string, CharacterStat>(charStats);
            character.AttributesBeforeLevel = ScriptableObject.CreateInstance<StatCollection>();
            character.CombatStatsBeforeLevel = ScriptableObject.CreateInstance<StatCollection>();
            character.AttributePoints = character.AttributePointsBeforeLevel = attributePoints;
            character.CombatSkillPoints = character.CombatSkillPointsBeforeLevel = combatSkillPoints;
            character.Equipment = CreateInstance<Equipment>();
            character.Equipment.Init();
            character.FoodCost = character.GoldCost = foodCost;
            character.MaxHealth = character.CurrentHealth = maxHealth;
            character.Level = level;

            CharacterDatabase.AddCharacter(character);
            return character;
        }

        public void SetStatsBeforeLevel()
        {
            AttributesBeforeLevel.Stats = new SerializedDictionary<string, CharacterStat>();

            foreach (var kvp in Attributes.Stats)
            {
                AttributesBeforeLevel.Stats.Add(kvp.Key, new CharacterStat(kvp.Value.Value));
            }

            CombatStatsBeforeLevel.Stats = new SerializedDictionary<string, CharacterStat>();
            
            foreach (var kvp in CombatStats.Stats)
            {
                CombatStatsBeforeLevel.Stats.Add(kvp.Key, new CharacterStat(kvp.Value.Value));
            }
            
            CombatSkillPointsBeforeLevel = CombatSkillPoints;
            AttributePointsBeforeLevel = AttributePoints;
        }

        public void ResetToStatsBeforeLevel()
        {
            Attributes.Stats = AttributesBeforeLevel.GetDictionary();
            
            CombatStats.Stats = CombatStatsBeforeLevel.GetDictionary();
            
            
            CombatSkillPoints = CombatSkillPointsBeforeLevel;
            AttributePoints = AttributePointsBeforeLevel;
            StatsChanged(this);
        }

        public void IncreaseStat(string statName, float value)
        {
            var statType = GetStatType(statName);

            switch (statType)
            {
                case StatType.Attribute:
                    if (AttributePoints <= 0)
                        return;
                    Attributes[statName].AddModifier(new StatModifier(value, StatModType.Flat));
                    AttributePoints -= (int)value;
                    StatsChanged(this);
                    break;
                case StatType.Combat:
                    if (CombatSkillPoints <= 0)
                        return;
                    CombatStats[statName].AddModifier(new StatModifier(value, StatModType.Flat));
                    CombatSkillPoints -= (int)value;
                    StatsChanged(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void DecreaseStat(string statName, float value)
        {
            var statType = GetStatType(statName);

            switch (statType)
            {
                case StatType.Attribute:
                    Attributes[statName].AddModifier(new StatModifier(-value, StatModType.Flat));
                    AttributePoints += (int)value;
                    StatsChanged(this);
                    break;
                case StatType.Combat:
                    CombatStats[statName].AddModifier(new StatModifier(-value, StatModType.Flat));
                    CombatSkillPoints += (int)value;
                    StatsChanged(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetFaction(Faction faction)
        {
            FactionId = faction.Id;
        }

        public float GetStat(string statName)
        {
            try
            {
                return Attributes[statName].Value;
            }
            catch (Exception)
            {
                try
                {
                    return CombatStats[statName].Value;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }
        
        public float GetStatBeforeLevel(string statName)
        {
            try
            {
                return AttributesBeforeLevel[statName].Value;
            }
            catch (Exception)
            {
                try
                {
                    return CombatStatsBeforeLevel[statName].Value;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }

        public StatType GetStatType(string statName)
        {
            try
            {
                var _ = Attributes[statName];
                return StatType.Attribute;
            }
            catch (Exception)
            {
                try
                {
                    var _ = CombatStats[statName];
                    return StatType.Combat;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }

        public void RegisterStatsChangedCB(Action<Character> cb)
        {
            StatsChanged += cb;
        }
        
        public void UnregisterStatsChangedCB(Action<Character> cb)
        {
            StatsChanged -= cb;
        }
        
        public void RegisterHealthChangedCB(Action<Character> cb)
        {
            HealthChanged += cb;
        }
        
        public void UnregisterHealthChangedCB(Action<Character> cb)
        {
            HealthChanged -= cb;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public Weapon GetInitialWeapon()
        {
            return (Weapon)ItemDatabase.GetItem(Equipment.EquipmentSlots[ItemSlot.Weapon][0]);
        }

        public string GetId() => Id;

        public Equipment GetEquipment() =>  Equipment;

        public float GetMaxHealth()=>  MaxHealth;

        public float GetCurrentHealth() => CurrentHealth;
    }
}