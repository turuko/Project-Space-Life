using System;
using System.Collections.Generic;
using System.Linq;
using Model.Factions;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Model.Databases
{
    public static class UnitDatabase
    {
        private static List<Unit> allUnits;
        private static Dictionary<string, Unit> unitMap;

        private static EntityGameObjectMap unitGameObjectMap;

        public static void Init()
        {
            allUnits = new List<Unit>();
            var guids = AssetDatabase.FindAssets("t:Unit");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var unitAsset = AssetDatabase.LoadAssetAtPath<Unit>(path);

                var unit = unitAsset.Clone();
                
                allUnits.Add(unit);
            }

            unitMap = allUnits.ToDictionary(u => u.Id);

            unitGameObjectMap = AssetDatabase.FindAssets("t:EntityGameObjectMap").Select(x =>
                    AssetDatabase.LoadAssetAtPath<EntityGameObjectMap>(AssetDatabase.GUIDToAssetPath(x)))
                .First(x => x.Type == EntityMapType.Unit);
        }
        
        public static Unit GetUnit(string id)
        {
            return unitMap[id];
        }

        public static List<Unit> GetUnits(params string[] ids)
        {
            List<Unit> units = new List<Unit>();

            foreach (string id in ids)
            {
                if (unitMap.TryGetValue(id, out var unit))
                {
                    units.Add(unit);
                }
            }

            return units;
        }

        public static List<Unit> GetPartyUnits(Dictionary<string, int> unitDictionary)
        {
            List<Unit> units = new List<Unit>();
            foreach (var i in unitDictionary)
            {
                if(unitMap.TryGetValue(i.Key, out var unit))
                {
                    for (int j = 0; j < i.Value; j++)
                    {
                        units.Add(unit);
                    }
                }
            }

            return units;
        }

        public static List<Unit> Query(Func<Unit, bool> predicate)
        {
            return allUnits.Where(predicate).ToList();
        }

        public static GameObject GetUnitGameObject(Unit unit)
        {
            return unitGameObjectMap[unit];
        }
    }
}