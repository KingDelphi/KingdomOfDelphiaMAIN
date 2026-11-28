using UnityEngine;

/// <summary>
/// SOLO PARA LIMPIEZA (borrar después).
/// Mantén presionado Shift izquierdo y presiona F12 para borrar TODOS los PlayerPrefs.
/// </summary>
public class ClearPlayerPrefsTool : MonoBehaviour
{
    private float messageTimer;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F12))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            messageTimer = 5f;
            Debug.Log("[ClearPlayerPrefsTool] PlayerPrefs borrados.");
        }

        if (messageTimer > 0f) messageTimer -= Time.unscaledDeltaTime;
    }

    private void OnGUI()
    {
        if (messageTimer <= 0f) return;
        GUI.skin.label.fontSize = 24;
        GUI.Label(new Rect(10, 120, 900, 60), "PlayerPrefs borrados. Cierra el juego y vuelve a abrirlo.");
    }
}