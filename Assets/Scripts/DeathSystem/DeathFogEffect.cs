using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathFogEffect : MonoBehaviour
{
    [SerializeField] private Image fogImage;

    private void Start()
    {
        if (fogImage == null)
        {
            Debug.LogError("⚠️ No se asignó un Image en DeathFogEffect.");
        }
        else
        {
            fogImage.color = new Color(1, 1, 1, 0);
        }
    }

    public void StartFogEffect()
    {
        if (fogImage != null)
        {
            StartCoroutine(FadeFogSequence());
        }
    }

    private IEnumerator FadeFogSequence()
    {
        yield return FadeToBlack(6f);
        yield return new WaitForSeconds(2f);
        yield return FadeToWhite(2f);
    }

    private IEnumerator FadeToBlack(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            fogImage.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fogImage.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FadeToWhite(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1, 0.75f, elapsedTime / duration); // Ahora baja hasta 50% de opacidad
            fogImage.color = new Color(1, 1, 1, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fogImage.color = new Color(1, 1, 1, 0.75f); // Se queda en 50% de opacidad al final
    }
}
