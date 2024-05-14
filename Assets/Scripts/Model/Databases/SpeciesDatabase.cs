using System;
using System.Collections.Generic;
using System.Linq;
using Model.Universe;
using UnityEditor;
using Utility;

namespace Model.Databases
{
    public class SpeciesDatabase
    {
        private static List<Species> allSpeciesList;

        public static void Init()
        {
            allSpeciesList = new List<Species>();
            var guids = AssetDatabase.FindAssets("t:Species");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var speciesAsset = AssetDatabase.LoadAssetAtPath<Species>(path);

                var species = speciesAsset.Clone();
                allSpeciesList.Add(species);
            }
        }

        public static Species GetSpecies(string id)
        {
            return allSpeciesList.FirstOrDefault(c => c.Id.Equals(id));
        }

        public static List<Species> GetSpecies(params string[] ids)
        {
            return allSpeciesList.Where(c => ids.Contains(c.Id)).ToList();
        }

        public static List<Species> Query(Func<Species, bool> predicate)
        {
            return allSpeciesList.Where(predicate).ToList();
        }
    }
}