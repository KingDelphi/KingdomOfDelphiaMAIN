using UnityEngine;

/// <summary>
/// Vista previa de un personaje que NO es el activo.
/// Lee su equipo desde el guardado (PlayerPrefs, con la misma llave que usa SaveLoadSystem)
/// y calcula sus stats: stats base del prefab + stats del equipo puesto.
/// No modifica nada: solo lectura.
/// </summary>
public class SavedCharacterPreview
{
    public const int SlotCount = 9;

    // Mismo orden que GearMenu: 0 Helmet, 1 Armor, 2 Legs, 3 Boots, 4 Weapon, 5 Shield, 6 Ring, 7 Amulet, 8 Holder
    public GearItem[] equipped = new GearItem[SlotCount];

    // false si no se encontró el prefab del personaje (no hay stats base que mostrar)
    public bool hasStats = false;

    public int level;
    public int hp;
    public int mana;
    public float movementSpeed;
    public float criticalChance;
    public float criticalBonus;
    public int physicalAttack;
    public int demonAttack;
    public int hollyAttack;
    public int physicalDefense;
    public int demonDefense;
    public int hollyDefense;
    public float weight;

    public static int SlotIndexOf(GearItem.ItemType type)
    {
        switch (type)
        {
            case GearItem.ItemType.Helmet: return 0;
            case GearItem.ItemType.Armor:  return 1;
            case GearItem.ItemType.Legs:   return 2;
            case GearItem.ItemType.Boots:  return 3;
            case GearItem.ItemType.Weapon: return 4;
            case GearItem.ItemType.Shield: return 5;
            case GearItem.ItemType.Ring:   return 6;
            case GearItem.ItemType.Amulet: return 7;
            case GearItem.ItemType.Holder: return 8;
            default: return -1; // Consumable u otro: no se equipa
        }
    }

    public static SavedCharacterPreview Build(string heroName, string accountName, string cleanServerName)
    {
        var preview = new SavedCharacterPreview();
        if (string.IsNullOrEmpty(heroName)) return preview;

        LoadSavedEquipment(preview, heroName, accountName, cleanServerName);
        ComputeStats(preview, heroName);
        return preview;
    }

    // ------------------------------------------------------------------
    // Equipo guardado
    // ------------------------------------------------------------------

    private static void LoadSavedEquipment(SavedCharacterPreview preview, string heroName, string accountName, string cleanServerName)
    {
        // Mismas llaves y mismo orden de respaldo que SaveLoadSystem.LoadStateForSaveable
        string typeName = typeof(PlayerInventory).FullName;
        string key = typeName + "_" + cleanServerName + "_" + accountName + "_" + heroName;
        string legacyKey = typeName + "_" + cleanServerName + "_" + accountName;
        string betaTesterKey = typeName + "_" + cleanServerName + "_" + "BetaTester" + "_" + heroName;

        string json = null;
        if (PlayerPrefs.HasKey(key)) json = PlayerPrefs.GetString(key);
        else if (PlayerPrefs.HasKey(legacyKey)) json = PlayerPrefs.GetString(legacyKey);
        else if (PlayerPrefs.HasKey(betaTesterKey)) json = PlayerPrefs.GetString(betaTesterKey);

        if (string.IsNullOrEmpty(json)) return; // personaje sin guardado: sin equipo

        PlayerInventory.PlayerInventorySaveData data = null;
        try
        {
            data = JsonUtility.FromJson<PlayerInventory.PlayerInventorySaveData>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("SavedCharacterPreview: no se pudo leer el guardado de '" + heroName + "': " + ex.Message);
            return;
        }

        if (data == null || data.equippedItems == null || data.inventoryItems == null) return;

        foreach (PlayerInventory.EquippedItemSaveData e in data.equippedItems)
        {
            if (e == null || string.IsNullOrEmpty(e.itemName)) continue;

            // Mismo criterio que PlayerInventory.RestoreEquipment
            PlayerInventory.GearItemSaveData found = data.inventoryItems.Find(i => i != null
                                                                                && i.itemName == e.itemName
                                                                                && i.itemRarity == e.itemRarity
                                                                                && i.itemType == e.slot);
            if (found == null) continue;

            int slot = SlotIndexOf(found.itemType);
            if (slot < 0) continue;

            preview.equipped[slot] = ToGearItem(found);
        }
    }

    private static GearItem ToGearItem(PlayerInventory.GearItemSaveData s)
    {
        return new GearItem(
            s.itemName,
            s.itemRarity,
            s.hp,
            s.mana,
            s.movementSpeed,
            s.criticalChance,
            s.criticalBonus,
            s.physicalAttack,
            s.demonAttack,
            s.hollyAttack,
            s.physicalDefense,
            s.demonDefense,
            s.hollyDefense,
            s.weight,
            s.hpRegen,
            s.manaRegen,
            s.effectDuration,
            s.itemType
        ) { quantity = s.quantity };
    }

    // ------------------------------------------------------------------
    // Stats: base del prefab + equipo (misma fórmula que Player.CalculatePlayerStats)
    // ------------------------------------------------------------------

    private static Player FindPrefabPlayer(string heroName)
    {
        GameObject prefab = null;

        // Mismo diccionario que usa Client para spawnear a cada personaje
        Client client = GameManager.Instance != null ? GameManager.Instance.client : null;
        if (client != null && client.characterPrefabs != null)
        {
            client.characterPrefabs.TryGetValue(heroName, out prefab);
        }

        // Respaldo: misma ruta que Client.Awake
        if (prefab == null) prefab = Resources.Load<GameObject>("Prefabs/" + heroName);

        return prefab != null ? prefab.GetComponent<Player>() : null;
    }

    private static void ComputeStats(SavedCharacterPreview preview, string heroName)
    {
        Player basePlayer = FindPrefabPlayer(heroName);
        if (basePlayer == null) return; // sin prefab no hay stats base

        preview.hasStats = true;

        // El nivel no se guarda en ningún ISaveable actual, así que se toma del prefab de ese personaje
        preview.level = basePlayer.level;

        preview.hp = basePlayer.initialHp;
        preview.mana = basePlayer.initialMana;
        preview.movementSpeed = basePlayer.initialMovementSpeed;
        preview.criticalChance = basePlayer.initialCriticalChance;
        preview.criticalBonus = basePlayer.initialCriticalBonus;
        preview.physicalAttack = basePlayer.initialPhysicalAttack;
        preview.demonAttack = basePlayer.initialDemonAttack;
        preview.hollyAttack = basePlayer.initialHollyAttack;
        preview.physicalDefense = basePlayer.initialPhysicalDefense;
        preview.demonDefense = basePlayer.initialDemonDefense;
        preview.hollyDefense = basePlayer.initialHollyDefense;
        preview.weight = basePlayer.initialWeight;

        foreach (GearItem g in preview.equipped)
        {
            if (g == null) continue;

            preview.hp += g.hp;
            preview.mana += g.mana;
            preview.movementSpeed += g.movementSpeed;
            preview.criticalChance += g.criticalChance;
            preview.criticalBonus += g.criticalBonus;
            preview.physicalAttack += g.physicalAttack;
            preview.demonAttack += g.demonAttack;
            preview.hollyAttack += g.hollyAttack;
            preview.physicalDefense += g.physicalDefense;
            preview.demonDefense += g.demonDefense;
            preview.hollyDefense += g.hollyDefense;
            preview.weight += g.weight;
        }
    }
}