using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

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

        // Guardar el índice de la escena actual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("SavedSceneIndex", currentSceneIndex);

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

    public int GetSavedSceneIndex()
    {
        string key = "SavedSceneIndex";
        if (PlayerPrefs.HasKey(key))
        {
            int sceneIndex = PlayerPrefs.GetInt(key);
            Debug.Log($"SaveLoadSystem - Saved Scene Index: {sceneIndex}");
            return sceneIndex;
        }
        else
        {
            Debug.LogWarning("SaveLoadSystem - No saved scene index found, returning default scene index (1).");
            return 1; // Valor por defecto si no hay datos guardados
        }
    }

    public Vector3 GetSavedPosition()
    {
        string key = typeof(CharacterController).FullName;
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            var saveData = JsonUtility.FromJson<CharacterController.CharacterControllerSaveData>(json);
            return new Vector3(saveData.lastPositionX, saveData.lastPositionY, saveData.lastPositionZ);
        }
        else
        {
            return new Vector3(0, 1, 0); // Valor por defecto si no hay datos guardados
        }
    }

}