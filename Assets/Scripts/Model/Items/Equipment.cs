using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Editors;
using Model.Databases;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Model.Items
{
    [Serializable]
    public class Equipment : ScriptableObject
    {
        
        //[JsonConverter(typeof(SerializableDictionaryConverter<ItemSlot,Item[]>))]
        [SerializedDictionary("Item Slot", "Items")][SerializeField] 
        private SerializedDictionary<ItemSlot,string[]> equipmentSlots;

        //[JsonConverter(typeof(SerializableDictionaryConverter<ItemSlot,Item[]>))]
        public SerializedDictionary<ItemSlot, string[]> EquipmentSlots
        {
            get => equipmentSlots;
            set
            {
                equipmentSlots = value;

            }
        }

        //Armor value that reduces damage from "laser"-weapons, eg. blaster, "lightsabers" etc.
        private float laserArmor;
        
        //Armor value that reduces damage from non-laser melee weapons, eg. knife, mace, sword etc.
        private float meleeArmor;
        
        //Value that reduces the impact of psychic attacks and effects, eg. pull/push, mind control etc.
        private float psychicResistance;

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

        public void Init()
        {
            Debug.Log("Init Equipment");
            EquipmentSlots = new SerializedDictionary<ItemSlot, string[]>
            {
                { ItemSlot.Helmet, new string[1] },
                { ItemSlot.Back, new string[1] },
                { ItemSlot.Chest, new string[1] },
                { ItemSlot.Boots, new string[1] },
                { ItemSlot.Legs, new string[1] },
                { ItemSlot.Gloves, new string[1] },
                { ItemSlot.Weapon, new string[4] }
            };
        }
        
        public void Init(Equipment other)
        {
            EquipmentSlots = new SerializedDictionary<ItemSlot, string[]>(other.EquipmentSlots);
        }

        public bool AnyItems()
        {
            try
            {
                return EquipmentSlots.Any();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string[] this[ItemSlot index]
        {
            get => EquipmentSlots[index];
            private set
            {
                foreach (var id in value)
                {
                    var item = ItemDatabase.GetItem(id);
                    if (item.ItemSlot != index)
                    {
                        throw new ArgumentException("Item does not have the same type as the item slot: " + item.Name);
                    }
                    
                    EquipmentSlots[index] = value;
                }
            }
        }

        public void EquipItem(Item item, int index = 0)
        {
            this[item.ItemSlot][index] = item.Id;

            switch (item.ItemSlot)
            {
                case ItemSlot.Helmet:
                case ItemSlot.Gloves:
                case ItemSlot.Boots:
                case ItemSlot.Chest:
                case ItemSlot.Back:
                case ItemSlot.Legs:
                    var armor = (Armor)item;
                    LaserArmor += armor.LaserArmorValue;
                    MeleeArmor += armor.MeleeArmorValue;
                    PsychicResistance += armor.PsychicResistance;
                    break;
                case ItemSlot.Weapon:
                    var weapon = (Weapon)item;
                    break;
                case ItemSlot.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UnequipItem(Item item, int index = 0)
        {
            this[item.ItemSlot][index] = null;

            switch (item.ItemSlot)
            {
                case ItemSlot.Helmet:
                case ItemSlot.Gloves:
                case ItemSlot.Boots:
                case ItemSlot.Chest:
                case ItemSlot.Back:
                case ItemSlot.Legs:
                    var armor = (Armor)item;
                    LaserArmor -= armor.LaserArmorValue;
                    MeleeArmor -= armor.MeleeArmorValue;
                    PsychicResistance -= armor.PsychicResistance;
                    break;
                case ItemSlot.Weapon:
                    var weapon = (Weapon)item;
                    break;
                case ItemSlot.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}