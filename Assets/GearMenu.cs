using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GearMenu : MonoBehaviour
{
    private Player player;

    public PlayerInventory playerInventory;
    public List<TMP_Dropdown> dropdownList;
    public List<GearItem> equippedGear;
    public TMP_Text moveSpeed, critChance, critBonus, physAtt, demAtt, holAtt, physDef, demDef, holDef;

    void Awake()
    {
        // Debug.Log("GearMenu Awake() called.");
        player = FindObjectOfType<Player>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        equippedGear = new List<GearItem>(new GearItem[9]); // Inicializar lista de equipo equipado
        UpdateGearMenu();
    }

    public void UpdateGearMenu() {
        // Debug.Log("Updating GearMenu.");    
        CheckEquipped();
        CheckInvGear();
        UpdateStatTexts();
    }

    void CheckGear(PlayerInventory inventory, PlayerGear gear, GearItem.ItemType newArmorType, string gearType, int index) 
    {
        // Debug.Log("Checking Gear for: " + gearType);

        TMP_Dropdown dropdown = dropdownList[index];

        // Verificar que el dropdown no sea nulo
        if (dropdown == null)
        {
            Debug.LogError("Dropdown is null at index: " + index);
            return;
        }

        // Obtener el ítem equipado actualmente
        GearItem equippedItem = null;
        switch (newArmorType)
        {
            case GearItem.ItemType.Helmet:
                equippedItem = gear.helmet;
                break;
            case GearItem.ItemType.Armor:
                equippedItem = gear.armor;
                break;
            case GearItem.ItemType.Legs:
                equippedItem = gear.legs;
                break;
            case GearItem.ItemType.Boots:
                equippedItem = gear.boots;
                break;
            case GearItem.ItemType.Weapon:
                equippedItem = gear.weapon;
                break;
            case GearItem.ItemType.Shield:
                equippedItem = gear.shield;
                break;
            case GearItem.ItemType.Ring:
                equippedItem = gear.ring;
                break;
            case GearItem.ItemType.Amulet:
                equippedItem = gear.amulet;
                break;
            case GearItem.ItemType.Holder:
                equippedItem = gear.holder;
                break;
        }

        // Guardar el ítem seleccionado actualmente (si hay alguno)
        string selectedItem = dropdown.options.Count > dropdown.value ? dropdown.options[dropdown.value].text : "";
        // Debug.Log($"Selected item: {selectedItem}");

        // Limpiar opciones del dropdown
        dropdown.ClearOptions();

        // Agregar opción por defecto
        dropdown.options.Add(new TMP_Dropdown.OptionData("None"));

        // Agregar el ítem equipado como la primera opción si existe
        if (equippedItem != null)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(equippedItem.itemName));
        }

        // Agregar ítems del inventario al dropdown
        foreach (GearItem inventoryItem in inventory.inventory)
        {
            if (inventoryItem.itemType == newArmorType)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(inventoryItem.itemName));
            }
        }

        // Configurar el valor del dropdown al ítem equipado o al ítem seleccionado anteriormente
        dropdown.value = dropdown.options.FindIndex(option => option.text == selectedItem);
        if (dropdown.value == -1 && equippedItem != null)
        {
            dropdown.value = dropdown.options.FindIndex(option => option.text == equippedItem.itemName);
        }
        dropdown.value = dropdown.value == -1 ? 0 : dropdown.value; // Si no se encuentra, seleccionar "None"

        // Esto asegura que se actualice visualmente
        dropdown.captionText.text = dropdown.options[dropdown.value].text;
        dropdown.RefreshShownValue(); // Actualizar el valor mostrado en el dropdown

        // Verificar si las opciones se han agregado correctamente
        // Debug.Log("Dropdown options count: " + dropdown.options.Count);
        foreach (var option in dropdown.options)
        {
            // Debug.Log("Dropdown option: " + option.text);
        }
    }

    // Método para actualizar los textos de estadísticas
    public void UpdateStatTexts()
    {
        // Debug.Log("Updating Stat Texts.");

        // Verificar si equippedGear es nulo o está vacío antes de continuar
        if (equippedGear == null || equippedGear.Count == 0)
        {
            Debug.LogWarning("equippedGear is null or empty. Cannot update stat texts.");
            return; // Salir de la función si no hay equipo equipado
        }

        float totalMoveSpeed = 0, totalCritChance = 0, totalCritBonus = 0, totalPhysAtt = 0, totalDemAtt = 0, totalHolAtt = 0, totalPhysDef = 0, totalDemDef = 0, totalHolDef = 0;

        foreach (var gear in equippedGear)
        {
            if (gear != null)
            {
                totalMoveSpeed += gear.movementSpeed;
                totalCritChance += gear.criticalChance;
                totalCritBonus += gear.criticalBonus;
                totalPhysAtt += gear.physicalAttack;
                totalDemAtt += gear.demonAttack;
                totalHolAtt += gear.hollyAttack;
                totalPhysDef += gear.physicalDefense;
                totalDemDef += gear.demonDefense;
                totalHolDef += gear.hollyDefense;
            }
        }

        moveSpeed.text = "Movement Speed: " + totalMoveSpeed;
        critChance.text = "Critical Chance: " + totalCritChance;
        critBonus.text = "Critical Bonus: " + totalCritBonus;
        physAtt.text = "Physical Attack: " + totalPhysAtt;
        demAtt.text = "Demon Attack: " + totalDemAtt;
        holAtt.text = "Holy Attack: " + totalHolAtt;
        physDef.text = "Physical Def: " + totalPhysDef;
        demDef.text = "Demon Def: " + totalDemDef;
        holDef.text = "Holy Def: " + totalHolDef;

    }

    void CheckEquipped() 
    {
        // Debug.Log("Checking Equipped Gear.");

        equippedGear.Clear(); // Limpiar lista de equipo equipado

        // Agrega el equipo actualmente equipado solo si está presente en el inventario del jugador
        AddEquippedItem(player.gear.helmet);
        AddEquippedItem(player.gear.armor);
        AddEquippedItem(player.gear.legs);
        AddEquippedItem(player.gear.boots);
        AddEquippedItem(player.gear.weapon);
        AddEquippedItem(player.gear.shield);
        AddEquippedItem(player.gear.ring);
        AddEquippedItem(player.gear.amulet);
        AddEquippedItem(player.gear.holder);
    }

    void AddEquippedItem(GearItem item) {
    if (item != null) 
        {
            equippedGear.Add(item);
        }
    }

    void CheckInvGear() 
    {
        // Debug.Log("Checking Inventory Gear.");

        CheckGear(playerInventory, player.gear, GearItem.ItemType.Helmet, "Helmet", 0);
        CheckGear(playerInventory, player.gear, GearItem.ItemType.Armor, "Armor", 1);
        CheckGear(playerInventory, player.gear, GearItem.ItemType.Legs, "Legs",2);
        CheckGear(playerInventory, player.gear, GearItem.ItemType.Boots, "Boots", 3);
        CheckGear(playerInventory, player.gear, GearItem.ItemType.Weapon, "Weapon", 4);
        CheckGear(playerInventory, player.gear, GearItem.ItemType.Shield, "Shield", 5);
        CheckGear(playerInventory, player.gear, GearItem.ItemType.Ring, "Ring", 6);
        CheckGear(playerInventory, player.gear, GearItem.ItemType.Amulet, "Amulet", 7);
        CheckGear(playerInventory, player.gear, GearItem.ItemType.Holder, "Holder", 8);
    }
}
