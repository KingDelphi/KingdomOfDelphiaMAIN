using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class PlayerInventory : MonoBehaviour, ISaveable
{
    public string ID => GetEntityId().ToString(); // SAVELOAD
    public Type SaveType => typeof(PlayerInventorySaveData); // Tipo de estado guardado

    // ------------------------------------------------------------------
    // Datos de guardado
    // ------------------------------------------------------------------

    [System.Serializable]
    public class PlayerInventorySaveData
    {
        public List<GearItemSaveData> inventoryItems;
        public int gold;
        // NUEVO: equipo puesto. Las partidas viejas no lo tienen y se cargan sin equipo.
        public List<EquippedItemSaveData> equippedItems = new List<EquippedItemSaveData>();
    }

    [System.Serializable]
    public class EquippedItemSaveData
    {
        public GearItem.ItemType slot;
        public string itemName;
        public GearItem.Rarity itemRarity;
    }

    [System.Serializable]
    public class GearItemSaveData
    {
        public int quantity;
        public string itemName;
        public GearItem.Rarity itemRarity;
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
        public int hpRegen;
        public int manaRegen;
        public float effectDuration;
        public GearItem.ItemType itemType;
    }

    // ------------------------------------------------------------------
    // Campos públicos (no renombrar: están asignados en el Inspector)
    // ------------------------------------------------------------------

    public GearItemManager gearItemManager;
    public GearMenu gearMenu;

    // Inventario
    public List<GearItem> inventory;

    // Evento: se dispara cuando cambia el inventario o el equipo
    public Action InventoryUpdated;

    // Oro
    public int gold;

    public Counter counter;
    public GameManager gameManager;

    private Player player;

    // ------------------------------------------------------------------
    // Ciclo de vida
    // ------------------------------------------------------------------

    private void Awake()
    {
        gearItemManager = FindFirstObjectByType<GearItemManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        gearMenu = FindFirstObjectByType<GearMenu>(FindObjectsInactive.Include);
        player = GetComponent<Player>();
        if (counter == null) counter = GetComponent<Counter>();

        // El inventario empieza vacío. Se llena al cargar la partida (LoadState, desde Start).
        // IMPORTANTE: antes esto se hacía en Start DESPUÉS de cargar, y borraba lo cargado.
        inventory = new List<GearItem>();
        gold = 0;
    }

    private void Start()
    {
        SaveLoadSystem.Instance.RegisterSaveable(this);
    }

    // ------------------------------------------------------------------
    // ISaveable
    // ------------------------------------------------------------------

    public object SaveState()
    {
        var saveData = new PlayerInventorySaveData
        {
            gold = this.gold,
            inventoryItems = new List<GearItemSaveData>(),
            equippedItems = new List<EquippedItemSaveData>()
        };

        foreach (GearItem item in inventory)
        {
            GearItemSaveData data = ConvertGearItemToSaveData(item);
            if (data != null) saveData.inventoryItems.Add(data);
        }

        foreach (GearItem equipped in GetEquippedItems())
        {
            if (equipped == null) continue;
            saveData.equippedItems.Add(new EquippedItemSaveData
            {
                slot = equipped.itemType,
                itemName = equipped.itemName,
                itemRarity = equipped.rarity
            });
        }

        Debug.Log($"PlayerInventory SaveState called. Data to be saved: \n" +
                  $"Gold: {saveData.gold}\n" +
                  $"Inventory Items Count: {saveData.inventoryItems.Count}\n" +
                  $"Equipped Items Count: {saveData.equippedItems.Count}");

        for (int i = 0; i < saveData.inventoryItems.Count; i++)
        {
            var item = saveData.inventoryItems[i];
            Debug.Log($"Item {i + 1}: Name = {item.itemName}, Quantity = {item.quantity}, Type = {item.itemType}");
        }

        return saveData;
    }

    public void LoadState(object state)
    {
        if (!(state is PlayerInventorySaveData saveData))
        {
            Debug.LogError($"Estado de carga inválido para PlayerInventory. Tipo recibido: {state?.GetType()}");
            return;
        }

        this.gold = saveData.gold;

        if (inventory == null) inventory = new List<GearItem>();
        inventory.Clear();

        if (saveData.inventoryItems != null)
        {
            foreach (var itemData in saveData.inventoryItems)
            {
                GearItem item = ConvertSaveDataToGearItem(itemData);
                if (item != null) inventory.Add(item);
            }
        }

        RestoreEquipment(saveData.equippedItems);

        InventoryUpdated?.Invoke();
    }

    private GearItemSaveData ConvertGearItemToSaveData(GearItem gearItem)
    {
        if (gearItem == null) return null;

        return new GearItemSaveData
        {
            quantity = gearItem.quantity,
            itemName = gearItem.itemName,
            itemRarity = gearItem.rarity,
            hp = gearItem.hp,
            mana = gearItem.mana,
            movementSpeed = gearItem.movementSpeed,
            criticalChance = gearItem.criticalChance,
            criticalBonus = gearItem.criticalBonus,
            physicalAttack = gearItem.physicalAttack,
            demonAttack = gearItem.demonAttack,
            hollyAttack = gearItem.hollyAttack,
            physicalDefense = gearItem.physicalDefense,
            demonDefense = gearItem.demonDefense,
            hollyDefense = gearItem.hollyDefense,
            weight = gearItem.weight,
            hpRegen = gearItem.hpRegen,
            manaRegen = gearItem.manaRegen,
            effectDuration = gearItem.effectDuration,
            itemType = gearItem.itemType
        };
    }

    private GearItem ConvertSaveDataToGearItem(GearItemSaveData saveData)
    {
        if (saveData == null) return null;

        return new GearItem(
            saveData.itemName,
            saveData.itemRarity,
            saveData.hp,
            saveData.mana,
            saveData.movementSpeed,
            saveData.criticalChance,
            saveData.criticalBonus,
            saveData.physicalAttack,
            saveData.demonAttack,
            saveData.hollyAttack,
            saveData.physicalDefense,
            saveData.demonDefense,
            saveData.hollyDefense,
            saveData.weight,
            saveData.hpRegen,
            saveData.manaRegen,
            saveData.effectDuration,
            saveData.itemType
        ) { quantity = saveData.quantity };
    }

    // ------------------------------------------------------------------
    // Equipo (lectura/escritura sobre Player.gear)
    // ------------------------------------------------------------------

    private void EnsurePlayer()
    {
        if (player == null) player = GetComponent<Player>();
    }

    private GearItem[] GetEquippedItems()
    {
        EnsurePlayer();
        PlayerGear g = player != null ? player.gear : null;
        if (g == null) return new GearItem[0];

        return new GearItem[]
        {
            g.helmet, g.armor, g.legs, g.boots, g.weapon, g.shield, g.ring, g.amulet, g.holder
        };
    }

    /// <summary>True si este item (misma instancia) está puesto en algún slot.</summary>
    public bool IsEquipped(GearItem item)
    {
        if (item == null) return false;
        foreach (GearItem equipped in GetEquippedItems())
        {
            if (ReferenceEquals(equipped, item)) return true;
        }
        return false;
    }

    private void UnequipIfEquipped(GearItem item)
    {
        if (!IsEquipped(item)) return;

        PlayerGear g = player.gear;
        if (ReferenceEquals(g.helmet, item)) g.helmet = null;
        if (ReferenceEquals(g.armor, item)) g.armor = null;
        if (ReferenceEquals(g.legs, item)) g.legs = null;
        if (ReferenceEquals(g.boots, item)) g.boots = null;
        if (ReferenceEquals(g.weapon, item)) g.weapon = null;
        if (ReferenceEquals(g.shield, item)) g.shield = null;
        if (ReferenceEquals(g.ring, item)) g.ring = null;
        if (ReferenceEquals(g.amulet, item)) g.amulet = null;
        if (ReferenceEquals(g.holder, item)) g.holder = null;

        // Recalcula los stats del jugador (Player se suscribe a este evento)
        g.EquipmentChanged();
    }

    private void RestoreEquipment(List<EquippedItemSaveData> equipped)
    {
        EnsurePlayer();
        if (player == null) return;
        if (player.gear == null) player.gear = new PlayerGear();

        // Limpia los slots sin recalcular (se recalcula al equipar cada pieza)
        PlayerGear g = player.gear;
        g.helmet = null; g.armor = null; g.legs = null; g.boots = null; g.weapon = null;
        g.shield = null; g.ring = null; g.amulet = null; g.holder = null;

        if (equipped == null) return;

        foreach (EquippedItemSaveData e in equipped)
        {
            if (e == null) continue;

            GearItem item = inventory.Find(i => i != null
                                             && i.itemName == e.itemName
                                             && i.rarity == e.itemRarity
                                             && i.itemType == e.slot);
            if (item != null)
            {
                player.UpdateGear(item); // asigna al slot según item.itemType y recalcula stats
            }
            else
            {
                Debug.LogWarning($"PlayerInventory: no se encontró '{e.itemName}' ({e.itemRarity}) para re-equipar.");
            }
        }
    }

    // ------------------------------------------------------------------
    // Interfaz de inventario (lista de slots)
    // ------------------------------------------------------------------

    /// <summary>
    /// Crea un slot de UI por cada item del inventario. Los items equipados descuentan 1 de la
    /// cantidad mostrada (si era el único, no aparecen). No modifica el inventario.
    /// </summary>
    public void ListItems(List<GearItem> items, GameObject slot, GameObject inventoryContent)
    {
        if (items == null || slot == null || inventoryContent == null) return;

        // Limpiar lo que hubiera
        foreach (Transform child in inventoryContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (GearItem inventoryItem in items)
        {
            if (inventoryItem == null) continue;

            int shownQuantity = Mathf.Max(1, inventoryItem.quantity) - (IsEquipped(inventoryItem) ? 1 : 0);
            if (shownQuantity <= 0) continue;

            CreateSlotUI(inventoryItem, shownQuantity, slot, inventoryContent);
        }
    }

    public void ListItem(GearItem gearItem, GameObject slot, GameObject inventoryContent)
    {
        if (gearItem == null || slot == null || inventoryContent == null) return;
        CreateSlotUI(gearItem, Mathf.Max(1, gearItem.quantity), slot, inventoryContent);
    }

    private GameObject CreateSlotUI(GearItem item, int displayedQuantity, GameObject slot, GameObject content)
    {
        GameObject obj = Instantiate(slot, content.transform);

        Transform nameTransform = obj.transform.Find("ItemName");
        TMP_Text itemNameText = nameTransform != null ? nameTransform.GetComponent<TMP_Text>() : null;

        if (itemNameText != null)
        {
            itemNameText.text = item.itemName + " x" + displayedQuantity;
        }
        else
        {
            Debug.LogWarning("PlayerInventory: el prefab de slot no tiene un hijo 'ItemName' con TMP_Text.");
        }

        return obj;
    }

    /// <summary>(Legado) Copia al índice dado de GearMenu.equippedGear el item con ese nombre.</summary>
    public void CheckForItem(string item, int index)
    {
        if (gearMenu == null || gearMenu.equippedGear == null || index < 0 || index >= gearMenu.equippedGear.Count) return;

        foreach (GearItem inventoryItem in inventory)
        {
            if (inventoryItem != null && inventoryItem.itemName == item)
            {
                gearMenu.equippedGear[index] = inventoryItem;
            }
        }
    }

    // ------------------------------------------------------------------
    // Operaciones de inventario
    // ------------------------------------------------------------------

    private GearItem FindInInventory(GearItem item)
    {
        if (item == null) return null;
        if (inventory.Contains(item)) return item;
        return inventory.Find(i => i != null && i.itemName == item.itemName && i.rarity == item.rarity);
    }

    public void AddItemToInventory(GearItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("AddItemToInventory: item nulo.");
            return;
        }

        Debug.Log("Attempting to add item to inventory: " + item.itemName);

        GearItem existing = inventory.Find(i => i != null && i.itemName == item.itemName && i.rarity == item.rarity);

        if (existing != null)
        {
            // Ya existe: incrementar la cantidad
            existing.quantity++;
            Debug.Log("Item '" + item.itemName + "' quantity increased to " + existing.quantity + ".");

            // NOTA: comportamiento original conservado. El contador de misiones solo se avisa en
            // recogidas repetidas, no en la primera. Revisar con Counter.cs si es lo deseado.
            if (counter != null) counter.ItemCollected(item.itemName);
        }
        else
        {
            // Nuevo: agregarlo con cantidad inicial 1
            item.quantity = 1;
            inventory.Add(item);
            Debug.Log("Item '" + item.itemName + "' added to inventory.");
        }

        InventoryUpdated?.Invoke();
    }

    public void RemoveItemFromInventory(GearItem item)
    {
        GearItem found = FindInInventory(item);

        if (found != null)
        {
            UnequipIfEquipped(found);
            inventory.Remove(found);
            Debug.Log("Item '" + found.itemName + "' removed from inventory.");
        }
        else
        {
            Debug.Log("Item '" + (item != null ? item.itemName : "null") + "' not found in inventory.");
        }

        InventoryUpdated?.Invoke();
    }

    public bool HasItem(GearItem item)
    {
        if (item == null) return false;

        foreach (GearItem inventoryItem in inventory)
        {
            if (inventoryItem != null && inventoryItem.itemName == item.itemName && inventoryItem.rarity == item.rarity)
            {
                return true;
            }
        }
        return false;
    }

    public void UseItem(GearItem item)
    {
        GearItem found = FindInInventory(item);

        if (found == null)
        {
            Debug.Log("Item '" + (item != null ? item.itemName : "null") + "' not found in inventory.");
            return;
        }

        if (found.quantity > 1)
        {
            found.quantity--;
            Debug.Log("Used one " + found.itemName + ". Remaining quantity: " + found.quantity);
        }
        else
        {
            UnequipIfEquipped(found);
            inventory.Remove(found);
            Debug.Log("Used the last " + found.itemName + ". Item removed from inventory.");
        }

        InventoryUpdated?.Invoke();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log("Added " + amount + " gold. Total gold: " + gold);
    }

    public void RemoveGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            Debug.Log("Removed " + amount + " gold. Total gold: " + gold);
        }
        else
        {
            Debug.Log("Insufficient gold.");
        }
    }

    public int GetGoldAmount()
    {
        return gold;
    }

    public void PrintInventory()
    {
        Debug.Log("Inventory contents:");
        foreach (GearItem item in inventory)
        {
            if (item == null) continue;
            Debug.Log("- " + item.itemName + " x" + item.quantity + " (Rarity: " + item.rarity + ")");
        }
    }

    public GearItem GetGearItemByName(string itemName)
    {
        foreach (GearItem inventoryItem in inventory)
        {
            if (inventoryItem != null && inventoryItem.itemName == itemName)
            {
                return inventoryItem;
            }
        }
        return null;
    }

    // ------------------------------------------------------------------
    // SOLO PRUEBAS: clic derecho en el componente (con el juego corriendo) > DEBUG
    // Agrega una copia de un item de cada tipo tomada de GearItemManager.gearItems.
    // Se puede borrar cuando ya no se necesite.
    // ------------------------------------------------------------------

    [ContextMenu("DEBUG - Agregar un item de cada tipo (prueba)")]
    private void DebugAddTestItems()
    {
        if (gearItemManager == null || gearItemManager.gearItems == null || gearItemManager.gearItems.Count == 0)
        {
            Debug.LogWarning("DEBUG: GearItemManager no tiene items en su lista 'gearItems'.");
            return;
        }

        var addedTypes = new HashSet<GearItem.ItemType>();
        foreach (GearItem template in gearItemManager.gearItems)
        {
            if (template == null || addedTypes.Contains(template.itemType)) continue;

            GearItem copy = gearItemManager.GetDefaultItem(template.itemName);
            if (copy == null) continue;

            AddItemToInventory(copy);
            addedTypes.Add(template.itemType);
        }

        Debug.Log("DEBUG: items de prueba agregados: " + addedTypes.Count);
    }
}

[System.Serializable]
public class GearItem
{
    // NO cambiar el orden de estos enums: los guardados y prefabs guardan el número.
    public enum Rarity { Gold, Common, Uncommon, SemiRare, Rare, SuperRare, UltraRare, Hero, Epic, Mythological, Legendary, Dreamer }
    public enum ItemType { Weapon, Shield, Helmet, Armor, Legs, Boots, Ring, Amulet, Holder, Consumable }

    public int quantity;
    public string itemName;
    public Rarity rarity;
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
    public int hpRegen;
    public int manaRegen;
    public float effectDuration;
    public ItemType itemType;

    public GearItem(string itemName, Rarity rarity, int hp, int mana, float movementSpeed, float criticalChance, float criticalBonus,
        int physicalAttack, int demonAttack, int hollyAttack, int physicalDefense, int demonDefense, int hollyDefense, float weight, int hpRegen, int manaRegen, float effectDuration, ItemType itemType)
    {
        this.itemName = itemName;
        this.rarity = rarity;
        this.hp = hp;
        this.mana = mana;
        this.movementSpeed = movementSpeed;
        this.criticalChance = criticalChance;
        this.criticalBonus = criticalBonus;
        this.physicalAttack = physicalAttack;
        this.demonAttack = demonAttack;
        this.hollyAttack = hollyAttack;
        this.physicalDefense = physicalDefense;
        this.demonDefense = demonDefense;
        this.hollyDefense = hollyDefense;
        this.weight = weight;
        this.hpRegen = hpRegen;
        this.manaRegen = manaRegen;
        this.effectDuration = effectDuration;
        this.itemType = itemType;
    }

    /// <summary>Copia independiente del item (incluye la cantidad).</summary>
    public GearItem Clone()
    {
        return new GearItem(itemName, rarity, hp, mana, movementSpeed, criticalChance, criticalBonus,
            physicalAttack, demonAttack, hollyAttack, physicalDefense, demonDefense, hollyDefense,
            weight, hpRegen, manaRegen, effectDuration, itemType) { quantity = quantity };
    }

    /// <summary>Mismo item lógico (nombre y rareza), aunque sea otra instancia.</summary>
    public bool IsSameItem(GearItem other)
    {
        return other != null && itemName == other.itemName && rarity == other.rarity;
    }
}