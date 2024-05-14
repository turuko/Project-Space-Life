using System;
using System.Collections.Generic;
using Model.Databases;
using Model.Factions;
using Model.Universe;
using UnityEngine;
using Visual_Scripts;

namespace Controller
{
    public class UniverseController : MonoBehaviour
    {
        public GalaxyVisual GV;
    
        private Galaxy galaxy;
        
        public static UniverseController Instance;

        public Dictionary<GameObject, StarSystem> GOToStarSystem = new();
        
        public Dictionary<StarSystem, GameObject> StarSystemToGO = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("TwoUniverseManagers");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public void InitiateVisuals()
        {
            GOToStarSystem = new Dictionary<GameObject, StarSystem>();
            StarSystemToGO = new Dictionary<StarSystem, GameObject>();

            var groundSize = Vector3.one;
            
            switch (GameManager.Instance.GetGalaxySize())
            {
                case GalaxySize.Tiny:
                    groundSize = new Vector3(20, 0, 20);
                    break;
                case GalaxySize.Small:
                    groundSize = new Vector3(30, 0, 30);
                    break;
                case GalaxySize.Medium:
                    groundSize = new Vector3(50, 0, 50);
                    break;
                case GalaxySize.Large:
                    groundSize = new Vector3(65, 0, 65);
                    break;
                case GalaxySize.Huge:
                    groundSize = new Vector3(85, 0, 85);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            GameObject.FindWithTag("Ground").transform.localScale = groundSize;
            GV.InitiateVisuals(GameManager.Instance.GetGalaxy());
        }
    }
}