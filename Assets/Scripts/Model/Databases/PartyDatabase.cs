using System;
using System.Collections.Generic;
using System.Linq;
using Model.Factions;

namespace Model.Databases
{
    public class PartyDatabase
    {
        private static List<Party> allParties;

        public static void Init()
        {
            allParties = new List<Party>();
        }

        public static bool AddParty(Party party)
        {
            if (allParties.Contains(party))
                return false;
            
            allParties.Add(party);
            return true;
        }

        public static Party GetParty(string id)
        {
            return allParties.FirstOrDefault(p => p.Id.Equals(id));
        }

        public static List<Party> GetParties(params string[] ids)
        {
            return allParties.Where(p => ids.Contains(p.Id)).ToList();
        }

        public static List<Party> Query(Func<Party, bool> predicate)
        {
            return allParties.Where(predicate).ToList();
        }
    }
}