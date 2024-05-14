using System;
using System.Collections;
using AYellowpaper.SerializedCollections.KeysGenerators;

namespace Utility.KeyListGenerators
{
    [KeyListGenerator("Combat Stats", typeof(string))]
    public class CombatStatsKeyListGenerator : KeyListGenerator
    {
        public override IEnumerable GetKeys(Type type)
        {
            return new[] { "Rifle", "Pistol", "One Hand", "Two Hand", "Heavy Gun", "Pilot", "Ship Gun" };
        }
    }
    
    [KeyListGenerator("Attributes", typeof(string))]
    public class AttributeKeyListGenerator : KeyListGenerator
    {
        public override IEnumerable GetKeys(Type type)
        {
            return new[] { "Leadership", "Trade", "Ground Tactics", "Space Tactics", "Health", "Psychic Power" };
        }
    }
}