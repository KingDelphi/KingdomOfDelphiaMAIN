using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections.Generic;
using Fusion;
using System.Linq;

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
    public List<Client> clients = new List<Client>();
    public List<Object> scenes;
    public Transform clientHolder;
    public Transform characterHolder;
    public CharacterController characterController;
    public Client client;

    private void Awake()
    {   
        if (Instance == null)
        {
            Instance = this;
            Initiate();
        }
    }

    public void SetClient(Client newClient)
    {
        Debug.Log("GameManager(62).SetClient(Client newClient) called. VARIABLES: newClient: " + newClient);
        client = newClient;
        Debug.Log("GameManager(64) received Client: " + client.gameObject.name);
    }

    public void PlayerInventoryInitialized(PlayerInventory inventory)
    {
        playerInventory = inventory;
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory component not found.");
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
        SetupSceneForPlayer(player);
        Debug.Log("Before RPC");
        player.RpcUpdatePlayers();
    }

    public void SetupSceneForPlayer(Player _player)
    {
        Debug.Log("GameManager(132).SetupSceneForPlayer(_player) called. _player: " + _player);
        Debug.Log("GameManager(133) Before assigning player = _player. VARIABLES: player: " + player + ", _player: " + _player);
        player = _player;
        Debug.Log("GameManager(135) After assigning player = _player. VARIABLES (should be same): player: " + player + ", _player: " + _player);
        
        if (player == null)
        {
            Debug.Log("Player is null.");
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
    }

    public PlayerInventory GetPlayerInventory()
    {
        return playerInventory;
    }

    public void ActivatePlayersInScene()
    {
        Debug.Log("GameManager(181).ActivatePlayersInScene() called.");
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

    public void GameManagerVerification()
    {
        Debug.Log($"GameManager Verification: \n" +
                  //$"_player: {_player}\n" + 
                  $"player: {player}\n" +
                  //$"activePlayer: {activePlayer}\n" +
                  $"SceneManager.GetActiveScene().buildIndex: {SceneManager.GetActiveScene().buildIndex}\n" +
                  $"client.activePlayerSceneIndex: {client.activePlayerSceneIndex}");
        Debug.Log("GameManager Verification: ✔️ Success");
    }

    public void MaximumVerification()
    {
        Debug.Log("GameManager(215).MaximumVerification() called.");
        // Encuentra y verifica el ClientSpawner correcto
        var clientSpawners = FindObjectsOfType<ClientSpawner>();
        ClientSpawner correctClientSpawner = null;
        foreach (var cs in clientSpawners)
        {
            var networkRunner = cs.GetComponent<NetworkRunner>();
            if (networkRunner != null)
            {
                correctClientSpawner = cs;
                Debug.Log($"GameManager(225) - ClientSpawner '{cs.gameObject.name}' found with NetworkRunner.");
                cs.ClientSpawnerVerification();
                break;
            }
            else
            {
                Debug.LogWarning($"GameManager(231) - ClientSpawner '{cs.gameObject.name}' does not have a NetworkRunner.");
            }
        }

        // Encuentra y verifica el Client correcto
        var clients = FindObjectsOfType<Client>();
        Client correctClient = null;
        foreach (var c in clients)
        {
            var networkObject = c.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogWarning($"GameManager(243) - Client '{c.gameObject.name}' does not have a NetworkObject component.");
            }
            else if (!networkObject.HasStateAuthority)
            {
                Debug.LogWarning($"GameManager(247) - Client '{c.gameObject.name}' does not have StateAuthority.");
            }
            else
            {
                correctClient = c;
                Debug.Log($"GameManager(252) - Client '{c.gameObject.name}' found with StateAuthority.");
                c.ClientVerification();

                var clientNetworkObjectId = networkObject.Id;
                Debug.Log($"GameManager(256) - Client NetworkObject.Id: {clientNetworkObjectId}");

                // Verifica el Player
                if (player != null)
                {
                    Debug.Log($"GameManager(261) - Player '{player.gameObject.name}' found with matching NetworkObject.Id: {clientNetworkObjectId}");
                    player.PlayerVerification();
                }
                else
                {
                    Debug.LogWarning($"GameManager(266) - Player with matching NetworkObject.Id {clientNetworkObjectId} not found. PlayerVerification() not done.");
                }

                // Verifica el CharacterController correcto
                if (characterController != null)
                {
                    Debug.Log($"GameManager(272) - CharacterController '{characterController.gameObject.name}' found with matching Player NetworkObject.Id: {clientNetworkObjectId}");
                    characterController.CharacterControllerVerification();
                }
                else
                {
                    Debug.LogWarning($"GameManager(277) - CharacterController with matching Player NetworkObject.Id {clientNetworkObjectId} not found. CharacterControllerVerification() not done.");
                }
                break;
            }
        }

        if (correctClient == null)
        {
            Debug.LogWarning("GameManager(285) - No Client with StateAuthority found.");
        }

        // Llama a la verificación del propio GameManager
        GameManagerVerification();
        AuthorityHelperVerification();
        Debug.LogWarning("✔️✔️✔️✔️✔️ GameManager(291).MaximumVerification() Finished.");
    }

    public void AuthorityHelperVerification()
    {
        Debug.Log("GameManager(296).AuthorityHelperVerification() called.");

        var allNetworkObjectsWithAuthority = AuthorityHelper.FindAllNetworkObjectsWithStateAuthority();
        foreach (var networkObject in allNetworkObjectsWithAuthority)
        {
            Debug.Log($"NetworkObject with StateAuthority: {networkObject.name}, Id: {networkObject.Id}");
        }

        Debug.Log("AuthorityHelper Verification: ✔️ Success");
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