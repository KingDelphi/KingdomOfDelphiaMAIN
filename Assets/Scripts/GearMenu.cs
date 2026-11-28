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
    public TMP_Text levelText; // Asígnalo en el Inspector (el texto "Level" del panel) para ver el nivel

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

    // Personaje que se está mirando en el selector (null = no se eligió ninguno: se asume el activo)
    private string viewedCharacterName = null;
    // true cuando se mira a un personaje que NO es el activo (solo lectura, datos leídos del guardado)
    private bool previewMode = false;
    // Datos del personaje que se está previsualizando (equipo guardado + stats)
    private SavedCharacterPreview currentPreview = null;

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
        viewedCharacterName = null; // al cerrar el panel se olvida a quién se miraba
        currentPreview = null;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    // ------------------------------------------------------------------
    // Selector de personajes: indicar a qué personaje se está mirando
    // ------------------------------------------------------------------

    /// <summary>
    /// Lo llama CharacterClickHandler al elegir una silueta.
    /// Si es el personaje activo: equipo real, editable.
    /// Si no: se lee su equipo y stats del guardado y se muestran en solo lectura.
    /// </summary>
    public void ShowCharacter(string characterName)
    {
        viewedCharacterName = characterName;
        UpdateGearMenu();
    }

    private bool IsViewingActivePlayer(Player activePlayer)
    {
        if (string.IsNullOrEmpty(viewedCharacterName)) return true;

        // Nombre estable del héroe, igual que lo usa el guardado (PrefabInfo o nombre del objeto)
        if (SaveLoadSystem.GetHeroNameFromObject(activePlayer.gameObject) == viewedCharacterName) return true;

        // Respaldo: nombre del objeto sin sufijos
        return StripObjectName(activePlayer.gameObject.name) == viewedCharacterName;
    }

    // "Tres Deseos [Id:525312]" o "Tres Deseos(Clone)" -> "Tres Deseos"
    private static string StripObjectName(string objectName)
    {
        string n = objectName.Trim();

        int idIndex = n.IndexOf(" [Id:");
        if (idIndex >= 0) n = n.Substring(0, idIndex);

        if (n.EndsWith("(Clone)")) n = n.Substring(0, n.Length - "(Clone)".Length).Trim();

        return n.Trim();
    }

    // Cuenta y servidor con los que SaveLoadSystem arma las llaves de guardado
    private void ResolveSaveContext(Player activePlayer, out string accountName, out string cleanServerName)
    {
        SaveLoadSystem saveSystem = SaveLoadSystem.Instance;

        accountName = activePlayer != null ? activePlayer.playerName : null;
        if (string.IsNullOrEmpty(accountName) && saveSystem != null) accountName = saveSystem.currentPlayerName;
        if (string.IsNullOrEmpty(accountName) && GameManager.Instance != null) accountName = GameManager.Instance.GetPlayerName();

        cleanServerName = saveSystem != null ? SaveLoadSystem.CleanServerName(saveSystem.serverName) : "";
        if (string.IsNullOrEmpty(cleanServerName) && GameManager.Instance != null)
            cleanServerName = SaveLoadSystem.CleanServerName(GameManager.Instance.serverName);
    }

    // ------------------------------------------------------------------

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
            previewMode = !IsViewingActivePlayer(activePlayer);

            if (previewMode)
            {
                string accountName;
                string cleanServerName;
                ResolveSaveContext(activePlayer, out accountName, out cleanServerName);

                currentPreview = SavedCharacterPreview.Build(viewedCharacterName, accountName, cleanServerName);
                ShowPreviewDropdowns(currentPreview);
            }
            else
            {
                currentPreview = null;

                EnsureEquippedListSize();
                SyncEquippedListFromPlayer(activePlayer);

                for (int i = 0; i < SlotTypes.Length && i < dropdownList.Count; i++)
                {
                    PopulateSlotDropdown(activePlayer, SlotTypes[i], i);
                }
            }
        }
        finally
        {
            isRefreshingUI = false;
        }

        UpdateStatTexts();
    }

    // Personaje que no es el activo: cada dropdown muestra su pieza guardada (o None) y queda bloqueado.
    private void ShowPreviewDropdowns(SavedCharacterPreview preview)
    {
        for (int i = 0; i < dropdownList.Count && i < SlotTypes.Length; i++)
        {
            TMP_Dropdown dropdown = dropdownList[i];
            if (dropdown == null) continue;

            GearItem item = (preview != null && i < preview.equipped.Length) ? preview.equipped[i] : null;
            string label = item != null ? item.itemName : "None";

            optionItems[i] = null;

            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string> { label });
            dropdown.SetValueWithoutNotify(0);
            dropdown.RefreshShownValue();
            dropdown.interactable = false;
        }
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
        dropdown.interactable = true;
    }

    private void OnDropdownChanged(int slotIndex, int optionIndex)
    {
        if (isRefreshingUI) return;
        if (previewMode) return; // vista previa: solo lectura

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

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null) target.text = value;
    }

    private void ApplyStatTexts(int level, int hp, int mana, float speed, float critC, float critB,
                                int pAtk, int dAtk, int hAtk, int pDef, int dDef, int hDef, float weight)
    {
        SetText(levelText, "Level: " + level);
        SetText(hpText, "HP: " + hp);
        SetText(manaText, "Mana: " + mana);
        SetText(moveSpeed, "Movement Speed: " + speed);
        SetText(critChance, "Critical Chance: " + critC);
        SetText(critBonus, "Critical Bonus: " + critB);
        SetText(physAtt, "Physical Attack: " + pAtk);
        SetText(demAtt, "Demon Attack: " + dAtk);
        SetText(holAtt, "Holy Attack: " + hAtk);
        SetText(physDef, "Physical Def: " + pDef);
        SetText(demDef, "Demon Def: " + dDef);
        SetText(holDef, "Holy Def: " + hDef);
        SetText(capacityText, "Capacity: " + weight);
    }

    // Solo se usa si no hay forma de calcular los stats (personaje sin prefab registrado)
    private void ApplyStatDashes()
    {
        SetText(levelText, "Level: -");
        SetText(hpText, "HP: -");
        SetText(manaText, "Mana: -");
        SetText(moveSpeed, "Movement Speed: -");
        SetText(critChance, "Critical Chance: -");
        SetText(critBonus, "Critical Bonus: -");
        SetText(physAtt, "Physical Attack: -");
        SetText(demAtt, "Demon Attack: -");
        SetText(holAtt, "Holy Attack: -");
        SetText(physDef, "Physical Def: -");
        SetText(demDef, "Demon Def: -");
        SetText(holDef, "Holy Def: -");
        SetText(capacityText, "Capacity: -");
    }

    public void UpdateStatTexts()
    {
        Player activePlayer = GetActivePlayer();
        if (activePlayer == null) return;

        // Personaje que no es el activo: stats calculados desde su prefab + su equipo guardado
        if (previewMode)
        {
            SavedCharacterPreview p = currentPreview;
            if (p != null && p.hasStats)
            {
                ApplyStatTexts(p.level, p.hp, p.mana, p.movementSpeed, p.criticalChance, p.criticalBonus,
                               p.physicalAttack, p.demonAttack, p.hollyAttack,
                               p.physicalDefense, p.demonDefense, p.hollyDefense, p.weight);
            }
            else
            {
                ApplyStatDashes();
            }
            return;
        }

        // Personaje activo: stats totales calculados en Player (base + equipo)
        ApplyStatTexts(activePlayer.level, activePlayer.hp, activePlayer.mana, activePlayer.movementSpeed,
                       activePlayer.criticalChance, activePlayer.criticalBonus,
                       activePlayer.physicalAttack, activePlayer.demonAttack, activePlayer.hollyAttack,
                       activePlayer.physicalDefense, activePlayer.demonDefense, activePlayer.hollyDefense,
                       activePlayer.weight);
    }
}