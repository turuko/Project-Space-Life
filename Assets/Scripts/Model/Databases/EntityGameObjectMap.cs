using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Editors;
using Model.Factions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Model.Databases
{

    public enum EntityMapType
    {
        Character,
        Unit,
        Other
    }
    
    [CreateAssetMenu(fileName = "EntityGameObjectMap", menuName = "Custom/EntityGameObjectMap", order = 0)]
    public class EntityGameObjectMap : ScriptableObject
    {
        [SerializedDictionary("Entity ID", "Prefab")][SerializeField]
        private SerializedDictionary<string, GameObject> map = new SerializedDictionary<string, GameObject>();

        [SerializeField] private EntityMapType type;
        public EntityMapType Type { get => type; set => type = value; }

        [SerializedDictionary("Entity ID", "Prefab")]
        public SerializedDictionary<string, GameObject> Map => map;

        public GameObject this[Entity index]
        {
            get
            {
                string allKeys = "";
                Map.Keys.ToList().ForEach(x => allKeys += x + ",\n");
                Debug.Log("Map size: " + Map.Count + ", " + allKeys);
                return Map[index.GetId()];
            }
        }

        // Automatically populate the map with existing units when the object is created
        [ContextMenu("Populate Entity Map/Units")]
        public void PopulateUnitMap()
        {
            map = new SerializedDictionary<string, GameObject>();
            var unitGUIDs = AssetDatabase.FindAssets("t:Unit");
            foreach (var unitGUID in unitGUIDs)
            {
                var unitPath = AssetDatabase.GUIDToAssetPath(unitGUID);
                var unit = AssetDatabase.LoadAssetAtPath<Unit>(unitPath);

                if (unit != null)
                { 
                    map.Add(unit.Id, null);
                }
            }
            
            Type = EntityMapType.Unit;
        }
        
        [ContextMenu("Populate Entity Map/Characters")]
        public void PopulateCharacterMap()
        {
            map = new SerializedDictionary<string, GameObject>();
            var characterGUIDs = AssetDatabase.FindAssets("t:Character");
            foreach (var characterGuid in characterGUIDs)
            {
                var characterPath = AssetDatabase.GUIDToAssetPath(characterGuid);
                var character = AssetDatabase.LoadAssetAtPath<Character>(characterPath);

                if (character != null)
                {
                    // You can set GameObject values to null initially.
                    map.Add(character.Id, null);
                }
            }

            Type = EntityMapType.Character;
        }
    }
}