using System;
using System.Collections.Generic;
using System.Linq;
using Model.Databases;
using Model.Items;
using UnityEngine;

namespace Model.Factions
{
    [Serializable]
    public class Party
    {
        [SerializeField] private string LeaderId;

        [SerializeField] private string id;

        public string Id
        {
            get => id;
            set => id = value;
        }

        private List<Regiment> regiments;
        
        private List<string> companionsId;
        
        //Key represents the unit, value is the amount of those units in the party
        private Dictionary<string, int> unitsIdAmount;
        
        //Key is the unit Id, Value is a list which has the same length as the unitsIdAmount[key], each value representing whether a unit is combat ready
        private Dictionary<string, List<bool>> unitCombatReady;

        public List<string> CompanionsId { get => companionsId; private set => companionsId = value; }
        public Dictionary<string, int> UnitsId { get => unitsIdAmount; set => unitsIdAmount = value; }
        public Dictionary<string, List<bool>> UnitCombatReady { get => unitCombatReady; set => unitCombatReady = value; }

        public List<Regiment> Regiments
        {
            get => regiments;
            private set => regiments = value;
        }

        private float foodCost => CalculateFoodCost();
        private float goldCost => CalculateGoldCost();
        public float Morale { get; }

        public Inventory Inventory { get; private set; }

        public float MaxWeight;

        [SerializeField]
        private float gold;
        public float Gold
        {
            get => gold;
            set
            {
                gold = value;
                if(partyChanged != null)
                    partyChanged(this);
            }
        }

        public float Food;

        private Action<Party> partyChanged;

        public Party(string leaderId)
        {
            Id = Guid.NewGuid().ToString();
            LeaderId = leaderId;
            CompanionsId = new List<string>();
            UnitsId = new Dictionary<string, int>();
            UnitCombatReady = new Dictionary<string, List<bool>>();
            Inventory = new Inventory();
            Gold = 100;
            Food = 1;
        }
        

        private float CalculateFoodCost()
        {
            return UnitsId.Sum(u => UnitDatabase.GetUnit(u.Key).FoodCost * u.Value) + CharacterDatabase.GetCharacters(CompanionsId.ToArray()).Sum(companion => companion.FoodCost) + CharacterDatabase.GetCharacter(LeaderId).FoodCost;
        }

        private float CalculateGoldCost()
        {
            return UnitsId.Sum(u => UnitDatabase.GetUnit(u.Key).GoldCost * u.Value) + CharacterDatabase.GetCharacters(CompanionsId.ToArray()).Sum(companion => companion.GoldCost) + CharacterDatabase.GetCharacter(LeaderId).GoldCost;
        }

        public void UpgradeUnit(string unitToUpgrade, string upgradeUnit)
        {
            if (!UnitsId.ContainsKey(unitToUpgrade))
            {
                Debug.LogError("Trying to upgrade unit not in party");
                return;
            }

            if (!UnitDatabase.GetUnit(unitToUpgrade).UpgradesTo.Contains(upgradeUnit))
            {
                Debug.LogError("Unit cant be upgrade to that!!");
                return;
            }
            
            UnitsId[unitToUpgrade] --;
            if (UnitsId[unitToUpgrade] == 0)
                UnitsId.Remove(unitToUpgrade);
            
            if(!UnitsId.ContainsKey(upgradeUnit))
                UnitsId.Add(upgradeUnit, 1);
            else
                UnitsId[upgradeUnit]++;
        }

        public void AddUnit(string unit)
        {
            if (!UnitsId.ContainsKey(unit))
            {
                UnitsId.Add(unit, 1);
                UnitCombatReady.Add(unit, new List<bool>(){true});
            }
            else
            {
                UnitsId[unit]++;
                UnitCombatReady[unit].Add(true);
            }

            partyChanged(this);
        }

        public void CreateRegiment(Character commander, List<Unit> units, Item spaceship = null)
        {
            var idCount = units.Select(x => (x.Id, units.Count(u => u.Id == x.Id)));

            var unitDictionary = new Dictionary<string, int>();
            
            foreach (var tuple in idCount)
            {
                if (!UnitsId.ContainsKey(tuple.Id))
                {
                    Debug.Log("Can't create regiment with units not in party");
                    return;
                }

                if (UnitsId[tuple.Id] < tuple.Item2)
                {
                    Debug.Log("Can't create regiment with units not in party");
                    return;
                }
                unitDictionary.Add(tuple.Id, tuple.Item2);
            }

            var regiment = new Regiment(commander, unitDictionary, spaceship);
            Regiments.Add(regiment);
        }

        public int GetNumberInParty()
        {
            return 1 + UnitsId.Sum(u => u.Value) + CompanionsId.Count;
        }

        public float GetFoodCost()
        {
            return foodCost;
        }

        public float GetGoldCost()
        {
            return goldCost;
        }
        
        public void RegisterPartyChangedCB(Action<Party> cb)
        {
            partyChanged += cb;
        }
        
        public void UnregisterPartyChangedCB(Action<Party> cb)
        {
            partyChanged -= cb;
        }
    }
}