using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text feedbackText;

    private void Start()
    {
        loginButton.onClick.AddListener(HandleLogin);
    }

    private void HandleLogin()
    {
        string playerName = playerNameInput.text;
        string password = passwordInput.text;

        if (Authenticate(playerName, password))
        {
            // Almacenar el nombre del jugador y cargar la siguiente escena
            GameManager.Instance.SetPlayerName(playerName);
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
