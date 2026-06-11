using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ClientSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject clientPrefab;
    private PlayerRef clientRef;

    public void PlayerJoined(PlayerRef _player)
    {
        Debug.Log("ClientSpawner(13).PlayerJoined() called. _player: " + _player);
        if (_player == Runner.LocalPlayer)
        {
            Debug.Log("ClientSpawner(16) if(_player == Runner.LocalPlayer) = TRUE");

            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                Debug.Log("ClientSpawner(20) if(SceneManager.GetActiveScene().buildIndex != 0) = TRUE; FIRST BLOCK CALLED.");
                StartCoroutine(SpawnWithDelay());
            }
            else
            {
                Debug.Log("ClientSpawner(25) if(SceneManager.GetActiveScene().buildIndex != 0) = FALSE; SECOND BLOCK CALLED.");
                clientRef = _player;
                SceneManager.sceneLoaded += SpawnClient;

                StartCoroutine(LoadGameAndSetScene());
            }
        }
    }

    private IEnumerator LoadGameAndSetScene()
{
    yield return new WaitForSeconds(0.1f); 
    
    string playerName = GameManager.Instance.GetPlayerName();

    if (string.IsNullOrEmpty(playerName))
    {
        Debug.LogWarning("Player name not found.");
        yield break;
    }

    Debug.LogWarning($"✔️✔️✔️✔️✔️ Loading game for Player: {playerName}");
    SaveLoadSystem.Instance.SetCurrentPlayerName(playerName);

    SaveLoadSystem.Instance.LoadGame();

    int savedSceneIndex = SaveLoadSystem.Instance.GetSavedSceneIndex(playerName);
    SceneRef sceneRef = SceneRef.FromIndex(savedSceneIndex != -1 ? savedSceneIndex : 1);

    Debug.Log($"Loading scene: {sceneRef}");

    if (Runner.SceneManager != null)
    {
        var sceneParams = new NetworkLoadSceneParameters();
        
        // 💡 Cargar la nueva escena en modo aditivo
        Runner.SceneManager.LoadScene(sceneRef, sceneParams);
        
        // 🔄 Esperar hasta que la escena esté completamente cargada
        Scene newScene;
        do
        {
            yield return null;
            newScene = SceneManager.GetSceneByBuildIndex(savedSceneIndex);
        } while (!newScene.isLoaded);

        // 🔥 Cambiar la escena activa a la nueva
        SceneManager.SetActiveScene(newScene);

        // 🚀 Descargar la escena 0 después de hacer el cambio
        SceneManager.UnloadSceneAsync(0);
    }
    else
    {
        Debug.LogError("⚠️ Runner.SceneManager is null! Make sure NetworkSceneManager is assigned.");
    }
}






    public void SpawnClient(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SpawnWithDelay());
        SceneManager.sceneLoaded -= SpawnClient;
    }

    IEnumerator SpawnWithDelay()
    {
        Debug.Log("ClientSpawner(55).SpawnWithDelay() called.");
        yield return new WaitForSeconds(1.5f);

        Vector3 savedPosition = SaveLoadSystem.Instance.GetSavedPosition();
        Runner.Spawn(clientPrefab, savedPosition, Quaternion.identity, clientRef, InitializeClientBeforeSpawn);
    }

    private void InitializeClientBeforeSpawn(NetworkRunner runner, NetworkObject obj)
    {
        // Implementa la inicialización si es necesario
    }

    public void ClientSpawnerVerification()
    {
        Debug.Log($"ClientSpawner Verification: \n" +
                  $"Runner.LocalPlayer: {Runner.LocalPlayer}\n" +
                  $"SceneManager.GetActiveScene().buildIndex: {SceneManager.GetActiveScene().buildIndex}");
        Debug.Log("ClientSpawner Verification: ✔️ Success");
    }
}
