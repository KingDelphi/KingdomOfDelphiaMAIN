// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Fusion;
// using Cinemachine;

// public class Client : NetworkBehaviour
// {
//     public NetworkRunner networkRunner;

//     public Vector3 spawnPosition = new Vector3(-36, -28, 0);
//     public Transform characterHolder;
//     public Player activePlayer;
//     public GameObject playerPrefab;

//     [Networked]
//     public int activePlayerSceneIndex { get; set; }

//     public static Client Instance { get; private set; }

//     public GameManager gameManager;
//     public NetworkObject localPlayerNetworkObject;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//             Debug.Log("Client Awake: Instance assigned. GameObject name: " + gameObject.name);
//         }
//         else
//         {
//             Destroy(gameObject);
//             Debug.Log("Client Awake: Duplicate instance destroyed.");
//             return;
//         }

//         networkRunner = FindObjectOfType<NetworkRunner>();
//         if (networkRunner == null)
//         {
//             Debug.LogError("NetworkRunner not found in the scene.");
//         }
//         else
//         {
//             Debug.Log("NetworkRunner found: " + networkRunner.name);
//         }

//         if (networkRunner != null)
//         {
//             Debug.Log("Client: NetworkRunner found and assigned: " + networkRunner.gameObject.name);
//         }
//         else
//         {
//             Debug.LogError("Client: NetworkRunner not found for this client.");
//         }

//         gameManager = FindObjectOfType<GameManager>();
//         GameManager.OnPlayerSpawned += AssignLocalPlayerObject;

//         if (characterHolder == null)
//         {
//             characterHolder = GameObject.Find("CharacterHolder")?.transform;
//             if (characterHolder == null)
//             {
//                 Debug.LogError("CharacterHolder not found in scene.");
//             }
//         }

//         if (playerPrefab == null)
//         {
//             playerPrefab = GameManager.Instance?.GetCharacterInstance("Cerebrito");
//             if (playerPrefab == null)
//             {
//                 Debug.LogError("PlayerPrefab could not be assigned from GameManager.");
//             }
//         }

//         if (activePlayer == null && playerPrefab != null)
//         {
//             activePlayer = playerPrefab.GetComponent<Player>();
//             if (activePlayer == null)
//             {
//                 Debug.LogError("ActivePlayer could not be assigned from playerPrefab.");
//             }
//         }
//     }

//     private void OnDestroy()
//     {
//         GameManager.OnPlayerSpawned -= AssignLocalPlayerObject;
//     }

//     public override void Spawned()
//     {
//         Debug.Log($"Client Spawned. Debug BEFORE assigning networkRunner = Runner. NetworkRunner: {networkRunner}, HasStateAuthority: {HasStateAuthority}");
//         networkRunner = Runner; // Runner es una propiedad proporcionada por NetworkBehaviour
//         Debug.Log($"Client Spawned. Debug AFTER assigning networkRunner = Runner NetworkRunner: {networkRunner}, HasStateAuthority: {HasStateAuthority}");
//         activePlayerSceneIndex = 1;
//         DontDestroyOnLoad(gameObject);
//         transform.SetParent(GameManager.Instance?.clientHolder);
//         gameObject.name = "Client " + GetComponent<NetworkObject>()?.Id.ToString();

//         if (GameManager.Instance.clients.Contains(this))
//         {
//             GameManager.Instance.clients.Remove(this);
//         }
//         GameManager.Instance.clients.Add(this);

//         Debug.Log("Client(111).Spawned() called, gameObject.name: " + gameObject.name);
//         if (HasStateAuthority)
//         {
//             Debug.Log("Client(114) (if (HasStateAuthority)) = TRUE, therefore [SOMETHING] has state authority at this point. It should be gameObject.name: " + gameObject.name);

//             if (activePlayer != null)
//             {
//                 Debug.Log("SetActivePlayer: Despawning current active player NetworkObject.");
//                 NetworkObject activePlayerNetworkObject = activePlayer.gameObject.GetComponent<NetworkObject>();
//                 if (activePlayerNetworkObject != null)
//                 {
//                     networkRunner.Despawn(activePlayerNetworkObject);
//                 }
//                 else
//                 {
//                     Debug.LogError("SetActivePlayer: activePlayer's NetworkObject is null.");
//                 }
//             }

//             GameObject initialCharacter = GameManager.Instance.GetCharacterInstance("Cerebrito");
//             if (initialCharacter != null)
//             {
//                 initialCharacter.transform.position = spawnPosition;
//                 activePlayer = initialCharacter.GetComponent<Player>();
//                 SetActivePlayer(activePlayer);
//             }
//             else
//             {
//                 Debug.LogError("Initial character 'Cerebrito' could not be found.");
//                 NetworkObject networkObject = networkRunner.Spawn(
//                     playerPrefab,
//                     new Vector3(0, 1, 0),
//                     Quaternion.identity,
//                     networkRunner.LocalPlayer,
//                     InitializeCharacterBeforeSpawn 
//                 );

//                 Debug.Log("Client.Spawned: New networkObject spawned: " + networkObject);
//                 activePlayer = networkObject.GetComponent<Player>();
//                 SetActivePlayer(activePlayer);
//             }

//             Debug.Log("Client.Spawned: Active player set: " + activePlayer);
//         }
//     }


//     private void UpdateCameraFollow(Player player)
//     {
//         CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
//         if (virtualCamera != null)
//         {
//             virtualCamera.Follow = player.transform;
//         }
//         else
//         {
//             Debug.LogError("CinemachineVirtualCamera not found.");
//         }
//     }

//     private void InitializeCharacterBeforeSpawn(NetworkRunner runner, NetworkObject obj)
//     {
//         Player player = obj.GetComponent<Player>();
//         player.clientId = GetComponent<NetworkObject>().Id;
//         Debug.Log("Player ClientID = " + player.clientId);
//         Debug.Log("Object Name = " + player.name);

//         RegisterActivePlayer(player);
//     }

//     private void RegisterActivePlayer(Player player)
//     {
//         GameManager.Instance.player = player;
//         Debug.Log("Client(150).InitializeCharacterBeforeSpawn() called, activePlayer = player, activePlayer registered in GameManager as player.name: " + player.name);
//     }

//     public void SetActivePlayer(Player player)
//     {
//         if (HasStateAuthority)
//         {
//             if (activePlayer != null)
//             {
//                 NetworkObject activePlayerNetworkObject = activePlayer.gameObject.GetComponent<NetworkObject>();
//                 if (activePlayerNetworkObject != null)
//                 {
//                     Debug.Log("SetActivePlayer: Despawning current active player NetworkObject.");
//                     networkRunner.Despawn(activePlayerNetworkObject);
//                 }
//                 else
//                 {
//                     Debug.LogError("SetActivePlayer: activePlayer's NetworkObject is null.");
//                 }
//             }

//             activePlayer = player;
//             RegisterActivePlayer(activePlayer);

//             NetworkObject characterObject = activePlayer.GetComponent<NetworkObject>();
//             if (characterObject != null)
//             {
//                 AssignAuthorityToCharacter(characterObject);
//             }
//             else
//             {
//                 Debug.LogError("SetActivePlayer: Character NetworkObject is null.");
//             }

//             activePlayer.SetupLocalPlayer();
//             activePlayer.transform.SetParent(characterHolder, false);
//         }
//         else
//         {
//             activePlayer.SetupOtherPlayer();
//         }
//     }

//     public void AssignAuthorityToCharacter(NetworkObject characterObject)
//     {
//         if (networkRunner.LocalPlayer.IsNone)
//         {
//             Debug.LogError("LocalPlayer is None. Authority cannot be assigned.");
//             return;
//         }
    
//         Debug.Log("Assigning authority to characterObject: " + characterObject);
//         networkRunner.SetPlayerObject(networkRunner.LocalPlayer, characterObject);
//         Debug.Log($"Authority assigned to character. InputAuthority: {characterObject.InputAuthority}, StateAuthority: {characterObject.StateAuthority}");
//     }

//     private void AssignLocalPlayerObject(NetworkObject networkObject)
//     {
//         if (networkRunner != null && networkRunner.LocalPlayer.IsValid)
//         {
//             localPlayerNetworkObject = networkObject;
//             if (localPlayerNetworkObject != null)
//             {
//                 Debug.Log("Assigning LocalPlayerNetworkObject: " + localPlayerNetworkObject);
//                 networkRunner.SetPlayerObject(networkRunner.LocalPlayer, localPlayerNetworkObject);
//             }
//             else
//             {
//                 Debug.LogError("Local Player NetworkObject is null.");
//             }
//         }
//         else
//         {
//             Debug.LogError("Runner or Local Player is null.");
//         }
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class Client : NetworkBehaviour
{
    List<Player> characters;
    public Player activePlayer;
    public GameObject playerPrefab;

    [Networked]
    public int activePlayerSceneIndex {get; set;}
    public Transform characterHolder;
    public Dictionary<string, GameObject> characterPrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> characterInstances = new Dictionary<string, GameObject>(); // Agregado


    private void Awake()
    {
        if (characterHolder == null)
        {
            characterHolder = GameObject.Find("CharacterHolder")?.transform;
            if (characterHolder == null)
            {
                Debug.LogError("CharacterHolder not found in scene.");
            }
        }

        // Asigna los prefabs a los nombres de los personajes
        characterPrefabs["Cerebrito"] = Resources.Load<GameObject>("Prefabs/Cerebrito");
        characterPrefabs["Tres Deseos"] = Resources.Load<GameObject>("Prefabs/TresDeseos");
    }
    
    public override void Spawned()
    {
        activePlayerSceneIndex = 1;
        DontDestroyOnLoad(gameObject);
        transform.SetParent(GameManager.Instance.clientHolder);
        gameObject.name = "Client " + GetComponent<NetworkObject>().Id.ToString();
        
        if (GameManager.Instance.clients.Contains(this))
        {
            GameManager.Instance.clients.Remove(this);
        }
        GameManager.Instance.clients.Add(this);
        
        Debug.Log("Spawned");
        Debug.Log("HasStateAuthority: " + HasStateAuthority);
        if(HasStateAuthority)
        {
            if (playerPrefab != null)
            {
                Runner.Spawn(
                playerPrefab, 
                new Vector3(0, 1, 0), 
                Quaternion.identity, 
                Runner.LocalPlayer,
                InitializeCharacterBeforeSpawn
                );  
            }
            else
            {
                Debug.LogError("playerPrefab is null.");
            }               
        }
    }

    private void InitializeCharacterBeforeSpawn(NetworkRunner runner, NetworkObject obj)
    {
        Player player = obj.GetComponent<Player>();
        player.clientId = GetComponent<NetworkObject>().Id;

        PrefabInfo prefabInfo = player.GetComponent<PrefabInfo>();
        if (prefabInfo != null)
        {
            player.gameObject.name = $"{prefabInfo.prefabName} {player.clientId.ToString()}";
        }
        else
        {
            player.gameObject.name = $"Player {player.clientId.ToString()}";
        }

        // Añadir la nueva instancia al diccionario
        characterInstances[player.gameObject.name] = player.gameObject;

    }

    public void SetActivePlayer(Player player)
    {
        activePlayer = player;

        if (activePlayer != null)
        {
            activePlayer.SetClient(this);

            if (HasStateAuthority)
            {
                activePlayer.SetupLocalPlayer();
                activePlayer.transform.SetParent(characterHolder, false);
            }
            else
            {
                activePlayer.SetupOtherPlayer();
            }
        }
    }

    public void ChangePlayerPrefab(string characterName)
{
    if (characterPrefabs.ContainsKey(characterName))
    {
        playerPrefab = characterPrefabs[characterName];
        Debug.Log($"Player prefab changed to: {characterName}. playerPrefab: {playerPrefab}");

        // Imprime las instancias de personajes antes de destruir las anteriores
        PrintCharacterInstances();

        // Destruir la instancia anterior si existe
        if (characterInstances.Count > 0)
        {
            foreach (var kvp in characterInstances)
            {
                GameObject instance = kvp.Value;
                if (instance != null)
                {
                    Debug.Log($"Destroying previous instance: {instance.name}");
                    Destroy(instance);
                }
            }
            characterInstances.Clear(); // Limpia el diccionario de instancias
        }

        // Imprime las instancias de personajes después de destruir las anteriores
        PrintCharacterInstances();

        // Instanciar el nuevo personaje
        GameObject newInstance = Instantiate(playerPrefab, characterHolder);
        newInstance.name = $"{characterName} {GetComponent<NetworkObject>().Id.ToString()}";
        newInstance.SetActive(true);

        // Asignar el nombre del prefab al nuevo objeto
        PrefabInfo prefabInfo = newInstance.GetComponent<PrefabInfo>();
        if (prefabInfo == null)
        {
            prefabInfo = newInstance.AddComponent<PrefabInfo>();
        }
        prefabInfo.prefabName = characterName;

        // Añadir la nueva instancia al diccionario
        characterInstances[newInstance.name] = newInstance;

        // Imprimir las instancias de personajes después de añadir la nueva instancia
        PrintCharacterInstances();

        // Opcional: Configura la nueva instancia según sea necesario
    }
    else
    {
        Debug.LogError($"Character prefab not found: {characterName}");
    }
}

    public void PrintCharacterInstances()
    {
        foreach (var kvp in characterInstances)
        {
            Debug.Log($"Character Name: {kvp.Key}, Instance: {kvp.Value}");
        }
    }
}