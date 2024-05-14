using System.Collections.Generic;
using Controller;
using Model.Databases;
using Model.Factions;
using Model.Universe;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Visual_Scripts.Factions;

namespace Visual_Scripts
{
    public class StarSystemVisual : MonoBehaviour
    {

        public GameObject BannerPrefab;
        public void InitiateVisuals(GameObject[] planetPrefabs, GameObject starPrefab, StarSystem ss)
        {
            Debug.Log("StarSystemVisual::Init");
            var systemGO = Instantiate(starPrefab, Vector3.zero, Quaternion.identity);
            systemGO.name = ss.Name;
            systemGO.transform.localScale = Vector3.one;
            foreach (var sphere in systemGO.GetComponentsInChildren<Rotator>())
            {
                sphere.transform.localScale = new Vector3(15, 15, 15);
                sphere.transform.localPosition *= 50f;
            }
                
            
            foreach (var planetId in ss.PlanetIds)
            {
                if (planetId == null)
                    continue;

                var planet = PlanetDatabase.GetPlanet(planetId);
                
                Debug.Log("Planet Index: " + planet.planetIndex);
                
                var planetGO = Instantiate(planetPrefabs[(int)planet.planetType], systemGO.transform);
                planetGO.GetComponentInChildren<Rotator>().transform.localScale =
                    new Vector3((int)planet.planetSize, (int)planet.planetSize, (int)planet.planetSize);

                float randomAngle = Random.Range(0f, 360f);
                float randomAngleRad = randomAngle * Mathf.Deg2Rad;

                float randomZAngleRad = Random.Range(-14f, 14f) * Mathf.Deg2Rad;
                
                float radius = (planet.planetIndex+1) * 20f;
                float posX = 0 + radius * Mathf.Cos(randomAngleRad);
                float posY = 0 + radius * Mathf.Sin(randomAngleRad);
                float positionZ = 0 + radius * Mathf.Tan(randomZAngleRad);

                var planetPos = new Vector3(posX, positionZ, posY);
                planet.SetPosition(planetPos);
                
                planetGO.transform.localPosition = planetPos;
                planetGO.GetComponent<RotateAround>().target = systemGO.transform;

                List<Faction> factionsSeen = new List<Faction>();
                Dictionary<Faction, BannerVisual> factionBanners = new Dictionary<Faction, BannerVisual>();
                Dictionary<Faction, int> numSettlementsPrFaction = new Dictionary<Faction, int>();
                foreach (var settlement in planet.GetSettlements())
                {
                    if (factionsSeen.Contains(FactionDatabase.GetFaction(settlement.FactionId)))
                    {
                        numSettlementsPrFaction[FactionDatabase.GetFaction(settlement.FactionId)]++;
                    }
                    else
                    {
                        var banner = Instantiate(BannerPrefab, planetGO.GetComponentInChildren<Canvas>().transform);
                        banner.GetComponent<BannerVisual>().Banner = BannerDatabase.GetBanner(FactionDatabase.GetFaction(settlement.FactionId).BannerId);
                        banner.GetComponent<BannerVisual>().setBanner = true;
                        banner.transform.localScale *= 5f;
                        factionsSeen.Add(FactionDatabase.GetFaction(settlement.FactionId));
                        factionBanners.Add(FactionDatabase.GetFaction(settlement.FactionId), banner.GetComponent<BannerVisual>());
                        numSettlementsPrFaction.Add(FactionDatabase.GetFaction(settlement.FactionId), 1);
                    }
                    factionBanners[FactionDatabase.GetFaction(settlement.FactionId)].GetComponentInChildren<TextMeshProUGUI>().text =
                        "x" + numSettlementsPrFaction[FactionDatabase.GetFaction(settlement.FactionId)];
                }
                SystemController.Instance.PlanetGOs.Add(planetGO, planet);
            }
        }
    }
}