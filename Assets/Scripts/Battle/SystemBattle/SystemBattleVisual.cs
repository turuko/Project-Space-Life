using System;
using Cinemachine;
using Model.Universe;
using UnityEngine;
using UnityEngine.Serialization;
using Visual_Scripts;

namespace Battle.SystemBattle
{
    public class SystemBattleVisual : MonoBehaviour
    {
        [SerializeField] private GameObject[] StarPrefabs;
        [SerializeField] private GameObject PlanetPrefab;
        [FormerlySerializedAs("light")] [SerializeField] private Light starLight;
        [SerializeField] private CinemachineFreeLook freeLookCam;

        private const float COLOR_CONVERSION = 1f / 255f;
        
        public void InitVisuals(SystemBattleTerrainInfo terrainInfo)
        {
            var starGO = Instantiate(StarPrefabs[(int)terrainInfo.starType], terrainInfo.relativeStarPosition * 1000,
                Quaternion.identity, transform);
            
            starGO.transform.localScale = Vector3.one;
            foreach (var sphere in starGO.GetComponentsInChildren<Rotator>())
            {
                sphere.transform.localScale = new Vector3(10000, 10000, 10000);
                //sphere.transform.localPosition *= 50f;
            }

            if (freeLookCam == null)
                freeLookCam = FindObjectOfType<CinemachineFreeLook>();
            
            starLight.transform.position = starGO.transform.position + new Vector3(0,1000,0);
            var color = GetStarLightColor(terrainInfo.starType);
            starLight.color = color;
    
            for (int i = 0; i < terrainInfo.relativePlanetPositions.Length; i++)
            {
                var planetPosition = terrainInfo.relativePlanetPositions[i];
                var planetSize = terrainInfo.planetSizes[i];
                var planetGO = Instantiate(PlanetPrefab, planetPosition * 1000, Quaternion.identity, transform);
                planetGO.GetComponentInChildren<RotateAround>().enabled = false;
                planetGO.GetComponentInChildren<Rotator>().transform.localScale =
                    new Vector3((int)planetSize, (int)planetSize, (int)planetSize) * 500;
            }
        }

        private void Update()
        {
            if (freeLookCam == null)
                return;
            
            starLight.transform.LookAt(freeLookCam.LookAt.position - new Vector3(4,0,0));
        }

        private Color GetStarLightColor(StarType type)
        {
            switch (type)
            {
                case StarType.Red:
                    return new Color(255 * COLOR_CONVERSION, 193* COLOR_CONVERSION, 181* COLOR_CONVERSION);
                case StarType.Orange:
                    return new Color(255* COLOR_CONVERSION, 213* COLOR_CONVERSION, 181* COLOR_CONVERSION);
                case StarType.Yellow:
                    return new Color(255* COLOR_CONVERSION, 251* COLOR_CONVERSION, 181* COLOR_CONVERSION);
                case StarType.White:
                    return new Color(237* COLOR_CONVERSION, 237* COLOR_CONVERSION, 237* COLOR_CONVERSION);
                case StarType.Violet:
                    return new Color(246* COLOR_CONVERSION, 217* COLOR_CONVERSION, 255* COLOR_CONVERSION);
                case StarType.Blue:
                    return new Color(217* COLOR_CONVERSION, 233* COLOR_CONVERSION, 255* COLOR_CONVERSION);
                case StarType.Twin:
                    return (new Color(255* COLOR_CONVERSION, 193* COLOR_CONVERSION, 181* COLOR_CONVERSION) + new Color(217* COLOR_CONVERSION, 233* COLOR_CONVERSION, 255* COLOR_CONVERSION)) * 0.5f;
                case StarType.Triple:
                    return (new Color(255* COLOR_CONVERSION, 193* COLOR_CONVERSION, 181* COLOR_CONVERSION) + new Color(217* COLOR_CONVERSION, 233* COLOR_CONVERSION, 255* COLOR_CONVERSION) + new Color(246* COLOR_CONVERSION, 217* COLOR_CONVERSION, 255* COLOR_CONVERSION)) / 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}