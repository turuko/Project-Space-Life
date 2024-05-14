using System;
using System.Collections.Generic;
using Model.Databases;
using Model.Universe;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Model.Factions
{
    [CreateAssetMenu(fileName = "New Faction", menuName = "Custom/Faction")]
    public class Faction : ScriptableObject
    {
        [SerializeField] private new string name;

        [SerializeField] private string id;
        public string Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        [FormerlySerializedAs("Settlements")] public SerializableSet<string> SettlementIds;
        [FormerlySerializedAs("Capital")] public string CapitalId;

        [FormerlySerializedAs("AllCharacters")] public SerializableSet<string> AllCharacterIds;
    
        [FormerlySerializedAs("Commanders")] public SerializableSet<string> CommanderIds;

        [FormerlySerializedAs("Leader")] public string LeaderId;

        
        [SerializeField] private string bannerId;
        public string BannerId { get => bannerId; set => bannerId = value; }

        [FormerlySerializedAs("CoreSpecies")] public SerializableSet<string> CoreSpeciesIds;

        public Faction(string name, SerializableSet<string> settlementIds, SerializableSet<string> characterIds,
            SerializableSet<string> commanderIds, string leaderId, string bannerId)
        {
            Name = name;
            SettlementIds = settlementIds;
            foreach (var settlement in SettlementIds)
            {
                SettlementDatabase.GetSettlement(settlement).SetFaction(Id);
            }
            AllCharacterIds = characterIds;
            foreach (var characterId in AllCharacterIds)
            {
                var character = CharacterDatabase.GetCharacter(characterId);
                character.SetFaction(this);
            }
            CommanderIds = commanderIds;
            foreach (var commanderId in CommanderIds)
            {
                if (!AllCharacterIds.Contains(commanderId))
                {
                    Debug.LogError("Commander not a character of faction!!");
                    CommanderIds.Remove(commanderId);
                }
            }
            LeaderId = leaderId;
            BannerId = bannerId;
        }

        public void AddSettlement(Settlement settlement)
        {
            SettlementIds.Add(settlement.Id);
            settlement.SetFaction(Id);
        }

        public bool RemoveSettlement(Settlement settlement)
        {
            return SettlementIds.Remove(settlement.Id);
        }
        
        public void AddCharacter(Character character)
        {
            AllCharacterIds.Add(character.Id);
            character.SetFaction(this);
        }

        public bool RemoveCharacter(Character character)
        {
            character.SetFaction(null);
            return AllCharacterIds.Remove(character.Id);
        }
        
        public void AddCommander(Character character)
        {
            CommanderIds.Add(character.Id);
            AllCharacterIds.Add(character.Id);
            character.SetFaction(this);
        }

        public bool RemoveCommander(Character character)
        {
            return CommanderIds.Remove(character.Id);
        }

        public void ChangeLeader(Character newLeader)
        {
            LeaderId = newLeader.Id;
        }

        public void SetBanner(Banner banner)
        {
            BannerId = banner.Id;
        }
    }
}