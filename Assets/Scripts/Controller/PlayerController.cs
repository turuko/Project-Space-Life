using System;
using System.Linq;
using Model.Universe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utility;

namespace Controller
{
    public class PlayerController : MonoBehaviour, IDataPersistence
    {
        private void Awake()
        {
            InstanceTracker<IDataPersistence>.RegisterInstance(this);
        }

        private void OnDestroy()
        {
            InstanceTracker<IDataPersistence>.UnregisterInstance(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            targetPos = transform.position;
        }

        public Camera Camera;

        public Vector3 targetPos;

        public float MoveSpeed;

        public GameObject PlayerGraphic;

        private StarSystem hitStar = null;

        private Planet hitPlanet = null;
        private bool hitBack = false;

        // Update is called once per frame
        void Update()
        {
            switch (GameManager.Instance.GetCurrentState())
            {
                case GameManager.GameState.Galaxy:
                    GalaxyUpdate();
                    break;
                case GameManager.GameState.System:
                    SystemUpdate();
                    break;
                case GameManager.GameState.InfoPanelOpen:
                    break;
                case GameManager.GameState.InfoMenu:
                    break;
                default:
                    return;
            }
        }

        private void GalaxyUpdate()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                GetTargetPos();
            }
        
            if (Vector3.Distance(transform.position, targetPos) >= 0.001f)
            {
                PlayerGraphic.transform.LookAt(targetPos);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, MoveSpeed * Time.deltaTime);
            }
            else
            {
                if (hitStar == null) return;
                GameManager.Instance.LoadSystemScene(hitStar.Id);
                hitStar = null;
            }
        }

        private void SystemUpdate()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                GetTargetPos();
            }
            
            if (Vector3.Distance(transform.position, targetPos) >= 0.001f)
            {
                PlayerGraphic.transform.LookAt(targetPos);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, MoveSpeed * Time.deltaTime);
            }
            else
            {
                if (hitPlanet != null)
                {
                    GameManager.Instance.OpenPlanetInfo(hitPlanet);
                    hitPlanet = null;
                }
                else if (hitBack)
                {
                    GameManager.Instance.LoadGalaxyScene();
                    hitBack = false;
                }
            }
        }

        private void GetTargetPos()
        {
            var ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var hits = Physics.RaycastAll(ray);
            if (hits.Any(hit => hit.transform.gameObject.layer == LayerMask.NameToLayer("Star")))
            {
                var hit = hits.First(hit => hit.transform.gameObject.layer == LayerMask.NameToLayer("Star"));
                targetPos = hit.transform.position;
                hitStar = UniverseController.Instance.GOToStarSystem[GetStarParent(hit.transform)];
            }
            else if (hits.Any(hit => hit.transform.gameObject.layer == LayerMask.NameToLayer("Planet")))
            {
                var hit = hits.First(hit => hit.transform.gameObject.layer == LayerMask.NameToLayer("Planet"));
                targetPos = hit.transform.position;
                hitPlanet = SystemController.Instance.PlanetGOs[GetPlanetParent(hit.transform)];
            }
            else if (hits.Any(hit => hit.transform.gameObject.layer == LayerMask.NameToLayer("SystemViewBack")))
            {
                var hit = hits.First(hit => hit.transform.gameObject.layer == LayerMask.NameToLayer("SystemViewBack"));
                hitBack = true;
                targetPos = new Vector3(hit.point.x, 0, hit.point.z);
            }
            else if (hits.Any(hit => hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")))
            {
                var hit = hits.First(hit => hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"));
                targetPos = new Vector3(hit.point.x, 0, hit.point.z);
            }
        }

        private GameObject GetStarParent(Transform t)
        {
            if (t.CompareTag("GalaxyViewStar"))
            {
                return t.gameObject;
            }

            return GetStarParent(t.parent);
        }
        
        private GameObject GetPlanetParent(Transform t)
        {
            if (t.CompareTag("SystemViewPlanet"))
            {
                return t.gameObject;
            }

            return GetPlanetParent(t.parent);
        }

        public void LoadData(GameData data)
        {
            if (GameManager.Instance.GetCurrentState() == GameManager.GameState.Galaxy)
            {
                transform.position = data.playerGalaxyPos;
                targetPos = data.playerGalaxyPos;
            }
            else if (GameManager.Instance.GetCurrentState() == GameManager.GameState.System)
            {
                transform.position = data.playerSystemPos;
                targetPos = data.playerSystemPos;
            }
            
        }

        public void SaveData(GameData data)
        {
            if (GameManager.Instance.GetCurrentState() == GameManager.GameState.Galaxy)
            {
                data.playerGalaxyPos = transform.position;
            }
            else if (GameManager.Instance.GetCurrentState() == GameManager.GameState.System)
            {
                data.playerSystemPos = transform.position;
            }
        }
    }
}
