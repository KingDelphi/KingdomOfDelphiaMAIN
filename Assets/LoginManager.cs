using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class LoginManager : MonoBehaviour
{
    public TMP_Dropdown serverDropdown;
    public TMP_InputField playerNameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text feedbackText;
    public NetworkDebugStart networkDebugStart;
    public GameObject loginManagerCanvas;
    

    private void Start()
    {
        // // Mostrar la pantalla de carga
        // if (LoadingScreenManager.Instance != null)
        // {
        //     LoadingScreenManager.Instance.ShowLoadingScreen();
        // }
        
        loginButton.onClick.AddListener(HandleLogin);
        // gameImage.SetActive(true);
    }

    public void HandleLogin()
    {
        loginManagerCanvas.SetActive(false);

        // Mostrar la pantalla de carga
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.ShowLoadingScreen();
        }
        
        string playerName = playerNameInput.text;
        string password = passwordInput.text;

        if (Authenticate(playerName, password))
        {
            // Almacenar el nombre del jugador y cargar la siguiente escena
            GameManager.Instance.SetPlayerName(playerName);

            // Llenar el campo "Room" con el nombre del servidor seleccionado
            string selectedServer = serverDropdown.options[serverDropdown.value].text;
            networkDebugStart.DefaultRoomName = selectedServer;

            // Iniciar el cliente compartido
            networkDebugStart.StartSharedClient();

            // Ocultar la interfaz de Photon Fusion y mostrar la imagen del juego
            // gameImage.SetActive(false);
        }
        else
        {
            feedbackText.text = "Invalid login credentials.";
        }
    }

    private bool Authenticate(string playerName, string password)
    {
        // Aquí se debe realizar la autenticación, por ejemplo, con un servidor o base de datos
        return !string.IsNullOrEmpty(playerName) && !string.IsNullOrEmpty(password); // Simplificado para este ejemplo
    }
}