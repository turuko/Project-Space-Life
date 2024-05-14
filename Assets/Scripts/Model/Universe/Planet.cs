using System;
using System.Collections.Generic;
using Controller;
using Model.Databases;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Universe
{
    public enum PlanetType
    {
        Asteroid,
        Ocean,
        Desert,
        Ice,
        GasGiant,
        Radiated,
        Jungle,
        Continental
        //... more types?
    }

    public enum PlanetSize
    {
        Tiny = 1,
        Small = 3,
        Medium = 5,
        Large = 7,
        Huge = 10
    }
    
    [Serializable]
    public class Planet
    {
        public string Name;

        public int planetIndex;

        public PlanetType planetType;

        public PlanetSize planetSize;

        public int maxSettlementSlots;

        [SerializeField] private Vector3 Position;

        [SerializeField] private int availableSettlementSlots;

        [SerializeField] private string starSystemId;
        
        [SerializeField] private string id;

        public string Id
        {
            get => id;
            set => id = value;
        }

        [SerializeField] private List<string> SettlementIds;
        
        public Planet()
        {
            SettlementIds = new List<string>();
        }

        public void Generate(string _name, PlanetType type, PlanetSize size, string _starSystemId, int index)
        {
            Name = _name;
            planetIndex = index;
            Id = Guid.NewGuid().ToString();
            Position = Vector3.zero;
            starSystemId = _starSystemId;
            planetType = type;
            planetSize = size;
            var slots = planetType == PlanetType.GasGiant ? 0 : (int)planetSize;

            slots = planetType == PlanetType.Radiated ? slots - 2 : slots;
        
            availableSettlementSlots = maxSettlementSlots = slots;
            PlanetDatabase.AddPlanet(this);
        }

        public void AddSettlement(Settlement settlement)
        {
            if (availableSettlementSlots >= settlement.RequiredSettlementSlots)
            {
                SettlementIds.Add(settlement.Id);
                availableSettlementSlots -= settlement.RequiredSettlementSlots;
                settlement.SetPlanet(Id);
                GameManager.Instance.GetGalaxy().AddSettlement(settlement);
            }
            else
            {
                Debug.Log("Not enough space for settlement on this planet");
            }
        }

        public void SetPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public int GetAvailableSettlementSlots()
        {
            return availableSettlementSlots;
        }

        public List<Settlement> GetSettlements()
        {
            return SettlementDatabase.GetSettlements(SettlementIds.ToArray());
        }

        public StarSystem GetStarSystem()
        {
            return StarSystemDatabase.GetStarSystem(starSystemId);
        }
    }
}