using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

public class CharacterController : NetworkBehaviour, ISaveable // SAVELOAD
{
    public string ID =>  GetEntityId().ToString(); // SAVELOAD
    public Type SaveType => typeof(CharacterControllerSaveData); // Implementación correcta de `SaveType`

    public float moveSpeed = 15f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    [Header("Suavizado de movimiento (en segundos)")]
    [Tooltip("Tiempo en llegar a la velocidad máxima. Menor = más seco, mayor = más suave.")]
    [Range(0.01f, 0.5f)] public float accelerationTime = 0.1f;
    [Tooltip("Tiempo en detenerse al soltar las teclas.")]
    [Range(0.01f, 0.5f)] public float decelerationTime = 0.08f;

    private Vector2 moveInput;
    private Vector2 currentVelocity; // velocidad suavizada actual
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public Rigidbody2D rb;
    
    public GameObject mapObject;
    public Animator animator;
    public Transform body;
    public Player player;

    // 🔹 Variables para interpolación manual (solo se usan si NO hay NetworkTransform)
    private Vector2 targetPosition;
    private Vector2 previousPosition;
    private float interpolationFactor = 20f; // Ajusta según pruebas
    private NetworkTransform netTransform;

    // 🔹 Variables de guardado
    public Vector3 lastPosition;
    public bool hasLastPosition = false;

    private void Start()
    {
        SaveLoadSystem.Instance.RegisterSaveable(this);
        ConfigureRigidbody();
    }

    public void Initiate()
    {        
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        mapObject = GameObject.FindGameObjectWithTag("Map");
        ConfigureRigidbody();
    }

    // 🔹 Interpolación del Rigidbody para evitar tirones visuales
    private void ConfigureRigidbody()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        targetPosition = rb.position; // evita que el valor inicial sea (0,0)

        if (netTransform == null) netTransform = GetComponent<NetworkTransform>();
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
            // GetAxisRaw: respuesta inmediata; el suavizado se hace en FixedUpdate
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            // ClampMagnitude(1): la diagonal nunca es más rápida que una dirección simple
            moveInput = Vector2.ClampMagnitude(new Vector2(horizontalInput, verticalInput), 1f);

            animator.SetBool("Run", moveInput != Vector2.zero);
        }
        else if (netTransform == null)
        {
            // 🔥 Solo si no hay NetworkTransform: interpolamos a mano la posición
            rb.position = Vector2.Lerp(rb.position, targetPosition, Time.deltaTime * interpolationFactor);
        }
        // Con NetworkTransform, él se encarga de la posición de los jugadores remotos

        lastPosition = transform.position; // Guardamos la posición actual
    }

    private void FixedUpdate()
    {
        if (player == null || !player.HasStateAuthority)
            return;

        // 🔹 Velocidad objetivo (módulo máximo = moveSpeed en cualquier dirección)
        Vector2 targetVelocity = moveInput * moveSpeed;

        // 🔹 La aceleración se calcula a partir del tiempo: funciona igual con cualquier moveSpeed
        bool accelerating = moveInput.sqrMagnitude > 0.0001f;
        float time = accelerating ? accelerationTime : decelerationTime;
        float rate = moveSpeed / Mathf.Max(time, 0.001f);

        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);

        if (currentVelocity.sqrMagnitude > 0.0001f)
        {
            MovePlayer(currentVelocity);

            // 🔥 Solo se manda el RPC si no hay NetworkTransform sincronizando la posición
            if (netTransform == null)
            {
                RPC_SetPosition(rb.position);
            }
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

    // 🔹 Recibe la velocidad ya suavizada (no una dirección)
    private void MovePlayer(Vector2 velocity)
    {
        Vector2 moveVector = velocity * Time.fixedDeltaTime;
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