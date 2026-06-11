using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;



public class DeathAnimationController : MonoBehaviour
{
    private Animator animator;
    private bool isDead = false; // Evitar múltiples activaciones
    private CinemachineZoomController camController;

    [Header("🎭 UI Death Panel")]
    public DeathUIManager deathUIManager;
    [SerializeField] private TextMeshProUGUI deathText; // 🔥 Compatible con TextMeshPro

    [Header("🔊 Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clashSound;
    [SerializeField] private AudioClip fadeSound;
    [SerializeField] private AudioClip ghostWhisper;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject.");
        }

        camController = FindObjectOfType<CinemachineZoomController>();
        if (camController == null)
        {
            Debug.LogError("CinemachineZoomController no encontrado en la escena.");
        }

    
        deathUIManager = FindObjectOfType<DeathUIManager>();
        if (deathUIManager == null)
        {
            Debug.LogError("❌ DeathUIManager no encontrado en la escena.");
        }

        // Encuentra el TextMeshPro dentro del DeathUIManager
        if (deathUIManager != null)
        {
            deathText = deathUIManager.GetComponentInChildren<TextMeshProUGUI>();
            if (deathText == null)
            {
                Debug.LogError("❌ No se encontró TextMeshProUGUI en DeathUIManager.");
            }
        }
    }

    public void PlayDeathAnimation()
    {
        if (animator == null || isDead) return; // Evita que se active más de una vez
        isDead = true;

        Debug.Log("[INICIO] Activando DeathBack...");

        // 🎵 Reproducir sonido de impacto antes de empezar
        if (audioSource != null && clashSound != null)
        {
            audioSource.PlayOneShot(clashSound);
        }

        // 📸 Zoom rápido a la muerte
        if (camController != null)
        {
            camController.ZoomToTarget(transform);
        }

        // ❄️ Pausar antes de iniciar la animación
        StartCoroutine(WaitBeforeDeathAnimation());
    }

    private IEnumerator WaitBeforeDeathAnimation()
    {
        yield return new WaitForSeconds(0.5f); // 🕒 Breve pausa antes de animación

        // 🔥 Activar animación de DeathBack
        animator.SetInteger("State", 6);

        //🕒 Esperar y forzar transición a DeathFront con la duración correcta
        StartCoroutine(TransitionToDeathFront());
    }

    private IEnumerator TransitionToDeathFront()
{
    if (animator == null) yield break;

    yield return new WaitForSeconds(2f); // ⏳ Esperar la animación DeathBack

    Debug.Log("[TRANSICIÓN] Pasando a DeathFront...");
    animator.SetInteger("State", 7);

    audioSource.PlayOneShot(ghostWhisper);

    yield return new WaitForSeconds(1.5f); // ⏳ Esperar antes de mostrar UI

    audioSource.PlayOneShot(fadeSound);

    Debug.Log("[UI] Mostrando la pantalla de muerte...");

    if (DeathUIManager.Instance != null)
    {
        List<DeathSystem.ResurrectionType> resurrectionOptions = new List<DeathSystem.ResurrectionType>
        {
            DeathSystem.ResurrectionType.Scroll,
            DeathSystem.ResurrectionType.SoulStone,
            DeathSystem.ResurrectionType.PhoenixEmblem
        };

        // 🔍 Buscar el DeathSystem del personaje
        DeathSystem deathSystem = GetComponentInParent<DeathSystem>();

        if (deathSystem != null)
        {
            DeathUIManager.Instance.ShowDeathScreen(
                "You are Dead...",
                resurrectionOptions,
                deathSystem.OnResurrectSelected // ✅ Correcto
            );
        }
        else
        {
            Debug.LogError("❌ No se encontró DeathSystem en el personaje.");
        }
    }
    else
    {
        Debug.LogError("❌ DeathUIManager.Instance es NULL. La UI no se activará.");
    }
}


    private IEnumerator ScaleDeathText()
    {
        if (deathText == null) yield break;

        float duration = 1.5f; // ⏳ Duración de la animación
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        while (elapsedTime < duration)
        {
            deathText.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        deathText.transform.localScale = endScale; // Asegurar tamaño final
    }

    
}
