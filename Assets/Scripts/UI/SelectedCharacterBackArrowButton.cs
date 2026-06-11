using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacterBackArrowButton : MonoBehaviour
{
    public Canvas selectedCharacterInfoCanvas;
    public Canvas characterSelectionCanvas; // Referencia al canvas que se activará

    // Método que se llama cuando se hace clic en el objeto
    public void OnClick()
    {
        // Desactiva el canvas de información del personaje seleccionado
        selectedCharacterInfoCanvas.gameObject.SetActive(false);
        
        // Activa el canvas de selección de personaje
        characterSelectionCanvas.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}