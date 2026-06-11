using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Referencia al transform del personaje a seguir
    public CameraLimits cameraLimits; // Referencia al script CameraLimits

    private void LateUpdate()
    {
        if (target != null)
        {
            // Obtener la posición actual de la cámara
            Vector3 currentPosition = transform.position;

            // Obtener la posición del personaje
            Vector3 targetPosition = target.position;

            // Limitar la posición del objetivo dentro de los límites de la cámara
            float clampedX = Mathf.Clamp(targetPosition.x, cameraLimits.minX, cameraLimits.maxX);
            float clampedY = Mathf.Clamp(targetPosition.y, cameraLimits.minY, cameraLimits.maxY);

            // Establecer la nueva posición de la cámara
            transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
        }
    }
}
