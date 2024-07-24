using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownBehaviour : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public GameManager gameManager;
    public TMP_Dropdown dropdown;
    public GameObject character;
    public TMP_Text selectedItem;
    public string previousItem;
    public GearItem gearItem, previousGearItem;
    public GearMenu gearMenu;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
      previousItem = "None";
      selectedItem.text = "None";
      gameManager = FindObjectOfType<GameManager>();
      playerInventory = FindObjectOfType<PlayerInventory>();
      dropdown = GetComponent<TMP_Dropdown>();

      playerInventory.InventoryUpdated += UpdateDropdown;
      dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });

      PopulateDropdown();
      SubscribeToUpdateEvent();
    }

    private void OnDestroy()
    {
        playerInventory.InventoryUpdated -= UpdateDropdown;
        dropdown.onValueChanged.RemoveAllListeners();

    }

    public void SubscribeToUpdateEvent()
    {
        playerInventory.InventoryUpdated += UpdateDropdown; // Suscribir al evento
        PopulateDropdown(); // Llamar a PopulateDropdown también al suscribirse
    }

    public void UpdateInventory() {
      playerInventory = FindObjectOfType<PlayerInventory>();
      PopulateDropdown();
    }

    void UpdateDropdown()
    {
      PopulateDropdown();
    }

    private void PopulateDropdown()
    {
        List<string> options = new List<string>();

        // Agregar una opción por defecto
        options.Add("None");

        // Agregar items del inventario al dropdown
        foreach (GearItem item in playerInventory.inventory)
        {
            options.Add(item.itemName + " x" + item.quantity);
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        // Seleccionar el ítem previamente guardado en `selectedItem`
        if (options.Contains(selectedItem.text))
        {
            dropdown.value = options.IndexOf(selectedItem.text);
        }
        else
        {
            dropdown.value = 0; // seleccionar "None" si el ítem previo ya no está en la lista
        }

        // Asegura que el dropdown se muestre correctamente
        dropdown.RefreshShownValue();
    }

    void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        string selectedItemName = dropdown.options[dropdown.value].text;
            
            // Equipar el ítem si no es "None"
        if (selectedItemName != "None")
        {
            GearItem selectedGearItem = playerInventory.inventory.Find(item => item.itemName == selectedItemName);
            if (selectedGearItem != null)
            {
                selectedItem.text = selectedItemName; // Guardar el ítem seleccionado
                EquipItem(selectedGearItem, index); // Equipar el índice del dropdown
            }
            else
            {
                Debug.LogError($"Item {selectedItemName} not found in inventory.");
            }
        }
        else
        {
            selectedItem.text = "None"; // Guardar "None" cuando no hay ítem seleccionado
            UnequipItem(index); // Des-equipar el ítem en el índice del dropdown
        }
    }


void EquipItem(GearItem item, int gearIndex)
{
    if (item == null)
    {
        Debug.LogError("Item is null.");
        return;
    }

    if (gearIndex < 0)
    {
        Debug.LogError($"Invalid gear index: {gearIndex}. Index must be non-negative.");
        return;
    }

    // Asegúrate de que `equippedGear` tenga suficiente espacio
    while (gearIndex >= gearMenu.equippedGear.Count)
    {
        gearMenu.equippedGear.Add(null);
    }

    GearItem previousGearItem = gearMenu.equippedGear[gearIndex];
    if (previousGearItem != null)
    {
        playerInventory.inventory.Add(previousGearItem);
    }


    // if (gearMenu.equippedGear[gearIndex] != null)
    // {
    //     playerInventory.inventory.Add(gearMenu.equippedGear[gearIndex]);
    // }
    
    gearMenu.equippedGear[gearIndex] = item;
    playerInventory.inventory.Remove(item);
    gearMenu.UpdateStatTexts();

    // Verificar y depurar posibles referencias nulas
        if (character == null)
        {
            Debug.LogError("Character is not assigned.");
            return;
        }

        Player player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError($"Player component is not found on the character: {character.name}.");
            return;
        }

        Debug.Log($"Player component found on the character: {character.name}.");

        // Actualizar equipo en el script Player
        player.UpdateGear(item);
}

void UnequipItem(int gearIndex)
{
    GearItem gearItem = gearMenu.equippedGear[gearIndex];
    if (gearItem != null)
    {
        playerInventory.inventory.Add(gearItem);
        gearMenu.equippedGear[index] = null;
        gearMenu.UpdateStatTexts();

        // Verificar y depurar posibles referencias nulas
            if (character == null)
            {
                Debug.LogError("Character reference is null.");
                return;
            }

            // Player player = character.GetComponent<Player>();
            Player player = FindObjectOfType<Player>();

            if (player == null)
            {
                Debug.LogError("Player component is not found on the character.");
                // Intentar encontrar el componente Player en los hijos
                player = character.GetComponentInChildren<Player>();
                if (player == null)
                {
                    Debug.LogError("Player component is not found on the character or its children.");
                    // return;
                }
                else
                {
                    Debug.Log("Player component found on a child of the character.");
                }
            }

            // Actualizar equipo en el script Player
            player.UpdateGear(null);
    }
}
}
