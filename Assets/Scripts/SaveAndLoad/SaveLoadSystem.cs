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

    private const string DefaultHero = "Tres Deseos";

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

    public static string CleanServerName(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return "";
        return Regex.Replace(rawName, @"\s*\(.*?\)\s*", "").Trim();
    }

    /// <summary>
    /// "Cerebrito 12" / "La Momia Verdulera 3" / "Tres Deseos(Clone)" → nombre estable del héroe
    /// </summary>
    public static string ExtractHeroName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return DefaultHero;

        string n = objectName.Replace("(Clone)", "").Trim();
        int lastSpace = n.LastIndexOf(' ');
        if (lastSpace > 0 && lastSpace < n.Length - 1)
        {
            string tail = n.Substring(lastSpace + 1);
            bool allDigit = true;
            for (int i = 0; i < tail.Length; i++)
            {
                if (!char.IsDigit(tail[i])) { allDigit = false; break; }
            }
            if (allDigit) return n.Substring(0, lastSpace);
        }
        return n;
    }

    public static string GetHeroNameFromObject(GameObject go)
    {
        if (go == null) return DefaultHero;

        var prefabInfo = go.GetComponent<PrefabInfo>();
        if (prefabInfo != null && !string.IsNullOrEmpty(prefabInfo.prefabName))
            return prefabInfo.prefabName;

        return ExtractHeroName(go.name);
    }

    private string BuildSaveKey(string typeFullName, string cleanServer, string accountName, string heroName)
    {
        return typeFullName + "_" + cleanServer + "_" + accountName + "_" + heroName;
    }

    public void SetServer(string server)
    {
        serverName = CleanServerName(server);
    }

    public void SetCurrentPlayer(Player player)
    {
        currentPlayer = player;
        if (player != null && !string.IsNullOrEmpty(player.playerName))
            currentPlayerName = player.playerName;
    }

    public void SetCurrentPlayerName(string playerName)
    {
        currentPlayerName = playerName;
    }

    public void SetLastHero(string accountName, string heroName)
    {
        if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(heroName)) return;
        PlayerPrefs.SetString("LastHero_" + accountName, heroName);
        PlayerPrefs.Save();
        Debug.Log($"SaveLoadSystem: LastHero for '{accountName}' = '{heroName}'");
    }

    public string GetLastHero(string accountName)
    {
        if (string.IsNullOrEmpty(accountName)) return DefaultHero;

        string key = "LastHero_" + accountName;
        if (PlayerPrefs.HasKey(key))
        {
            string hero = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(hero)) return hero;
        }
        return DefaultHero;
    }

    public void RegisterSaveable(ISaveable saveable)
    {
        if (saveable == null) return;
        if (!saveables.Contains(saveable))
            saveables.Add(saveable);

        if (initialLoadComplete)
            LoadStateForSaveable(saveable);
    }

    public void SaveGame()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("No Player with State Authority found.");
            return;
        }

        string playerName = currentPlayer.playerName;
        if (string.IsNullOrEmpty(playerName))
            playerName = currentPlayerName;

        string cleanServerName = CleanServerName(serverName);
        string currentHero = GetHeroNameFromObject(currentPlayer.gameObject);

        SetLastHero(playerName, currentHero);

        var found = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        foreach (var saveable in found)
        {
            var mb = saveable as MonoBehaviour;
            string heroName = mb != null ? GetHeroNameFromObject(mb.gameObject) : currentHero;

            string key = BuildSaveKey(saveable.GetType().FullName, cleanServerName, playerName, heroName);
            string json = JsonUtility.ToJson(saveable.SaveState());
            PlayerPrefs.SetString(key, json);
            Debug.Log($"Saving data for key: {key}, JSON: {json}");
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Escena por cuenta + héroe (Fase A ampliada)
        PlayerPrefs.SetInt("SavedSceneIndex_" + playerName + "_" + currentHero, currentSceneIndex);
        PlayerPrefs.SetInt("SavedSceneIndex_" + playerName, currentSceneIndex); // compat
        PlayerPrefs.Save();

        Debug.LogWarning($"✔️ Game Saved | Account: {playerName} | Hero: {currentHero} | Server: {cleanServerName} | Map: {currentSceneName}");
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

        if (saveables.Count >= 1)
        {
            initialLoadComplete = true;
            ApplySavedState();
            CancelInvoke("InstantiateAllSaveables");

            if (LoadingScreenManager.Instance != null)
                LoadingScreenManager.Instance.HideLoadingScreen();

            StartCoroutine(ShowLoadingScreenForOneSecond());
        }
    }

    private IEnumerator ShowLoadingScreenForOneSecond()
    {
        if (LoadingScreenManager.Instance == null) yield break;
        LoadingScreenManager.Instance.ShowLoadingScreen();
        yield return new WaitForSeconds(1f);
        LoadingScreenManager.Instance.HideLoadingScreen();
    }

    private void ApplySavedState()
    {
        foreach (var saveable in saveables)
            LoadStateForSaveable(saveable);

        Debug.LogWarning($"✔️ Game Loaded | Account: {currentPlayerName} | Server: {serverName}");
    }

    private void LoadStateForSaveable(ISaveable saveable)
    {
        if (saveable == null)
        {
            Debug.LogWarning("⚠️ Saltando LoadStateForSaveable porque saveable es NULL.");
            return;
        }

        string cleanServerName = CleanServerName(serverName);
        string accountName = currentPlayerName;
        if (string.IsNullOrEmpty(accountName) && GameManager.Instance != null)
            accountName = GameManager.Instance.GetPlayerName();

        var mb = saveable as MonoBehaviour;
        string heroName = mb != null ? GetHeroNameFromObject(mb.gameObject) : GetLastHero(accountName);

        string key = BuildSaveKey(saveable.GetType().FullName, cleanServerName, accountName, heroName);
        string betaTesterKey = BuildSaveKey(saveable.GetType().FullName, cleanServerName, BetaTesterName, heroName);

        // Compat: key vieja SIN héroe (solo una vez, para no perder el último save global)
        string legacyKey = saveable.GetType().FullName + "_" + cleanServerName + "_" + accountName;

        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            Debug.Log($"Loading data for key: {key}, JSON: {json}");
            object state = JsonUtility.FromJson(json, saveable.SaveType);
            saveable.LoadState(state);
        }
        else if (PlayerPrefs.HasKey(legacyKey))
        {
            string json = PlayerPrefs.GetString(legacyKey);
            Debug.Log($"Loading LEGACY data for key: {legacyKey}, JSON: {json}");
            object state = JsonUtility.FromJson(json, saveable.SaveType);
            saveable.LoadState(state);
        }
        else if (PlayerPrefs.HasKey(betaTesterKey))
        {
            string json = PlayerPrefs.GetString(betaTesterKey);
            Debug.Log($"Loading default data from BetaTester for key: {betaTesterKey}");
            object state = JsonUtility.FromJson(json, saveable.SaveType);
            saveable.LoadState(state);

            if (saveable is Player player)
                player.playerName = accountName;
        }
        else
        {
            Debug.LogWarning($"⚠️ No data for key: {key}. Estado por defecto.");
        }
    }

    public int GetSavedSceneIndex(string playerName)
    {
        string hero = GetLastHero(playerName);
        string keyHero = "SavedSceneIndex_" + playerName + "_" + hero;
        string key = "SavedSceneIndex_" + playerName;
        string betaTesterKey = "SavedSceneIndex_" + BetaTesterName;

        if (PlayerPrefs.HasKey(keyHero))
        {
            int sceneIndex = PlayerPrefs.GetInt(keyHero);
            return sceneIndex == 0 ? 1 : sceneIndex;
        }
        if (PlayerPrefs.HasKey(key))
        {
            int sceneIndex = PlayerPrefs.GetInt(key);
            return sceneIndex == 0 ? 1 : sceneIndex;
        }
        if (PlayerPrefs.HasKey(betaTesterKey))
        {
            int sceneIndex = PlayerPrefs.GetInt(betaTesterKey);
            return sceneIndex == 0 ? 1 : sceneIndex;
        }
        return 1;
    }

    /// <summary>
    /// Posición guardada para cuenta + héroe actual (Fase A).
    /// </summary>
    public Vector3 GetSavedPosition(string accountName = null, string heroName = null)
    {
        if (string.IsNullOrEmpty(accountName))
        {
            accountName = currentPlayerName;
            if (string.IsNullOrEmpty(accountName) && GameManager.Instance != null)
                accountName = GameManager.Instance.GetPlayerName();
        }

        if (string.IsNullOrEmpty(heroName))
            heroName = GetLastHero(accountName);

        string cleanServerName = CleanServerName(serverName);
        if (string.IsNullOrEmpty(cleanServerName) && GameManager.Instance != null)
            cleanServerName = CleanServerName(GameManager.Instance.GetServerName());

        string typeName = typeof(CharacterController).FullName;
        string key = BuildSaveKey(typeName, cleanServerName, accountName, heroName);
        string legacyKey = typeName + "_" + cleanServerName + "_" + accountName;
        string betaKey = BuildSaveKey(typeName, cleanServerName, BetaTesterName, heroName);

        string json = null;
        if (PlayerPrefs.HasKey(key)) json = PlayerPrefs.GetString(key);
        else if (PlayerPrefs.HasKey(legacyKey)) json = PlayerPrefs.GetString(legacyKey);
        else if (PlayerPrefs.HasKey(betaKey)) json = PlayerPrefs.GetString(betaKey);

        if (json != null)
        {
            var saveData = JsonUtility.FromJson<CharacterController.CharacterControllerSaveData>(json);
            if (saveData != null)
                return new Vector3(saveData.lastPositionX, saveData.lastPositionY, saveData.lastPositionZ);
        }

        return new Vector3(0, 1, 0);
    }
}