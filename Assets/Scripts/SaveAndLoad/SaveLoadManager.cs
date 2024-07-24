using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    private class SaveData
    {
        public Dictionary<string, object> gameObjects = new Dictionary<string, object>();
    }

    public void SaveGame()
    {
        var saveData = new SaveData();
        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

        foreach (var saveable in saveables)
        {
            string key = saveable.GetType().FullName + "_" + saveable.ID;
            saveData.gameObjects[key] = saveable.SaveState();
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SaveData"))
        {
            string json = PlayerPrefs.GetString("SaveData");
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

            foreach (var saveable in saveables)
            {
                string key = saveable.GetType().FullName + "_" + saveable.ID;
                if (saveData.gameObjects.TryGetValue(key, out object state))
                {
                    saveable.LoadState(state);
                }
            }
        }
    }
}