using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Model.Databases;
using Model.Factions;
using Name_Generation;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Utility;

using Random = UnityEngine.Random;

namespace Model.Universe
{
    public static class GalaxySettings
    {
        public static int minStars, maxStars;
        public static int seed;
    }

    public enum GalaxySize
    {
        Tiny,
        Small,
        Medium,
        Large,
        Huge
    }
    public class Galaxy
    {
        [SerializeField] private readonly List<StarSystem> starSystems;

        [SerializeField] public List<Faction> factions;
        [SerializeField] public List<Character> characters;
        [SerializeField] private List<Settlement> settlements;
        [SerializeField] private List<Planet> planets;

        private Dictionary<int, List<StarSystem>> sections;

        public Galaxy()
        {
            starSystems = new List<StarSystem>();
            factions = new List<Faction>();
            characters = new List<Character>();
            settlements = new List<Settlement>();
            planets = new List<Planet>();
        }

        public async Task GenerateGalaxy()
        {
            Random.InitState(GalaxySettings.seed);
            int numStars = Random.Range(GalaxySettings.minStars, GalaxySettings.maxStars + 1);

            await InitStars(numStars, GalaxySettings.seed);
            
            InitSpecies();

            InitCharacters();
            
            InitFactions();
            
            InitiatePlanetCities();
            
            foreach (var kvp in sections)
            {
                Debug.Log("Amount of stars in section " + kvp.Key + ": " + kvp.Value.Count);
            }

            InitSettlements(numStars);

            #region DebugPrints

            foreach (var kvp in factions)
            {
                Debug.Log(kvp.Name + " has " + kvp.SettlementIds.Count + ", " + kvp.SettlementIds.Count(s =>
                          {
                              //Debug.Log("Galaxy::Debug:: s: " + s);
                              return SettlementDatabase.GetSettlement(s).SettlementType == SettlementType.City;
                          }) +
                          " are Cities, " + kvp.SettlementIds.Count(s => SettlementDatabase.GetSettlement(s).SettlementType == SettlementType.MediumTown) + " are Medium Towns, " + 
                          kvp.SettlementIds.Count(s => SettlementDatabase.GetSettlement(s).SettlementType == SettlementType.SmallTown) + " are Small towns, " +
                          kvp.SettlementIds.Count(s => SettlementDatabase.GetSettlement(s).SettlementType == SettlementType.SpecializedColony) + " are Specialized Colonies, their capital is " +
                          SettlementDatabase.GetSettlement(kvp.CapitalId).Name + " and is on: " +
                          PlanetDatabase.GetPlanet(SettlementDatabase.GetSettlement(kvp.CapitalId).PlanetId).Name);
            }

            Debug.Log("Asteroids: " + asteroid + "\nOcean Planets: " + ocean + "\nDesert Planets: " + desert +
                      "\nIce Planets: " + ice +
                      "\nGas Giants: " + gg + "\nRadiated Planets: " + rad + "\n Jungle Planets: " + jgl +
                      "\nContinental Planets: " + con +
                      "\n ----------- \nHuge Planets: " + huge + " (Huge Gas Giants: " + GGHuge +")\nLarge Planets: " + large + "\nMedium Planets: " +
                      medium + "\nSmall Planets: " + small + "\nTiny Planets: " + tiny);

            var longestDistance = starSystems.Max(ss =>
                Vector3.Distance(Vector3.zero, new Vector3(ss.Position.x, 0, ss.Position.z)));
            var ssFurthest = starSystems.First(ss =>
                Math.Abs(Vector3.Distance(Vector3.zero, new Vector3(ss.Position.x, 0, ss.Position.z)) - longestDistance) < 0.5f);
            Debug.Log("The star furhtest from the center is " + longestDistance + " units away and is: " + ssFurthest.Name + "Pos: "+ssFurthest.Position);
            
            Debug.Log("Amount of planets: " + planets.Count);

            #endregion

        }

        private void InitCharacters()
        {
            var charactersList = CharacterDatabase.Query(_ => true);
            
            foreach (var character in charactersList)
            {
                characters.Add(character);
            }
        }

        private void InitSpecies()
        {
            List<Species> speciesList = SpeciesDatabase.Query( _ => true);

            float sectionAngle = (2 * Mathf.PI) / speciesList.Count;

            var center = new Vector2(0, 0);

            var speciesSections = new Dictionary<int, List<StarSystem>>();

            foreach (var system in starSystems)
            {
                float angle = Mathf.Atan2(system.Position.z - center.y, system.Position.x - center.x);

                if (angle < 0)
                    angle += 2 * Mathf.PI;

                int sectionNumber = Mathf.FloorToInt(angle / sectionAngle);

                if (!speciesSections.ContainsKey(sectionNumber))
                {
                    speciesSections.Add(sectionNumber, new List<StarSystem>());
                }
                
                speciesSections[sectionNumber].Add(system);
            }
            
            float sectionSize = 360.0f / speciesList.Count;
            // Assign each faction a capital system closest to its section's center
            List<StarSystem> speciesSystems = new List<StarSystem>();
            for (int i = 0; i < speciesList.Count; i++)
            {
                float midPoint = (i * sectionSize) + (sectionSize / 2);
                Vector2 sectionMidpoint =
                    new Vector2(Mathf.Cos(midPoint * Mathf.Deg2Rad), Mathf.Sin(midPoint * Mathf.Deg2Rad));
                sectionMidpoint *= Random.Range(.25f, 1.6f);
                
                StarSystem closestSystem = null;
                float closestDistance = float.MaxValue;

                foreach (var system in starSystems)
                {
                    float distance = Vector2.Distance(new Vector2(system.Position.x, system.Position.z),
                        sectionMidpoint);

                    if (distance < closestDistance)
                    {
                        closestSystem = system;
                        closestDistance = distance;
                    }
                }
                speciesSystems.Add(closestSystem);
            }

            Dictionary<StarSystem, List<StarSystem>> closestToSpecies = new Dictionary<StarSystem, List<StarSystem>>();
            foreach (var starSystem in speciesSystems)
            {
                if(!closestToSpecies.ContainsKey(starSystem))
                    closestToSpecies.Add(starSystem, new List<StarSystem>());
            }

            foreach (var system in starSystems)
            {
                if (speciesSystems.Contains(system))
                    continue;
                
                float closestDistance = float.MaxValue;
                StarSystem closestSpeciesSystem = null;
                foreach (var speciesSystem in speciesSystems)
                {
                    float distance = Vector2.Distance(new Vector2(system.Position.x, system.Position.z),
                        new Vector2(speciesSystem.Position.x, speciesSystem.Position.z));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestSpeciesSystem = speciesSystem;
                    }
                }
                closestToSpecies[closestSpeciesSystem].Add(system);
            }

            Debug.Log("Species sections: " + closestToSpecies.Count);
            foreach (var kvp in closestToSpecies)
            {
                Debug.Log("Species: " + speciesList[closestToSpecies.Keys.ToList().IndexOf(kvp.Key)].Name);
                kvp.Key.PrimarySpeciesId = speciesList[closestToSpecies.Keys.ToList().IndexOf(kvp.Key)].Id;

                foreach (var system in kvp.Value)
                {
                    system.PrimarySpeciesId = speciesList[closestToSpecies.Keys.ToList().IndexOf(kvp.Key)].Id;
                }
            }
            
            Debug.Log("Any planets without species: " + starSystems.Any(s => s.PrimarySpeciesId == Guid.Empty.ToString()));
        }
        
        private void InitSettlements(int numStars)
        {
            Debug.Log("Galaxy::InitSettlements, numStars: " + numStars);
            int numCities = Mathf.RoundToInt(numStars * 0.15f);
            int numMediumTowns = Mathf.RoundToInt(numStars * 0.3f);
            int numSmallTowns = Mathf.RoundToInt(numStars * 0.35f);
            int numSpecializedColonies = Mathf.RoundToInt(numStars * 0.2f);

            int numCitiesPerFaction = numCities / factions.Count;
            int numMediumTownsPerFaction = numMediumTowns / factions.Count;
            int numSmallTownsPerFaction = numSmallTowns / factions.Count;
            int numSpecializedColoniesPerFaction = numSpecializedColonies / factions.Count;

            Debug.Log("number of cities per faction: " + numCitiesPerFaction + ", number of medium towns per faction: " +
                      numMediumTownsPerFaction + ", number of small towns per faction: " + numSmallTownsPerFaction +
                      ", number of specialized colonies per faction: " + numSpecializedColoniesPerFaction);

            var stars = Resources.Load<TextAsset>("Settlement names");
            var nameGenerator = new NameGenerator(stars.text.Split("\n"), 20, 0.015f);

            //At this point only the planet cities should be initiated.
            Debug.Log("Amount of settlements: " + settlements.Count + ", the are all planet cities: " + settlements.All(s => s.SettlementType == SettlementType.PlanetCity));
            foreach (var s in settlements)
            {
                Debug.Log( s.Name + " is a " + s.SettlementType);
            }
            List<Settlement> planetCities = new List<Settlement>(settlements);
            foreach (var planetCity in planetCities)
            {
                var faction = FactionDatabase.GetFaction(planetCity.FactionId);
                Debug.Log("PlanetCity: " + planetCity.Name + ", faction: " + faction.Name);
                var planets = starSystems.Where(ss =>
                    Vector2.Distance(
                        new Vector2(PlanetDatabase.GetPlanet(planetCity.PlanetId).GetStarSystem().Position.x, PlanetDatabase.GetPlanet(planetCity.PlanetId).GetStarSystem().Position.z),
                        new Vector2(ss.Position.x, ss.Position.z)) <= 0.75f).SelectMany(system => system.PlanetIds).ToList();
                
                for (int i = 0; i < numCitiesPerFaction; i++)
                {
                    var cityPlanets = planets.Where(p => p != null && PlanetDatabase.GetPlanet(p).GetAvailableSettlementSlots() >= (int)SettlementType.City).ToArray();
                    if (cityPlanets.Length == 0)
                        continue;
                    var planet = cityPlanets[Random.Range(0, cityPlanets.Length)];
                    var settlement = ScriptableObject.CreateInstance<Settlement>();
                    settlement.Id = Guid.NewGuid().ToString();
                    settlement.SettlementType = SettlementType.City;
                    settlement.RequiredSettlementSlots = (int)settlement.SettlementType;
                    settlement.Name = faction.Name + " Some City " + Utilities.ConvertToRomanNumeral(i);
                    settlement.characterRecruitEvents = new Dictionary<string, int>();
                    var planetInstance = PlanetDatabase.GetPlanet(planet);
                    planetInstance.AddSettlement(settlement);
                    FactionDatabase.GetFaction(planetCity.FactionId).AddSettlement(settlement);
                    SettlementDatabase.AddSettlement(settlement);
                }
                
                for (int i = 0; i < numMediumTownsPerFaction; i++)
                {
                    var mediumTownPlanets = planets.Where(p => p != null && PlanetDatabase.GetPlanet(p).GetAvailableSettlementSlots() >= (int)SettlementType.MediumTown).ToArray();
                    if (mediumTownPlanets.Length == 0)
                        continue;
                    //Debug.Log("medium town planets: " + mediumTownPlanets.Length);
                    var planet = mediumTownPlanets[Random.Range(0, mediumTownPlanets.Length)];
                    var settlement = ScriptableObject.CreateInstance<Settlement>();
                    settlement.Id = Guid.NewGuid().ToString();
                    settlement.SettlementType = SettlementType.MediumTown; 
                    settlement.RequiredSettlementSlots = (int)settlement.SettlementType;
                    settlement.Name = faction.Name + " Some Medium Town " + Utilities.ConvertToRomanNumeral(i);
                    settlement.characterRecruitEvents = new Dictionary<string, int>();
                    var planetInstance = PlanetDatabase.GetPlanet(planet);
                    planetInstance.AddSettlement(settlement);
                    FactionDatabase.GetFaction(planetCity.FactionId).AddSettlement(settlement);
                    SettlementDatabase.AddSettlement(settlement);
                }
                
                for (int i = 0; i < numSmallTownsPerFaction; i++)
                {
                    var smallTownPlanets = planets.Where(p => p != null && PlanetDatabase.GetPlanet(p).GetAvailableSettlementSlots() >= (int)SettlementType.SmallTown).ToArray();
                    if (smallTownPlanets.Length == 0)
                        continue;
                    var planet = smallTownPlanets[Random.Range(0, smallTownPlanets.Length)];
                    var settlement = ScriptableObject.CreateInstance<Settlement>();
                    settlement.Id = Guid.NewGuid().ToString();
                    settlement.SettlementType = SettlementType.SmallTown;
                    settlement.RequiredSettlementSlots = (int)settlement.SettlementType;
                    settlement.Name = faction.Name + " Some Small Town " + Utilities.ConvertToRomanNumeral(i);
                    settlement.characterRecruitEvents = new Dictionary<string, int>();
                    var planetInstance = PlanetDatabase.GetPlanet(planet);
                    planetInstance.AddSettlement(settlement);
                    FactionDatabase.GetFaction(planetCity.FactionId).AddSettlement(settlement);
                    SettlementDatabase.AddSettlement(settlement);
                }
                
                for (int i = 0; i < numSpecializedColoniesPerFaction; i++)
                {
                    var specializedColonyPlanets = planets.Where(p => p != null && PlanetDatabase.GetPlanet(p).GetAvailableSettlementSlots() >= (int)SettlementType.SpecializedColony).ToArray();
                    if (specializedColonyPlanets.Length == 0)
                        continue;
                    var planet = specializedColonyPlanets[Random.Range(0, specializedColonyPlanets.Length)];
                    var values = Enum.GetValues(typeof(SpecializedColonyType));
                    var settlement = ScriptableObject.CreateInstance<Colony>();
                    settlement.Id = Guid.NewGuid().ToString();
                    settlement.SettlementType = SettlementType.SpecializedColony;
                    settlement.RequiredSettlementSlots = (int)settlement.SettlementType;
                    settlement.Name = faction.Name + " Some Specialized " +
                                      Utilities.ConvertToRomanNumeral(i);
                    settlement.characterRecruitEvents = new Dictionary<string, int>();                  
                    settlement.ColonyType = (SpecializedColonyType)values.GetValue(Random.Range(0, values.Length));
                    var planetInstance = PlanetDatabase.GetPlanet(planet);
                    Debug.Log("Planet instance = null: " + (planetInstance == null));
                    planetInstance.AddSettlement(settlement);
                    faction.AddSettlement(settlement);
                    SettlementDatabase.AddSettlement(settlement);
                }
            }
            
        }

        private void InitiatePlanetCities()
        {
            
            float sectionAngle = (2 * Mathf.PI) / factions.Count;

            var center = new Vector2(0, 0);

            sections = new Dictionary<int, List<StarSystem>>();

            foreach (var system in starSystems)
            {
                float angle = Mathf.Atan2(system.Position.z - center.y, system.Position.x - center.x);

                if (angle < 0)
                    angle += 2 * Mathf.PI;

                int sectionNumber = Mathf.FloorToInt(angle / sectionAngle);

                if (!sections.ContainsKey(sectionNumber))
                {
                    sections.Add(sectionNumber, new List<StarSystem>());
                }
                
                sections[sectionNumber].Add(system);
            }
            
            float sectionSize = 360.0f / sections.Count;
            // Assign each faction a capital system closest to its section's center
            List<StarSystem> capitalSystems = new List<StarSystem>();
            for (int i = 0; i < sections.Count; i++)
            {
                float midPoint = (i * sectionSize) + (sectionSize / 2);
                Vector2 sectionMidpoint =
                    new Vector2(Mathf.Cos(midPoint * Mathf.Deg2Rad), Mathf.Sin(midPoint * Mathf.Deg2Rad));
                
                StarSystem closestSystem = null;
                float closestDistance = float.MaxValue;

                foreach (var system in starSystems)
                {
                    float distance = Vector2.Distance(new Vector2(system.Position.x, system.Position.z),
                        sectionMidpoint);

                    if (distance < closestDistance)
                    {
                        closestSystem = system;
                        closestDistance = distance;
                    }
                }
                capitalSystems.Add(closestSystem);
            }
            
            Debug.Log("Amount of capital systems: " + capitalSystems.Count);

            // Spawn the faction capitals on the capital systems
            for (int i = 0; i < factions.Count; i++)
            {
                var capitalSystem = capitalSystems[i];
                var planetCity = factions[i].CapitalId;
                var anyHugePlanet = capitalSystem.PlanetIds.Any(p => p != null && PlanetDatabase.GetPlanet(p).GetAvailableSettlementSlots() == 10);
                if (!anyHugePlanet)
                {
                    var firstNull = Array.FindIndex(capitalSystem.PlanetIds, p => p == null);
                    firstNull = firstNull == -1 ? capitalSystem.PlanetIds.Length / 2 : firstNull;
                    //Generate new huge planet in system:
                    var planet = new Planet();
                    PlanetType planetType = PlanetType.GasGiant;
                    while (planetType is PlanetType.GasGiant or PlanetType.Radiated or PlanetType.Asteroid)
                    {
                        planetType = capitalSystem.GeneratePlanetTypePublic(firstNull);
                    }
                    Debug.Log("Planet Type: " + planetType);
                    Debug.Log("First null:" + firstNull + ", arraySize: " + capitalSystem.PlanetIds.Length);
                    planet.Generate(capitalSystem.Name + " " + firstNull, planetType, PlanetSize.Huge, capitalSystem.Id, firstNull);
                    capitalSystem.PlanetIds[firstNull] = planet.Id;
                    planets.Add(planet);
                    PlanetDatabase.AddPlanet(planet);
                }

                var settlement = SettlementDatabase.GetSettlement(planetCity);
                settlement.characterRecruitEvents = new Dictionary<string, int>();
                PlanetDatabase.GetPlanet(capitalSystem.PlanetIds.First(p => p != null && PlanetDatabase.GetPlanet(p).GetAvailableSettlementSlots() == 10)).AddSettlement(settlement);

            }
        }

        private void InitFactions()
        {
            var factionList = FactionDatabase.Query(_ => true);
            
            foreach (var faction in factionList)
            {
                factions.Add(faction);
            }
        }
            
        private async Task InitStars(int numStars, int seed)
        {
            Random.InitState(seed);
            int numArms = 5;
            float armDist = 2 * Mathf.PI / numArms;
            float armOffsetMax = 1f;
            float rotationFactor = 1.7f;
            float randomOffsetXY = 0.1f;
            for (int i = 0; i < numStars; i++)
            {
                StarSystem ss = new StarSystem();

                float dist = Mathf.Sqrt(Random.value) * 1.6f;
                dist = Mathf.Pow(dist, 2);

                float angle = Random.value * 2 * Mathf.PI;
                float armOffset = Random.value * armOffsetMax;
                armOffset -= armOffsetMax / 2;
                armOffset *= (1 / dist);

                float armOffsetSquared = Mathf.Pow(armOffset, 2);
                if (armOffset < 0)
                    armOffsetSquared *= -1;
                armOffset = armOffsetSquared;

                float rotation = dist * rotationFactor;

                angle = (int)(angle / armDist) * armDist + armOffset + rotation;

                float starX = Mathf.Cos(angle) * dist;
                float starY = Mathf.Sin(angle) * dist;

                float randomOffsetX = Random.value * randomOffsetXY;
                float randomOffsetY = Random.value * randomOffsetXY;

                starX += randomOffsetX;
                starY += randomOffsetY;
                float starZ = Random.Range(-.06f,.06f);

                ss.Position = new Vector3(starX, starZ, starY);
            
                starSystems.Add(ss);
            }

            var stars = Resources.Load<TextAsset>("IAU star names - Official IAU Catalog");
            var nameGenerator = new NameGenerator(stars.text.Split("\n"), 20, 0.015f);
            var starNames = await nameGenerator.GenerateNames(starSystems.Count, 5, 10, 1,  null, new System.Random());
            foreach (var ss in starSystems)
            {
                var starName = starNames[Random.Range(0, starNames.Count)];
                starNames.Remove(starName);
                TextInfo ti = new CultureInfo("en-US", false).TextInfo;
                starName = ti.ToTitleCase(starName);
                starName = Utilities.TrimNonLetterChars(starName);
                GenerateStar(ss, starName);
            }
        }

        public int huge= 0, large= 0, medium= 0, small= 0, tiny= 0, GGHuge = 0;
        public int asteroid = 0, ocean = 0, desert= 0, ice= 0, gg= 0, rad= 0, jgl= 0, con = 0;
        private void GenerateStar(StarSystem ss, string starName)
        {
            float starTypeChance = Random.value;
        
            if (starTypeChance > .9f)
                planets.AddRange(ss.Generate(StarType.Triple, starName, this));
            else if (starTypeChance > .85f)
                planets.AddRange(ss.Generate(StarType.Twin, starName, this));
            else if (starTypeChance > .70f)
                planets.AddRange(ss.Generate(StarType.Violet, starName, this));
            else if (starTypeChance > .65f)
                planets.AddRange(ss.Generate(StarType.Blue, starName, this));
            else if (starTypeChance > .45f)
                planets.AddRange(ss.Generate(StarType.White, starName, this));
            else
                planets.AddRange(ss.Generate(StarType.Yellow, starName, this));
        }
    
        public StarSystem GetStar(int starIndex)
        {
            return starSystems[starIndex];
        }

        public int GetNumStars()
        {
            return starSystems.Count;
        }

        public void AddSettlement(Settlement settlement)
        {
            settlements.Add(settlement);
        }

        public List<Planet> GetPlanets()
        {
            return planets;
        }

        public List<Settlement> GetSettlements()
        {
            return settlements;
        }

        public List<StarSystem> GetStarSystems()
        {
            return starSystems;
        }
    }
}