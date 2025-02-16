using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BloodEffectController : MonoBehaviour
{
    public Image bloodImage;
    public float fadeDuration = 0.2f; // Ahora se desvanece en 0.2s
    public float maxAlpha = 0.7f; // Opacidad máxima

    private Coroutine fadeCoroutine;

    void Start()
    {
        if (bloodImage == null)
        {
            Debug.LogError("[BloodEffect] La imagen de sangre no está asignada.");
            return;
        }

        bloodImage.color = new Color(bloodImage.color.r, bloodImage.color.g, bloodImage.color.b, 0);
    }

    public void ShowBloodEffect()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeBloodEffect());
    }

    private IEnumerator FadeBloodEffect()
    {
        // Aparece la sangre
        float t = 0;
        while (t < 0.3f) // Se activa rápido
        {
            t += Time.deltaTime * 3;
            bloodImage.color = new Color(bloodImage.color.r, bloodImage.color.g, bloodImage.color.b, Mathf.Lerp(0, maxAlpha, t));
            yield return null;
        }

        // Espera 0.2 segundos antes de desvanecer
        yield return new WaitForSeconds(0.2f);
        
        // Se desvanece en 0.2 segundos
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            bloodImage.color = new Color(bloodImage.color.r, bloodImage.color.g, bloodImage.color.b, Mathf.Lerp(maxAlpha, 0, t / fadeDuration));
            yield return null;
        }
    }
}
