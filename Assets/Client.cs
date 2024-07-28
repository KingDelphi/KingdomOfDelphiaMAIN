using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using System.Linq;

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

    public NetworkObject networkObject;

    public NetworkObject newNetworkObject;

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
        characterPrefabs["Tres Deseos"] = Resources.Load<GameObject>("Prefabs/Tres Deseos");
        characterPrefabs["Esqueletico"] = Resources.Load<GameObject>("Prefabs/Esqueletico");
        characterPrefabs["La Momia Verdulera"] = Resources.Load<GameObject>("Prefabs/La Momia Verdulera");
        characterPrefabs["Lobin"] = Resources.Load<GameObject>("Prefabs/Lobin");

    }
    
    public override void Spawned()
    {
        Debug.Log("Client(46).Spawned() called.");
        activePlayerSceneIndex = 1;
        DontDestroyOnLoad(gameObject);
        transform.SetParent(GameManager.Instance.clientHolder);
        gameObject.name = "Client " + GetComponent<NetworkObject>().Id.ToString();
        
        if (GameManager.Instance.clients.Contains(this))
        {
            GameManager.Instance.clients.Remove(this);
        }
        GameManager.Instance.clients.Add(this);
        
        Debug.Log("Client(58) " + gameObject.name + " Spawned. ");

        // Asociar el cliente al GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();

        if(GetComponent<NetworkObject>().HasStateAuthority)
        {
            if (gameManager != null)
            {
                gameManager.SetClient(this); 
                Debug.Log("Client(68) Client assigned to GameManager.");
            }
            else
            {
                Debug.LogError("GameManager not found.");
            }
        }
        
        networkObject = GetComponent<NetworkObject>();
        if(GetComponent<NetworkObject>().HasStateAuthority)
        {
            Debug.Log($"Client(79) networkObject.name: {networkObject.name} has StateAuthority: {networkObject.HasStateAuthority}, Runner.LocalPlayer: {Runner.LocalPlayer}, playerPrefab {playerPrefab}.");
        }

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

            Debug.LogWarning("✔️✔️✔️✔️✔️ Client(99) - Player Spawned Successfully - Running gameManager.MaximumVerification().");
            gameManager.MaximumVerification();

        }
    }

    private void InitializeCharacterBeforeSpawn(NetworkRunner runner, NetworkObject obj)
    {
        Debug.Log("CUASIMODISIMO TIENE QUE APARECER AL INICIAR Y AL CAMBIAR DE PERSONAJE. Client(107).InitializeCharacterBeforeSpawn() called. VARIABLES: NetworkRunner: " + runner + ", NetworkObject: " + obj);
        
        Player player = obj.GetComponent<Player>();
        Debug.Log("Client(110) After assigning Player player = obj.GetComponent<Player>(). VARIABLES: player: " + player);

        Debug.Log("Client(112) Before assigning player.clientId = GetComponent<NetworkObject>().Id. VARIABLES: player.clientId: " + player.clientId);
        player.clientId = GetComponent<NetworkObject>().Id;
        Debug.Log("Client(114) After assigning player.clientId = GetComponent<NetworkObject>().Id. VARIABLES: player.clientId: " + player.clientId);

        PrefabInfo prefabInfo = player.GetComponent<PrefabInfo>();
        if (prefabInfo != null)
        {
            player.gameObject.name = $"{prefabInfo.prefabName} {player.clientId.ToString()}";
        }
        else
        {
            player.gameObject.name = $"Player {player.clientId.ToString()}";
        }

        // Antes de añadir la nueva instancia al diccionario, imprime información sobre la instancia
        Debug.Log($"Client(127) Agregando nueva instancia al diccionario. Nombre (player.gameObject.name): {player.gameObject.name}, Instancia (player.gameObject): {player.gameObject}");
        // Añadir la nueva instancia al diccionario
        characterInstances[player.gameObject.name] = player.gameObject;
        // Después de añadir, verifica que la instancia se haya agregado correctamente
        if (characterInstances.ContainsKey(player.gameObject.name))
        {
            Debug.Log($"Client(133) Instancia agregada correctamente al diccionario. Nombre (player.gameObject.name): {player.gameObject.name}, Instancia (player.gameObject): {player.gameObject}");
        }
        else
        {
            Debug.LogError($"Client(137) Error al agregar la instancia al diccionario. Nombre (player.gameObject.name): {player.gameObject.name}, Instancia (player.gameObject): {player.gameObject}");
        }
    }

    public void SetActivePlayer(Player player)
    {
        if (GetComponent<NetworkObject>().HasStateAuthority)
        {
            Debug.Log("Client(145).SetActivePlayer() called. VARIABLES: player: " + player);
        }        
        
        //Debug.Log("Client(144) Before assigning activePlayer = player. VARIABLES: activePlayer: " + activePlayer + ", player: " + player);
        activePlayer = player;
        //Debug.Log("Client(146) After assigning activePlayer = player. VARIABLES: activePlayer: " + activePlayer + ", player: " + player);

        if (activePlayer != null)
        {
            activePlayer.SetClient(this);

            if (HasStateAuthority)
            {
                Debug.Log("Client(136) activePlayer: " + activePlayer + ", (should be True) HasStateAuthority: " + HasStateAuthority);
                activePlayer.SetupLocalPlayer();
                activePlayer.transform.SetParent(characterHolder, false);
            }
            // else
            // {
            //     activePlayer.SetupOtherPlayer();
            // }
        }
    }

    public void ChangePlayerPrefab(string characterName)
{
    Debug.Log("Client(167)ChangePlayerPrefab(string characterName) called. VARIABLES: characterName: " + characterName);
    if (characterPrefabs.ContainsKey(characterName))
    {
        playerPrefab = characterPrefabs[characterName];
        Debug.Log($"Client(171) Player prefab changed to playerPrefab: {playerPrefab}");

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
                    Debug.Log($"Client(184) Destroying previous instance: {instance}");
                    Destroy(instance);
                }
            }
            characterInstances.Clear(); // Limpia el diccionario de instancias
        }

        // Imprime las instancias de personajes después de destruir las anteriores
        PrintCharacterInstances();

        NetworkRunner networkRunner = FindObjectOfType<NetworkRunner>();
        Debug.Log("Client(195) AFTER assigning NetworkRunner networkRunner = FindObjectOfType<NetworkRunner>(). VARIABLES: (should be Client A) networkRunner: " + networkRunner);

        // Instanciar y activar el nuevo personaje
        Debug.LogWarning($"Client(198)(Should be same values as Client(67)) networkObject.name: {networkObject.name} has StateAuthority: {networkObject.HasStateAuthority}, Runner.LocalPlayer: {Runner.LocalPlayer}, playerPrefab: {playerPrefab}.");        
        if(HasStateAuthority)
        {
            if (playerPrefab != null)
            {                
                newNetworkObject = Runner.Spawn(
                playerPrefab, 
                new Vector3(0, 1, 0), 
                Quaternion.identity, 
                Runner.LocalPlayer,
                InitializeCharacterBeforeSpawn
                );

                if (newNetworkObject == null)
                {
                    Debug.LogError("Failed to spawn the player prefab.");
                }
                else
                {
                    Debug.Log("Client(217) Successfully spawned the player prefab newNetworkObject: " + newNetworkObject);
                    InitializeCharacterBeforeSpawn(Runner, newNetworkObject); 
                }
            }
            else
            {
                Debug.LogError("playerPrefab is null.");
            }               
        }

        // NetworkObject newNetworkObject = Runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, Runner.LocalPlayer, InitializeCharacterBeforeSpawn);
        Debug.Log("Client(228) Before assigning GameObject newInstance = newNetworkObject.gameObject. VARIABLES: newNetworkObject.gameObject: " + newNetworkObject.gameObject);
        GameObject newInstance = newNetworkObject.gameObject;
        Debug.Log("Client(230) After assigning GameObject newInstance = newNetworkObject.gameObject. VARIABLES: newInstance: " + newInstance);

        newInstance.name = $"{characterName} {GetComponent<NetworkObject>().Id.ToString()}";
        Debug.Log("Client(233) Despues de instanciar el personaje y nombrarlo - newInstance.name: " + newInstance.name);
        newInstance.SetActive(true);
        Debug.Log("Client(235) Despues de activar el personaje y nombrarlo - newInstance.name: " + newInstance.name);
    
            if (networkRunner != null)
            {
                // Encuentra el objeto del cliente con autoridad de estado
                var clientWithAuthority = FindObjectsOfType<Client>().FirstOrDefault(c => c.GetComponent<NetworkObject>().HasStateAuthority);
                Debug.Log("Client(241) After the assignation var clientWithAuthority = FindObjectsOfType<Client>().FirstOrDefault(c => c.GetComponent<NetworkObject>().HasStateAuthority). VARIABLES: clientWithAuthority: " + clientWithAuthority);
                if (clientWithAuthority != null)
                {
                    Debug.Log($"Client(244) Client with StateAuthority found. VARIABLES: clientWithAuthority: {clientWithAuthority}, (should be True) clientWithAuthority.HasStateAuthority: {clientWithAuthority.HasStateAuthority}");
                }
                else
                {
                    Debug.LogError("Client(248) clientWithAuthority is null.");
                }
            }
            else
            {
                Debug.LogError("Client(253) networkRunner is null.");
            }

        // Asignar el nombre del prefab al nuevo objeto
        PrefabInfo prefabInfo = newInstance.GetComponent<PrefabInfo>();
        if (prefabInfo == null)
        {
            prefabInfo = newInstance.AddComponent<PrefabInfo>();
        }
        prefabInfo.prefabName = characterName;

        // Antes de añadir la nueva instancia al diccionario, imprime información sobre la instancia
        Debug.Log($"Client(265) Antes de agregar nueva instancia al diccionario. (Nombre) newInstance.name: {newInstance.name}, (Instancia) newInstance: {newInstance}");
        // Añadir la nueva instancia al diccionario
        characterInstances[newInstance.name] = newInstance;
        Debug.Log($"Client(268) Despues de agregar nueva instancia al diccionario. (Nombre) newInstance.name: {newInstance.name}, (Instancia) newInstance: {newInstance}");

        // Después de añadir, verifica que la instancia se haya agregado correctamente
        if (characterInstances.ContainsKey(newInstance.name))
        {
            Debug.Log($"Client(273) Instancia agregada correctamente al diccionario. (Nombre) newInstance.name: {newInstance.name}, (Instancia) characterInstances[newInstance.name]: {characterInstances[newInstance.name]}");
        }
        else
        {
            Debug.LogError($"Client(277) Error al agregar la instancia al diccionario. (Nombre) newInstance.name: {newInstance.name}");
        }

        // Imprimir las instancias de personajes después de añadir la nueva instancia
        PrintCharacterInstances();

        // Opcional: Configura la nueva instancia según sea necesario
    }
    else
    {
        Debug.LogError($"Client(287) Character prefab not found: {characterName}");
    }
}

    private IEnumerator AssignAuthorityWithTimeout(NetworkRunner networkRunner, NetworkObject newInstanceNetworkObject, NetworkObject clientNetworkObject)
    {
        Debug.Log("Client(293).AssignAuthorityWithTimeout(NetworkRunner networkRunner, NetworkObject newInstanceNetworkObject, NetworkObject clientNetworkObject) called. newInstanceNetworkObject.Id: " + newInstanceNetworkObject.Id);
        float timeout = 5f; // Tiempo de espera en segundos
        float elapsedTime = 0f;
        bool idAssigned = false;

        // Esperar hasta que el nuevo objeto tenga un ID de red asignado
        while (elapsedTime < timeout)
        {
            if (newInstanceNetworkObject.Id.IsValid)
            {
                idAssigned = true;
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (idAssigned)
        {
            Debug.Log($"Client(313) Setting player object authority. VARIABLES: newInstanceNetworkObject: {newInstanceNetworkObject}, newInstanceNetworkObject.Id: {newInstanceNetworkObject.Id}");

            // Asignar autoridad al nuevo objeto
            networkRunner.SetPlayerObject(clientNetworkObject.InputAuthority, newInstanceNetworkObject);

            Debug.Log($"Client(318) Player object authority set. VARIABLES: clientNetworkObject.InputAuthority: {clientNetworkObject.InputAuthority}, newInstanceNetworkObject: {newInstanceNetworkObject}");
        }
        else
        {
            Debug.LogError($"Client(322) NetworkObject with StateAuthority did not receive a NetworkId in time. NetworkObject: {newInstanceNetworkObject}");
        }
    }


    public void PrintCharacterInstances()
    {
        Debug.LogWarning("Client(329) PrintCharacterInstances() called.");
        
        foreach (var kvp in characterInstances)
        {
            var characterName = kvp.Key;
            var characterInstance = kvp.Value;
            Debug.Log($"Character Name: {characterName}, Instance: {characterInstance}");

            if (characterInstance != null)
            {
                var isActive = characterInstance.activeSelf;
                var playerScript = characterInstance.GetComponent<Player>();
                Debug.Log($"Character Instance: {characterInstance}, IsActive: {isActive}, PlayerScript: {playerScript}");

                if (playerScript != null)
                {
                    Debug.Log($"Player Script is not null, proceeding with final log.");

                    Debug.Log($"Character Name: {characterName}, " +
                            $"Instance: {characterInstance}, " +
                            //$"Client ID: {playerScript.clientId}, " +
                            $"State Authority: {playerScript.HasStateAuthority}, " +
                            $"Active: {isActive}");
                }
                else
                {
                    Debug.LogError($"Player script is null for character instance: {characterInstance}");
                }
            }
            else
            {
                Debug.LogError($"Character instance is null for character name: {characterName}");
            }
        }
    }

    public GameObject GetCharacterInstance(string characterName)
    {
        Debug.Log("Client(367).GetCharacterInstance(string characterName) called. VARIABLES: characterName: " + characterName);
        // Busca la instancia
        if (characterInstances.ContainsKey(characterName))
        {
            Debug.Log($"Client(371) Character instance found for {characterName}");
            return characterInstances[characterName];
        }
        else
        {
            Debug.LogError($"Character instance not found for {characterName}");
            return null;
        }
    }

    public void ClientVerification()
    {
        Debug.Log($"Client Verification: \n" +
                  $"NetworkObject Name: {networkObject?.name}\n" +
                  $"Has State Authority: {networkObject?.HasStateAuthority}\n" +
                  //$"NetworkRunner: {runner}\n" +
                  //$"NetworkObject: {obj}\n" +
                  //$"Player: {player}\n" +
                  //$"Player Client ID: {player?.clientId}\n" +
                  //$"Nombre (player.gameObject.name): {player.gameObject.name}\n" +
                  //$"Instancia (player.gameObject): {player.gameObject}\n" +
                  $"activePlayer: {activePlayer}\n" +
                  $"HasStateAuthority: {HasStateAuthority}\n");
        Debug.Log("Client Verification: ✔️ Success");
    }
}