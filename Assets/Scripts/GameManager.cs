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

    public GameObject circle;

    private CinemachineVirtualCamera virtualCamera;
    private PolygonCollider2D confinerCollider;
    // public GameObject character;

    public Player player;
    private List<GearItem> savedPlayerInventory;
    private int savedPlayerHealth;
    private Vector3 savedEntryPosition;

    public GameObject UI;

    public List<GameObject> characters; // Agrega los personajes en el Inspector

    public List<Client> clients = new List<Client>();

    public List<Object> scenes;

    public Transform clientHolder;
    public Transform characterHolder;

    private void Awake()
    {   
        if (Instance == null)
        {
            Instance = this;
            Initiate();
        }
        // else
        // {
        //     Destroy(gameObject);
        //     return;
        // }
    }

    private void Start()
    {
        //circle = GameObject.FindGameObjectWithTag("Circle");
        //character = GameObject.FindGameObjectWithTag("Player");
        //player = character.GetComponent<Player>();
        
    }

    private void Initiate()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UI);
        DontDestroyOnLoad(circle);

        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory component not found.");
        }

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
        //DontDestroyOnLoad(player.gameObject);
        // DontDestroyOnLoad(circle);
        // DontDestroyOnLoad(UI);
        
        // Guardar los datos del inventario y la vida del personaje
        //savedPlayerInventory = new List<GearItem>(playerInventory.inventory);
        //savedPlayerHealth = player.currentHealth;
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

    // private void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // private void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    public void SetupSceneForPlayer(Player _player)
    {
        player = _player;
        //circle = GameObject.FindGameObjectWithTag("Circle");
        //playerInventory = FindObjectOfType<PlayerInventory>();
        //UI = GameObject.FindGameObjectWithTag("UI");
        // player.GetComponent<CharacterController>().Initiate();
        
        if (player == null /*|| playerInventory == null*/)
        {
            Debug.Log("Player or PlayerInventory not found in the scene.");
            return;
        }

        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.hasLastPosition = false;
        characterController.circle = circle;
        circle.transform.SetParent(player.transform);
        circle.transform.localPosition = Vector3.zero;
        characterController.Initiate();

        // Restaurar los datos del inventario y la vida del personaje en la nueva escena
        player.currentHealth = savedPlayerHealth; // Restaurar la vida
        //playerInventory.inventory = savedPlayerInventory;

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
            circle.transform.position = savedEntryPosition;
        }
        //player.transform.position = savedEntryPosition;
        // circle.transform.position = savedEntryPosition;

        virtualCamera.Follow = player.transform;
        
        GameObject confinerObject = GameObject.Find("Map");
        if (confinerObject == null)
        {
            Debug.Log("Object with Confiner not found.");
            //return;
        }

        confinerCollider = confinerObject.GetComponent<PolygonCollider2D>();
        if (confinerCollider == null)
        {
            Debug.Log("PolygonCollider2D not found on the object with Confiner.");
            //return;
        }

        // virtualCamera.m_Lens.OrthographicSize = confinerCollider.bounds.extents.y;

        if (player == null || circle == null)
        {
            Debug.Log("Character or Circle not found in the scene.");
            return;
        }

        Debug.Log("Llegamos a donde la transform position cambia a savedEntryPosition de manera exitosa.");
        //character.transform.position = savedEntryPosition;
        //circle.transform.position = savedEntryPosition;

        //virtualCamera.Follow = character.transform;
        //virtualCamera.m_Lens.OrthographicSize = confinerCollider.bounds.extents.y;
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

    // [Rpc(RpcSources.All, RpcTargets.All)]
    // public void RpcUpdatePlayers()
    // {
    //     Debug.Log("RpcUpdatePlayers");
    //     ActivatePlayersInScene();
    // }

    public static string GetScene(int index)
    {
        // Debug.Log("Getting Scene...");
        // foreach(Object scene in GameManager.Instance.scenes)
        // {
        //     Debug.Log("Scene Name = " + scene.name);
        // }
        // Debug.Log("Afterprint");
        return SceneUtility.GetScenePathByBuildIndex(index);
        // return GameManager.Instance.scenes[index].name;
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