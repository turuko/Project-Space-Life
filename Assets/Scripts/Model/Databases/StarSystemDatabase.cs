using System;
using System.Collections.Generic;
using System.Linq;
using Model.Universe;

namespace Model.Databases
{
    public class StarSystemDatabase
    {
        private static List<StarSystem> allStarSystems;
        private static Dictionary<string, StarSystem> systemMap;

        public static void Init()
        {
            allStarSystems = new List<StarSystem>();
            systemMap = allStarSystems.ToDictionary(x => x.Id);
        }

        public static bool AddStarSystem(StarSystem system)
        {
            if (allStarSystems.Contains(system))
                return false;
            
            allStarSystems.Add(system);
            systemMap.Add(system.Id, system);
            return true;
        }

        public static StarSystem GetStarSystem(string id)
        {
            return systemMap[id];
        }

        public static List<StarSystem> GetStarSystems(params string[] ids)
        {
            List<StarSystem> systems = new List<StarSystem>();

            foreach (string id in ids)
            {
                if (systemMap.TryGetValue(id, out var character))
                {
                    systems.Add(character);
                }
            }

            return systems;
        }

        public static List<StarSystem> Query(Func<StarSystem, bool> predicate)
        {
            return allStarSystems.Where(predicate).ToList();
        }
    }
}