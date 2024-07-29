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
                Debug.Log("ClientSpawner(20) if(SceneManager.GetActiveScene().buildIndex != 0) = TRUE; FIRST BLOCK CALLED. ");
                StartCoroutine(SpawnWithDelay());
            }
            else
            {
                Debug.Log("ClientSpawner(25) if(SceneManager.GetActiveScene().buildIndex != 0) = FALSE; SECOND BLOCK CALLED. ");
                clientRef = _player;
                SceneManager.sceneLoaded += SpawnClient;

                // Llama al método de carga del juego
                StartCoroutine(LoadGameAndSetScene());

                //Runner.SetActiveScene(GameManager.GetScene(1));
            }
        }
    }

    private IEnumerator LoadGameAndSetScene()
    {
        yield return new WaitForSeconds(0.1f); // Espera un segundo para asegurarte de que todo se ha inicializado

        SaveLoadSystem.Instance.LoadGame();
        
        // Aquí se decide la escena a cargar
        int savedSceneIndex = SaveLoadSystem.Instance.GetSavedSceneIndex();
        if (savedSceneIndex != -1)
        {
            Debug.Log($"Loading saved scene index: {savedSceneIndex}");
            Runner.SetActiveScene(GameManager.GetScene(savedSceneIndex));
        }
        else
        {
            Debug.Log("No saved scene found, loading default scene 1");
            Runner.SetActiveScene(GameManager.GetScene(1));
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
        // Obtener la posición guardada
        Vector3 savedPosition = SaveLoadSystem.Instance.GetSavedPosition();
        Runner.Spawn(clientPrefab, savedPosition, Quaternion.identity, clientRef, InitializeClientBeforeSpawn);
    }

    private void InitializeClientBeforeSpawn(NetworkRunner runner, NetworkObject obj)
    {
        
    }

    public void ClientSpawnerVerification()
    {
        Debug.Log($"ClientSpawner Verification: \n" +
                  //$"_player: {_player}\n" +
                  $"Runner.LocalPlayer: {Runner.LocalPlayer}\n" +
                  $"SceneManager.GetActiveScene().buildIndex: {SceneManager.GetActiveScene().buildIndex}");
        Debug.Log("ClientSpawner Verification: ✔️ Success");
    }
}