using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string fileExtension = "";
    private bool useEncryption = false;

    private readonly byte[] encryptionKey = Encoding.UTF8.GetBytes("16ByteKeyForAES!");
    private readonly byte[] encryptionIV = Encoding.UTF8.GetBytes("116ByteIVForAES!");


    public FileDataHandler(string dirPath, string fileExtension, bool useEncryption)
    {
        dataDirPath = dirPath;
        this.fileExtension = fileExtension;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string saveGameName)
    {

        if (saveGameName == null)
        {
            return null;
        }
        
        string fullPath = Path.Combine(dataDirPath, saveGameName + fileExtension);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (useEncryption)
                {
                    dataToLoad = Decrypt(dataToLoad);
                }

                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad, settings);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred when trying to load data from file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData data, string saveGameName)
    {
        if (saveGameName == null)
        {
            return;
        }
        
        string fullPath = Path.Combine(dataDirPath, saveGameName + fileExtension);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            
            Newtonsoft.Json.Serialization.IContractResolver _resolver = new FixedUnityTypeContractResolver ();

            var settings = new JsonSerializerSettings 
            {
                ContractResolver = _resolver,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            string dataToStore = JsonConvert.SerializeObject(data, settings);

            if (useEncryption)
            {
                dataToStore = Encrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
    
    public List<string> LoadAllSaveGames()
    {
         
        var saveGameNames = new List<string>();

        string[] fileNames = Directory.GetFiles(dataDirPath);
        Debug.Log("fileNames: " + fileNames.Length);
        foreach (var fileName in fileNames)
        {
            var saveGameName = Path.GetFileNameWithoutExtension(fileName);

            var fullPath = Path.Combine(dataDirPath, saveGameName + fileExtension);
            if (!File.Exists(fullPath))
            {
                Debug.Log("File doesnt exists: " + fullPath);
                continue;
            }
            saveGameNames.Add(saveGameName);
        }
        
        saveGameNames.Sort((a, b) =>
        {
            var fullPathA = Path.Combine(dataDirPath, a + fileExtension);
            var fullPathB = Path.Combine(dataDirPath, b + fileExtension);

            var lastAccessTimeA = File.GetLastWriteTime(fullPathA);
            var lastAccessTimeB = File.GetLastWriteTime(fullPathB);
            
            return DateTime.Compare(lastAccessTimeB, lastAccessTimeA);
        });

        return saveGameNames;
    }

    public DateTime GetLastWriteTime(string saveGameName)
    {
        var fullPath = Path.Combine(dataDirPath, saveGameName + fileExtension);

        return File.GetLastWriteTime(fullPath);
    }
    

    public string GetMostRecentlyUpdatedSaveGame()
    {
        string mostRecentSaveGame = null;
        List<string> saveGames = LoadAllSaveGames();

        foreach (var saveGame in saveGames)
        {
            var path = Path.Combine(dataDirPath, saveGame + fileExtension);
            var fi = new FileInfo(path);
            if (!fi.Exists)
            {
                Debug.LogWarning("File doesnt exists but its name is found?");
                continue;
            }

            if (mostRecentSaveGame == null)
            {
                mostRecentSaveGame = saveGame;
            }
            else
            {
                var mostRecentDateTime = new FileInfo(Path.Combine(dataDirPath, mostRecentSaveGame + fileExtension))
                    .LastWriteTime;
                var newDateTime = fi.LastWriteTime;
                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentSaveGame = saveGame;
                }
            }
        }

        return mostRecentSaveGame;
    }

    private string Encrypt(string data)
    {
        using (AesManaged aes = new AesManaged())
        {
            aes.Key = encryptionKey;
            aes.IV = encryptionIV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] encryptedBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(data);
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                }

                encryptedBytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(encryptedBytes);
        }
    }

    private string Decrypt(string data)
    {
        using (AesManaged aes = new AesManaged())
        {
            aes.Key = encryptionKey;
            aes.IV = encryptionIV;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] encryptedBytes = Convert.FromBase64String(data);
            byte[] decryptedBytes;
            using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cryptoStream))
                    {
                        string decryptedText = reader.ReadToEnd();
                        decryptedBytes = Encoding.UTF8.GetBytes(decryptedText);
                    }
                }
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}