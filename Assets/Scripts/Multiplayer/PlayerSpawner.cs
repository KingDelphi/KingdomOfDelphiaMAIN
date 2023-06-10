using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    private PlayerRef player;

    public void PlayerJoined(PlayerRef _player)
    {
        Debug.Log("PlayerJoined called." + _player);
        if (_player == Runner.LocalPlayer)
        {
            Debug.Log("Is local");
            player = _player;
            SceneManager.sceneLoaded += SpawnPlayer;
            Runner.SetActiveScene("1. (0,0,0) La Taberna del Tio Juancho");
        }
    }

    public void SpawnPlayer(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SpawnWithDelay());
        SceneManager.sceneLoaded -= SpawnPlayer;
    }

    IEnumerator SpawnWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
    }
}