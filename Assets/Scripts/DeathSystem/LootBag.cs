using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LootBag : NetworkBehaviour
{
    public List<GearItem> items = new List<GearItem>();
    public int goldAmount = 0;
    private float despawnTime = 900f; // 15 minutos

    public SpriteRenderer spriteRenderer;
    public Sprite defaultBackpackSprite;
    public Sprite backpackSprite;
    public Sprite greatBackpackSprite;

    private Dictionary<string, Sprite> backpackSprites = new Dictionary<string, Sprite>();

    private bool isPlayerNearby = false;
    private PlayerInventory interactingPlayer;

    void Start()
    {
        backpackSprites.Add("Backpack", backpackSprite);
        backpackSprites.Add("Great Backpack", greatBackpackSprite);
    }

    public void SetBackpackSprite(string backpackName)
    {
        if (backpackSprites.TryGetValue(backpackName, out Sprite selectedSprite))
        {
            spriteRenderer.sprite = selectedSprite;
        }
        else
        {
            spriteRenderer.sprite = defaultBackpackSprite;
        }
    }

    private void Update()
    {
        if (isPlayerNearby && interactingPlayer != null && Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Looting the backpack...");
            Loot(interactingPlayer);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactingPlayer = collision.GetComponent<PlayerInventory>();
            if (interactingPlayer != null)
            {
                isPlayerNearby = true;
                Debug.Log("Player is near the LootBag. Press 'L' to loot.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactingPlayer = null;
        }
    }

    public static LootBag CreateLootBag(NetworkRunner runner, Vector3 position, List<GearItem> droppedItems, int droppedGold, string backpackName)
    {
        Debug.Log($"Creating LootBag with sprite for: {backpackName}");

        GameObject prefab = Resources.Load<GameObject>("Prefabs/LootBag");
        if (prefab == null)
        {
            Debug.LogError("Error crítico: No se encontró el prefab LootBag en Resources.");
            return null;
        }

        NetworkObject lootBagObject = runner.Spawn(prefab, position, Quaternion.identity);
        if (lootBagObject.TryGetComponent(out LootBag lootBag))
        {
            lootBag.Initialize(droppedItems, droppedGold);
            lootBag.SetBackpackSprite(backpackName);
            return lootBag;
        }

        Debug.LogError("Failed to create LootBag object!");
        return null;
    }

    public void Initialize(List<GearItem> droppedItems, int droppedGold)
    {
        items = new List<GearItem>(droppedItems);
        goldAmount = droppedGold;
        StartCoroutine(DespawnAfterTime());
    }

    public void Loot(PlayerInventory playerInventory)
    {
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory is null!");
            return;
        }

        foreach (GearItem item in items)
        {
            playerInventory.AddItemToInventory(item);
        }
        playerInventory.AddGold(goldAmount);

        Debug.Log($"Player looted {items.Count} items and {goldAmount} gold.");

        Runner.Despawn(Object);
    }

    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(despawnTime);
        if (Object != null && Object.IsValid)
        {
            Runner.Despawn(Object);
        }
    }
}
 