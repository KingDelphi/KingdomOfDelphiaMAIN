using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class PlayerInventory : MonoBehaviour, ISaveable
{
    public string ID => GetInstanceID().ToString(); //SAVELOAD
    public Type SaveType => typeof(PlayerInventorySaveData); // Tipo de estado guardado


    [System.Serializable]
    public class PlayerInventorySaveData
    {
        public List<GearItemSaveData> inventoryItems;
        public int gold;
    }

    public GearItemManager gearItemManager;
    public GearMenu gearMenu;
    
    // Inventario
    public List<GearItem> inventory;

    // Define un delegado para el evento
    public Action InventoryUpdated;

    // Oro
    public int gold;

    public Counter counter;
    public GameManager gameManager;

// Comienza Implementación de ISaveable en el script PlayerInventory

    [System.Serializable]
    public class GearItemSaveData
    {
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



    public object SaveState()
    {
        var saveData = new PlayerInventorySaveData
        {
            gold = this.gold,
            inventoryItems = this.inventory.ConvertAll(item => ConvertGearItemToSaveData(item))
        };

        // Agrega el Debug.Log para mostrar los datos que se están guardando
        Debug.Log($"PlayerInventory SaveState called. Data to be saved: \n" +
                $"Gold: {saveData.gold}\n" +
                $"Inventory Items Count: {saveData.inventoryItems.Count}");

        for (int i = 0; i < saveData.inventoryItems.Count; i++)
        {
            var item = saveData.inventoryItems[i];
            Debug.Log($"Item {i + 1}: Name = {item.itemName}, Type = {item.itemType}");
        }

        return saveData;
    }

    public void LoadState(object state)
{
    if (state is PlayerInventorySaveData saveData)
    {
        this.gold = saveData.gold;
        this.inventory = saveData.inventoryItems.ConvertAll(item => ConvertSaveDataToGearItem(item));
        InventoryUpdated?.Invoke();
    }
    else
    {
        Debug.LogError($"Estado de carga inválido para PlayerInventory. Tipo recibido: {state.GetType()}");
    }
}

    private GearItemSaveData ConvertGearItemToSaveData(GearItem gearItem)
    {
        if (gearItem == null) return null;

        return new GearItemSaveData
        {
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
        );
    }

        // Termina Implementación de ISaveable en el script PlayerInventory


    private void Start()
    {
        // Inicializar inventario, oro y gear
        inventory = new List<GearItem>();
        gold = 0;
    }
    
    void Awake() {
        gearItemManager = FindObjectOfType<GearItemManager>();
        gameManager = FindObjectOfType<GameManager>();
        gearMenu = FindObjectOfType<GearMenu>(true);
    }
    
    public void ListItems(List<GearItem> inventory, GameObject slot, GameObject inventoryContent) {
        List<string> strings = new List<String>();
        List<GearItem> gearItems = new List<GearItem>();
        List<GameObject> gameObjects = new List<GameObject>(); 
        foreach(GearItem inventoryItem in inventory) {
            
            if(gearMenu.equippedGear.Contains(inventoryItem)) {

            } else {
             GameObject obj = Instantiate(slot, inventoryContent.transform);
             
             
             var itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
             itemName.text = inventoryItem.itemName + " x" + inventoryItem.quantity;
             foreach(GearItem newItem in gearItems) {
                if(newItem.itemName == inventoryItem.itemName) {
                    foreach(GameObject go in gameObjects) {
                        if(newItem.itemName + " x1" == go.transform.Find("ItemName").GetComponent<TMP_Text>().text) {
                            Destroy(go);
                        }
                    }
                    inventory.Remove(newItem);
                    inventoryItem.quantity += 1;
                    itemName.text = inventoryItem.itemName + " x" + inventoryItem.quantity;
                }
             }
             gameObjects.Add(obj);
             gearItems.Add(inventoryItem);
             strings.Add(inventoryItem.itemName);
            }
            
        }
    }
        public void ListItem(GearItem gearItem, GameObject slot, GameObject inventoryContent) {
        List<string> strings = new List<String>();
        List<GearItem> gearItems = new List<GearItem>();
        List<GameObject> gameObjects = new List<GameObject>(); 
             GameObject obj = Instantiate(slot, inventoryContent.transform);
             var itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
             itemName.text = gearItem.itemName + " x" + gearItem.quantity;
             foreach(GearItem newItem in gearItems) {
                if(newItem.itemName == gearItem.itemName) {
                    foreach(GameObject go in gameObjects) {
                        if(newItem.itemName + " x1" == go.transform.Find("ItemName").GetComponent<TMP_Text>().text) {
                            Destroy(go);
                        }
                    }
                    inventory.Remove(newItem);
                    gearItem.quantity += 1;
                    itemName.text = gearItem.itemName + " x" + gearItem.quantity;
                }
                gameObjects.Add(obj);
             gearItems.Add(gearItem);
             strings.Add(gearItem.itemName);
             }
             
            
            
        }
    public void CheckForItem(string item, int index) {
        foreach(GearItem inventoryItem in inventory) {
            if(inventoryItem.itemName == item){
             gearMenu.equippedGear[index] = inventoryItem;
            }
        }
    }

    public void AddItemToInventory(GearItem item)
    {
        Debug.Log("Attempting to add item to inventory: " + item.itemName);

        
        // Buscar si el objeto ya existe en el inventario
        foreach (GearItem inventoryItem in inventory)
        {
            if (inventoryItem.itemName == item.itemName && inventoryItem.rarity == item.rarity)
            {
                // El objeto ya existe en el inventario, incrementar la cantidad
                inventoryItem.quantity++;
                Debug.Log("Item '" + item.itemName + "' quantity increased to " + inventoryItem.quantity + ".");
                counter.ItemCollected(item.itemName);
                return;
            }
        }

        // El objeto no existe en el inventario, agregarlo como nuevo
        item.quantity = 1; // Establecer la cantidad inicial en 1
        inventory.Add(item);
        Debug.Log("Item '" + item.itemName + "' added to inventory.");
        
        InventoryUpdated?.Invoke();
    }

    public void RemoveItemFromInventory(GearItem item)
    {
        // Eliminar un objeto del inventario
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            Debug.Log("Item '" + item.itemName + "' removed from inventory.");
        }
        else
        {
            Debug.Log("Item '" + item.itemName + "' not found in inventory.");
        }

        InventoryUpdated?.Invoke();
    }

    public bool HasItem(GearItem item)
    {
        Debug.Log(item + "AQUI NOS DICE CUAL ITEM ESTA TOMANDO LA FUNCION HasItem");
        // Verificar si el inventario contiene un objeto
        if (item != null)
        {
            Debug.Log("Item no es nulo.");
            foreach (GearItem inventoryItem in inventory)
            {
                Debug.Log("Foreach corre de manera exitosa.");
                if (inventoryItem.itemName == item.itemName && inventoryItem.rarity == item.rarity)
                {
                    Debug.Log("Item encontrado.");
                    return true;
                }
            }
        }
        else
        {
            Debug.Log("Item es nulo.");
        }

        Debug.Log("Item no encontrado.");
        return false;
    }

    public void UseItem(GearItem item)
    {
        // Verificar si el objeto existe en el inventario
        if (inventory.Contains(item))
        {
            // Verificar la cantidad del objeto
            if (item.quantity > 1)
            {
                // Si la cantidad es mayor que 1, disminuir la cantidad en 1
                item.quantity--;
                Debug.Log("Used one " + item.itemName + ". Remaining quantity: " + item.quantity);
            }
            else
            {
                // Si la cantidad es 1, eliminar el objeto del inventario
                inventory.Remove(item);
                Debug.Log("Used the last " + item.itemName + ". Item removed from inventory.");
            }
            
            // Agrega aquí la lógica correspondiente al uso del objeto
        }
        else
        {
            Debug.Log("Item '" + item.itemName + "' not found in inventory.");
        }
    }

    public void AddGold(int amount)
    {
        // Añadir oro al personaje
        gold += amount;
        Debug.Log("Added " + amount + " gold. Total gold: " + gold);
    }

    public void RemoveGold(int amount)
    {
        // Remover oro del personaje
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
        // Obtener la cantidad de oro actual
        return gold;
    }

    public void PrintInventory()
    {
        // Imprimir el contenido del inventario
        Debug.Log("Inventory contents:");
        foreach (GearItem item in inventory)
        {
            Debug.Log("- " + item.itemName + " (Rarity: " + item.rarity + ")");
        }
    }

    public GearItem GetGearItemByName(string itemName)
    {
        foreach (GearItem inventoryItem in inventory)
        {
            if (inventoryItem.itemName == itemName)
            {
                return inventoryItem;
            }
        }

        return null;
    }
}

[System.Serializable]
public class GearItem
{
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
}