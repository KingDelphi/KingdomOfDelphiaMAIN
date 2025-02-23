using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions; // Importamos Regex para limpiar el nombre

public class PermaDeathButton : MonoBehaviour
{
    public Button permaDeathButton;

    private void Awake()
    {
        permaDeathButton.onClick.AddListener(() =>
        {
            string characterName = GetCharacterNameFromGameManager();
            if (!string.IsNullOrEmpty(characterName))
            {
                EnsureCharacterSelectionHandlerIsActive();
                CharacterSelectionHandler.Instance?.HandlePermaDeath(characterName);
                Debug.Log($"PermaDeath activado para: {characterName}");
            }
            else
            {
                Debug.LogWarning("No se pudo obtener el nombre del personaje desde GameManager.");
            }
        });
    }

    private string GetCharacterNameFromGameManager()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.player != null)
        {
            string fullName = gameManager.player.name;

            // Eliminamos la ID del nombre (ejemplo: "Tres Deseos [Id:525312]" → "Tres Deseos")
            string cleanName = Regex.Replace(fullName, @"\s*\[.*?\]", "");

            Debug.Log($"CORRECTO gameManager.player.name limpio: {cleanName}");
            return cleanName;
        }
        else
        {
            Debug.Log("INCORRECTO gameManager.player.name: No se encontró el GameManager o el Player.");
            return null;
        }
    }

    private void EnsureCharacterSelectionHandlerIsActive()
    {
        if (CharacterSelectionHandler.Instance == null)
        {
            // Buscar todos los CharacterSelectionHandler en la escena (incluye objetos inactivos)
            CharacterSelectionHandler handler = FindObjectOfType<CharacterSelectionHandler>(true);
            
            if (handler != null)
            {
                handler.gameObject.SetActive(true);
                Debug.Log("CharacterSelectionHandler activado.");
            }
            else
            {
                Debug.LogError("No se encontró CharacterSelectionHandler en la escena.");
            }
        }
    }
}
