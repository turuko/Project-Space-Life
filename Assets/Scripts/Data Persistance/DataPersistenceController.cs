using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utility;

public class DataPersistenceController : MonoBehaviour
{

    [FormerlySerializedAs("fileName")]
    [Header("File Storage Config")] 
    [SerializeField] private string fileExtension;

    [SerializeField] private bool useEncryption;

    private FileDataHandler dataHandler;
    public static DataPersistenceController Instance { get; private set; }

    private GameData data;
    private List<IDataPersistence> dataPersistenceObjects;

    private string selectedSaveGameName = "test";
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one DataPersistenceController");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dataHandler = new FileDataHandler(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Devulai Studio", "Project Space Life", "Saves"), fileExtension, useEncryption);

        selectedSaveGameName = dataHandler.GetMostRecentlyUpdatedSaveGame();
        OnSceneLoaded();
    }

    public void OnSceneLoaded()
    {
        dataPersistenceObjects = FindDataPersistenceObjects();
        Debug.Log("DataPersistenceController::OnLoaded");
        foreach (var dataPersistenceObject in dataPersistenceObjects)
        {
            Debug.Log(dataPersistenceObject);
        }
    }

    public void ChangeSelectedSaveGame(string newSaveGame)
    {
        selectedSaveGameName = newSaveGame;
    }

    public void NewGame()
    {
        data = new GameData();
    }

    public void LoadGame()
    {
        data = dataHandler.Load(selectedSaveGameName);
        if (data == null)
        {
            return;
        }

        //Debug.Log("DataPersistenceController::LoadData DataPersistenceObjects == null: " + (dataPersistenceObjects == null));
        
        foreach (var dataPersistenceObject in dataPersistenceObjects)
        {
            //Debug.Log("DataPersistenceObject: " + dataPersistenceObject + " == null: " + (dataPersistenceObjects == null));
            dataPersistenceObject.LoadData(data);
        }
    }

    public void SaveGame()
    {
        if (data == null)
        {
            return;
        }
        
        foreach (var dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(data);
        }

        dataHandler.Save(data, selectedSaveGameName);
    }

    private List<IDataPersistence> FindDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistencesSO =
            FindObjectsOfType<ScriptableObject>().OfType<IDataPersistence>();
        
        IEnumerable<IDataPersistence> dataPersistencesMONO =
            FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return InstanceTracker<IDataPersistence>.GetAllInstances();
    }

    public bool HasGameData()
    {
        return GetAllSaveGameNames().Count > 0;
    }

    public List<string> GetAllSaveGameNames()
    {
        return dataHandler.LoadAllSaveGames();
    }

    public DateTime GetLastWriteTime(string saveGameName)
    {
        return dataHandler.GetLastWriteTime(saveGameName);
    }
}