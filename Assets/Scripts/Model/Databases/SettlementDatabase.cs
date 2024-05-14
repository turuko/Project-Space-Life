using System;
using System.Collections.Generic;
using System.Linq;
using Model.Universe;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Model.Databases
{
    public static class SettlementDatabase
    {
        private static List<Settlement> allSettlements;

        public static void Init()
        {
            allSettlements = new List<Settlement>();
            var guids = AssetDatabase.FindAssets("t:Settlement");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var characterAsset = AssetDatabase.LoadAssetAtPath<Settlement>(path);

                var character = characterAsset.Clone();
                allSettlements.Add(character);
            }
        }

        public static bool AddSettlement(Settlement character)
        {
            if (allSettlements.Contains(character))
                return false;
            
            allSettlements.Add(character);
            return true;
        }

        public static Settlement GetSettlement(string id)
        {
            //Debug.Log("id null: " + (id == null) + ", list null: " +(allSettlements == null));
            return allSettlements.FirstOrDefault(c => c.Id.Equals(id));
        }

        public static List<Settlement> GetSettlements(params string[] ids)
        {
            return allSettlements.Where(c => ids.Contains(c.Id)).ToList();
        }

        public static List<Settlement> Query(Func<Settlement, bool> predicate)
        {
            return allSettlements.Where(predicate).ToList();
        }
    }
}