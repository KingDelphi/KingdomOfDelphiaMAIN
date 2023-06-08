using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlianzaDelphi : MonoBehaviour
{
    public float followDistance = 4f;
    public float maxDistanceFromSpawn = 24f;
    public float movementSpeed = 5f;

    public GameObject disappearanceEffect; // Objeto de efecto de desaparición (debe contener una animación)

    private Transform playerTransform;
    private Vector3 spawnPoint;
    private bool isFollowingPlayer;
    private bool isMoving;

    public float stoppingDistance = 1.5f; // Distancia de detención antes de la colisión

    public string monsterName = "Alianza Delphi";
    private TextMeshPro textMeshPro;
    private Animator animator;

    private Transform characterTransform;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spawnPoint = transform.position;
        isFollowingPlayer = false;
        isMoving = true;
        characterTransform = transform;
    }

    private void Update()
    {
        // Actualiza el nombre del monstruo en el TextMeshPro
        textMeshPro.text = monsterName;
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float distanceFromSpawn = Vector3.Distance(transform.position, spawnPoint);

        if (isFollowingPlayer)
        {
            if (distanceToPlayer > followDistance)
            {
                isFollowingPlayer = false;
                animator.SetBool("Run", false);
            }
            else
            {
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                if (isMoving && distanceToPlayer > stoppingDistance)
                {
                    transform.position += direction * movementSpeed * Time.deltaTime;
                    animator.SetBool("Run", true);

                    if (direction.x > 0)
                    {
                        characterTransform.rotation = Quaternion.Euler(0f, 0f, 0f); // Sin rotación
                    }
                    else if (direction.x < 0)
                    {
                        characterTransform.rotation = Quaternion.Euler(0f, 180f, 0f); // Rotación en Y para voltear el personaje
                    }
                }
            }
        }
        else if (distanceToPlayer < followDistance)
        {
            isFollowingPlayer = true;
        }

        if (distanceFromSpawn > maxDistanceFromSpawn)
        {
            PlayDisappearanceEffect();
            transform.position = spawnPoint;
            animator.SetBool("Run", false);
        }
    }

    private void PlayDisappearanceEffect()
    {
        if (disappearanceEffect != null)
        {
            Instantiate(disappearanceEffect, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isMoving = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isMoving = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Vector2 avoidanceDirection = (transform.position - collision.transform.position).normalized;
            Vector3 newPosition = transform.position + (Vector3)avoidanceDirection * movementSpeed * Time.deltaTime;
            if (!Physics2D.OverlapCircle(newPosition, 0.5f, LayerMask.GetMask("Monsters")))
            {
                transform.position = newPosition;
            }
        }
    }

    private void OnEnable()
    {
        MonsterAttack.OnAttack += PlayAnimation;
    }

    private void OnDisable()
    {
        MonsterAttack.OnAttack -= PlayAnimation;
    }

    private void PlayAnimation()
    {
        animator.Play("SlashMelee1H", 0);
    }
}