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


    public static GameManager Instance { get; private set; }
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

    public List<Client> client = new List<Client>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(UI);
            DontDestroyOnLoad(circle);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory component not found.");
        }
    }

    private void Start()
    {
        //circle = GameObject.FindGameObjectWithTag("Circle");
        //character = GameObject.FindGameObjectWithTag("Player");
        //player = character.GetComponent<Player>();
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

            if(inventoryCanvas.active) {
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
            

    public void ChangeScene(string targetSceneName, Vector3 entryPosition)
    {
        //DontDestroyOnLoad(player.gameObject);
        // DontDestroyOnLoad(circle);
        // DontDestroyOnLoad(UI);
        
        // Guardar los datos del inventario y la vida del personaje
        //savedPlayerInventory = new List<GearItem>(playerInventory.inventory);
        //savedPlayerHealth = player.currentHealth;
        savedEntryPosition = entryPosition;
        SceneManager.LoadScene(targetSceneName);
        
    }

    public void ChangeScene(string targetSceneName)
    {
        ChangeScene(targetSceneName, Vector3.zero);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupSceneForPlayer(player);
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
        
        player.GetComponent<CharacterController>().Initiate();

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
}