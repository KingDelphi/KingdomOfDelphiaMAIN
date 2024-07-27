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

                // Imprime el nombre de la escena en el debug
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;                
                Scene currentScene = SceneManager.GetSceneByBuildIndex(currentSceneIndex);
                string currentSceneName = currentScene.name;
                Debug.Log($"ClientSpawner(26) Current Scene Name: {currentSceneName}");

                StartCoroutine(SpawnWithDelay());
            }
            else
            {
                Debug.Log("ClientSpawner(25) if(SceneManager.GetActiveScene().buildIndex != 0) = FALSE; SECOND BLOCK CALLED. ");
                clientRef = _player;
                SceneManager.sceneLoaded += SpawnClient;
                
                // Imprime el nombre de la escena en el debug
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                Scene currentScene = SceneManager.GetSceneByBuildIndex(currentSceneIndex);
                string currentSceneName = currentScene.name;
                Debug.Log($"ClientSpawner(40) Current Scene Name: {currentSceneName}");

                Runner.SetActiveScene(GameManager.GetScene(1));
            }
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
        yield return new WaitForSeconds(0.5f);
        Runner.Spawn(clientPrefab, new Vector3(0, 1, 0), Quaternion.identity, clientRef, InitializeClientBeforeSpawn);
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