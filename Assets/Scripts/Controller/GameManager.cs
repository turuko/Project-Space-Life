using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Battle.SystemBattle;
using Controller.Menus;
using Model;
using Model.Databases;
using Model.Factions;
using Model.Items;
using Model.Stat_System;
using Model.Universe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utility;
using Visual_Scripts;

namespace Controller
{
    public class GameManager : MonoBehaviour, IDataPersistence
    {
        
        //--------------- Player Data --------------
        private Character PlayerCharacter;
        private Party PlayerParty;

        private Vector3? playerGalaxyPos;
        private Vector3? playerSystemPos;
        //------- Campaign Player Controller -------
        private PlayerController PlayerController;
        //------- Battle Player prefab -------------
        
        public static GameManager Instance;

        private GamesMenu gamesMenu;
        public bool gameMenuIsOpen = false;

        private Galaxy galaxy;
        [SerializeField] private int minStars, maxStars, seed;
        private GalaxySize galaxySize;
        
        private string SystemInFocusId;
        
        private InfoMenuType MenuType;

        private BottomBar bottomBar = null;

        private StandardRotationController RotationController;

        private CameraController cameraController;

        public Item DEBUG_WEAPON;
        
        public enum GameState
        {
            MainMenu,
            InfoMenu,
            Galaxy,
            System,
            InfoPanelOpen
        }

        private bool gameLoaded = false;
        private bool galaxyVisualsInitialized = false;
        private bool starSystemInitialized = false;
        private bool battleSceneInitialized = false;

        private GameState gameState;
        private GameState previousGameState;

        private AsyncOperation asyncOp;
        private Action onCompleted;

        //0 = paused, 1 = normal speed, 2 = 2x speed, 3 = 3x speed.
        private int gameSpeed = 1;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Two GameManagers");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            InstanceTracker<IDataPersistence>.RegisterInstance(this);
            gameState = GameState.MainMenu;
            Time.timeScale = gameSpeed;
            RotationController = new StandardRotationController();
            
            InitDatabases();
            
            playerGalaxyPos = Vector3.zero;
            playerSystemPos = new Vector3(0,0,-25);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            InstanceTracker<IDataPersistence>.UnregisterInstance(this);
        }

        public async Task GalaxyInit()
        {
            galaxy = new Galaxy();
            GalaxySettings.minStars = minStars;
            GalaxySettings.maxStars = maxStars;
            GalaxySettings.seed = seed;
            await galaxy.GenerateGalaxy();
        }

        public void SetGalaxySize(GalaxySize size)
        {
            galaxySize = size;
            switch (size)
            {
                case GalaxySize.Tiny:
                    Debug.Log("Tiny");
                    SetMinStars(45);
                    SetMaxStars(90);
                    break;
                case GalaxySize.Small:
                    Debug.Log("Small");
                    SetMinStars(70);
                    SetMaxStars(110);
                    break;
                case GalaxySize.Medium:
                    Debug.Log("Medium");
                    SetMinStars(130);
                    SetMaxStars(180);
                    break;
                case GalaxySize.Large:
                    Debug.Log("Large");
                    SetMinStars(180);
                    SetMaxStars(250);
                    break;
                case GalaxySize.Huge:
                    Debug.Log("Huge");
                    SetMinStars(250);
                    SetMaxStars(300);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetMinStars(int minStars)
        {
            this.minStars = minStars;
        }
    
        private void SetMaxStars(int maxStars)
        {
            this.maxStars = maxStars;
        }
    
        public void SetSeed(string seed)
        {
            this.seed = seed.ConvertToInt();
        }

        public Galaxy GetGalaxy()
        {
            return galaxy;
        }

        public void Update()
        {
            if (asyncOp != null)
            {
                StartCoroutine(WaitForAsyncOperation(asyncOp, onCompleted));
            }
            else
            {
                switch (gameState)
                {
                    case GameState.Galaxy:
                        GalaxyUpdate();
                        break;
                    case GameState.System:
                        SystemUpdate();
                        break;
                    case GameState.InfoPanelOpen:
                        InfoPanelUpdate();
                        break;
                    case GameState.InfoMenu:
                        InfoMenuUpdate();
                        break;
                    default:
                        break;
                }
            }
        }

        private void GalaxyUpdate()
        {
            RotationController.UpdateTimeSinceRotation(Time.deltaTime);
            bottomBar.UpdateRotationProgress(RotationController);
        }
        private void SystemUpdate()
        {
            RotationController.UpdateTimeSinceRotation(Time.deltaTime);
            bottomBar.UpdateRotationProgress(RotationController);

            /*if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                LoadSystemBattleScene(FindObjectOfType<SystemController>().CreateTerrainInfo());
            }*/
        }
        private void InfoPanelUpdate()
        {
            
        }
        private void InfoMenuUpdate()
        {
            //Debug.Log("I am running");
        }

        #region Input Actions
        public void OnFreeCamera(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            cameraController.OnFreeCamera(context);
        }

        public void OnPanCameraUp(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            cameraController.OnPanCameraUp(context);
        }
        
        public void OnPanCameraDown(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            cameraController.OnPanCameraDown(context);
        }
        
        public void OnPanCameraLeft(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            cameraController.OnPanCameraLeft(context);
        }
        
        public void OnPanCameraRight(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            cameraController.OnPanCameraRight(context);
        }
        
        public void OnCameraZoomIn(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            cameraController.OnCameraZoomIn(context);
        }
        
        public void OnCameraZoomOut(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            cameraController.OnCameraZoomOut(context);
        }
        
        public void OnRotateCameraLeft(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            cameraController.OnRotateCameraLeft(context);
        }

        public void OnRotateCameraRight(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            cameraController.OnRotateCameraRight(context);
        }
        
        public void OnIncreaseGameSpeed(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            if (context.phase != InputActionPhase.Started)
            {
                return;
            }
            
            if (gameSpeed >= 3)
            {
                Debug.Log("Game at max speed");
                return;
            }

            gameSpeed ++;
            Time.timeScale = gameSpeed;
            bottomBar.UpdateGameSpeedIndicator(gameSpeed);
        }
        
        public void OnDecreaseGameSpeed(InputAction.CallbackContext context)
        {
            if (gameMenuIsOpen)
                return;
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            if (context.phase != InputActionPhase.Started)
            {
                return;
            }
            
            if (gameSpeed <= 0)
            {
                Debug.Log("Game at minimum speed");
                return;
            }

            gameSpeed --;
            Time.timeScale = gameSpeed;
            bottomBar.UpdateGameSpeedIndicator(gameSpeed);
        }
        
        public void OnEscape(InputAction.CallbackContext context)
        {
            if (gameState is GameState.MainMenu or GameState.InfoMenu)
            {
                return;
            }
            if (context.phase != InputActionPhase.Started)
            {
                return;
            }
            
            if (gameState is not (GameState.Galaxy or GameState.System)) return;
            if (!gameMenuIsOpen)
            {
                gamesMenu.Open();
                gameMenuIsOpen = true;
            }
            else
            {
                gamesMenu.CloseMenuPage();
            }
        }

        #endregion
        public void LoadCreationScene()
        {
            var combatBaseStats = new Dictionary<string, CharacterStat>()
            {
                { "Rifle", new CharacterStat(10f) }, { "Pistol", new CharacterStat(10f) },
                { "One Hand", new CharacterStat(10f) }, { "Two Hand", new CharacterStat(10f) },
                { "Heavy Gun", new CharacterStat(10f) },
                { "Pilot", new CharacterStat(10f) }, { "Ship Gun", new CharacterStat(10f) }
            };

            var charBaseStats = new Dictionary<string, CharacterStat>()
            {
                { "Leadership", new CharacterStat(5f) }, { "Trade", new CharacterStat(2f) },
                { "Ground Tactics", new CharacterStat(1f) }, { "Space Tactics", new CharacterStat(1f) },
                { "Health", new CharacterStat(5f) },
                { "Psychic Power", new CharacterStat(5f) }
            };

            PlayerCharacter = Character.CreateCharacter(combatBaseStats, charBaseStats, 5, 10, .4f, 60f, 1);
            PlayerCharacter.Equipment.EquipItem(DEBUG_WEAPON);

            PlayerParty = new Party(PlayerCharacter.Id);

            PartyDatabase.AddParty(PlayerParty);
            
            asyncOp = SceneManager.LoadSceneAsync("Character Creation Scene");

            onCompleted = () =>
            {
                previousGameState = gameState;
                gameState = GameState.InfoMenu;
                galaxyVisualsInitialized = false;

                DataPersistenceController.Instance.OnSceneLoaded();
                asyncOp = null;
            };
        }

        public void LoadGalaxyScene()
        {
            asyncOp = SceneManager.LoadSceneAsync("Galaxy Scene");

            starSystemInitialized = false;
            onCompleted = () =>
            {
                Debug.Log("LoadGalaxyScene::OnCompleted");

                var bottomBars = Resources.FindObjectsOfTypeAll<BottomBar>();

                if (bottomBars.Length > 1)
                {
                    Debug.Log("More than one bottomBar");
                    return;
                }

                if (bottomBars.Length < 1)
                {
                    Debug.Log("Less than one bottomBar");
                    return;
                }

                bottomBar = bottomBars[0];

                if (PlayerController == null)
                {
                    var pc = FindObjectOfType<PlayerController>();
                    PlayerController = pc;
                }

                if (gamesMenu == null)
                {
                    var gameMenus = Resources.FindObjectsOfTypeAll<GamesMenu>();
                    
                    if (gameMenus.Length > 1)
                    {
                        Debug.Log("More than one bottomBar");
                        return;
                    }

                    if (gameMenus.Length < 1)
                    {
                        Debug.Log("Less than one bottomBar");
                        return;
                    }

                    gamesMenu = gameMenus[0];
                    
                    Debug.Log("gamesMenu = null: " + (gamesMenu == null));
                }

                
                PlayerController.transform.position = playerGalaxyPos.Value;
                PlayerController.targetPos = playerGalaxyPos.Value;
                cameraController = FindObjectOfType<CameraController>();
                cameraController.transform.position = playerGalaxyPos.Value;
                cameraController.newPosition = playerGalaxyPos.Value;

                playerGalaxyPos ??= PlayerController.transform.position;
                
                previousGameState = gameState;

                if (previousGameState == GameState.System && SystemInFocusId != null)
                {
                    SystemInFocusId = null;
                }
                
                gameState = GameState.Galaxy;

                DataPersistenceController.Instance.OnSceneLoaded();
                
                if (gameLoaded)
                {
                    DataPersistenceController.Instance.LoadGame();
                    Debug.Log("GameLoaded");
                    gameLoaded = false;
                }
                
                if (!galaxyVisualsInitialized)
                {
                    Debug.Log("GalaxyVisuals Initializing");
                    UniverseController.Instance.InitiateVisuals();
                    galaxyVisualsInitialized = true;
                }

                Debug.Log("LoadGalaxyScene::OnCompleted is past gameLoaded");
                
                bottomBar.Init();
                bottomBar.UpdateGameSpeedIndicator(gameSpeed);
                bottomBar.UpdatePlayerHealth(PlayerCharacter);
                bottomBar.UpdatePlayerPartyValues(PlayerParty);
                bottomBar.UpdateRotationIndicator(RotationController);
                
                Debug.Log("Setting asyncOp to null");
                asyncOp = null;
            };
        }
        
        public void LoadMainMenuScene()
        {
            asyncOp = SceneManager.LoadSceneAsync("Main Menu Scene");
            
            previousGameState = gameState;
            starSystemInitialized = false;
            galaxyVisualsInitialized = false;

            onCompleted = () =>
            {
                gameState = GameState.MainMenu;
                galaxyVisualsInitialized = false;
                DataPersistenceController.Instance.OnSceneLoaded();

                asyncOp = null;
            };
        }

        public void LoadSystemScene(string systemId)
        {
            asyncOp = SceneManager.LoadSceneAsync("System Scene");

            Debug.Log("System in focus id: " + SystemInFocusId);
            if (!gameLoaded)
            {
                SystemInFocusId = systemId;
            }
            Time.timeScale = 1;
            
            previousGameState = gameState;
            galaxyVisualsInitialized = false;

            if (previousGameState == GameState.Galaxy) playerGalaxyPos = PlayerController.transform.position;

            onCompleted = () =>
            {
                Debug.Log("onComplete LoadSystem");
                if (bottomBar == null)
                {
                    var bottomBars = Resources.FindObjectsOfTypeAll<BottomBar>();

                    if (bottomBars.Length > 1)
                    {
                        return;
                    }

                    if (bottomBars.Length < 1)
                    {
                        return;
                    }

                    bottomBar = bottomBars[0];
                    bottomBar.Init();
                    bottomBar.UpdateGameSpeedIndicator(gameSpeed);
                    bottomBar.UpdatePlayerHealth(PlayerCharacter);
                    bottomBar.UpdatePlayerPartyValues(PlayerParty);
                    bottomBar.UpdateRotationIndicator(RotationController);
                }
                
                if (gamesMenu == null)
                {
                    var gameMenus = Resources.FindObjectsOfTypeAll<GamesMenu>();
                    
                    if (gameMenus.Length > 1)
                    {
                        Debug.Log("More than one bottomBar");
                        return;
                    }

                    if (gameMenus.Length < 1)
                    {
                        Debug.Log("Less than one bottomBar");
                        return;
                    }

                    gamesMenu = gameMenus[0];
                    
                    Debug.Log("gamesMenu = null: " + (gamesMenu == null));
                }

                if (!starSystemInitialized)
                {
                    FindObjectOfType<SystemController>().Init();
                    starSystemInitialized = true;
                }

                if (PlayerController == null)
                {
                    PlayerController = FindObjectOfType<PlayerController>();
                }

                PlayerController.transform.position = playerSystemPos.Value;
                PlayerController.targetPos = playerSystemPos.Value;
                
                if(playerSystemPos != null)
                {
                    cameraController = FindObjectOfType<CameraController>();
                    cameraController.transform.position = playerSystemPos.Value;
                    cameraController.newPosition = playerSystemPos.Value;
                }
                

                playerSystemPos ??= PlayerController.transform.position;

                gameState = GameState.System;

                if (playerSystemPos != null && previousGameState != GameState.Galaxy)
                {
                    playerSystemPos = PlayerController.transform.position;
                }
                
                
                
                DataPersistenceController.Instance.OnSceneLoaded();
                
                asyncOp = null;
            };
        }

        public void LoadSystemBattleScene(SystemBattleTerrainInfo terrainInfo)
        {
            asyncOp = SceneManager.LoadSceneAsync("System Battle Scene");
            
            GetComponent<PlayerInput>().enabled = false;

            onCompleted = () =>
            {
                FindObjectOfType<SystemBattleController>().SetTerrainInfo(terrainInfo);
                if (!battleSceneInitialized)
                {
                    FindObjectOfType<SystemBattleController>().Init();
                    battleSceneInitialized = true;
                }
                
            };
        }

        public void LoadInfoMenu(InfoMenuType menuType)
        {
            asyncOp = SceneManager.LoadSceneAsync("Info Menus Scene");

            previousGameState = gameState;

            starSystemInitialized = false;
            galaxyVisualsInitialized = false;
            
            switch (previousGameState)
            {
                case GameState.Galaxy:
                    playerGalaxyPos = PlayerController.transform.position;
                    break;
                case GameState.System:
                    playerSystemPos = PlayerController.transform.position;
                    break;
            }
            
            onCompleted = () =>
            {
                var uiControllers = Resources.FindObjectsOfTypeAll<UIController>();

                if (uiControllers.Length > 1)
                {
                    return;
                }

                if (uiControllers.Length < 1)
                {
                    return;
                }

                var infoMenuController = uiControllers[0] as InfoMenuController;

                MenuType = menuType;


                switch (MenuType)
                {
                    case InfoMenuType.PartyMenu:
                        var partyMenus = Resources.FindObjectsOfTypeAll<PartyMenu>();

                        if (partyMenus.Length > 1)
                            return;
                        if (partyMenus.Length < 1)
                        {
                            return;
                        }

                        var partyMenu = partyMenus[0];

                        partyMenu.GameObject().SetActive(true);
                        if (infoMenuController != null) infoMenuController.Initialize(partyMenu);
                        break;
                    case InfoMenuType.InventoryMenu:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(MenuType), MenuType, null);
                }

                gameState = GameState.InfoMenu;
                
                DataPersistenceController.Instance.OnSceneLoaded();
                
                asyncOp = null;
            };
        }

        public void OpenPlanetInfo(Planet planet)
        {
            FindObjectOfType<SystemUIController>().OpenPlanetInfo(planet);
            previousGameState = gameState;
            gameState = GameState.InfoPanelOpen;
        }

        public void ClosePlanetInfo()
        {
            FindObjectOfType<SystemUIController>().ClosePlanetInfo();
            (gameState, previousGameState) = (previousGameState, gameState);
        }

        public void OpenSettlementInfo(Settlement settlement)
        {
            FindObjectOfType<SystemUIController>().OpenSettlementInfo(settlement);
        }

        public void CloseSettlementInfo(Planet planet)
        {
            FindObjectOfType<SystemUIController>().CloseSettlementInfo(planet);
        }

        public string GetSystemInFocus()
        {
            return SystemInFocusId;
        }

        public GameState GetCurrentState()
        {
            return gameState;
        }

        public GameState GetPreviousState()
        {
            return previousGameState;
        }

        public Party GetPlayerParty()
        {
            return PlayerParty;
        }

        public Character GetPlayerCharacter()
        {
            return PlayerCharacter;
        }

        public StandardRotationController GetRotationController()
        {
            return RotationController;
        }

        public GalaxySize GetGalaxySize()
        {
            return galaxySize;
        }

        private IEnumerator WaitForAsyncOperation(AsyncOperation asyncOperation, Action onComplete)
        {
            if (asyncOperation != null && asyncOperation.isDone == false)
            {
                Debug.Log("Async operation already in progress");
                yield break;
            }
            
            yield return asyncOperation;

            onComplete();
        }

        public void SetGameLoaded()
        {
            gameLoaded = true;
            galaxyVisualsInitialized = false;
            starSystemInitialized = false;
        }

        public void LoadData(GameData data)
        {
            Debug.Log("GameManager::LoadData");
            gameState = data.gameState;
            galaxySize = data.galaxySize;
            SystemInFocusId= data.systemInFocusId;
            //player character
            PlayerCharacter = data.playerCharacter;
            PlayerParty = data.playerParty;
            
            InitDatabases();

            if (!CharacterDatabase.AddCharacter(PlayerCharacter))
            {
                CharacterDatabase.UpdateCharacter(PlayerCharacter);
            }
            PartyDatabase.AddParty(PlayerParty);
            
            galaxy = data.galaxy;
            
            foreach (var character in galaxy.characters)
            {
                CharacterDatabase.UpdateCharacter(character);
            }
            
            foreach (var faction in galaxy.factions)
            {
                FactionDatabase.UpdateFaction(faction);
            }

            foreach (var planet in galaxy.GetPlanets())
            {
                PlanetDatabase.AddPlanet(planet);
            }

            foreach (var settlement in galaxy.GetSettlements())
            {
                SettlementDatabase.AddSettlement(settlement);
            }

            foreach (var ss in galaxy.GetStarSystems())
            {
                StarSystemDatabase.AddStarSystem(ss);
            }

            string output = "System Id:\n";
            foreach (var system in StarSystemDatabase.Query(x=> true))
            {
                output += system.Id + ",\n";
            }
            Debug.Log(output);
            
            playerGalaxyPos = data.playerGalaxyPos;
            playerSystemPos = data.playerSystemPos;
        }

        private void InitDatabases()
        {
            BannerDatabase.Init();
            CharacterDatabase.Init();
            FactionDatabase.Init();
            PartyDatabase.Init();
            PlanetDatabase.Init();
            SettlementDatabase.Init();
            SpeciesDatabase.Init();
            StarSystemDatabase.Init();
            UnitDatabase.Init();
            ItemDatabase.Init();
        }

        public void SaveData(GameData data)
        {
            data.galaxy = galaxy;
            
            switch (gameState)
            {
                
                case GameState.Galaxy:
                    data.gameState = gameState;
                    break;
                case GameState.System:
                    data.gameState = gameState;
                    data.playerGalaxyPos = playerGalaxyPos.Value;
                    break;
                case GameState.MainMenu:
                case GameState.InfoMenu:
                case GameState.InfoPanelOpen:
                default:
                    data.gameState = previousGameState;
                    break;
            }

            data.galaxySize = galaxySize;

            data.playerCharacter = PlayerCharacter;
            data.playerParty = PlayerParty;
            data.systemInFocusId = SystemInFocusId;
        }
    }
}