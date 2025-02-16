using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GearCategory
{
    public string categoryName;
    public List<GearItem> gearItems;
}

public class GearItemManager : MonoBehaviour
{
    public List<GearCategory> gearCategories;
    public List<GearItem> gearItems = new List<GearItem>();
    public static GearItemManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        // Acceder a los GearItems en cada categoría
        foreach (GearCategory category in gearCategories)
        {
            string categoryName = category.categoryName;
            List<GearItem> gearItems = category.gearItems;

            // Realizar operaciones con los GearItems de la categoría
            foreach (GearItem gearItem in gearItems)
            {
                // Acceder a las propiedades del GearItem
                string itemName = gearItem.itemName;
                float weight = gearItem.weight;
            }
        }
    }

    public GearItem GetGearItemByName(string itemName)
    {
        return gearItems.Find(item => item.itemName == itemName);
    }

    public GearItem GetDefaultItem(string itemName)
    {
        // Buscar el GearItem en la lista de gearItems
        GearItem gearItem = gearItems.Find(item => item.itemName == itemName);

        // Si se encuentra el GearItem, devolver una copia con las estadísticas predeterminadas
        if (gearItem != null)
        {
            // Crear una nueva instancia de GearItem con las estadísticas predeterminadas
            GearItem defaultItem = new GearItem(
            gearItem.itemName,
            gearItem.rarity,
            gearItem.hp,
            gearItem.mana,
            gearItem.movementSpeed,
            gearItem.criticalChance,
            gearItem.criticalBonus,
            gearItem.physicalAttack,
            gearItem.demonAttack,
            gearItem.hollyAttack,
            gearItem.physicalDefense,
            gearItem.demonDefense,
            gearItem.hollyDefense,
            gearItem.weight,
            gearItem.hpRegen,
            gearItem.manaRegen,
            gearItem.effectDuration,
            gearItem.itemType
            );
            // Asignar las estadísticas predeterminadas al nuevo GearItem
            defaultItem.hp = gearItem.hp;
            defaultItem.mana = gearItem.mana;
            defaultItem.movementSpeed = gearItem.movementSpeed;
            defaultItem.criticalChance = gearItem.criticalChance;
            defaultItem.criticalBonus = gearItem.criticalBonus;
            defaultItem.physicalAttack = gearItem.physicalAttack;
            defaultItem.demonAttack = gearItem.demonAttack;
            defaultItem.hollyAttack = gearItem.hollyAttack;
            defaultItem.physicalDefense = gearItem.physicalDefense;
            defaultItem.demonDefense = gearItem.demonDefense;
            defaultItem.hollyDefense = gearItem.hollyDefense;
            defaultItem.weight = gearItem.weight;
            defaultItem.hpRegen = gearItem.hpRegen;
            defaultItem.manaRegen = gearItem.manaRegen;
            defaultItem.effectDuration = gearItem.effectDuration;
            defaultItem.itemType = gearItem.itemType;

            return defaultItem;
        }

        // Si no se encuentra el GearItem, devolver null
        return null;
    }
}