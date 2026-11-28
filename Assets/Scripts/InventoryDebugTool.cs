using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// SOLO DIAGNÓSTICO (borrar después).
/// F5: agrega un item de cada tipo (tomado de GearItemManager) al jugador activo.
/// F6: muestra el estado del jugador, su inventario y su equipo.
/// El resultado se ve en pantalla y en la Console.
/// </summary>
public class InventoryDebugTool : MonoBehaviour
{
    private string status = "F5: agregar items de prueba  |  F6: ver estado del inventario";
    private float statusTime = 0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) AddTestItems();
        if (Input.GetKeyDown(KeyCode.F6)) PrintState();
    }

    private void Report(string message)
    {
        status = message;
        statusTime = Time.unscaledTime;
        Debug.Log("[InventoryDebugTool] " + message);
    }

    private PlayerInventory GetInventory()
    {
        Player p = GameManager.Instance != null ? GameManager.Instance.player : null;
        if (p == null)
        {
            Report("No hay jugador activo (GameManager.Instance.player es null). Entra primero con un personaje.");
            return null;
        }

        PlayerInventory inv = p.GetComponent<PlayerInventory>();
        if (inv == null)
        {
            Report("El jugador '" + p.name + "' no tiene componente PlayerInventory.");
        }
        return inv;
    }

    private void AddTestItems()
    {
        PlayerInventory inv = GetInventory();
        if (inv == null) return;

        GearItemManager mgr = FindFirstObjectByType<GearItemManager>();
        if (mgr == null)
        {
            Report("No se encontró GearItemManager en la escena.");
            return;
        }

        // Candidatos: lista plana + categorías
        var candidates = new List<GearItem>();
        int flatCount = 0;
        int categoryCount = 0;

        if (mgr.gearItems != null)
        {
            flatCount = mgr.gearItems.Count;
            candidates.AddRange(mgr.gearItems);
        }

        if (mgr.gearCategories != null)
        {
            foreach (GearCategory cat in mgr.gearCategories)
            {
                if (cat == null || cat.gearItems == null) continue;
                categoryCount += cat.gearItems.Count;
                candidates.AddRange(cat.gearItems);
            }
        }

        if (candidates.Count == 0)
        {
            Report("GearItemManager no tiene items. gearItems: " + flatCount + ", dentro de categorías: " + categoryCount +
                   ". Hay que llenar esas listas en el Inspector.");
            return;
        }

        // Cuántos items hay de cada tipo (sirve para detectar Item Type mal configurado)
        var perType = new Dictionary<GearItem.ItemType, int>();
        foreach (GearItem c in candidates)
        {
            if (c == null) continue;
            perType.TryGetValue(c.itemType, out int n);
            perType[c.itemType] = n + 1;
        }

        var added = new List<string>();
        var addedTypes = new HashSet<GearItem.ItemType>();
        foreach (GearItem template in candidates)
        {
            if (template == null || string.IsNullOrEmpty(template.itemName)) continue;
            if (template.itemType == GearItem.ItemType.Consumable) continue;
            if (addedTypes.Contains(template.itemType)) continue;

            inv.AddItemToInventory(template.Clone());
            addedTypes.Add(template.itemType);
            added.Add(template.itemName + " (" + template.itemType + ")");
        }

        var sb = new StringBuilder();
        sb.AppendLine("Agregados " + added.Count + " items: " + string.Join(", ", added));
        sb.Append("Items en GearItemManager por tipo: ");
        foreach (var kv in perType) sb.Append(kv.Key + "=" + kv.Value + "  ");
        Report(sb.ToString());
    }

    private void PrintState()
    {
        PlayerInventory inv = GetInventory();
        if (inv == null) return;

        Player p = GameManager.Instance.player;
        var sb = new StringBuilder();
        sb.AppendLine("Personaje: " + p.name + "  |  Player.enabled: " + p.enabled + "  |  gear: " + (p.gear != null ? "ok" : "NULL"));
        sb.AppendLine("Oro: " + inv.gold + "  |  Items en inventario: " + inv.inventory.Count);

        foreach (GearItem item in inv.inventory)
        {
            if (item == null) continue;
            sb.AppendLine(" - " + item.itemName + " x" + item.quantity + " [" + item.itemType + ", " + item.rarity + "]" +
                          (inv.IsEquipped(item) ? "  (EQUIPADO)" : ""));
        }

        sb.AppendLine("Stats: HP " + p.hp + "  Mana " + p.mana + "  Speed " + p.movementSpeed +
                      "  PhysAtk " + p.physicalAttack + "  PhysDef " + p.physicalDefense);
        Report(sb.ToString());
    }

    private void OnGUI()
    {
        // Muestra el último mensaje 25 segundos
        if (statusTime > 0f && Time.unscaledTime - statusTime > 25f) return;

        var style = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
        style.normal.textColor = Color.yellow;
        GUI.Label(new Rect(10, 10, Screen.width * 0.65f, Screen.height - 20), status, style);
    }
}