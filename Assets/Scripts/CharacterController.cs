using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

public class CharacterController : NetworkBehaviour, ISaveable // SAVELOAD
{
    public string ID => GetInstanceID().ToString(); // SAVELOAD
    public Type SaveType => typeof(CharacterControllerSaveData); // Implementación correcta de `SaveType`

    public float moveSpeed = 15f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    private Vector2 moveInput;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public Rigidbody2D rb;
    
    public GameObject mapObject;
    public Animator animator;
    public Transform body;
    public Player player;

    // 🔹 Variables para interpolación manual
    private Vector2 targetPosition;
    private Vector2 previousPosition;
    private float interpolationFactor = 20f; // Ajusta según pruebas

    // 🔹 Variables de guardado
    public Vector3 lastPosition;
    public bool hasLastPosition = false;

    private void Start()
    {
        SaveLoadSystem.Instance.RegisterSaveable(this);
    }

    public void Initiate()
    {        
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        mapObject = GameObject.FindGameObjectWithTag("Map");
    }

    private void Update()
    {   
        if (player == null)
        {
            player = GetComponent<Player>();
            if (player == null) return;
        }

        if (Object.HasInputAuthority)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            moveInput = new Vector2(horizontalInput, verticalInput);

            animator.SetBool("Run", moveInput != Vector2.zero);
        }
        else
        {
            // 🔥 Interpolamos suavemente la posición
            rb.position = Vector2.Lerp(rb.position, targetPosition, Time.deltaTime * interpolationFactor);
        }

        lastPosition = transform.position; // Guardamos la posición actual
    }

    private void FixedUpdate()
    {
        if (player == null || !player.HasStateAuthority)
            return;

        if (moveInput != Vector2.zero)
        {
            MovePlayer(moveInput);
            RPC_SetPosition(rb.position); // 🔥 Sincronizamos la nueva posición con los demás clientes
        }

        if (moveInput.x > 0)
        {
            body.localEulerAngles = Vector3.zero;
        }
        else if (moveInput.x < 0)
        {
            body.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void MovePlayer(Vector2 direction)
    {
        Vector2 moveVector = direction.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVector);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetPosition(Vector2 newPos)
    {
        targetPosition = newPos; // 🔥 Actualizamos la posición objetivo para interpolación
    }

    public void CharacterControllerVerification()
    {
        Debug.Log($"CharacterController Verification: \n" +
                  $"player: {player}");
        Debug.Log("CharacterController Verification: ✔️ Success");
    }

    // 🔹 Implementación de ISaveable
    public object SaveState() // SAVELOAD
    {
        var saveData = new CharacterControllerSaveData
        {
            moveSpeed = this.moveSpeed,
            lastPositionX = this.lastPosition.x,
            lastPositionY = this.lastPosition.y,
            lastPositionZ = this.lastPosition.z,
            mapSpriteName = this.mapObject.GetComponent<SpriteRenderer>().sprite.name // Guardar el nombre del sprite
        };
        
        Debug.Log($"CharacterController SaveState called. Data to be saved: \n" +
                $"moveSpeed: {saveData.moveSpeed}, \n" +
                $"mapSpriteName: {saveData.mapSpriteName}, \n" +
                $"lastPosition: ({saveData.lastPositionX}, {saveData.lastPositionY}, {saveData.lastPositionZ})");
        
        return saveData;
    }

    public void LoadState(object state) // SAVELOAD
    {
        if (state is CharacterControllerSaveData saveData)
        {
            this.moveSpeed = saveData.moveSpeed;
            this.lastPosition = new Vector3(saveData.lastPositionX, saveData.lastPositionY, saveData.lastPositionZ);
            this.hasLastPosition = true;

            // 🔥 Restauramos la posición del personaje
            if (this.rb != null)
            {
                this.rb.position = this.lastPosition;
            }
            else
            {
                this.transform.position = this.lastPosition;
            }

            // 🔥 Restauramos el SpriteRenderer del mapa
            if (this.mapObject != null)
            {
                var spriteRenderer = this.mapObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    var sprite = Resources.Load<Sprite>(saveData.mapSpriteName);
                    if (sprite != null)
                    {
                        spriteRenderer.sprite = sprite;
                    }
                    else
                    {
                        Debug.LogWarning($"Sprite not found for name: {saveData.mapSpriteName}");
                    }
                }
                else
                {
                    Debug.LogError("SpriteRenderer component not found on mapObject.");
                }
            }
            else
            {
                Debug.LogWarning("mapObject is not assigned.");
            }
        }
        else
        {
            Debug.LogError("Invalid load state for CharacterController.");
        }
    }

    [Serializable]
    public class CharacterControllerSaveData // SAVELOAD
    {
        public float moveSpeed;
        public float lastPositionX;
        public float lastPositionY;
        public float lastPositionZ;
        public string mapSpriteName;
    }
}