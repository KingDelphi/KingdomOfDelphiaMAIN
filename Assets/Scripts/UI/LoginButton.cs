using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;
using Fusion;


public class LoginButton : MonoBehaviour
{
    public GameObject selectedCharacterInfoCanvas; 

    private Unity.Cinemachine.CinemachineVirtualCamera virtualCamera;
    
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

    // 1) Jugador activo REAL (no la lista de plantillas)
    Player currentPlayer = gameManager.player;
    if (currentPlayer == null)
    {
        Debug.LogError("Current player not found.");
        return;
    }

    // 2) Client estable: primero desde el player; si falla, desde el campo del botón o búsqueda
    Client client = currentPlayer.GetClient();
    if (client == null)
        client = this.client; // el que asignaste en el Inspector del LoginButton
    if (client == null)
        client = FindObjectOfType<Client>(); // último recurso en single-client test

    if (client == null)
    {
        Debug.LogError("Client not found for the current player.");
        return;
    }

    // 3) Desactivar solo la instancia actual en juego
    Debug.Log("Deactivating active character: " + currentPlayer.gameObject.name);
    currentPlayer.gameObject.SetActive(false);

    // 4) Cambio de prefab / instancia
    client.ChangePlayerPrefab(characterName);
    Debug.LogWarning("LoginButton(73) After client.ChangePlayerPrefab(characterName);");

    string clientId = client.GetComponent<NetworkObject>().Id.ToString();
    string characterNameWithId = $"{characterName} {clientId}";

    client.PrintCharacterInstances();
    GameObject selectedCharacter = client.GetCharacterInstance(characterNameWithId);
    if (selectedCharacter == null)
    {
        Debug.LogError("Character instance not found: " + characterNameWithId);
        return;
    }

    selectedCharacter.SetActive(true);
    Debug.Log($"LoginButton(91) selectedCharacter: {selectedCharacter} activated: {selectedCharacter.activeSelf}");

    Player selectedPlayerComponent = selectedCharacter.GetComponent<Player>();
    if (selectedPlayerComponent == null)
    {
        Debug.LogError($"Player component not found on {selectedCharacter}");
        return;
    }

    gameManager.player = selectedPlayerComponent;

    client.playerPrefab = selectedCharacter;
    client.SetActivePlayer(selectedPlayerComponent);
    client.activePlayer = selectedPlayerComponent;

    selectedPlayerComponent.transform.SetParent(client.characterHolder, false);

    if (selectedPlayerComponent.inventory != null)
        gameManager.playerInventory = selectedPlayerComponent.inventory;
    else
        Debug.LogError("Selected player's inventory not found.");

    if (selectedCharacterInfoCanvas != null)
    {
        selectedCharacterInfoCanvas.SetActive(false);
        Debug.Log("SelectedCharacterInfoCanvas deactivated.");
    }
    else
        Debug.LogError("SelectedCharacterInfoCanvas not assigned.");

    foreach (DropdownBehaviour drop in dropdownList)
        drop.UpdateInventory();

    selectedPlayerComponent.RpcUpdatePlayers();

    Debug.LogWarning("✔️ Character Changed Successfully - MaximumVerification()");
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