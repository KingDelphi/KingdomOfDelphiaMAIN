using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectorButton : MonoBehaviour
{
    public Canvas characterSelectionCanvas;
    public Canvas selectedCharacterInfoCanvas;
    public List<DropdownBehaviour> dropdownList;
    public GearMenu gearMenu;

    public void OnClick()
    {
        Debug.Log("CharacterSelectorButton.OnClick()");

        if (characterSelectionCanvas == null)
        {
            Debug.LogError("characterSelectionCanvas no asignado.");
            return;
        }

        // Si el panel de info está abierto, lo cerramos para no bloquear el selector
        if (selectedCharacterInfoCanvas != null && selectedCharacterInfoCanvas.gameObject.activeSelf)
        {
            selectedCharacterInfoCanvas.gameObject.SetActive(false);
            Debug.Log("SelectedCharacterInfo cerrado.");
        }

        bool open = !characterSelectionCanvas.gameObject.activeSelf;
        characterSelectionCanvas.gameObject.SetActive(open);
        Debug.Log(open ? "CharacterSelectorCanvas ABIERTO" : "CharacterSelectorCanvas CERRADO");
    }
}