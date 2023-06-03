using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Yotepo : MonoBehaviour
{
    public float followDistance = 7f;
    public float maxDistanceFromSpawn = 25f;
    public float movementSpeed = 2.5f;

    public GameObject disappearanceEffect; // Objeto de efecto de desaparición (debe contener una animación)

    private Transform playerTransform;
    private Vector3 spawnPoint;
    private bool isFollowingPlayer;
    private bool isMoving;

    public float stoppingDistance = 1.5f; // Distancia de detención antes de la colisión

    public string monsterName = "Yotepo";
    private TextMeshPro textMeshPro;

    private void Start()
    {
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spawnPoint = transform.position;
        isFollowingPlayer = false;
        isMoving = true;
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
            }
            else
            {
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                if (isMoving && distanceToPlayer > stoppingDistance)
                {
                    transform.position += direction * movementSpeed * Time.deltaTime;
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
}