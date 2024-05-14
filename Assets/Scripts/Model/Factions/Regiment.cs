using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Model.Items;

namespace Model.Factions
{
    public class Regiment
    {
        public Character Commander;

        public Dictionary<string, int> units;

        public Item ship;

        public Regiment(Character commander, Dictionary<string, int> units, Item spaceship)
        {
            Commander = commander;
            this.units = units;
            ship = spaceship;
        }
    }
}