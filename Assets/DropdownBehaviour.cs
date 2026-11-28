using UnityEngine;
using TMPro;

/// <summary>
/// OBSOLETO. GearMenu.cs ahora controla por completo los dropdowns de equipo
/// (llenado filtrado por tipo, equipar, desequipar).
///
/// Este script se conserva SOLO para no romper referencias existentes en la escena
/// y en otros scripts (por ejemplo CharacterSelectorButton.dropdownList).
/// No hace nada a propósito: la versión anterior duplicaba listeners, llenaba los
/// dropdowns sin filtrar y modificaba el inventario al equipar.
/// </summary>
public class DropdownBehaviour : MonoBehaviour
{
    // Campos públicos conservados para no perder las asignaciones del Inspector
    public PlayerInventory playerInventory;
    public GameManager gameManager;
    public TMP_Dropdown dropdown;
    public GameObject character;
    public TMP_Text selectedItem;
    public string previousItem;
    public GearItem gearItem, previousGearItem;
    public GearMenu gearMenu;
    public int index;

    // Métodos públicos conservados por compatibilidad. Intencionalmente vacíos.
    public void SubscribeToUpdateEvent() { }

    public void UpdateInventory() { }
}