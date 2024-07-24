// using UnityEngine;
// using UnityEngine.SceneManagement;
// using Cinemachine;
// using System.Collections.Generic;
// using Fusion;
// using System;
// using System.Linq;

// public class GameManager : MonoBehaviour
// {   
//     public GameObject playerPrefab; // Prefab del jugador
//     public Vector3 spawnPosition; // Posición de spawn

//     public NetworkRunner runner; // Añade esta línea para referenciar el NetworkRunner

    
//     public PlayerInventory playerInventory;
//     public GameObject inventoryCanvas;
    
//     public GameObject slot;
//     public GearMenu gearMenu;

//     public NetworkObject playerNetworkObject;
//     public static event Action<NetworkObject> OnPlayerSpawned;

//     public GameObject existingPlayer;
//     public NetworkObject existingNetworkObject;


    


//     private static GameManager _instance;
//     public static GameManager Instance 
//     { 
//         get
//         {
//             if(_instance == null)
//             {
//                 _instance = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();
//                 _instance.Initiate();
//             }
//             return _instance;
//         }    
//         private set
//         {
//             _instance = value;
//         } 
//     }


//     private CinemachineVirtualCamera virtualCamera;
//     private PolygonCollider2D confinerCollider;
//     public Player player;
//     private List<GearItem> savedPlayerInventory;
//     private int savedPlayerHealth;
//     private Vector3 savedEntryPosition;

//     public GameObject UI;

//     public List<GameObject> characters;
//     private Dictionary<string, GameObject> characterInstances = new Dictionary<string, GameObject>();


//     public List<Client> clients = new List<Client>();

//     public List<UnityEngine.Object> scenes;

//     public Transform clientHolder;
//     public Transform characterHolder;

//     public void SaveGameState()
//     {
//         if (SaveLoadSystem.Instance != null)
//         {
//             SaveLoadSystem.Instance.SaveGame();
//         }
//         else
//         {
//             Debug.LogWarning("SaveLoadSystem not found. Unable to save game.");
//         }
//     }

//     public void LoadGameState()
//     {
//         if (SaveLoadSystem.Instance != null)
//         {
//             SaveLoadSystem.Instance.LoadGame();
//         }
//         else
//         {
//             Debug.LogWarning("SaveLoadSystem not found. Unable to load game.");
//         }
//     }

//     private void OnApplicationQuit()
//     {
//         SaveGameState();
//     }

//     private void Awake()
//     {   
//         if (Instance == null)
//         {
//             Instance = this;
//             Initiate();
//         }

//         // Crear instancias de los prefabs y mantenerlas desactivadas
//         foreach (var prefab in characters)
//         {
//             GameObject instance = Instantiate(prefab);
//             instance.SetActive(false);

// // Establecer el CharacterHolder como padre de los clones
//                 if (characterHolder != null)
//                 {
//                     instance.transform.SetParent(characterHolder, false);
//                 }
//                 else
//                 {
//                     Debug.LogWarning("CharacterHolder is not assigned.");
//                 }

//             DontDestroyOnLoad(instance);
//             characterInstances.Add(prefab.name, instance);
//         }
//     }

//     public GameObject GetCharacterInstance(string characterName)
//     {
//         if (characterInstances.ContainsKey(characterName))
//         {
//             return characterInstances[characterName];
//         }
//         return null;
//     }

//     public void PlayerInventoryInitialized(PlayerInventory inventory)
//     {
//         playerInventory = inventory;
//         if (playerInventory == null)
//         {
//             Debug.LogError("PlayerInventory component not found.");
//         }
//         else
//         {
//             Debug.Log("GameManager(148) Inventario del jugador inicializado con playerInventory = inventory: " + inventory); // OJOLOCUAZ se puso debug para saber cual es el inventory.
//         }
//     }

//     private void Initiate()
//     {
//         DontDestroyOnLoad(gameObject);
//         DontDestroyOnLoad(UI);
//         DontDestroyOnLoad(clientHolder);
//         DontDestroyOnLoad(characterHolder);
//     }

//     private void Update() {
//            if(Input.GetKeyDown(KeyCode.K))
//            {
//                 SaveGameState();
//            }

//            if(Input.GetKeyDown(KeyCode.L))
//            {
//                 LoadGameState();
//            }
           
//            if(Input.GetKeyDown(KeyCode.I)) {
//             playerInventory = FindObjectOfType<PlayerInventory>();
//             int newIndex = new int();
//             foreach(GearItem item in playerInventory.inventory) {
//             if(gearMenu.equippedGear[0].itemName == item.itemName) {    
//                 playerInventory.inventory.RemoveAt(newIndex);
//                 break;
//             }
//             newIndex += 1;
//             }

//             if(inventoryCanvas.activeInHierarchy) {
//                 foreach(Transform t in inventoryCanvas.transform.GetChild(0)) {
//                         Destroy(t.gameObject);
//                     }
//               inventoryCanvas.SetActive(false);
//               } else {
//               inventoryCanvas.SetActive(true);
//               foreach(Transform t in inventoryCanvas.transform.GetChild(0)) {
//                         Destroy(t.gameObject);
//                     }
//               playerInventory.ListItems(playerInventory.inventory, slot, inventoryCanvas.transform.GetChild(0).gameObject);
//               }
//             }
//     }
            

//     public void ChangeScene(int targetSceneIndex, Vector3 entryPosition)
//     {
//         savedEntryPosition = entryPosition;
        
//         SceneManager.LoadScene(targetSceneIndex);
        
//     }

//     public void ChangeScene(int targetSceneIndex)
//     {
//         ChangeScene(targetSceneIndex, Vector3.zero);
//     }

//     public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {        
//         SetupSceneForPlayer(player, runner);

//         player.RpcUpdatePlayers();
//     }

//     public void SetupSceneForPlayer(Player player, NetworkRunner runner)
// {
//     Debug.Log($"Setting up scene for player. Player: {player}, Runner: {runner}");

//     Debug.Log("NEW (3rd Iteration)GameManager(222) BEFORE making this assignment this.player = player. Variables: this.player: " + this.player + " player: " + player); // OJOLOCUAZ...Vamos a ver que sale en las variables
//     this.player = player; // POSIBLE ERROR, DESCOMENTAR SI SALE ERROR.
//     Debug.Log("NEW (3rd Iteration)GameManager(224) AFTER making this assignment this.player = player. Variables: this.player: " + this.player + " player: " + player); // OJOLOCUAZ...Vamos a ver que sale en las variables
    
//     Debug.Log("NEW (3rd Iteration)GameManager(226) BEFORE making this assignment this.playerPrefab = player.gameObject. Variables: this.playerPrefab: " + this.playerPrefab + " player.gameObject: " + player.gameObject); // OJOLOCUAZ...Vamos a ver que sale en las variables
//     this.playerPrefab = player.gameObject; //POSIBLE SOLUCION
//     Debug.Log("NEW (3rd Iteration)GameManager(228) AFTER making this assignment this.playerPrefab = player.gameObject. Variables: this.playerPrefab: " + this.playerPrefab + " player.gameObject: " + player.gameObject); // OJOLOCUAZ...Vamos a ver que sale en las variables    

//     playerNetworkObject = player.GetComponent<NetworkObject>();
//     Debug.Log("Assigned playerNetworkObject: " + playerNetworkObject);
//     Debug.Log("playerNetworkObject.InputAuthority: " + playerNetworkObject.InputAuthority);


//     Debug.Log("NEW (3rd Iteration)GameManager(230) BEFORE checking if (runner == null) Variables: runner: " + runner); // OJOLOCUAZ...Vamos a ver que sale en las variables
//             if (runner == null)
//             {
//                 runner = FindObjectOfType<NetworkRunner>();
//                 Debug.Log("GameManager(234) Assigned runner = FindObjectOfType<NetworkRunner>(); runner: " + runner);
//             }

//             if (runner == null)
//             {
//                 Debug.LogError("NetworkRunner is null.");
//                 return;
//             }

//     Debug.Log("NEW (3rd Iteration)GameManager(243) BEFORE checking if (playerPrefab == null) Variables: playerPrefab: " + playerPrefab); // OJOLOCUAZ...Vamos a ver que sale en las variables
//             if (playerPrefab == null)
//             {
//                 Debug.LogError("Player prefab is null.");
//                 return;
//             }

//     Debug.Log("NEW (3rd Iteration)GameManager(250) BEFORE checking if (player == null) Variables: player: " + player); // OJOLOCUAZ...Vamos a ver que sale en las variables
//             if (player == null)
//             {
//                 Debug.LogError("Player is null.");
//                 return;
//             }

//             // Asegúrate de que player tiene NetworkObject y si no, agregarlo
//             // Debug.Log("UPDATED To 3rd Iteration. GameManager(258) BEFORE Assigning playerNetworkObject = player.GetComponent<NetworkObject>(). VARIABLES: playerNetworkObject: " + playerNetworkObject + " player.GetComponent<NetworkObject>(): " + player.GetComponent<NetworkObject>()); // OJOLOCUAZ - Se agrego un debug AFTER
//             // playerNetworkObject = player.GetComponent<NetworkObject>();
//             // Debug.Log("UPDATED To 3rd Iteration. GameManager(260) AFTER Assigning playerNetworkObject = player.GetComponent<NetworkObject>(). VARIABLES: playerNetworkObject: " + playerNetworkObject + " player.GetComponent<NetworkObject>(): " + player.GetComponent<NetworkObject>()); // OJOLOCUAZ - Se agrego un debug AFTER
            
//     Debug.Log("NEW (3rd Iteration)GameManager(262) BEFORE checking if (playerNetworkObject == null) Variables: playerNetworkObject: " + playerNetworkObject); // OJOLOCUAZ...Vamos a ver que sale en las variables        
//             if (playerNetworkObject == null)
//             {
//                 Debug.Log("UPDATED To 3rd Iteration. GameManager(263) Assigning playerNetworkObject = player.gameObject.AddComponent<NetworkObject>(). VARIABLES: playerNetworkObject: " + playerNetworkObject + " player.gameObject.AddComponent<NetworkObject>(): " + player.gameObject.AddComponent<NetworkObject>()); // OJOLOCO DEBUG - 2da Iteracion
//                 playerNetworkObject = player.gameObject.AddComponent<NetworkObject>();
//             }

//             if (playerNetworkObject == null)
//             {
//                 Debug.LogError("Player NetworkObject is null.");
//                 return;
//             }
//             else
//             {
//                 runner.SetPlayerObject(runner.LocalPlayer, playerNetworkObject);
//                 Debug.Log($"Player object set for runner. LocalPlayer: {runner.LocalPlayer}, PlayerObject: {playerNetworkObject}");
//             }

//     Debug.Log("NEW (3rd Iteration)GameManager(279) BEFORE checking if (playerNetworkObject.InputAuthority == null) Variables: playerNetworkObject.InputAuthority: " + playerNetworkObject.InputAuthority); // OJOLOCUAZ...Vamos a ver que sale en las variables      
//             if (playerNetworkObject.InputAuthority == null)
//             {
//                 Debug.LogError("InputAuthority is null.");
//                 return;
//             }
//             else
//             {
//                 Debug.Log("2ND ITERATION! GameManager(287) Checking variable playerNetworkObject.InputAuthority: " + playerNetworkObject.InputAuthority + " NOTE: Should be Player[0] or Character[Id:524290]."); // OJOLOCUAZ Updated a 3ra Iteracion - playerNetworkObject.InputAuthority = [Player:None], lo cual debe ser un PROBLEMA y talvez deberia ser [Player:0] o Character [Id:524290]
//             }

//     // Línea 233 - Asegúrate de que `player.character` puede almacenar un `NetworkObject` o usa un `GameObject`.
// Debug.Log("NEW (3rd Iteration)GameManager(291) BEFORE making this assignment existingPlayer = FindInactiveObjectByName(playerPrefab.name). Variables: existingPlayer: " + existingPlayer + " FindInactiveObjectByName(playerPrefab.name): " + FindInactiveObjectByName(playerPrefab.name)); // OJOLOCUAZ...Vamos a ver que sale en las variables
// existingPlayer = FindInactiveObjectByName(playerPrefab.name);
// Debug.Log("NEW (3rd Iteration)GameManager(293) AFTER making this assignment existingPlayer = FindInactiveObjectByName(playerPrefab.name). Variables: existingPlayer: " + existingPlayer + " FindInactiveObjectByName(playerPrefab.name): " + FindInactiveObjectByName(playerPrefab.name)); // OJOLOCUAZ...Vamos a ver que sale en las variables
// Debug.Log("GameManager(294) BEFORE CHECKING if(existingPlayer == null). VARIABLES: runner: " + runner + " playerPrefab: " + playerPrefab + " player: " + player + " assigned playerNetworkObject = player.GetComponent<NetworkObject>(): " + playerNetworkObject + " playerNetworkObject.InputAuthority: " + playerNetworkObject.InputAuthority + " assigned existingPlayer = playerPrefab.name: " + existingPlayer);
// if(existingPlayer == null) // OJOLOCUAZ DE LA 2da Iteracion existingPlayer == null (FALSE), se brinco esta parte.
// {
//     Debug.Log("NEW 3rd Iteration!GameManager(297) if(existingPlayer == null) = TRUE");
//     NetworkObject playerNetworkObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, runner.LocalPlayer);
//     Debug.Log("GAMEMANAGER: Nuevo jugador spawneado: " + playerNetworkObject);
//     OnPlayerSpawned?.Invoke(playerNetworkObject);
// }
// else // OJOLOCUAZ DE LA 2da Iteracion existingPlayer == null (FALSE), se llamo esta parte.
// {
//     Debug.Log("NEW 3rd Iteration!GameManager(304) if(existingPlayer == null) = FALSE");
//     Debug.Log("GameManager(305) Since (existingPlayer == null) = false, variable existingPlayer: " + existingPlayer + " ya existe, no se ha creado un nuevo prefab, sino que SE VA A ACTIVAR el Clone existingPlayer: " + existingPlayer);
//     existingPlayer.SetActive(true);
//     Debug.Log("GameManager(307) No se ha creado un nuevo prefab, sino que SE HA ACTIVADO el Clone existingPlayer con existingPlayer.SetActive(true), variable existingPlayer: " + existingPlayer + " Ahora se creara el Network Object existingNetworkObject con Network Object existingNetworkObject = existingPlayer.GetComponent<NetworkObject>()");
//     NetworkObject existingNetworkObject = existingPlayer.GetComponent<NetworkObject>();
//     Debug.Log("GameManager(309) existingPlayer: " + existingPlayer + " existe y se ha activado. La variable NetworkObject existingNetworkObject: " + existingNetworkObject + " se ha creado con el valor existingPlayer.GetComponent<NetworkObject>(): " + existingPlayer.GetComponent<NetworkObject>() + " Se llamara a la Accion OnPlayerSpawned?.Invoke(existingNetworkObject)");
//     OnPlayerSpawned?.Invoke(existingNetworkObject);
//     Debug.Log("GameManager(311). Debug.Log despues de que se llamo OnPlayerSpawned?.Invoke(existingNetworkObject) con el valor existingNetworkObject: " + existingNetworkObject);

// }

// Debug.Log("2ND ITERATION! GameManager(315) Just BEFORE assigning player.character = playerNetworkObject.gameObject. VARIABLES: player.character: " + player.character + " playerNetworkObject.gameObject: " + playerNetworkObject.gameObject); // OJOLOCO NUEVO DEBUG - 2da Iteracion
// player.character = playerNetworkObject.gameObject;
// Debug.Log("2ND ITERATION! GameManager(317) Just AFTER assigning player.character = playerNetworkObject.gameObject. VARIABLES: player.character: " + player.character + " playerNetworkObject.gameObject: " + playerNetworkObject.gameObject); // OJOLOCO NUEVO DEBUG - 2da Iteracion

// // Obtener el `GameObject` del `NetworkObject`.
// Debug.Log("2ND ITERATION! GameManager(320) Just BEFORE assigning GameObject characterGameObject = player.character. VARIABLES: NEW GAMEOBJECT characterGameObject, & player.character: " + player.character); // OJOLOCO NUEVO DEBUG - 2da Iteracion
// GameObject characterGameObject = player.character;
// Debug.Log("2ND ITERATION! GameManager(322) Just AFTER assigning GameObject characterGameObject = player.character. VARIABLES: characterGameObject: " + characterGameObject + " player.character: " + player.character); // OJOLOCO NUEVO DEBUG - 2da Iteracion

// // Actualizar la referencia del GameManager al nuevo personaje
// Debug.Log("2ND ITERATION! GameManager(325) Just BEFORE assigning this.player = characterGameObject.GetComponent<Player>(). VARIABLES: this.player: " + this.player + " characterGameObject.GetComponent<Player>(): " + characterGameObject.GetComponent<Player>()); // OJOLOCO NUEVO DEBUG - 2da Iteracion
// this.player = characterGameObject.GetComponent<Player>();
// Debug.Log("2ND ITERATION! GameManager(327) Just AFTER assigning this.player = characterGameObject.GetComponent<Player>(). VARIABLES: this.player: " + this.player + " characterGameObject.GetComponent<Player>(): " + characterGameObject.GetComponent<Player>()); // OJOLOCO NUEVO DEBUG - 2da Iteracion

// // Asegúrate de que `player.characterController` esté definido o accede al `CharacterController` desde el `GameObject` del `NetworkObject`.
// Debug.Log("2ND ITERATION! GameManager(330) Just BEFORE assigning CharacterController characterController = characterGameObject.GetComponent<CharacterController>(). VARIABLES: NEW CHARACTERCONTROLLER characterController, & characterGameObject.GetComponent<CharacterController>(): " + characterGameObject.GetComponent<CharacterController>()); // OJOLOCO NUEVO DEBUG - 2da Iteracion
// CharacterController characterController = characterGameObject.GetComponent<CharacterController>();
// Debug.Log("2ND ITERATION! GameManager(332) Just AFTER assigning CharacterController characterController = characterGameObject.GetComponent<CharacterController>(). VARIABLES: characterController: " + characterController + " characterGameObject.GetComponent<CharacterController>(): " + characterGameObject.GetComponent<CharacterController>()); // OJOLOCO NUEVO DEBUG - 2da Iteracion
// if (characterController != null)
//     {
//         characterController.Initiate();
//     }
//     else
//     {
//         Debug.LogError("CharacterController not found on the character GameObject.");
//     }

//     // // Actualizar la cámara para seguir al nuevo personaje
//     virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
//     if (virtualCamera != null)
//     {
//         virtualCamera.Follow = characterGameObject.transform;
//     }
//     else
//     {
//         Debug.LogError("CinemachineVirtualCamera not found.");
//     }

//     GameObject confinerObject = GameObject.Find("Map");
//     if (confinerObject != null)
//     {
//         confinerCollider = confinerObject.GetComponent<PolygonCollider2D>();
//     }

//     if (savedEntryPosition != Vector3.zero)
//     {
//         player.transform.position = savedEntryPosition;
//     }
// }


//     public PlayerInventory GetPlayerInventory()
//     {
//         return playerInventory;
//     }

//     public void ActivatePlayersInScene()
//     {
//         Debug.Log("Activating Players in Scene.");
//         foreach(Client client in clients)
//         {
//             Player player = client.activePlayer;
//             if(player != null)
//             {
//                 bool shouldBeActive = client.activePlayerSceneIndex == SceneManager.GetActiveScene().buildIndex;
//                 player.gameObject.SetActive(shouldBeActive);
//                 Debug.Log($"Player {player.name} active state set to {shouldBeActive}");
//             }       
//         }
//     }

//     public static string GetScene(int index)
//     {
//         return SceneUtility.GetScenePathByBuildIndex(index);
//     }

//     private GameObject FindInactiveObjectByName(string name)
//     {
//         GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
//         return allObjects.FirstOrDefault(obj => obj.name == name && obj.hideFlags == HideFlags.None);
//     }

//     #if !UNITY_EDITOR
//             static string myLog = "";
//             private string output;
//             private string stack;

//             void OnEnable()
//             {
//                 Application.logMessageReceivedThreaded += Log;
//             }

//             void OnDisable()
//             {
//                 Application.logMessageReceivedThreaded -= Log;
//             }

//             public void Log(string logString, string stackTrace, UnityEngine.LogType type)
//             {
//                 output = logString;
//                 stack = stackTrace;
//                 myLog = output + "\n" + stack + "\n" + myLog;
//                 if (myLog.Length > 5000)
//                 {
//                 myLog = myLog.Substring(0, 4000);
//                 }
//             }
//             void OnGUI()
//             {
//                 {
//                     myLog = GUI.TextArea(new Rect(30, 30, Screen.width / 2, Screen.height - 30), myLog);
//                 }
//             }
//     #endif
// }

using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections.Generic;
using Fusion;

public class GameManager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public GameObject inventoryCanvas;
    public GameObject anotherInv;
    public GameObject slot;
    public GearMenu gearMenu;
    public NetworkManager networkManager;


    private static GameManager _instance;
    public static GameManager Instance 
    { 
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindWithTag("Game Manager").GetComponent<GameManager>();
                _instance.Initiate();
            }
            return _instance;
        }    
        private set
        {
            _instance = value;
        } 
    }

    private CinemachineVirtualCamera virtualCamera;
    private PolygonCollider2D confinerCollider;

    public Player player;
    private List<GearItem> savedPlayerInventory;
    private int savedPlayerHealth;
    private Vector3 savedEntryPosition;

    public GameObject UI;

    public List<GameObject> characters;

    private Dictionary<string, GameObject> characterInstances = new Dictionary<string, GameObject>();

    public List<Client> clients = new List<Client>();

    public List<Object> scenes;

    public Transform clientHolder;
    public Transform characterHolder;
    public CharacterController characterController;

    private void Awake()
    {   
        if (Instance == null)
        {
            Instance = this;
            Initiate();
        }
    }

    public GameObject GetCharacterInstance(string characterName)
    {
        if (characterInstances.ContainsKey(characterName))
        {
            Debug.Log($"Character instance found for {characterName}");
            return characterInstances[characterName];
        }
        else
        {
            Debug.LogError($"Character instance not found for {characterName}");
            return null;
        }
    }

    public void PlayerInventoryInitialized(PlayerInventory inventory)
    {
        playerInventory = inventory;
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory component not found.");
        }
        else
        {
            // Debug.Log("Inventario del jugador inicializado con playerInventory = inventory: " + inventory);
        }
    }

    private void Initiate()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UI);
        DontDestroyOnLoad(clientHolder);
        DontDestroyOnLoad(characterHolder);
    }

    private void Update() {
           if(Input.GetKeyDown(KeyCode.I)) {
            playerInventory = FindObjectOfType<PlayerInventory>();
            int newIndex = new int();
            foreach(GearItem item in playerInventory.inventory) {
            if(gearMenu.equippedGear[0].itemName == item.itemName) {    
                playerInventory.inventory.RemoveAt(newIndex);
                break;
            }
            newIndex += 1;
            }

            if(inventoryCanvas.activeInHierarchy) {
                foreach(Transform t in inventoryCanvas.transform.GetChild(0)) {
                        Destroy(t.gameObject);
                    }
              inventoryCanvas.SetActive(false);
              } else {
              inventoryCanvas.SetActive(true);
              foreach(Transform t in inventoryCanvas.transform.GetChild(0)) {
                        Destroy(t.gameObject);
                    }
              playerInventory.ListItems(playerInventory.inventory, slot, inventoryCanvas.transform.GetChild(0).gameObject);
              }
            }
    }
            

    public void ChangeScene(int targetSceneIndex, Vector3 entryPosition)
    {
        savedEntryPosition = entryPosition;
        
        SceneManager.LoadScene(targetSceneIndex);   
    }

    public void ChangeScene(int targetSceneIndex)
    {
        ChangeScene(targetSceneIndex, Vector3.zero);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded");
        
        SetupSceneForPlayer(player);
        Debug.Log("Before RPC");
        player.RpcUpdatePlayers();
    }

    public void SetupSceneForPlayer(Player _player)
    {
        player = _player;
        
        if (player == null)
        {
            Debug.Log("Player or PlayerInventory not found in the scene.");
            return;
        }

        characterController = player.GetComponent<CharacterController>();
        characterController.hasLastPosition = false;
        characterController.Initiate();

        player.currentHealth = savedPlayerHealth;

        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        if (virtualCamera == null)
        {
            Debug.Log("CinemachineVirtualCamera not found in the scene.");
            return;
        }

        if (savedEntryPosition != Vector3.zero)
        {
            Debug.Log("Setting Position to " + savedEntryPosition);
            player.transform.position = savedEntryPosition;
        }

        virtualCamera.Follow = player.transform;
        
        GameObject confinerObject = GameObject.Find("Map");
        if (confinerObject == null)
        {
            Debug.Log("Object with Confiner not found.");
        }

        confinerCollider = confinerObject.GetComponent<PolygonCollider2D>();
        if (confinerCollider == null)
        {
            // Debug.Log("PolygonCollider2D not found on the object with Confiner.");
        }

        if (player == null)
        {
            Debug.Log("Character not found in the scene.");
            return;
        }

        // Debug.Log("Llegamos a donde la transform position cambia a savedEntryPosition de manera exitosa.");
    }

    public PlayerInventory GetPlayerInventory()
    {
        return playerInventory;
    }

    public void ActivatePlayersInScene()
    {
        Debug.Log("Activating Players in Scene.");
        foreach(Client client in clients)
        {
            Player player = client.activePlayer;
            if(player != null)
            {
                player.gameObject.SetActive(client.activePlayerSceneIndex == SceneManager.GetActiveScene().buildIndex);
            }
               
        }
    }

    public static string GetScene(int index)
    {
        return SceneUtility.GetScenePathByBuildIndex(index);
    }

    #if !UNITY_EDITOR
            static string myLog = "";
            private string output;
            private string stack;

            void OnEnable()
            {
                Application.logMessageReceivedThreaded += Log;
            }

            void OnDisable()
            {
                Application.logMessageReceivedThreaded -= Log;
            }

            public void Log(string logString, string stackTrace, UnityEngine.LogType type)
            {
                output = logString;
                stack = stackTrace;
                myLog = output + "\n" + stack + "\n" + myLog;
                if (myLog.Length > 5000)
                {
                myLog = myLog.Substring(0, 4000);
                }
            }
            void OnGUI()
            {
                //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
                {
                    myLog = GUI.TextArea(new Rect(30, 30, Screen.width / 2, Screen.height - 30), myLog);
                }
            }
    #endif
        





}