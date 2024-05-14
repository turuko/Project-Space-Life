using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Editors;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

namespace Model.Stat_System
{
    public enum StatType
    {
        Attribute,
        Combat
    }
    
    [CreateAssetMenu(fileName = "New Combat Stats", menuName = "Custom/Stats Collection")]
    public class StatCollection : ScriptableObject
    {
        
        [SerializedDictionary("Stat name", "Stat")][SerializeField] 
        private SerializedDictionary<string, CharacterStat> stats;
        [SerializeField] private StatType type;

        public StatType Type
        {
            get => type;
            set => type = value;
        }

        public SerializedDictionary<string, CharacterStat> Stats
        {
            get => stats;
            set
            {
                stats = new SerializedDictionary<string, CharacterStat>(value);
            }
        }

        public CharacterStat this[string name]
        {
            get
            {
                return Stats[name];
            }
            
            set
            {
                Stats[name] = value;
            }
        }
        
        public SerializedDictionary<string, CharacterStat> GetDictionary()
        {
            return Stats;
        }

        public void Init(List<string> statNames)
        {
            Stats = new SerializedDictionary<string, CharacterStat>(statNames.ToDictionary(x => x, _ => new CharacterStat()));
        }
    }
}