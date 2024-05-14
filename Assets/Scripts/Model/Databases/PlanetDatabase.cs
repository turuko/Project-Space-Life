using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Universe;
using UnityEngine;

namespace Model.Databases
{
    public static class PlanetDatabase
    {
        private static List<Planet> allPlanets;

        public static void Init()
        {
            allPlanets = new List<Planet>();
        }

        public static bool AddPlanet(Planet character)
        {
            if (allPlanets.Contains(character))
                return false;
            
            allPlanets.Add(character);
            return true;
        }

        public static void PrintIds()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Amount of planets in PlanetDatabase: " + allPlanets.Count + "Planet ids in PlanetDatabase:\n");
            foreach (var planet in allPlanets)
            {
                sb.Append(planet.Id + "\n");
            }
            Debug.Log(sb.ToString());
        }

        public static Planet GetPlanet(string id)
        {
            return allPlanets.FirstOrDefault(c => c.Id.Equals(id));
        }

        public static List<Planet> GetPlanets(params string[] ids)
        {
            return allPlanets.Where(c => ids.Contains(c.Id)).ToList();
        }

        public static List<Planet> Query(Func<Planet, bool> predicate)
        {
            return allPlanets.Where(predicate).ToList();
        }
    }
}