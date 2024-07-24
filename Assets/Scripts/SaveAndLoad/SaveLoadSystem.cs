using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem Instance { get; private set; }

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

    public void SaveGame()
    {
        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        foreach (var saveable in saveables)
        {
            string key = saveable.GetType().FullName;
            string json = JsonUtility.ToJson(saveable.SaveState());
            PlayerPrefs.SetString(key, json);
        }
        PlayerPrefs.Save();
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        foreach (var saveable in saveables)
        {
            string key = saveable.GetType().FullName;
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                object state = JsonUtility.FromJson(json, saveable.SaveType);
                saveable.LoadState(state);
            }
        }
        Debug.Log("Game Loaded");
    }
}