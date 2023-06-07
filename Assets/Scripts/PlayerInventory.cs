using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public GearItemManager gearItemManager;
    public GearMenu gearMenu;
    
    // Inventario
    public List<GearItem> inventory;

    // Oro
    public int gold;

    // Gear
    public Gear gear;

    public Counter counter;
    public GameManager gameManager;
    private void Start()
    {
        // Inicializar inventario, oro y gear
        inventory = new List<GearItem>();
        gold = 0;
        gear = new Gear();
    }
    
    void Awake() {
        gearItemManager = FindObjectOfType<GearItemManager>();
        gameManager = FindObjectOfType<GameManager>();
        gearMenu = FindObjectOfType<GearMenu>(true);
    }
    public void ListItems(List<GearItem> inventory, GameObject slot, GameObject inventoryContent) {
        //gearMenu = GameObject.Find("Gears").GetComponent<GearMenu>();
        foreach(GearItem inventoryItem in inventory) {
            if(gearMenu.equippedGear.Contains(inventoryItem)) {

            } else {
             GameObject obj = Instantiate(slot, inventoryContent.transform);
             var itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
             itemName.text = inventoryItem.itemName + " x" + inventoryItem.quantity;
            }
            
        }
    }
        public void ListItem(GearItem gearItem, GameObject slot, GameObject inventoryContent) {
        //gearMenu = GameObject.Find("Gears").GetComponent<GearMenu>();
             GameObject obj = Instantiate(slot, inventoryContent.transform);
             var itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
             itemName.text = gearItem.itemName + " x" + gearItem.quantity;
            
        }
    public void CheckForItem(string item, int index) {
        foreach(GearItem inventoryItem in inventory) {
            if(inventoryItem.itemName == item){
             ListItem(gearMenu.equippedGear[index], gameManager.slot, gameManager.inventoryCanvas.transform.GetChild(0).gameObject);
             gearMenu.equippedGear[index] = inventoryItem;
            }
        }
    }

    public void AddItemToInventory(GearItem item)
    {
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
        //counter.ItemCollected(item);
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
public class Gear
{
    public GearItem holder;
    public GearItem helmet;
    public GearItem armor;
    public GearItem legs;
    public GearItem boots;
    public GearItem weapon;
    public GearItem shield;
    public GearItem ring;
    public GearItem amulet;

    public event Action OnEquipmentChanged;

    public void EquipmentChanged()
    {
        if (OnEquipmentChanged != null)
        {
            OnEquipmentChanged.Invoke();
        }
    }
}

[System.Serializable]
public class GearItem
{
    public enum Rarity { Gold, Common, Uncommon, SemiRare, Rare, SuperRare, UltraRare, Hero, Epic, Mythological, Legendary, Dreamer }
    public enum ItemType { Helmet, Armor, Legs, Boots, Weapon, Shield, Ring, Amulet, Holder, Consumable }
    public int quantity;
    public string itemName;
    public Rarity rarity;
    public ItemType itemType;
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

    public GearItem(string itemName, Rarity rarity, int hp, int mana, float movementSpeed, float criticalChance, float criticalBonus,
        int physicalAttack, int demonAttack, int hollyAttack, int physicalDefense, int demonDefense, int hollyDefense, float weight, int hpRegen, int manaRegen, float effectDuration, ItemType itemType = ItemType.Helmet)
    {
        this.itemName = itemName;
        this.rarity = rarity;
        this.itemType = itemType;
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
    }
}