using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Model.Databases;
using Model.Factions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utility;
using Visual_Scripts;
using Visual_Scripts.Factions;
using Random = UnityEngine.Random;

namespace Model.Universe
{
    public enum StarType
    {
        Red = 0,
        Orange = 1,
        Yellow = 2,
        White = 3,
        Violet = 4,
        Blue = 5,
        Twin = 6,
        Triple = 7
        //... more types?
    }

   
    [Serializable]
    public class StarSystem
    {
        private const int MAX_PLANETS = 6;

        [SerializeField] private string id;

        public string Id
        {
            get => id;
            private set => id = value;
        }

        [SerializeField] [ItemCanBeNull] private string[] planetIds; 
        [ItemCanBeNull] public string[] PlanetIds { get => planetIds; private set => planetIds = value; }

        public StarType StarType { get; set; }

        public Vector3 Position;

        public string Name;

        public string PrimarySpeciesId;
        public StarSystem()
        {
            PlanetIds = new string[MAX_PLANETS];
            PrimarySpeciesId = Guid.Empty.ToString();
        }

        public List<Planet> Generate(StarType _starType, string name, Galaxy galaxy)
        {
            Id = Guid.NewGuid().ToString();
            StarType = _starType;
            Name = name;
            StarSystemDatabase.AddStarSystem(this);
            return GeneratePlanets(galaxy);
        }
    
        public Planet GetPlanet(int planetIndex)
        {
            return PlanetDatabase.GetPlanet(PlanetIds[planetIndex]);
        }

        public int NumPlanets()
        {
            int numPlanets = 0;
            for (int i = 0; i < MAX_PLANETS; i++)
            {
                if (PlanetIds[i] != null)
                {
                    numPlanets++;
                }
            }

            return numPlanets;
        }

        private List<Planet> GeneratePlanets(Galaxy galaxy)
        {
            var planets = new List<Planet>();
            Random.InitState(Random.Range(int.MinValue, Int32.MaxValue));

            var planetChanceLowType = 0.65f + ((int)StarType + 1) * (-0.075f);
            var planetChanceHighType = 0.48f + Mathf.Pow((int)StarType + 1, 0.8f) * (-0.085f);
            var planetChance = (int)StarType < 5.5f ? planetChanceLowType : planetChanceHighType;

        
            for (int i = 0; i < PlanetIds.Length; i++)
            {
                if (Random.value <= planetChance)
                {
                    var planet = new Planet();
                    var planetType = GeneratePlanetType(i);
                    var planetSize = GeneratePlanetSize(planetType, i);
                    var name = Name + " " + Utilities.ConvertToRomanNumeral(i);
                    planet.Generate(name, planetType, planetSize, Id, i);
                    PlanetIds[i] = planet.Id;
                    planets.Add(planet);

                    if (planetType == PlanetType.GasGiant && planetSize == PlanetSize.Huge)
                        galaxy.GGHuge++;
                    //TODO: these switch statements are only for debugging.
                    switch (planetType)
                    {
                        case PlanetType.Asteroid:
                            galaxy.asteroid++;
                            break;
                        case PlanetType.Ocean:
                            galaxy.ocean++;
                            break;
                        case PlanetType.Desert:
                            galaxy.desert++;
                            break;
                        case PlanetType.Ice:
                            galaxy.ice++;
                            break;
                        case PlanetType.GasGiant:
                            galaxy.gg++;
                            break;
                        case PlanetType.Radiated:
                            galaxy.rad++;
                            break;
                        case PlanetType.Jungle:
                            galaxy.jgl++;
                            break;
                        case PlanetType.Continental:
                            galaxy.con++;
                            break;
                    }

                    switch (planetSize)
                    {
                        case PlanetSize.Huge:
                            galaxy.huge++;
                            break;
                        case PlanetSize.Large:
                            galaxy.large++;
                            break;
                        case PlanetSize.Medium:
                            galaxy.medium++;
                            break;
                        case PlanetSize.Small:
                            galaxy.small++;
                            break;
                        case PlanetSize.Tiny:
                            galaxy.tiny++;
                            break;
                    
                    }
                }
            }

            return planets;
        }

        private PlanetSize GeneratePlanetSize(PlanetType type, int pos)
        {
            if (type == PlanetType.Asteroid)
            {
                return PlanetSize.Tiny;
            }

            if (type == PlanetType.GasGiant)
            {
                return PlanetSize.Huge;
            }

            float distance = (float)(pos + 1) / (float)MAX_PLANETS;

            var r = Random.Range(0f, distance);

            if (r >= 0.6f)
                return PlanetSize.Huge;
            if (r >= 0.3f)
                return PlanetSize.Large;
            if (r > 0.15f)
                return PlanetSize.Medium;
        
            return PlanetSize.Small;

        }

        public PlanetType GeneratePlanetTypePublic(int pos)
        {
            return GeneratePlanetType(pos);
        }

        private PlanetType GeneratePlanetType(int pos)
        {
            float goldilocksRange = CalculateGoldilocksRange();
        
            float distance = (float)pos / (float)MAX_PLANETS;
            float distanceSquared = distance * distance;

            float gasGiantWeight = Mathf.Lerp(0f, 1f, distanceSquared);
            float goldilocksWeight = Mathf.Lerp(1f, 0f, 2f * Mathf.Abs(goldilocksRange - distance));
            float desertWeight = Mathf.Lerp(CalculateDesertWeight(),0f, distance);
            float iceWeight = Mathf.Lerp(0f, CalculateIceWeight(), distance);
            float radiatedWeight = Mathf.Lerp( CalculateRadiatedWeight(), 0f, distanceSquared);
            float asteroidWeight = 0.5f;

            float allWeights = gasGiantWeight + desertWeight + iceWeight + radiatedWeight + goldilocksWeight + asteroidWeight;

            var r = Random.Range(0f, allWeights);

            if (r < gasGiantWeight)
            {
                return PlanetType.GasGiant;
            }
            r -= gasGiantWeight;
        
            if (r < desertWeight)
            {
                return PlanetType.Desert;
            }
            r -= desertWeight;

            if (r < iceWeight)
            {
                return PlanetType.Ice;
            }
            r -= iceWeight;

            if (r < radiatedWeight)
            {
                return PlanetType.Radiated;
            }
            r -= radiatedWeight;
        
            if (r < goldilocksWeight)
            {
                var golditype = Random.Range(0f, 3f);
                if (golditype < 1f)
                {
                    return PlanetType.Continental;
                }

                if (golditype < 2f)
                {
                    return PlanetType.Jungle;
                }

                if (golditype < 3f)
                {
                    return PlanetType.Ocean;
                }
            }
            r -= goldilocksWeight;

            return PlanetType.Asteroid;
        }

        private float CalculateRadiatedWeight()
        {
            switch (StarType)
            {
                case StarType.Red:
                    return 0.1f;
                case StarType.Orange:
                    return 0.15f;
                case StarType.Yellow:
                    return 0.25f;
                case StarType.White:
                    return 0.4f;
                case StarType.Violet:
                    return 0.6f;
                case StarType.Blue:
                    return 0.7f;
                case StarType.Twin:
                    return 0.75f;
                case StarType.Triple:
                    return 0.8f;
                default:
                    Debug.LogError("Not a StarType!");
                    return -1;
            }
        }
    
        private float CalculateDesertWeight()
        {
            switch (StarType)
            {
                case StarType.Red:
                    return 0.1f;
                case StarType.Orange:
                    return 0.15f;
                case StarType.Yellow:
                    return 0.25f;
                case StarType.White:
                    return 0.4f;
                case StarType.Violet:
                    return 0.6f;
                case StarType.Blue:
                    return 0.7f;
                case StarType.Twin:
                    return 0.75f;
                case StarType.Triple:
                    return 0.5f;
                default:
                    Debug.LogError("Not a StarType!");
                    return -1;
            }
        }

        private float CalculateIceWeight()
        {
            switch (StarType)
            {
                case StarType.Red:
                    return 0.75f;
                case StarType.Orange:
                    return 0.7f;
                case StarType.Yellow:
                    return 0.6f;
                case StarType.White:
                    return 0.5f;
                case StarType.Violet:
                    return 0.4f;
                case StarType.Blue:
                    return 0.1f;
                case StarType.Twin:
                    return 0.15f;
                case StarType.Triple:
                    return 0.25f;
                default:
                    Debug.LogError("Not a StarType!");
                    return -1;
            }
        }

        private float CalculateGoldilocksRange()
        {
            switch (StarType)
            {
                case StarType.Red:
                    return 0.167f;
                case StarType.Orange:
                    return 0.2500015f;
                case StarType.Yellow:
                    return 0.41666f;
                case StarType.White:
                    return 0.333f;
                case StarType.Violet:
                    return 0.5f;
                case StarType.Blue:
                    return 0.5833f;
                case StarType.Twin:
                    return 0.666f;
                case StarType.Triple:
                    return 0.749f;
                default:
                    Debug.LogError("Not a StarType!");
                    return -1;
            }
        }
    }
}