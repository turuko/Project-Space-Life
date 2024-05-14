using System;
using System.Collections.Generic;
using System.Linq;
using Model.Factions;
using UnityEditor;
using Utility;

namespace Model.Databases
{
    public class FactionDatabase
    {
        private static List<Faction> allFactions;

        public static void Init()
        {
            allFactions = new List<Faction>();
            var guids = AssetDatabase.FindAssets("t:Faction");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var factionAsset = AssetDatabase.LoadAssetAtPath<Faction>(path);

                var faction = factionAsset.Clone();
                
                allFactions.Add(faction);
            }
        }

        public static bool AddFaction(Faction faction)
        {
            if (allFactions.Contains(faction))
                return false;
            
            allFactions.Add(faction);
            return true;
        }

        public static Faction GetFaction(string id)
        {
            return allFactions.FirstOrDefault(c => c.Id.Equals(id));
        }

        public static void UpdateFaction(Faction faction)
        {
            var existingFaction = allFactions.FirstOrDefault(f => f.Id == faction.Id);

            if (existingFaction == null) return;
            var index = allFactions.IndexOf(existingFaction);
            allFactions[index] = faction;
        }

        public static List<Faction> GetFactions(params string[] ids)
        {
            return allFactions.Where(c => ids.Contains(c.Id)).ToList();
        }

        public static List<Faction> Query(Func<Faction, bool> predicate)
        {
            return allFactions.Where(predicate).ToList();
        }
    }
}