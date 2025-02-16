using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DeathSystem : NetworkBehaviour
{
    public bool isDead = false;
    public float experienceLossPercentage = 0.1f;
    public bool dropBackpack = true;
    public NetworkPrefabRef lootBagPrefab;
    public System.Action OnDeathUI;

    public enum ResurrectionType { Scroll, SoulStone, PhoenixEmblem, GuardianFeather, DemonContract }
    public Dictionary<ResurrectionType, System.Action> resurrectionMethods;

    void Start()
    {
        resurrectionMethods = new Dictionary<ResurrectionType, System.Action>
        {
            { ResurrectionType.Scroll, ReviveWithScroll },
            { ResurrectionType.SoulStone, ReviveAtLastNPC },
            { ResurrectionType.PhoenixEmblem, ReviveWithBuff },
            { ResurrectionType.GuardianFeather, ReduceDeathPenalty },
            { ResurrectionType.DemonContract, ReviveWithDebt }
        };
    }
    

    public void Die(PlayerInventory playerInventory)
{
    if (isDead) return;
    isDead = true;

    ApplyDeathPenalties(playerInventory);
    Debug.Log("Player has died. Waiting for resurrection...");

    // 🔥 Buscar la animación de muerte en los hijos
    DeathAnimationController animController = GetComponentInChildren<DeathAnimationController>();

    if (animController != null)
    {
        animController.PlayDeathAnimation();
    }
    else
    {
        Debug.LogError("❌ DeathAnimationController no encontrado en los hijos del personaje.");
    }


    // 🔥 Notificar a la red
    NotifyNearbyPlayers();
    SendDeathLog();
}


    void ApplyDeathPenalties(PlayerInventory playerInventory)
    {
        DropItems(playerInventory);
        if (dropBackpack) DropBackpack();
    }

    void DropItems(PlayerInventory playerInventory)
    {
        if (playerInventory == null || playerInventory.inventory.Count == 0) return;

        Vector3 dropPosition = transform.position + Vector3.right * Random.Range(-1f, 1f);
        GearMenu gearMenu = FindObjectOfType<GearMenu>();
        
        string backpackName = "DefaultBackpack";
        if (gearMenu != null && gearMenu.equippedGear.Count > 8)
        {
            GearItem equippedBackpack = gearMenu.equippedGear[8];
            if (equippedBackpack != null)
            {
                backpackName = equippedBackpack.itemName;
            }
        }

        LootBag lootBag = LootBag.CreateLootBag(Runner, dropPosition, new List<GearItem>(playerInventory.inventory), playerInventory.gold, backpackName);
        playerInventory.inventory.Clear();
        Debug.Log("Dropped all items and gold into a loot bag with backpack: " + backpackName);
    }

    void NotifyNearbyPlayers()
    {
        Debug.Log("Notifying nearby players about the death...");
        // Implementar lógica para enviar notificación solo a jugadores relevantes
    }

    void SendDeathLog()
    {
        Debug.Log("Logging death details...");
        // Guardar datos sobre la muerte (daño recibido, atacante, oro perdido, etc.)
    }

    public void Revive(ResurrectionType type)
    {
        if (resurrectionMethods.ContainsKey(type))
        {
            resurrectionMethods[type].Invoke();
            isDead = false;
            Debug.Log("Player revived using: " + type);
        }
    }

    public void OnResurrectSelected(DeathSystem.ResurrectionType type)
    {
        Debug.Log($"El jugador ha elegido revivir con: {type}");
        
        // Aquí puedes agregar lógica específica para cada opción de resurrección
        switch (type)
        {
            case DeathSystem.ResurrectionType.Scroll:
                Debug.Log("Usando Pergamino del Renacimiento...");
                break;

            case DeathSystem.ResurrectionType.SoulStone:
                Debug.Log("Usando Piedra del Alma...");
                break;

            case DeathSystem.ResurrectionType.PhoenixEmblem:
                Debug.Log("Usando Emblema del Fénix...");
                break;
        }

        // Ocultar la UI de muerte después de revivir
        DeathUIManager.Instance.HideDeathScreen();
    }

    void DropBackpack() { Debug.Log("Dropping backpack..."); }
    void ReviveWithScroll() { transform.position = GetLastDeathPosition(); }
    void ReviveAtLastNPC() { transform.position = GetLastNPCPosition(); }
    void ReviveWithBuff() { transform.position = GetLastDeathPosition(); ApplyBuff(); }
    void ReduceDeathPenalty() { ModifyDeathPenalties(); transform.position = GetLastDeathPosition(); }
    void ReviveWithDebt() { transform.position = GetLastDeathPosition(); ApplyDemonDebt(); }

    Vector3 GetLastDeathPosition() { return Vector3.zero; }
    Vector3 GetLastNPCPosition() { return new Vector3(5, 0, 5); }
    void ApplyBuff() { Debug.Log("Applying Phoenix Buff!"); }
    void ModifyDeathPenalties() { Debug.Log("Reducing death penalties..."); }
    void ApplyDemonDebt() { Debug.Log("Applying Demon Contract penalties..."); }
}
