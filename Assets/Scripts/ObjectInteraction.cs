using UnityEngine;
using System.Collections.Generic;

public class ObjectInteraction : MonoBehaviour
{
    private bool isTouchingPlayer = false;
    private bool isClicked = false;
    private bool isClickable = true;

    private void OnMouseDown()
    {
        if (isTouchingPlayer && Input.GetMouseButton(0) && isClickable)
        {
            isClicked = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }

    private void Update()
    {
        if (isClicked)
        {
            // Realizar acciones de loot aquí
            GetLoot();
            isClicked = false;
        }
    }

    private void GetLoot()
    {
        Debug.Log("Metodo GetLoot() llamado exitosamente.");
        Collider2D[] colliders = GetCollidersInTouch();

        foreach (Collider2D collider in colliders)
        {
            LootSystem lootSystem = collider.GetComponent<LootSystem>();
            if (lootSystem != null)
            {
                lootSystem.OnMonsterDeath(); // Llamar al método OnMonsterDeath del LootSystem
                isClickable = false;
                return;
            }
        }
        Debug.Log("Obtén loot del objeto");
    }

    private Collider2D[] GetCollidersInTouch()
    {
        // Obtener todos los colliders del personaje
        Collider2D[] playerColliders = GameObject.FindGameObjectWithTag("Player").GetComponents<Collider2D>();

        // Obtener todos los colliders del objeto
        Collider2D[] objectColliders = GetComponents<Collider2D>();

        // Lista para almacenar los colliders que están en contacto
        List<Collider2D> collidersInTouch = new List<Collider2D>();

        // Verificar si hay contacto entre los colliders
        foreach (Collider2D playerCollider in playerColliders)
        {
            foreach (Collider2D objectCollider in objectColliders)
            {
                if (playerCollider.IsTouching(objectCollider))
                {
                    collidersInTouch.Add(objectCollider);
                }
            }
        }

        return collidersInTouch.ToArray();
    }
}