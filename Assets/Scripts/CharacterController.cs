using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour, ISaveable //SAVELOAD
{
    public string ID => GetInstanceID().ToString(); //SAVELOAD
    public Type SaveType => typeof(CharacterControllerSaveData); // Implementación correcta de `SaveType`

    public float moveSpeed = 15f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    private Vector2 moveInput;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private Rigidbody2D rb;

    private float minX, maxX, minY, maxY;

    public GameObject mapObject;

    public Animator animator;

    public Vector3 lastPosition;
    public bool hasLastPosition = false;
    public Transform body;

    public Player player;

    public void Initiate()
    {
        player = GetComponent<Player>();
        
        rb = GetComponent<Rigidbody2D>();
        if (hasLastPosition)
        {
            transform.position = lastPosition;
        }
        else
        {
            lastPosition = transform.position;
            hasLastPosition = true;
        }

        mapObject = GameObject.FindGameObjectWithTag("Map");

        SpriteRenderer mapRenderer = mapObject.GetComponent<SpriteRenderer>();

        float mapLeft = mapRenderer.bounds.min.x;
        float mapRight = mapRenderer.bounds.max.x;
        float mapBottom = mapRenderer.bounds.min.y;
        float mapTop = mapRenderer.bounds.max.y;

        minX = mapLeft;
        maxX = mapRight;
        minY = mapBottom;
        maxY = mapTop;
    }

    private void Update()
    {
        if (!player.HasStateAuthority) return;
        
        if (player == null)
        {
            player = GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("Player component not found on CharacterController.");
                return;
            }
        }

        if (!player.HasStateAuthority)
        {
            Debug.LogError("CharacterController.Update() called. player: " + player + " does not have StateAuthority.");
            return;
        }
        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveInput = new Vector2(horizontalInput, verticalInput);

        if (moveInput != Vector2.zero)
        {
            animator.SetInteger("State", 2);
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetInteger("State", 0);
            animator.SetBool("Run", false);
        }

        if ((moveInput.x < 0 && transform.position.x <= minX) || (moveInput.x > 0 && transform.position.x >= maxX))
        {
            moveInput.x = 0;
        }

        if ((moveInput.y < 0 && transform.position.y <= minY) || (moveInput.y > 0 && transform.position.y >= maxY))
        {
            moveInput.y = 0;
        }
        
        lastPosition = transform.position;
        
    }

    private void FixedUpdate()
    {
        if (player == null || !player.HasStateAuthority)
            return;

        if (moveInput != Vector2.zero)
            MovePlayer(moveInput);

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

    // Comienza Implementación de ISaveable en el script CharacterController

    // Implementación de ISaveable
    public object SaveState() //SAVELOAD
    {
        return new CharacterControllerSaveData
        {
            moveSpeed = this.moveSpeed,
            mapObjectName = this.mapObject != null ? this.mapObject.name : "",
            lastPositionX = this.lastPosition.x,
            lastPositionY = this.lastPosition.y,
            lastPositionZ = this.lastPosition.z
        };
    }

    public void LoadState(object state) //SAVELOAD
    {
        if (state is CharacterControllerSaveData saveData)
        {
            this.moveSpeed = saveData.moveSpeed;
        
            if (!string.IsNullOrEmpty(saveData.mapObjectName))
            {
                this.mapObject = GameObject.Find(saveData.mapObjectName);
            }

            this.lastPosition = new Vector3(saveData.lastPositionX, saveData.lastPositionY, saveData.lastPositionZ);
            this.hasLastPosition = true;

            // Actualiza la posición del personaje
            if (this.rb != null)
            {
                this.rb.position = this.lastPosition;
            }
            else
            {
                this.transform.position = this.lastPosition;
            }
        }
        else
        {
            Debug.LogError("Invalid load state for CharacterController.");
        }
    }

    [Serializable]
    private class CharacterControllerSaveData //SAVELOAD
    {
        public float moveSpeed;
        public string mapObjectName;
        public float lastPositionX;
        public float lastPositionY;
        public float lastPositionZ;
    }
        // Termina Implementación de ISaveable en el script Player

}