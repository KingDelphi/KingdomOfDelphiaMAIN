using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 15f; // Velocidad de movimiento del personaje
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    private Vector2 moveInput;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private Rigidbody2D rb;
    //private Transform characterTransform;

    private float minX, maxX, minY, maxY; // Límites de movimiento del personaje

    public GameObject mapObject; // Referencia al objeto del mapa

    public Animator animator; // Referencia al componente Animator para el cuerpo del personaje

    public Vector3 lastPosition;
    public bool hasLastPosition = false;

    public GameObject circle;

    public Transform body;

    // private void Start()
    // {
    //     Initiate();
    // }

    public void Initiate()
    {
        rb = GetComponent<Rigidbody2D>();
        //characterTransform = transform;
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

        // GameManager.Instance.OnSceneLoaded();

        // Debug.Log("Map Bounds - Min X: " + mapLeft + ", Max X: " + mapRight + ", Min Y: " + mapBottom + ", Max Y: " + mapTop);
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveInput = new Vector2(horizontalInput, verticalInput);

        // Verificar si se presionan o se sueltan las teclas WASD o las flechas
        if (moveInput != Vector2.zero)
        {
            animator.SetInteger("State", 2); // Establecer el parámetro "Run" como verdadero para iniciar la transición a Run
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetInteger("State", 0); // Establecer el parámetro "Run" como falso para iniciar la transición a Stand
            animator.SetBool("Run", false);
        }

        // Verificar los límites de movimiento en la dirección horizontal
        if ((moveInput.x < 0 && transform.position.x <= minX) || (moveInput.x > 0 && transform.position.x >= maxX))
        {
            moveInput.x = 0; // Detener el movimiento horizontal
        }

        // Verificar los límites de movimiento en la dirección vertical
        if ((moveInput.y < 0 && transform.position.y <= minY) || (moveInput.y > 0 && transform.position.y >= maxY))
        {
            moveInput.y = 0; // Detener el movimiento vertical
        }

        lastPosition = transform.position;
        if (circle != null)
        {
            circle.transform.position = transform.position;
            circle.transform.LookAt(transform.position + new Vector3 (moveInput.x, moveInput.y, 0));
            if (circle.transform.localEulerAngles.y == 0)
            {
                Vector3 temp = circle.transform.localEulerAngles;
                temp.y = 90;
                circle.transform.localEulerAngles = temp;
            }
        }
    }

    private void FixedUpdate()
    {
        // Try to move player in input direction, followed by left right and up down input if failed
        MovePlayer(moveInput);

        if (moveInput.x > 0)
        {
            // transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Sin rotación
            body.localEulerAngles = Vector3.zero;
        }
        else if (moveInput.x < 0)
        {
            // transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Rotación en Y para voltear el personaje
            body.localEulerAngles = new Vector3(0, 180, 0);
        }
        
        // // Verificar los límites de movimiento en la dirección horizontal
        // if ((moveInput.x < 0 && transform.position.x <= minX) || (moveInput.x > 0 && transform.position.x >= maxX))
        // {
        //     moveInput.x = 0; // Detener el movimiento horizontal
        // }

        // // Verificar los límites de movimiento en la dirección vertical
        // if ((moveInput.y < 0 && transform.position.y <= minY) || (moveInput.y > 0 && transform.position.y >= maxY))
        // {
        //     moveInput.y = 0; // Detener el movimiento vertical
        // }

        // // Try to move player in input direction, followed by left right and up down input if failed
        // bool success = MovePlayer(moveInput);

        // if (moveInput.x > 0)
        // {
        //     transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Sin rotación
        // }
        // else if (moveInput.x < 0)
        // {
        //     transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Rotación en Y para voltear el personaje
        // }

        // if (!success)
        // {
        //     // Try Left / Right
        //     success = MovePlayer(new Vector2(moveInput.x, 0));

        //     if (!success)
        //     {
        //         success = MovePlayer(new Vector2(0, moveInput.y));
        //     }
        // }
    }

    // // Tries to move the player in a direction by casting in that direction by the amount
    // // moved plus an offset. If no collisions are found, it moves the players
    // // Returns true or false depending on if a move was executed
    // private bool MovePlayer(Vector2 direction)
    // {
    //     // Check for potential collisions
    //     int count = rb.Cast(
    //         direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
    //         movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
    //         castCollisions, // List of collisions to store the found collisions into after the Cast is finished
    //         moveSpeed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

    //     if (count == 0)
    //     {
    //         Vector2 moveVector = direction * moveSpeed * Time.fixedDeltaTime;

    //         // No collisions
    //         rb.MovePosition(rb.position + moveVector);
    //         return true;
    //     }
    //     else
    //     {
    //         // // Print collisions
    //         // foreach (RaycastHit2D hit in castCollisions)
    //         // {
    //         //     print(hit.ToString());
    //         // }

    //         return false;
    //     }
    // }



    private void MovePlayer(Vector2 direction)
    {
        Vector2 moveVector = direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVector);
    }
}
