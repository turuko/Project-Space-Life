using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model.Databases;
using Model.Factions;
using Model.Universe;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Visual_Scripts.Factions;

namespace Visual_Scripts
{
    public class GalaxyVisual : MonoBehaviour
    {
        public GameObject[] StarPrefabs = new GameObject[Enum.GetValues(typeof(StarType)).Length];
        public GameObject[] PlanetPrefabs;
        public GameObject bannerContainer;
        public GameObject bannerGO;

        public void InitiateVisuals(Galaxy galaxy)
        {
            var allStarSystems = galaxy.GetStarSystems();

            Debug.Log("Keys in dictionaries: " + UniverseController.Instance.StarSystemToGO.Count + ", " + UniverseController.Instance.GOToStarSystem.Count);

            foreach (var faction in FactionDatabase.Query(_ => true))      
            {
                Debug.Log(faction.Name + ", " + faction.SettlementIds.Count);
            }

            for (int i = 0; i < allStarSystems.Count; i++)
            {
                StarSystem ss = allStarSystems[i];
                int spacing = ((int)GameManager.Instance.GetGalaxySize() + 1) * 3;
                GameObject ssGO = Instantiate(StarPrefabs[(int)ss.StarType], ss.Position * spacing, Quaternion.identity, transform);
                ssGO.name = ss.Name;

                GameObject container = Instantiate(bannerContainer, ssGO.GetComponentInChildren<Canvas>().transform);

                var settlementsInSystem = ss.PlanetIds.Where(p =>
                    {
                        if (p == null)
                            return false;
                        return PlanetDatabase.GetPlanet(p).GetSettlements() != null && PlanetDatabase.GetPlanet(p).GetSettlements().Count > 0;
                    }).SelectMany(p => PlanetDatabase.GetPlanet(p).GetSettlements()).ToList();
                

                List<Faction> factionsSeen = new List<Faction>();
                Dictionary<Faction, BannerVisual> factionBanners = new Dictionary<Faction, BannerVisual>();
                Dictionary<Faction, int> numSettlementsPrFaction = new Dictionary<Faction, int>();

                foreach (var settlement in settlementsInSystem)
                {
                    if (factionsSeen.Contains(FactionDatabase.GetFaction(settlement.FactionId)))
                    {
                        numSettlementsPrFaction[FactionDatabase.GetFaction(settlement.FactionId)]++;
                    }
                    else
                    {
                        var banner = Instantiate(bannerGO, container.transform);
                        banner.GetComponent<BannerVisual>().Banner = BannerDatabase.GetBanner(FactionDatabase.GetFaction(settlement.FactionId).BannerId);
                        banner.GetComponent<BannerVisual>().setBanner = true;
                        banner.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        factionsSeen.Add(FactionDatabase.GetFaction(settlement.FactionId));
                        factionBanners.Add(FactionDatabase.GetFaction(settlement.FactionId), banner.GetComponent<BannerVisual>());
                        numSettlementsPrFaction.Add(FactionDatabase.GetFaction(settlement.FactionId), 1);
                    }
                    factionBanners[FactionDatabase.GetFaction(settlement.FactionId)].GetComponentInChildren<TextMeshProUGUI>().text =
                        "x" + numSettlementsPrFaction[FactionDatabase.GetFaction(settlement.FactionId)];
                }

                UniverseController.Instance.GOToStarSystem.Add(ssGO, ss);
                UniverseController.Instance.StarSystemToGO.Add(ss, ssGO);
            }
        }

        public void DestroyVisuals()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
