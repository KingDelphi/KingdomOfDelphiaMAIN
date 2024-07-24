// using System.Linq;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using Cinemachine;
// using UnityEngine.SceneManagement;
// using Fusion;


// public class LoginButton : MonoBehaviour
// {
//     //public List<GameObject> characters; // Agrega los personajes en el Inspector
//     public GameObject selectedCharacterInfoCanvas; // Referencia al canvas "SelectedCharacterInfo"

//     private CinemachineVirtualCamera virtualCamera;
    
//     public GameManager gameManager;

//     public List<DropdownBehaviour> dropdownList;

//     public void OnClick()
//     {
//         GameObject characterNameTextObject = GameObject.Find("UI/Canvas/SelectedCharacterInfo/InfoPanel/CharacterNameText");

//         if (characterNameTextObject != null)
//         {
//             TextMeshProUGUI characterNameText = characterNameTextObject.GetComponent<TextMeshProUGUI>();

//             if (characterNameText != null)
//             {
//                 string characterName = characterNameText.text.Replace("Name: ", "");
//                 Debug.Log("Nombre del personaje: " + characterName);

//                 GameObject activeCharacter = FindActiveCharacter();
//                 if (activeCharacter != null)
//                 {
//                     Debug.Log("Deactivating active character: " + activeCharacter.name);
//                     activeCharacter.SetActive(false);
//                 }

//                 GameObject selectedCharacter = GameManager.Instance.GetCharacterInstance(characterName);
//                 if (selectedCharacter != null)
//                 {
//                     selectedCharacter.SetActive(true);
//                     Debug.Log($"Character {characterName} activated: {selectedCharacter.activeSelf}");

//                     Player selectedPlayerComponent = selectedCharacter.GetComponent<Player>();
//                     if (selectedPlayerComponent == null)
//                     {
//                         Debug.LogError($"Player component not found on {characterName}");
//                         return;
//                     }

//                     GameManager.Instance.player = selectedPlayerComponent;

//                     Client client = Client.Instance;
//                     if (client != null)
//                     {
//                         client.playerPrefab = selectedCharacter; // Cambiar el prefab del player
//                         client.SetActivePlayer(GameManager.Instance.player);

//                         client.activePlayer = selectedPlayerComponent;
//                         client.playerPrefab = selectedCharacter;
//                         // client.activePlayer.SetupLocalPlayer(); CREO QUE SE ESTA LLAMANDO 2 VECES.

//                         // Set the player as a child of the characterHolder
//                         selectedPlayerComponent.transform.SetParent(client.characterHolder, false);
//                     }
//                     else
//                     {
//                         Debug.LogError("Client not found.");
//                     }

//                     GameManager.Instance.playerInventory = selectedPlayerComponent.inventory;
//                 }
//                 else
//                 {
//                     Debug.LogError("Character instance not found: " + characterName);
//                 }
//             }
//             else
//             {
//                 Debug.LogError("CharacterNameText component not found.");
//             }
//         }
//         else
//         {
//             Debug.LogError("CharacterNameText object not found.");
//         }

//         if (selectedCharacterInfoCanvas != null)
//         {
//             selectedCharacterInfoCanvas.SetActive(false);
//             Debug.Log("SelectedCharacterInfoCanvas deactivated.");
//         }
//         else
//         {
//             Debug.LogError("SelectedCharacterInfoCanvas not assigned.");
//         }

//         foreach (DropdownBehaviour drop in dropdownList)
//         {
//             drop.UpdateInventory();
//         }
//     }

// private GameObject FindActiveCharacter()
// {
//     Player[] characters = FindObjectsOfType<Player>();
//     foreach (Player character in characters)
//     {
//         if (character.gameObject.activeSelf)
//         {
//             return character.gameObject;
//         }
//     }
//     return null;
// }














//     private GameObject FindCharacterByName(string name, bool includeInactive = false)
// {
//     GameObject[] characters = Resources.FindObjectsOfTypeAll<GameObject>();
//     foreach (GameObject character in characters)
//     {
//         if (character.name == name && (includeInactive || character.activeInHierarchy))
//         {
//             return character;
//         }
//     }
//     return null;
// }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.SceneManagement;


public class LoginButton : MonoBehaviour
{
    public GameObject selectedCharacterInfoCanvas; 

    private CinemachineVirtualCamera virtualCamera;
    
    public GameManager gameManager;

    public List<DropdownBehaviour> dropdownList;

    // public void OnClick()
    // {
    //     GameObject characterNameTextObject = GameObject.Find("UI/Canvas/SelectedCharacterInfo/InfoPanel/CharacterNameText");
        
    //     if (characterNameTextObject != null)
    //     {
    //         TextMeshProUGUI characterNameText = characterNameTextObject.GetComponent<TextMeshProUGUI>();
            
    //         if (characterNameText != null)
    //         {
    //             string characterName = characterNameText.text.Replace("Name: ", "");
    //             Debug.Log("Nombre del personaje: " + characterName);

    //             GameObject activeCharacter = FindActiveCharacter();

    //             if (activeCharacter != null)
    //             {
    //                 activeCharacter.SetActive(false);
    //             }

    //             GameObject selectedCharacter = FindCharacterByName(characterName, includeInactive: true);
    //             if (selectedCharacter != null)
    //             {
    //                 selectedCharacter.SetActive(true);
                    
    //                 GameManager gameManager = FindObjectOfType<GameManager>();

    //                 if (gameManager != null)
    //                 {
    //                         gameManager.player = selectedCharacter.GetComponent<Player>();
    //                         if(selectedCharacter.GetComponent<Player>().sceneIndex != SceneManager.GetActiveScene().buildIndex)
    //                         {
    //                             GameManager.Instance.ChangeScene(selectedCharacter.GetComponent<Player>().sceneIndex, Vector3.zero);
    //                         }
    //                         else
    //                         {
    //                             selectedCharacter.GetComponent<CharacterController>().Initiate();
    //                             virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
    //                             virtualCamera.Follow = selectedCharacter.transform;
    //                         }
    //                 }
    //                 else
    //                 {
    //                     Debug.LogError("GameManager not found.");
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.LogError("Character object not found: " + characterName);
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogError("CharacterNameText component not found.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("CharacterNameText object not found.");
    //     }

    //     if (selectedCharacterInfoCanvas != null)
    //     {
    //         selectedCharacterInfoCanvas.SetActive(false);
    //     }
    //     else
    //     {
    //         Debug.LogError("SelectedCharacterInfoCanvas not assigned.");
    //     }
    //     gameManager.playerInventory = FindObjectOfType<PlayerInventory>();
    //     foreach(DropdownBehaviour drop in dropdownList) {
    //         drop.UpdateInventory();
    //     }
    // }

        public void OnClick()
{
    GameObject characterNameTextObject = GameObject.Find("UI/Canvas/SelectedCharacterInfo/InfoPanel/CharacterNameText");

    if (characterNameTextObject != null)
    {
        TextMeshProUGUI characterNameText = characterNameTextObject.GetComponent<TextMeshProUGUI>();

        if (characterNameText != null)
        {
            string characterName = characterNameText.text.Replace("Name: ", "");
            Debug.Log("Nombre del personaje: " + characterName);

            GameObject activeCharacter = FindActiveCharacter();
            if (activeCharacter != null)
            {
                Debug.Log("Deactivating active character: " + activeCharacter.name);
                activeCharacter.SetActive(false);
            }

            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                // Obtén la instancia del Player actual
                Player currentPlayer = gameManager.player;

                if (currentPlayer != null)
                {
                    // Obtén el cliente específico del Player
                    Client client = currentPlayer.GetClient();
                    
                    if (client != null)
                    {
                        // Cambia el prefab del jugador
                        client.ChangePlayerPrefab(characterName);

                        // Obtén la instancia del personaje seleccionado
                        GameObject selectedCharacter = gameManager.GetCharacterInstance(characterName);
                        if (selectedCharacter != null)
                        {
                            selectedCharacter.SetActive(true);
                            Debug.Log($"Character {characterName} activated: {selectedCharacter.activeSelf}");

                            Player selectedPlayerComponent = selectedCharacter.GetComponent<Player>();
                            if (selectedPlayerComponent == null)
                            {
                                Debug.LogError($"Player component not found on {characterName}");
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
                        }
                        else
                        {
                            Debug.LogError("Character instance not found: " + characterName);
                        }
                    }
                    else
                    {
                        Debug.LogError("Client not found for the current player.");
                    }
                }
                else
                {
                    Debug.LogError("Current player not found.");
                }
            }
            else
            {
                Debug.LogError("GameManager not found.");
            }
        }
        else
        {
            Debug.LogError("CharacterNameText component not found.");
        }
    }
    else
    {
        Debug.LogError("CharacterNameText object not found.");
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