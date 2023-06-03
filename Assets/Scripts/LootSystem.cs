using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum Rarity
{
    Gold,
    Food,
    Common,
    Uncommon,
    SemiRare,
    Rare,
    SuperRare,
    UltraRare,
    Hero,
    Epic,
    Mythological,
    Legendary,
    Dreamer
}

[System.Serializable]
public class LootItem
{
    public Color rarityColor; // Color de la rareza
    public string itemName;
    public Rarity rarity;
    public float dropProbability; // Probabilidad de que el ítem sea tirado
    public int minQuantity;
    public int maxQuantity;
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
}

public class LootSystem : MonoBehaviour
{
    public LootItem[] lootItems;
    public GameObject lootTextPrefab;
    public Color lootTextColor = Color.white;

    private List<string> lootInfoList = new List<string>(); // Lista de información de loot

    private Dictionary<string, Rarity> rarityMapping = new Dictionary<string, Rarity>();

    private void Start()
    {
        // Crear el mapeo de cadenas de rareza a valores de enumeración Rarity
        foreach (Rarity rarity in System.Enum.GetValues(typeof(Rarity)))
        {
            rarityMapping.Add(rarity.ToString(), rarity);
        }
    }

    public void OnMonsterDeath()
    {
        lootInfoList.Clear(); // Limpiar la lista de información de loot

        foreach (LootItem lootItem in lootItems)
        {
            // Verificar si el ítem es tirado basado en su probabilidad de drop
            if (UnityEngine.Random.value <= lootItem.dropProbability)
            {
                int quantity = UnityEngine.Random.Range(lootItem.minQuantity, lootItem.maxQuantity + 1); // Generar cantidad aleatoria dentro del rango
                string lootInfo = string.Format("({0}) {1} X {2}", lootItem.rarity, lootItem.itemName, quantity);
                lootInfoList.Add(lootInfo); // Agregar la información de loot a la lista

                // Agregar la cantidad correcta de GearItem al inventario del jugador
                PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
                for (int i = 0; i < quantity; i++)
                {
                    GearItem gearItem = new GearItem(lootItem.itemName, (GearItem.Rarity)lootItem.rarity, lootItem.hp, lootItem.mana, lootItem.movementSpeed,
                        lootItem.criticalChance, lootItem.criticalBonus, lootItem.physicalAttack, lootItem.demonAttack, lootItem.hollyAttack,
                        lootItem.physicalDefense, lootItem.demonDefense, lootItem.hollyDefense, lootItem.weight, lootItem.hpRegen,
                        lootItem.manaRegen, lootItem.effectDuration);

                    inventory.AddItemToInventory(gearItem);
                    Debug.Log("inventory.AddItemToInventory(gearItem); llamado exitosamente");
                }
            }
        }

        ShowLootTexts(); // Mostrar los textos de loot en forma de lista
    }

    private void ShowLootTexts()
    {
        // Calcular la posición vertical del primer texto de loot por encima del monstruo
        Vector3 lootTextPosition;

        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (circleCollider != null)
        {
            // Calcular la posición vertical del primer texto de loot por encima del monstruo con CircleCollider2D
            lootTextPosition = transform.position + Vector3.up * 1.5f;
        }
        else if (capsuleCollider != null)
        {
            // Calcular la posición vertical del primer texto de loot por encima del monstruo con CapsuleCollider2D
            float yOffset = capsuleCollider.size.y * 0.5f;
            lootTextPosition = capsuleCollider.bounds.max + Vector3.up * yOffset;
        }
        else if (boxCollider != null)
        {
            // En caso de que no se encuentre ningún collider, usar una posición predeterminada
            lootTextPosition = transform.position + Vector3.up * 1.5f;
        }
        else
        {
            lootTextPosition = transform.position + Vector3.up * 1.5f;
        }

        // Ordenar la lista de información de loot por rareza
        lootInfoList.Sort((a, b) =>
        {
            Rarity rarityA = GetRarityFromLootInfo(a);
            Rarity rarityB = GetRarityFromLootInfo(b);
            return rarityA.CompareTo(rarityB);
        });

        foreach (string lootInfo in lootInfoList)
        {
            // Instanciar el objeto de texto del loot y almacenar la referencia en lootTextObject
            GameObject lootTextObject = Instantiate(lootTextPrefab, lootTextPosition, Quaternion.identity);

            // Obtener el componente TextMeshPro del objeto de texto del loot
            TextMeshPro lootText = lootTextObject.GetComponent<TextMeshPro>();

            // Obtener la rareza del texto de loot
            Rarity rarity = GetRarityFromLootInfo(lootInfo);

            // Obtener el color correspondiente a la rareza del lootItem
            Color rarityColor = GetColorFromRarity(rarity);

            // Establecer el color del texto del loot
            lootText.color = rarityColor;

            // Mostrar la información del loot en el texto
            lootText.text = lootInfo;

            StartCoroutine(MoveAndFadeOutLootText(lootTextObject, lootTextPosition + Vector3.up)); // Iniciar la corutina para animar el movimiento hacia arriba y desaparición del texto del loot

            lootTextPosition += Vector3.up * 0.5f; // Aumentar la posición vertical para el siguiente texto de loot en la lista
        }
    }

    private Color GetColorFromRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Gold:
                return Color.yellow;
            case Rarity.Common:
                return Color.white;
            case Rarity.Uncommon:
                return new Color(0.5f, 0.5f, 0.5f); // Gris medio
            case Rarity.SemiRare:
                return Color.cyan;
            case Rarity.Rare:
                return Color.blue;
            case Rarity.SuperRare:
                return new Color(0.25f, 0.75f, 0.5f); // Verde claro
            case Rarity.UltraRare:
                return Color.green;
            case Rarity.Hero:
                return Color.red;
            case Rarity.Epic:
                return Color.magenta;
            case Rarity.Mythological:
                return new Color(0.5f, 0.25f, 0.75f); // Morado
            case Rarity.Legendary:
                return new Color(0.75f, 0.5f, 0.25f); // Naranja
            case Rarity.Dreamer:
                return new Color(1f, 0.65f, 0f); // Dorado
            default:
                return Color.white;
        }
    }

    private Rarity GetRarityFromLootInfo(string lootInfo)
    {
        // Extraer la rareza del texto de loot
        int startIndex = lootInfo.IndexOf("(") + 1;
        int endIndex = lootInfo.IndexOf(")");
        string rarityString = lootInfo.Substring(startIndex, endIndex - startIndex);

        // Convertir la rareza a enum Rarity
        if (rarityMapping.TryGetValue(rarityString, out Rarity rarity))
        {
            return rarity;
        }

        // Si no se encuentra la rareza, devolver Rarity.Common como valor predeterminado
        return Rarity.Common;
    }

    private IEnumerator MoveAndFadeOutLootText(GameObject lootTextObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 1.2f;
        Vector3 initialPosition = lootTextObject.transform.position;

        while (elapsedTime < duration && lootTextObject != null) // Verificar si el objeto aún existe
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            lootTextObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            // Fade out del texto usando el canal alpha del color
            Color textColor = lootTextObject.GetComponent<TextMeshPro>().color;
            textColor.a = Mathf.Lerp(1f, 0f, t);
            lootTextObject.GetComponent<TextMeshPro>().color = textColor;

            yield return null;
        }

        // Destruir el objeto del loot después de la animación
        if (lootTextObject != null)
        {
            Destroy(lootTextObject);
        }
    }
}
