using System.Collections.Generic;

namespace Model.Stat_System
{
    public static class StatNames
    {
        public static readonly List<string> AttributeNames = new()
            { "Leadership", "Trade", "Ground Tactics", "Space Tactics", "Vitality", "Psychic Power" };
        
        public static readonly List<string> CombatNames = new()
            { "Rifle", "Pistol", "One hand", "Two hand", "Pilot", "Ship gun", "Heavy gun" };
    }
}