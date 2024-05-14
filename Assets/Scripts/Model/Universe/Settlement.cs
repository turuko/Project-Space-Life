using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Controller;
using Model.Databases;
using Model.Factions;
using Newtonsoft.Json;
using UnityEngine;

namespace Model.Universe
{
    public enum SettlementType
    {
        SpecializedColony = 1,
        SmallTown = 2,
        MediumTown = 3,
        City = 4,
        PlanetCity = 10
        //Are these types correct, maybe need more?
    }

    public enum SpecializedColonyType
    {
        MiningColony,
        ScienceColony,
        MercenaryBase,
        SmugglerHideout
        //... more types?
    }

    [CreateAssetMenu(fileName = "New Settlement", menuName = "Custom/Settlement")]
    public class Settlement : ScriptableObject
    {
        public SettlementType SettlementType;

        public string Name;
        
        [SerializeField] protected string id;

        public string Id
        {
            get => id;
            set => id = value;
        }

        public int RequiredSettlementSlots;

        public float Prosperity;

        [SerializeField] protected string planetId;
        
        public string PlanetId
        {
            get => planetId;
            set => planetId = value;
        }

        [SerializeField] protected string factionId;

        public string FactionId
        {
            get => factionId;
            private set => factionId = value;
        }

        public Dictionary<string, int> characterRecruitEvents;
        [JsonIgnore] public List<Unit> Recruits { get; private set; }

        public Settlement(SettlementType type, string name)
        {
            SettlementType = type;
            Name = name;
            RequiredSettlementSlots = (int)SettlementType;
            Recruits = new List<Unit>();
            Id = Guid.NewGuid().ToString();
        }

        public void SetPlanet(string planetId)
        {
            PlanetId = planetId;
        }

        public void SetFaction(string faction)
        {
            FactionId = faction;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"> Character for which recruits need to be determined</param>
        public void DetermineRecruits(Character character)
        {
            characterRecruitEvents ??= new Dictionary<string, int>();
            
            Recruits = new List<Unit>();
            var planet = PlanetDatabase.GetPlanet(PlanetId);
            var system = planet.GetStarSystem();
            var species = SpeciesDatabase.GetSpecies(system.PrimarySpeciesId);
            
            //Todo implement an algorithm for determining which units and how many show up as recruits.
            
            int baseAmount = 3;
            
            // add more based on character standing, time since character last recruited, etc.
            
            int rotationsSinceLastRecruit = 1000;

            if (characterRecruitEvents.ContainsKey(character.Id))
            {
                Debug.Log("found last recruit event index");
                rotationsSinceLastRecruit = GameManager.Instance.GetRotationController().TimeSinceEvent(this, characterRecruitEvents[character.Id]);
            }
                
            Debug.Log("rotationsSinceLastRecruit: " + rotationsSinceLastRecruit);
            
            
            var amountRecruits = baseAmount; // + modifiers

            amountRecruits = rotationsSinceLastRecruit switch
            {
                <= 5 => 0,
                > 5 and <= 7 => Mathf.RoundToInt(amountRecruits * 0.5f),
                > 7 and <= 9 => Mathf.RoundToInt(amountRecruits * 0.7f),
                > 9 and <= 14 => Mathf.RoundToInt(amountRecruits * 0.85f),
                > 14 => Mathf.RoundToInt(amountRecruits * 1f)
            };

            Debug.Log("Species: " + species.Name);
            //determine what type of recruits are available based on species of planet, character standing with settlement etc.
            var availableRecruits = UnitDatabase.Query(u => u.SpeciesId == species.Id);
            Debug.Log("availableRecruits: " + amountRecruits);
            for (int i = 0; i < amountRecruits; i++)
            {
                Recruits.Add(availableRecruits.First());
            }
        }

        public void Recruit(Unit unit, Character character)
        {
            if (!characterRecruitEvents.ContainsKey(character.Id))
            {
                characterRecruitEvents.Add(character.Id, -1);
            }
            characterRecruitEvents[character.Id] = GameManager.Instance.GetRotationController().LogEvent(this);
            Debug.Log("Settlement::Recruit :" + characterRecruitEvents[character.Id]);
            Recruits.Remove(unit);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("--" + Name + "--\n");
            sb.Append("SettlementType: " + SettlementType + "\n");
            sb.Append("Id: " + Id + "\n");
            sb.Append("RequiredSettlementSlots: " + RequiredSettlementSlots + "\n");
            sb.Append("Prosperity: " + Prosperity + "\n");
            sb.Append("PlanetId: " + PlanetId + "\n");
            sb.Append("FactionId: " + FactionId + "\n");
            return sb.ToString();
        }
    }

    public class Colony : Settlement
    {
        public SpecializedColonyType ColonyType;

        public Colony(SettlementType type, string name, SpecializedColonyType colonyType) : base(type, name)
        {
            ColonyType = colonyType;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(base.ToString());
            sb.Append("ColonyType: " + ColonyType);
            return sb.ToString();
        }
    }
}