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
        Debug.Log("PlayerJoined called." + _player);
        if (_player == Runner.LocalPlayer)
        {
            Debug.Log("Is local");
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                StartCoroutine(SpawnWithDelay());
            }
            else
            {
                clientRef = _player;
                SceneManager.sceneLoaded += SpawnClient;
                Debug.Log(GameManager.GetScene(1));
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
        yield return new WaitForSeconds(0.5f);
        Runner.Spawn(clientPrefab, new Vector3(0, 1, 0), Quaternion.identity, clientRef, InitializeClientBeforeSpawn);
    }

    private void InitializeClientBeforeSpawn(NetworkRunner runner, NetworkObject obj)
    {
        // obj.gameObject.name = "Client " + obj.Id.ToString();
    }
}