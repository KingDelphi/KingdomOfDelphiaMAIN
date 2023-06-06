using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectorButton : MonoBehaviour
{
    public Canvas characterSelectionCanvas; // Referencia al canvas que se activará
    public Canvas selectedCharacterInfoCanvas;
    public List<DropdownBehaviour> dropdownList;
    public GearMenu gearMenu;
    // Método que se llama cuando se hace clic en el objeto
    public void OnClick()
    {
        if(!selectedCharacterInfoCanvas.gameObject.activeSelf)
        {
            // Verifica el estado actual del canvas de selección de personaje
            if (characterSelectionCanvas.gameObject.activeSelf)
            {
                // Si está activo, lo desactiva
                characterSelectionCanvas.gameObject.SetActive(false);
            }
            else
            {
                // Si está desactivado, lo activa
                characterSelectionCanvas.gameObject.SetActive(true);
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}