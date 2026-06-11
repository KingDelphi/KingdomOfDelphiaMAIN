using UnityEngine;

public class CursorController : MonoBehaviour
{
    public float followSpeed = 45f; // Velocidad de seguimiento del círculo

    private Vector3 lastPlayerPosition; // Última posición del personaje

    private void Start()
    {
        lastPlayerPosition = GetPlayerPosition();
    }

    private void Update()
    {
        // Calcular la dirección desde el círculo hacia el personaje
        Vector3 direction = GetPlayerPosition() - transform.position;

/*
        // Verificar si el personaje se ha movido desde el último frame
        if (GetPlayerPosition() != lastPlayerPosition)
        {
            
        }
*/
        // Mover el objeto del círculo hacia la posición del personaje de forma suave
        transform.position = Vector3.Lerp(transform.position, GetPlayerPosition(), followSpeed * Time.deltaTime);

        // Calcular el ángulo de rotación en grados
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplicar la rotación al objeto de la flecha
        transform.GetChild(1).rotation = Quaternion.Euler(0f, 0f, angle);

        // Actualizar la última posición del personaje
        lastPlayerPosition = GetPlayerPosition();
    }

    private Vector3 GetPlayerPosition()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        return playerObject != null ? playerObject.transform.position : Vector3.zero;
    }
}