using UnityEngine;
using UnityEngine.UI;

public class BackgroundColorChanger : MonoBehaviour
{
    public Image backgroundImage; // La imagen del fondo
    public float duration = 9f; // La duración del cambio de color
    private float timer = 0f;

    private Color startColor = Color.black; // Color inicial
    private Color endColor = Color.white; // Color final

    private void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            backgroundImage.color = Color.Lerp(startColor, endColor, t);
        }
        else
        {
            backgroundImage.color = startColor; // Poner el color de fondo a negro inmediatamente
        }
    }
}
