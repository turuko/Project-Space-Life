using System.Linq;
using Battle.Entity_Components;
using Controller;
using Model.Databases;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Battle.SystemBattle
{
    public class SystemBattleController: MonoBehaviour
    {
        public static SystemBattleController Instance;

        private SystemBattleTerrainInfo terrainInfo;

        [SerializeField] private SystemBattleVisual visual;
        [SerializeField] private Transform attackingCompany;
        [SerializeField] private Transform defendingCompany;
        
        [SerializeField] private GameObject battlePlayerPrefab;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Init()
        {
            visual.InitVisuals(terrainInfo);
            //TODO: Determine if the player is the attacker or the defender. For now player is always attacker
            InitPlayerCompany(attackingCompany);
        }

        private void InitPlayerCompany(Transform spawnArea)
        {
            var playerGO = PlayerInput.Instantiate(battlePlayerPrefab, 0, "Keyboard & Mouse").gameObject;
            playerGO.GetComponent<CombatArmor>().Initialize(GameManager.Instance.GetPlayerCharacter());
            playerGO.GetComponent<CombatHealth>().Initialize(GameManager.Instance.GetPlayerCharacter());
            playerGO.GetComponent<Attack>().Initialize(GameManager.Instance.GetPlayerCharacter());
            
            var position = spawnArea.position;
            var localScale = spawnArea.localScale;
            Vector3 spawnPosition = new Vector3(
                Random.Range(position.x - localScale.x / 2, position.x + localScale.x / 2),
                position.y,
                Random.Range(position.z - localScale.z / 2, position.z + localScale.z / 2)
            );

            playerGO.transform.position = spawnPosition;


            foreach (var regiment in GameManager.Instance.GetPlayerParty().Regiments)
            {
                if (regiment.ship != null)
                {
                    // Spawn spaceship
                    // spawn units in regiment within spaceship?
                    spawnPosition = new Vector3(
                        Random.Range(position.x - localScale.x / 2, position.x + localScale.x / 2),
                        position.y,
                        Random.Range(position.z - localScale.z / 2, position.z + localScale.z / 2)
                    );

                    var spaceshipGO = Instantiate(ItemDatabase.GetSpaceshipGameObject(regiment.ship), spawnPosition,
                        Quaternion.identity);
                }
            }
            
            var units = UnitDatabase.GetPartyUnits(GameManager.Instance.GetPlayerParty().UnitsId);
            var readyUnits = units.GroupBy(x => x.Id).SelectMany(x =>
            {
                var ready = GameManager.Instance.GetPlayerParty().UnitCombatReady[x.Key].Count(y => y == true);
                return x.Take(ready);
            }).ToList();
            
            foreach (var unit in readyUnits)
            {
                // Determine spawn point.
                
                spawnPosition = new Vector3(
                    Random.Range(position.x - localScale.x / 2, position.x + localScale.x / 2),
                    position.y,
                    Random.Range(position.z - localScale.z / 2, position.z + localScale.z / 2)
                );
                var unitGO = Instantiate(UnitDatabase.GetUnitGameObject(unit), spawnPosition, Quaternion.identity);
                unitGO.GetComponent<CombatArmor>().Initialize(unit);
                unitGO.GetComponent<CombatHealth>().Initialize(unit);
            }

            
            //TODO: Figure out the best way to link characters to a gameObject. Challenge being that different equipment
            //TODO: could change what the gameObject looks like so a prefab may not be the best way to go about it? 
            /*foreach (var companion in CharacterDatabase.GetCharacters(GameManager.Instance.GetPlayerParty().CompanionsId.ToArray()))
            {
                var characterGO = Instantiate(CharacterDatabase.GetCharacterGameObject(companion));
            }

            var playerGO =
                Instantiate(CharacterDatabase.GetCharacterGameObject(GameManager.Instance.GetPlayerCharacter()));
                */
        }

        public void SetTerrainInfo(SystemBattleTerrainInfo newTerrainInfo)
        {
            terrainInfo = newTerrainInfo;
        }
    }
}