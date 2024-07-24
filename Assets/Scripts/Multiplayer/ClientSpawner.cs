// using Fusion;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Collections;
// using System;

// public class ClientSpawner : SimulationBehaviour, IPlayerJoined
// {
//     public GameObject clientPrefab;
//     private PlayerRef clientRef;
//     private bool hasSpawnedClient = false;
//     public GameObject existingPlayer;

//     public NetworkObject networkObject;
//     public static event Action<NetworkObject> OnPlayerSpawned;

//     public NetworkObject spawnerLocalPlayerNetworkObject;
//     private NetworkRunner networkRunner;

//     private void Awake()
//     {
//         networkRunner = GetComponent<NetworkRunner>();
//         if (networkRunner == null)
//         {
//             Debug.LogError("NetworkRunner component not found on this GameObject.");
//         }
//     }

//     public void PlayerJoined(PlayerRef _player)
//     {
//         Debug.Log("ClientSpawner(31).PlayerJoined() called. INFO: _player: " + _player);
        
//         if (_player == networkRunner.LocalPlayer) 
//         {
//             Debug.Log("ClientSpawner(35) if(_player == networkRunner.LocalPlayer) = TRUE, therefore: Is local.");
            
//             if (SceneManager.GetActiveScene().buildIndex != 0)
//             {                
//                 clientRef = networkRunner.LocalPlayer;
//                 Debug.Log("Assigned clientRef in first block: " + clientRef);
                
//                 StartCoroutine(SpawnWithDelay());
//             }
//             else
//             {                
//                 clientRef = _player;
//                 Debug.Log("Assigned clientRef in second block: " + clientRef);
                
//                 SceneManager.sceneLoaded += SpawnClient;
//                 Debug.Log("ClientSpawner(50) GameScene 1 Loaded: " + GameManager.GetScene(1));
//                 networkRunner.SetActiveScene(GameManager.GetScene(1));
//             }
//         }
//     }

//     public void SpawnClient(Scene scene, LoadSceneMode mode)
//     {
//         if (!hasSpawnedClient)
//         {
//             StartCoroutine(SpawnWithDelay());
//             hasSpawnedClient = true;
//         }
//         SceneManager.sceneLoaded -= SpawnClient;
//     }

//     IEnumerator SpawnWithDelay()
//     {
//         yield return new WaitForSeconds(0.5f);
        
//         GameObject existingPlayer = GameObject.Find(clientPrefab.name + "(Clone)");
//         Debug.Log("Existing player found: " + existingPlayer);

//         if (existingPlayer == null)
//         {
//             Debug.Log("No existing player found, spawning new one.");
//             Debug.Log($"Spawning new client. NetworkRunner: {networkRunner}, ClientPrefab: {clientPrefab}");
//             NetworkObject newClient = networkRunner.Spawn(clientPrefab);
//             InitializeClientBeforeSpawn(newClient);
//         }
//         else
//         {
//             Debug.Log("Existing player found, setting active.");
//             existingPlayer.SetActive(true);
//             NetworkObject existingNetworkObject = existingPlayer.GetComponent<NetworkObject>();
//             if (existingNetworkObject != null)
//             {
//                 OnPlayerSpawned?.Invoke(existingNetworkObject);
//             }
//             else
//             {
//                 Debug.LogError("Existing player's NetworkObject is null.");
//             }
//         }
//     }

//     private void InitializeClientBeforeSpawn(NetworkObject networkObject)
//     {
//         Debug.Log("InitializeClientBeforeSpawn called with NetworkObject: " + networkObject + ", clientRef: " + clientRef + " & NetworkRunner: " + networkRunner);

//         if (clientRef.IsNone)
//         {
//             Debug.LogError("clientRef is None. Cannot assign input authority.");
//             return;
//         }

//         networkObject.AssignInputAuthority(clientRef);
//         Debug.Log("Input authority assigned to NetworkObject: " + networkObject + " with clientRef: " + clientRef);
//     }
// }

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
        
    }
}