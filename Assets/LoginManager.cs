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
    public GameObject loginManagerCanvas;

    private NetworkRunner networkRunner; // Se mantiene privado para evitar manipulación externa

    private void Start()
    {
        loginButton.onClick.AddListener(HandleLogin);
    }

    public void HandleLogin()
    {
        // loginManagerCanvas.SetActive(false);

        // Mostrar la pantalla de carga si está disponible
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.ShowLoadingScreen();
        }

        string playerName = playerNameInput.text;
        string password = passwordInput.text;

        if (Authenticate(playerName, password))
        {
            GameManager.Instance.SetPlayerName(playerName);

            string selectedServer = serverDropdown.options[serverDropdown.value].text;
            GameManager.Instance.SetServer(selectedServer);

            // Iniciar la conexión a la red
            StartGame(selectedServer);
        }
        else
        {
            feedbackText.text = "Invalid login credentials.";
        }
    }

    private async void StartGame(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("⚠️ Room name is empty! Cannot start game.");
            return;
        }

        // Buscar si ya hay un NetworkRunner en la escena
        networkRunner = FindObjectOfType<NetworkRunner>();
        if (networkRunner == null)
        {
            networkRunner = gameObject.AddComponent<NetworkRunner>();
        }

        // Buscar si ya hay un NetworkSceneManagerDefault en la escena
        NetworkSceneManagerDefault sceneManager = FindObjectOfType<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        Debug.Log($"📡 Connecting to room: {roomName} as Shared Client...");

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,  // ✅ Equivalente a "Start Shared Client"
            SessionName = roomName,
            SceneManager = sceneManager,
        });

        if (result.Ok)
        {
            Debug.Log($"✅ Successfully connected to room: {roomName}");
        }
        else
        {
            Debug.LogError($"❌ Failed to start game: {result.ErrorMessage}");
        }
    }

    private bool Authenticate(string playerName, string password)
    {
        return !string.IsNullOrEmpty(playerName) && !string.IsNullOrEmpty(password);
    }
}
