using UnityEngine;

public class CameraLimitsTrigger : MonoBehaviour
{
    public CameraFollow cameraFollow; // Referencia al script CameraFollow

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CameraLimits"))
        {
            // Si la cámara alcanza los límites del escenario, dejar de seguir al personaje
            cameraFollow.target = null;
        }
    }
}
