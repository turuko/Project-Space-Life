using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Editors;
using Model.Stat_System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Model.Universe
{
    public class Species : ScriptableObject
    {
        [SerializeField] private new string name;

        [SerializeField] private string id;

        public string Id
        {
            get => id;
            set => id = value;
        }
        
        public string Name
        {
            get => name;
            set => name = value;
        }
         
        [SerializedDictionary("Species Id", "Relation")][SerializeField] 
        private SerializedDictionary<string, float> speciesRelation;
        public SerializedDictionary<string, float> SpeciesRelation
        {
            get => speciesRelation;
            private set
            {
                speciesRelation = value;
            }
        }


        public void InitRelations(List<Species> species)
        {
            SpeciesRelation = new SerializedDictionary<string, float>(species.ToDictionary(x => x.Id, _ => 0f));
        }

        public void ChangeSpeciesRelation(Species species, float value)
        {
            speciesRelation[species.Id] += value;
        }

        //[JsonConverter(typeof(SerializableDictionaryConverter<string,StatModifier>))]
        [SerializeField] private Dictionary<string, StatModifier> speciesStatBonuses;

        //[JsonConverter(typeof(SerializableDictionaryConverter<string,StatModifier>))]
        public Dictionary<string, StatModifier> SpeciesStatBonuses
        {
            get => speciesStatBonuses;
            private set => speciesStatBonuses = value;
        }

        public void InitBonuses(List<string> statNames)
        {
            SpeciesStatBonuses = new Dictionary<string, StatModifier>(statNames.ToDictionary(x => x, _ => new StatModifier(0,StatModType.None, this)));
        }

        
        //The initial value is the normal size of the species with the min and max representing the range of the species.
        [SerializeField] private ConstrainedFloat size;

        public ConstrainedFloat Size
        {
            get => size;
            set => size = value;
        }
    }
}
