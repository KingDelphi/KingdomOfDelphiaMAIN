using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.SceneManagement;
using Fusion;


public class LoginButton : MonoBehaviour
{
    public GameObject selectedCharacterInfoCanvas; 

    private CinemachineVirtualCamera virtualCamera;
    
    public GameManager gameManager;

    public List<DropdownBehaviour> dropdownList;

    public Client client;

    public void OnClick()
    {
        Debug.Log("LoginButton(24).OnClick() called.");

        GameObject characterNameTextObject = GameObject.Find("UI/Canvas/SelectedCharacterInfo/InfoPanel/CharacterNameText");
        if (characterNameTextObject == null)
        {
            Debug.LogError("CharacterNameText object not found.");
            return;
        }

        TextMeshProUGUI characterNameText = characterNameTextObject.GetComponent<TextMeshProUGUI>();
        if (characterNameText == null)
        {
            Debug.LogError("CharacterNameText component not found.");
            return;
        }

        string characterName = characterNameText.text.Replace("Name: ", "");
        Debug.Log("LoginButton(41) Nombre del personaje: " + characterName);

        GameManager gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found.");
            return;
        }

        GameObject activeCharacter = FindActiveCharacter();
        if (activeCharacter != null)
        {
            Debug.Log("Deactivating active character: " + activeCharacter.name);
            activeCharacter.SetActive(false);
        }

        Player currentPlayer = gameManager.player;
        if (currentPlayer == null)
        {
            Debug.LogError("Current player not found.");
            return;
        }

        Client client = currentPlayer.GetClient();
        if (client == null)
        {
            Debug.LogError("Client not found for the current player.");
            return;
        }

        // Cambia el prefab del jugador
        client.ChangePlayerPrefab(characterName);
        Debug.LogWarning("LoginButton(73) ESPARRAGO SALTARIN - After client.ChangePlayerPrefab(characterName);");

        // Obtén la ID del cliente actual
        string clientId = client.GetComponent<NetworkObject>().Id.ToString();

        // Construye el nombre con el formato correcto
        string characterNameWithId = $"{characterName} {clientId}";

        // Obtén la instancia del personaje seleccionado
        client.PrintCharacterInstances();
        GameObject selectedCharacter = client.GetCharacterInstance(characterNameWithId);
        if (selectedCharacter == null)
        {
            Debug.LogError("Character instance not found: " + characterNameWithId);
            return;
        }

        selectedCharacter.SetActive(true);
        Debug.Log($"LoginButton(91) (Character) selectedCharacter: {selectedCharacter} activated: {selectedCharacter.activeSelf}");

        Debug.Log("ESPARRAGO RADIOACTIVO - LoginButton(93) Before calling selectedCharacter.GetComponent<Player>().RpcUpdatePlayers()");
        selectedCharacter.GetComponent<Player>().RpcUpdatePlayers();

        Player selectedPlayerComponent = selectedCharacter.GetComponent<Player>();
        if (selectedPlayerComponent == null)
        {
            Debug.LogError($"Player component not found on {selectedCharacter}");
            return;
        }

        gameManager.player = selectedPlayerComponent;

        // Actualiza el cliente
        client.playerPrefab = selectedCharacter;
        client.SetActivePlayer(gameManager.player);
        client.activePlayer = selectedPlayerComponent;

        // Establece el jugador como hijo del characterHolder
        selectedPlayerComponent.transform.SetParent(client.characterHolder, false);

        if (selectedPlayerComponent.inventory != null)
        {
            gameManager.playerInventory = selectedPlayerComponent.inventory;
        }
        else
        {
            Debug.LogError("Selected player's inventory not found.");
        }

        if (selectedCharacterInfoCanvas != null)
        {
            selectedCharacterInfoCanvas.SetActive(false);
            Debug.Log("SelectedCharacterInfoCanvas deactivated.");
        }
        else
        {
            Debug.LogError("SelectedCharacterInfoCanvas not assigned.");
        }

        foreach (DropdownBehaviour drop in dropdownList)
        {
            drop.UpdateInventory();
        }

        Debug.LogWarning("✔️✔️✔️✔️✔️ LoginButton(137) - Character Changed Successfully - Running gameManager.MaximumVerification().");
        gameManager.MaximumVerification();
    }


    private GameObject FindActiveCharacter()
    {
        foreach (GameObject characterObject in GameManager.Instance.characters)
        {
            if (characterObject != null)
            {
                if (characterObject.activeSelf)
                {
                    return characterObject;
                }
            }
        }
        return null;
    }

    private GameObject FindCharacterByName(string characterName, bool includeInactive = false)
    {
        foreach (GameObject characterObject in GameManager.Instance.characters)
        {
            if (characterObject != null)
            {
                if (characterObject.name == characterName && (includeInactive || characterObject.activeSelf))
                {
                    return characterObject;
                }
            }
        }
        return null;
    }
}