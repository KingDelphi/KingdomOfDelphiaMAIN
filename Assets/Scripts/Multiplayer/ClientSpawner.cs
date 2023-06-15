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

            if (SceneManager.GetActiveScene().name != "0. CharacterLoader")
            {
                StartCoroutine(SpawnWithDelay());
            }
            else
            {
                Debug.Log("Is local");
                clientRef = _player;
                SceneManager.sceneLoaded += SpawnClient;
                Runner.SetActiveScene("1. (0,0,0) La Taberna del Tio Juancho");
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
        Runner.Spawn(clientPrefab, new Vector3(0, 1, 0), Quaternion.identity, clientRef);
    }
}