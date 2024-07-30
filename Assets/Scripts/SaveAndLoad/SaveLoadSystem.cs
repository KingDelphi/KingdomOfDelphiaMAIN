using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Fusion;

public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem Instance { get; private set; }
    private Player currentPlayer;
    private string currentPlayerName;
    private List<ISaveable> saveables = new List<ISaveable>();
    private bool initialLoadComplete = false;

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

    // Método para establecer el jugador actual
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

        // Asegúrate de que 'playerName' esté disponible en 'currentPlayer'
        string playerName = currentPlayer.playerName;
        
        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        foreach (var saveable in saveables)
        {
            string key = saveable.GetType().FullName + "_" + playerName;
            string json = JsonUtility.ToJson(saveable.SaveState());
            PlayerPrefs.SetString(key, json);
            Debug.Log($"Saving data for key: {key}, JSON: {json}");

        }

        // Guardar el índice de la escena actual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetInt("SavedSceneIndex_" + currentPlayer.playerName, currentSceneIndex);

        PlayerPrefs.Save();

        Debug.LogWarning($"✔️✔️✔️✔️✔️ Game Saved for Player: {playerName}, Character: {currentPlayer.name}, Map: {currentSceneName}");
    }

    public void LoadGame()
    {
        if (currentPlayerName == null)
        {
            Debug.LogWarning("No Player with State Authority found.");
            return;
        }

        // Asegúrate de que 'playerName' esté disponible en 'currentPlayer'
        string playerName = currentPlayerName;

        // Primera fase de carga: Instanciar los objetos ISaveable
        InvokeRepeating("InstantiateAllSaveables", 0f, 1f); // Llamar cada 1 segundo
    }

    private void InstantiateAllSaveables()
    {
        // Verificar si ya hay al menos un ISaveable registrado
        if (saveables.Count == 6)
        {
            initialLoadComplete = true;
            ApplySavedState();
            CancelInvoke("InstantiateAllSaveables"); // Detener la llamada repetida

        }
        else
        {
            // Podrías agregar lógica aquí para instanciar objetos ISaveable si es necesario.
            Debug.LogWarning("No ISaveable objects registered yet. Waiting for registration.");
        }
    }

    private void ApplySavedState()
    {
        foreach (var saveable in saveables)
        {
            LoadStateForSaveable(saveable);
        }

        Debug.LogWarning($"✔️✔️✔️✔️✔️ Game Loaded for Player: {currentPlayerName}");
    }

    private void LoadStateForSaveable(ISaveable saveable)
    {
        string key = saveable.GetType().FullName + "_" + currentPlayerName;
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            Debug.Log($"Loading data for key: {key}, JSON: {json}");

            object state = JsonUtility.FromJson(json, saveable.SaveType);
            saveable.LoadState(state);
        }
        else
        {
            Debug.LogWarning($"No data found for key: {key}");
        }
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