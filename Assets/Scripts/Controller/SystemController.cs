using System.Collections.Generic;
using System.Linq;
using Battle.SystemBattle;
using Model.Databases;
using Model.Universe;
using UnityEngine;
using Visual_Scripts;

namespace Controller
{
    public class SystemController : MonoBehaviour
    {
        private StarSystem System;

        public GameObject[] StarPrefabs;
        public GameObject[] PlanetPrefabs;
        public GameObject PlayerPrefab;

        public Dictionary<GameObject, Planet> PlanetGOs = new ();

        [SerializeField]
        private StarSystemVisual SystemVisual;

        public static SystemController Instance;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Two SystemManagers");
                Destroy(this);
                return;
            }
            
            Instance = this;
        }
        
        public void Init()
        {
            Debug.Log("System Controller Start");
            System = StarSystemDatabase.GetStarSystem(GameManager.Instance.GetSystemInFocus());

            SystemVisual.InitiateVisuals(PlanetPrefabs, StarPrefabs[(int)System.StarType], System);
            var playerController = FindObjectOfType<PlayerController>();
            playerController.GetComponentInChildren<MeshFilter>().transform.localScale *= 30f;
            
            playerController.targetPos = playerController.transform.position;
            playerController.Camera = Camera.main;
            playerController.MoveSpeed = 15f;

            var cameraController = FindObjectOfType<CameraController>();
            cameraController.followTransform = playerController.transform;
            cameraController.newZoom = new Vector3(0, 250, -250);
            cameraController.maxZoom = 250f;
            cameraController.minZoom = 35f;
            cameraController.minZoomY = 25f;
        }

        public SystemBattleTerrainInfo CreateTerrainInfo()
        {
            Vector3 battlePos = FindObjectOfType<PlayerController>().transform.position;
            var relativeStarPos = Vector3.zero - battlePos;
            var planetInfos = PlanetGOs.Select(x => (x.Key.transform.position - battlePos, x.Value.planetSize))
                .ToArray();
            
            return new SystemBattleTerrainInfo(System.StarType, relativeStarPos, planetInfos);
        }
    }
}