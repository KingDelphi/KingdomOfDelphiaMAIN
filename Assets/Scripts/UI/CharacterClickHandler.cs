using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterClickHandler : MonoBehaviour
{
    public Canvas characterSelectionCanvas;
    public Canvas selectedCharacterInfoCanvas;
    public Image characterImage;
    public CharacterSelectorButton characterSelectorButton;

    public GameObject characterNameTextObject;
    public GameObject characterVocationTextObject;
    private TextMeshProUGUI characterNameText;
    private TextMeshProUGUI characterVocationText;

    void Awake() {
        characterSelectorButton = FindObjectOfType<CharacterSelectorButton>();

    }

    public void OnClick()
    {
        // Nombre del personaje al que se le dio click (se llena más abajo)
        string clickedCharacterName = null;

        // Desactiva el canvas de selección de personaje
        characterSelectionCanvas.gameObject.SetActive(false);

        // Activa el canvas de información del personaje seleccionado
        selectedCharacterInfoCanvas.gameObject.SetActive(true);

        // Obtiene la imagen del objeto al que se le dio click
        Image clickedImage = GetComponent<Image>();

        if (clickedImage != null)
        {
            // Cambia la imagen del objeto "Character" en el canvas de información del personaje
            characterImage.sprite = clickedImage.sprite;

            // Obtiene el componente CharacterInformation del objeto al que se le dio click
            CharacterInformation characterInformation = GetComponent<CharacterInformation>();

            if (characterInformation != null)
            {
                // Obtiene el nombre y la vocación del CharacterInformation
                string characterName = characterInformation.characterName;
                string characterVocation = characterInformation.vocation.ToString().Replace("_", " ");
                clickedCharacterName = characterName;

                // Busca el componente TextMeshProUGUI en el objeto "CharacterNameText" si no se ha asignado antes
                if (characterNameText == null)
                {
                    characterNameText = characterNameTextObject.GetComponent<TextMeshProUGUI>();
                }

                if (characterNameText != null)
                {
                    // Activa el objeto "CharacterNameText" si está desactivado
                    if (!characterNameText.gameObject.activeSelf)
                    {
                        characterNameText.gameObject.SetActive(true);
                    }

                    // Modifica el texto del objeto TextMeshProUGUI "CharacterNameText"
                    characterNameText.text = "Name: " + characterName;
                }

                // Busca el componente TextMeshProUGUI en el objeto "CharacterVocationText" si no se ha asignado antes
                if (characterVocationText == null)
                {
                    characterVocationText = characterVocationTextObject.GetComponent<TextMeshProUGUI>();
                }

                if (characterVocationText != null)
                {
                    // Activa el objeto "CharacterVocationText" si está desactivado
                    if (!characterVocationText.gameObject.activeSelf)
                    {
                        characterVocationText.gameObject.SetActive(true);
                    }

                    // Modifica el texto del objeto TextMeshProUGUI "CharacterVocationText"
                    characterVocationText.text = "Vocation: " + characterVocation;
                }
            }
        }

        // Avisa al panel de equipo a qué personaje se está mirando.
        // Si es el activo, se muestra su equipo real y es editable.
        // Si no, el panel queda en vista previa (todo en None, solo lectura).
        if (characterSelectorButton != null && characterSelectorButton.gearMenu != null)
        {
            characterSelectorButton.gearMenu.ShowCharacter(clickedCharacterName);
        }
    }
}