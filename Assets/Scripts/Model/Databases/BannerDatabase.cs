using System;
using System.Collections.Generic;
using System.Linq;
using Model.Factions;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Model.Databases
{
    public static class BannerDatabase
    {
        private static List<Banner> allBanners;

        public static void Init()
        {
            allBanners = new List<Banner>();
            var guids = AssetDatabase.FindAssets("t:Banner");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var bannerAsset = AssetDatabase.LoadAssetAtPath<Banner>(path);

                var banner = bannerAsset.Clone();
                
                allBanners.Add(banner);
            }
        }

        public static bool AddBanner(Banner banner)
        {
            if (allBanners.Contains(banner))
                return false;
            
            allBanners.Add(banner);
            return true;
        }

        public static Banner GetBanner(string id)
        {
            return allBanners.FirstOrDefault(c => c.Id.Equals(id));
        }

        public static void UpdateBanner(Banner faction)
        {
            var existingFaction = allBanners.FirstOrDefault(f => f.Id == faction.Id);

            if (existingFaction == null) return;
            var index = allBanners.IndexOf(existingFaction);
            allBanners[index] = faction;
        }

        public static List<Banner> GetBanners(params string[] ids)
        {
            return allBanners.Where(c => ids.Contains(c.Id)).ToList();
        }

        public static List<Banner> Query(Func<Banner, bool> predicate)
        {
            return allBanners.Where(predicate).ToList();
        }
    }
}