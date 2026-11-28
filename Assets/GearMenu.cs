using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GearMenu : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public List<TMP_Dropdown> dropdownList; // Orden: 0:Helmet, 1:Armor, 2:Legs, 3:Boots, 4:Weapon, 5:Shield, 6:Ring, 7:Amulet, 8:Holder
    public List<GearItem> equippedGear = new List<GearItem>(new GearItem[9]);

    [Header("Stat Texts")]
    public TMP_Text moveSpeed, critChance, critBonus, physAtt, demAtt, holAtt, physDef, demDef, holDef;
    public TMP_Text hpText, manaText, capacityText; // Asígnalos en el Inspector para ver HP, Mana y Capacity

    private static readonly GearItem.ItemType[] SlotTypes = new GearItem.ItemType[]
    {
        GearItem.ItemType.Helmet, // 0
        GearItem.ItemType.Armor,  // 1
        GearItem.ItemType.Legs,   // 2
        GearItem.ItemType.Boots,  // 3
        GearItem.ItemType.Weapon, // 4
        GearItem.ItemType.Shield, // 5
        GearItem.ItemType.Ring,   // 6
        GearItem.ItemType.Amulet, // 7
        GearItem.ItemType.Holder  // 8
    };

    // Para cada dropdown: qué GearItem corresponde a cada opción (posición 0 = "None" = null)
    private readonly List<GearItem>[] optionItems = new List<GearItem>[9];

    private PlayerInventory subscribedInventory;
    private bool isRefreshingUI = false;

    private void Awake()
    {
        EnsureEquippedListSize();
        SetupDropdownListeners();
    }

    private void OnEnable()
    {
        RefreshReferences();
        UpdateGearMenu();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void EnsureEquippedListSize()
    {
        if (equippedGear == null) equippedGear = new List<GearItem>();
        while (equippedGear.Count < 9) equippedGear.Add(null);
    }

    private Player GetActivePlayer()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
            return GameManager.Instance.player;
        return FindFirstObjectByType<Player>();
    }

    private void Unsubscribe()
    {
        if (subscribedInventory != null)
        {
            subscribedInventory.InventoryUpdated -= UpdateGearMenu;
        }
        subscribedInventory = null;
    }

    private void RefreshReferences()
    {
        Player activePlayer = GetActivePlayer();
        if (activePlayer == null) return;

        if (activePlayer.gear == null)
            activePlayer.gear = new PlayerGear();

        PlayerInventory inv = activePlayer.GetComponent<PlayerInventory>();
        playerInventory = inv;

        // Si cambió el personaje activo, dejar de escuchar al anterior y escuchar al nuevo
        if (subscribedInventory != inv)
        {
            Unsubscribe();
            if (inv != null)
            {
                inv.InventoryUpdated += UpdateGearMenu;
                subscribedInventory = inv;
            }
        }
    }

    private void SetupDropdownListeners()
    {
        if (dropdownList == null) return;

        for (int i = 0; i < dropdownList.Count && i < SlotTypes.Length; i++)
        {
            if (dropdownList[i] == null) continue;
            int slotIndex = i;
            dropdownList[i].onValueChanged.RemoveAllListeners();
            dropdownList[i].onValueChanged.AddListener((optionIndex) => OnDropdownChanged(slotIndex, optionIndex));
        }
    }

    public void UpdateGearMenu()
    {
        if (isRefreshingUI) return;

        RefreshReferences();
        Player activePlayer = GetActivePlayer();
        if (activePlayer == null || playerInventory == null || dropdownList == null) return;

        isRefreshingUI = true;
        try
        {
            EnsureEquippedListSize();
            SyncEquippedListFromPlayer(activePlayer);

            for (int i = 0; i < SlotTypes.Length && i < dropdownList.Count; i++)
            {
                PopulateSlotDropdown(activePlayer, SlotTypes[i], i);
            }
        }
        finally
        {
            isRefreshingUI = false;
        }

        UpdateStatTexts();
    }

    private void SyncEquippedListFromPlayer(Player activePlayer)
    {
        equippedGear[0] = activePlayer.gear.helmet;
        equippedGear[1] = activePlayer.gear.armor;
        equippedGear[2] = activePlayer.gear.legs;
        equippedGear[3] = activePlayer.gear.boots;
        equippedGear[4] = activePlayer.gear.weapon;
        equippedGear[5] = activePlayer.gear.shield;
        equippedGear[6] = activePlayer.gear.ring;
        equippedGear[7] = activePlayer.gear.amulet;
        equippedGear[8] = activePlayer.gear.holder;
    }

    private void PopulateSlotDropdown(Player activePlayer, GearItem.ItemType slotType, int index)
    {
        TMP_Dropdown dropdown = dropdownList[index];
        if (dropdown == null) return;

        GearItem currentEquipped = equippedGear[index];

        // Si lo equipado ya no está en el inventario (o no corresponde a este slot), desequiparlo
        if (currentEquipped != null &&
            (currentEquipped.itemType != slotType || !playerInventory.inventory.Contains(currentEquipped)))
        {
            ClearSlotOnPlayer(activePlayer, slotType);
            equippedGear[index] = null;
            currentEquipped = null;
        }

        // Opciones: "None" + SOLO los items del inventario del tipo exacto de este slot
        var items = new List<GearItem> { null };
        foreach (GearItem invItem in playerInventory.inventory)
        {
            if (invItem != null && invItem.itemType == slotType)
            {
                items.Add(invItem);
            }
        }

        // Etiquetas: el nombre; si hay varios con el mismo nombre, se agrega la rareza
        var labels = new List<string> { "None" };
        for (int i = 1; i < items.Count; i++)
        {
            bool repeatedName = false;
            for (int j = 1; j < items.Count; j++)
            {
                if (j != i && items[j].itemName == items[i].itemName)
                {
                    repeatedName = true;
                    break;
                }
            }
            labels.Add(repeatedName ? items[i].itemName + " (" + items[i].rarity + ")" : items[i].itemName);
        }

        optionItems[index] = items;

        dropdown.ClearOptions();
        dropdown.AddOptions(labels);

        int selectedIndex = currentEquipped != null ? items.IndexOf(currentEquipped) : 0;
        if (selectedIndex < 0) selectedIndex = 0;

        dropdown.SetValueWithoutNotify(selectedIndex);
        dropdown.RefreshShownValue();
    }

    private void OnDropdownChanged(int slotIndex, int optionIndex)
    {
        if (isRefreshingUI) return;

        Player activePlayer = GetActivePlayer();
        if (activePlayer == null || playerInventory == null) return;
        if (activePlayer.gear == null) activePlayer.gear = new PlayerGear();

        List<GearItem> items = optionItems[slotIndex];
        if (items == null || optionIndex < 0 || optionIndex >= items.Count) return;

        GearItem.ItemType slotType = SlotTypes[slotIndex];
        GearItem chosen = items[optionIndex]; // null = "None"

        if (chosen == null)
        {
            ClearSlotOnPlayer(activePlayer, slotType);
        }
        else if (chosen.itemType == slotType)
        {
            activePlayer.UpdateGear(chosen);          // asigna por tipo y recalcula stats
            activePlayer.gear.EquipmentChanged();     // avisa a quien escuche el evento
        }
        else
        {
            // No debería pasar (las opciones ya están filtradas por tipo). Reconstruir la UI.
            UpdateGearMenu();
            return;
        }

        equippedGear[slotIndex] = chosen;
        UpdateStatTexts();
    }

    private void ClearSlotOnPlayer(Player activePlayer, GearItem.ItemType slotType)
    {
        switch (slotType)
        {
            case GearItem.ItemType.Helmet: activePlayer.gear.helmet = null; break;
            case GearItem.ItemType.Armor:  activePlayer.gear.armor = null; break;
            case GearItem.ItemType.Legs:   activePlayer.gear.legs = null; break;
            case GearItem.ItemType.Boots:  activePlayer.gear.boots = null; break;
            case GearItem.ItemType.Weapon: activePlayer.gear.weapon = null; break;
            case GearItem.ItemType.Shield: activePlayer.gear.shield = null; break;
            case GearItem.ItemType.Ring:   activePlayer.gear.ring = null; break;
            case GearItem.ItemType.Amulet: activePlayer.gear.amulet = null; break;
            case GearItem.ItemType.Holder: activePlayer.gear.holder = null; break;
        }

        // Recalcula los stats en Player (se suscribe a este evento)
        activePlayer.gear.EquipmentChanged();
    }

    public void UpdateStatTexts()
    {
        Player activePlayer = GetActivePlayer();
        if (activePlayer == null) return;

        // Stats totales calculados en Player (base + equipo)
        if (hpText != null) hpText.text = "HP: " + activePlayer.hp;
        if (manaText != null) manaText.text = "Mana: " + activePlayer.mana;
        if (moveSpeed != null) moveSpeed.text = "Movement Speed: " + activePlayer.movementSpeed;
        if (critChance != null) critChance.text = "Critical Chance: " + activePlayer.criticalChance;
        if (critBonus != null) critBonus.text = "Critical Bonus: " + activePlayer.criticalBonus;
        if (physAtt != null) physAtt.text = "Physical Attack: " + activePlayer.physicalAttack;
        if (demAtt != null) demAtt.text = "Demon Attack: " + activePlayer.demonAttack;
        if (holAtt != null) holAtt.text = "Holy Attack: " + activePlayer.hollyAttack;
        if (physDef != null) physDef.text = "Physical Def: " + activePlayer.physicalDefense;
        if (demDef != null) demDef.text = "Demon Def: " + activePlayer.demonDefense;
        if (holDef != null) holDef.text = "Holy Def: " + activePlayer.hollyDefense;
        if (capacityText != null) capacityText.text = "Capacity: " + activePlayer.weight;
    }
}