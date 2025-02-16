using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DeathUIManager : MonoBehaviour
{
    public static DeathUIManager Instance { get; private set; }

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private TextMeshProUGUI deathMessage;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform resurrectionOptionsParent;
    [SerializeField] private GameObject resurrectionOptionPrefab;

    private CanvasGroup canvasGroup;
    private Action<DeathSystem.ResurrectionType> onResurrectSelected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Buscar CanvasGroup en deathScreen para manejar opacidad
        canvasGroup = deathScreen.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = deathScreen.AddComponent<CanvasGroup>();
        }

        deathScreen.SetActive(false);
        canvasGroup.alpha = 0f; // Comienza invisible

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideDeathScreen);
        }
    }

    public void ShowDeathScreen(string message, List<DeathSystem.ResurrectionType> options, Action<DeathSystem.ResurrectionType> onResurrect)
    {
        if (deathScreen == null) return;

        Debug.Log("Mostrando la pantalla de muerte...");
        deathScreen.SetActive(true);
        canvasGroup.alpha = 0f; // Reinicia opacidad

        onResurrectSelected = onResurrect;

        // 🔥 Limpiar opciones previas
        foreach (Transform child in resurrectionOptionsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var option in options)
        {
            GameObject buttonObj = Instantiate(resurrectionOptionPrefab, resurrectionOptionsParent);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = option.ToString();
            button.onClick.AddListener(() => SelectResurrection(option));
        }

        // 🔥 Iniciar animación del mensaje de muerte
        StartCoroutine(AnimateDeathMessage(message));
    }

    private IEnumerator AnimateDeathMessage(string message)
    {
        deathMessage.text = message;
        deathMessage.transform.localScale = Vector3.zero;

        float fadeDuration = 1.5f;
        float scaleDuration = 1.5f;
        float elapsedTime = 0f;

        // 🔥 Fade-in y escalado simultáneamente
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            deathMessage.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        deathMessage.transform.localScale = Vector3.one;
    }

    public void HideDeathScreen()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    private void SelectResurrection(DeathSystem.ResurrectionType type)
    {
        onResurrectSelected?.Invoke(type);
        HideDeathScreen();
    }
}
