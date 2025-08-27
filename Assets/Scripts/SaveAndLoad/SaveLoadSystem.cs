using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Fusion;
using System.Collections;
using System.Text.RegularExpressions;

public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem Instance { get; private set; }
    public Player currentPlayer;
    public string currentPlayerName;
    private List<ISaveable> saveables = new List<ISaveable>();
    private bool initialLoadComplete = false;
    private const string BetaTesterName = "BetaTester";
    private const string BetaTesterServer = "Testia";
    public string serverName;

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

    // Nueva función para limpiar el nombre del servidor
    public static string CleanServerName(string rawName)
    {
        // Esto elimina cualquier texto entre paréntesis, incluyendo espacios
        return Regex.Replace(rawName, @"\s*\(.*?\)\s*", "").Trim();
    }

    public void SetServer(string server)
    {
        serverName = CleanServerName(server);
    }

    public void SetCurrentPlayer(Player player)
    {
        currentPlayer = player;
    }

    public void SetCurrentPlayerName(string playerName)
    {
        currentPlayerName = playerName;
    }

    public void RegisterSaveable(ISaveable saveable)
    {
        saveables.Add(saveable);
        if (initialLoadComplete)
        {
            LoadStateForSaveable(saveable);
        }
    }

    public void SaveGame()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("No Player with State Authority found.");
            return;
        }

        string playerName = currentPlayer.playerName;
        // Limpiar serverName antes de usarlo
        string cleanServerName = CleanServerName(serverName);

        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        foreach (var saveable in saveables)
        {
            string key = saveable.GetType().FullName + "_" + cleanServerName + "_" + playerName;
            string json = JsonUtility.ToJson(saveable.SaveState());
            PlayerPrefs.SetString(key, json);
            Debug.Log($"Saving data for key: {key}, JSON: {json}");
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetInt("SavedSceneIndex_" + currentPlayer.playerName, currentSceneIndex);
        PlayerPrefs.Save();

        Debug.LogWarning($"✔️✔️✔️✔️✔️ Game Saved for Player: {playerName}, Character: {currentPlayer.name}, Server: {cleanServerName}, Map: {currentSceneName}");
    }

    public void LoadGame()
    {
        if (currentPlayerName == null)
        {
            Debug.LogWarning("No Player with State Authority found.");
            return;
        }

        string testPlayerName = GameManager.Instance.GetPlayerName();
        Debug.Log($"FIDEO2 - LoadGameB: GameManager returned playerName = {testPlayerName}");

        string playerName = currentPlayerName;
        string rawServerName = GameManager.Instance.GetServerName();
        serverName = CleanServerName(rawServerName);

        Debug.Log($"FIDEO3 - LoadGameC: Server name cleaned to: {serverName}");

        InvokeRepeating("InstantiateAllSaveables", 0f, 1f);
    }

    private void InstantiateAllSaveables()
    {
        Debug.Log("InstantiateAllSaveables se está ejecutando.");

        if (saveables.Count == 6)
        {
            initialLoadComplete = true;
            ApplySavedState();
            CancelInvoke("InstantiateAllSaveables");

            if (LoadingScreenManager.Instance != null)
            {
                LoadingScreenManager.Instance.HideLoadingScreen();
            }

            StartCoroutine(ShowLoadingScreenForOneSecond());
        }
    }

    private IEnumerator ShowLoadingScreenForOneSecond()
    {
        LoadingScreenManager.Instance.ShowLoadingScreen();
        yield return new WaitForSeconds(1f);
        LoadingScreenManager.Instance.HideLoadingScreen();
    }

    private void ApplySavedState()
    {
        foreach (var saveable in saveables)
        {
            LoadStateForSaveable(saveable);
        }

        Debug.LogWarning($"✔️✔️✔️✔️✔️ Game Loaded for Player: {currentPlayerName}, Server: {serverName}");
    }

    private void LoadStateForSaveable(ISaveable saveable)
    {
        if (saveable == null)
        {
            Debug.LogWarning("⚠️ Saltando LoadStateForSaveable porque saveable es NULL.");
            return;
        }

        string cleanServerName = CleanServerName(serverName);

        string key = saveable.GetType().FullName + "_" + cleanServerName + "_" + currentPlayerName;
        string betaTesterKey = saveable.GetType().FullName + "_" + cleanServerName + "_" + BetaTesterName;

        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            Debug.Log($"Loading data for key: {key}, JSON: {json}");

            object state = JsonUtility.FromJson(json, saveable.SaveType);
            saveable.LoadState(state);
        }
        else if (PlayerPrefs.HasKey(betaTesterKey))
        {
            string json = PlayerPrefs.GetString(betaTesterKey);
            Debug.Log($"Loading default data from BetaTester for key: {betaTesterKey}, JSON: {json}");

            object state = JsonUtility.FromJson(json, saveable.SaveType);
            saveable.LoadState(state);

            if (saveable is Player player)
            {
                player.playerName = currentPlayerName;
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ No data found for key: {key}. Se usará el estado por defecto.");
        }
    }

    public int GetSavedSceneIndex(string playerName)
    {
        string key = "SavedSceneIndex_" + playerName;
        string betaTesterKey = "SavedSceneIndex_" + BetaTesterName;

        if (PlayerPrefs.HasKey(key))
        {
            int sceneIndex = PlayerPrefs.GetInt(key);
            return sceneIndex == 0 ? 1 : sceneIndex;
        }
        else if (PlayerPrefs.HasKey(betaTesterKey))
        {
            int sceneIndex = PlayerPrefs.GetInt(betaTesterKey);
            return sceneIndex == 0 ? 1 : sceneIndex;
        }
        else
        {
            return 1;
        }
    }

    public Vector3 GetSavedPosition()
    {
        string key = typeof(CharacterController).FullName;
        string betaTesterKey = typeof(CharacterController).FullName + "_" + BetaTesterName;

        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            var saveData = JsonUtility.FromJson<CharacterController.CharacterControllerSaveData>(json);
            return new Vector3(saveData.lastPositionX, saveData.lastPositionY, saveData.lastPositionZ);
        }
        else if (PlayerPrefs.HasKey(betaTesterKey))
        {
            string json = PlayerPrefs.GetString(betaTesterKey);
            var saveData = JsonUtility.FromJson<CharacterController.CharacterControllerSaveData>(json);
            return new Vector3(saveData.lastPositionX, saveData.lastPositionY, saveData.lastPositionZ);
        }
        else
        {
            return new Vector3(0, 1, 0);
        }
    }
}
