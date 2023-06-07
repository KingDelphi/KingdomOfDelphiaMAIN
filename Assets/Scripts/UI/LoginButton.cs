using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.SceneManagement;


public class LoginButton : MonoBehaviour
{
    //public List<GameObject> characters; // Agrega los personajes en el Inspector
    public GameObject selectedCharacterInfoCanvas; // Referencia al canvas "SelectedCharacterInfo"

    private CinemachineVirtualCamera virtualCamera;
    
    public GameManager gameManager;

    public List<DropdownBehaviour> dropdownList;

    public void OnClick()
    {
        // Obtener el objeto CharacterNameText
        GameObject characterNameTextObject = GameObject.Find("UI/Canvas/SelectedCharacterInfo/InfoPanel/CharacterNameText");
        
        if (characterNameTextObject != null)
        {
            TextMeshProUGUI characterNameText = characterNameTextObject.GetComponent<TextMeshProUGUI>();
            
            if (characterNameText != null)
            {
                string characterName = characterNameText.text.Replace("Name: ", ""); // Eliminar "Name: "
                Debug.Log("Nombre del personaje: " + characterName);

                // Buscar el objeto del personaje actualmente activo
                GameObject activeCharacter = FindActiveCharacter();

                if (activeCharacter != null)
                {
                    // Desactivar el personaje actualmente activo
                    activeCharacter.SetActive(false);
                }

                // Buscar el objeto del personaje seleccionado, incluso si está desactivado
                GameObject selectedCharacter = FindCharacterByName(characterName, includeInactive: true);
                if (selectedCharacter != null)
                {
                    // Activar el personaje seleccionado
                    selectedCharacter.SetActive(true);
                    
                    
                    

                    // Obtener la referencia al GameManager
                    GameManager gameManager = FindObjectOfType<GameManager>();

                    // Asignar el selectedCharacter al componente Character del GameManager
                    if (gameManager != null)
                    {
                            gameManager.player = selectedCharacter.GetComponent<Player>();
        
                            if(selectedCharacter.GetComponent<Player>().sceneName != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
                            {
                                Debug.Log("TEST: Scene is Loading from LoginButton script.");
                                GameManager.Instance.ChangeScene(selectedCharacter.GetComponent<Player>().sceneName, Vector3.zero);
                                //SceneManager.LoadScene(selectedCharacter.GetComponent<Player>().sceneName);
                            }
                            else
                            {
                                selectedCharacter.GetComponent<CharacterController>().Initiate();
                                virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
                                virtualCamera.Follow = selectedCharacter.transform;
                            }
                    }
                    else
                    {
                        Debug.LogError("GameManager not found.");
                    }
                }
                else
                {
                    Debug.LogError("Character object not found: " + characterName);
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

        // Desactivar el canvas "SelectedCharacterInfo"
        if (selectedCharacterInfoCanvas != null)
        {
            selectedCharacterInfoCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("SelectedCharacterInfoCanvas not assigned.");
        }
        gameManager.playerInventory = FindObjectOfType<PlayerInventory>();
        foreach(DropdownBehaviour drop in dropdownList) {
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