using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Items
{
    public enum ItemSlot
    {
        Helmet,
        Gloves,
        Boots,
        Chest,
        Back,
        Legs,
        Weapon,
        Other
    }
    
    public class Item : ScriptableObject
    {
        [SerializeField] private new string name;
        public string Name
        {
            get => name;
            set => name = value;
        }

        [SerializeField] private string id;

        public string Id
        {
            get => id;
            set => id = value;
        }

        [SerializeField] private float weight;

        public float Weight
        {
            get => weight;
            set => weight = value;
        }

        [SerializeField] private ItemSlot itemSlot;
        
        public ItemSlot ItemSlot
        {
            get => itemSlot;
            set => itemSlot = value;
        }
    }
}