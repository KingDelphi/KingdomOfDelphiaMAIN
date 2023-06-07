using UnityEngine;
using System.Collections.Generic;

public class TioJuancho : MonoBehaviour
{
    // Tarea
    private bool firstTaskCompleted;
    private bool secondTaskCompleted;
/*
    private int yitopoCount;
    private int yotepoCount;
    private int ranitomaCount;
    private int ranotemaCount;
*/
    // Tienda
    private bool shopOpen;

    // Regalos
    private bool firstClick;
    private bool taskAssigned;

    // Productos de la tienda
    public Dictionary<GearItem, int> itemPrices;

    PlayerInventory playerInventory;

    private bool isClickable = true;

    private void Start()
    {
        // Inicializar variables de tarea
        firstTaskCompleted = false;
        secondTaskCompleted = false;
/*
        yitopoCount = 0;
        yotepoCount = 0;
        ranitomaCount = 0;
        ranotemaCount = 0;
*/
        // Inicializar variable de tienda
        shopOpen = false;

        // Inicializar variable de regalos
        firstClick = true;
        taskAssigned = false;
        itemPrices = new Dictionary<GearItem, int>()
            {
                { new GearItem("Small Health Potion", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 0, 0, 0, 0.1f, 20, 0, .01f), 15 },
                { new GearItem("Small Mana Potion", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 0, 0, 0, 0.1f, 0, 20, .01f), 20 },
                { new GearItem("Backpack", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 0, 0, 0, 12f, 0, 0, 0f), 25 },
                { new GearItem("Small Dagger", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 2, 0, 0, 0, 0, 0, 6f, 0, 0, 0f), 50 },
                { new GearItem("Brass Shield", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 2, 0, 0, 32f, 0, 0, 0f), 75 },
                { new GearItem("Brass Armor", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 2, 0, 0, 99f, 0, 0, 0f), 120 },
                { new GearItem("Brass Legs", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 2, 0, 0, 45f, 0, 0, 0f), 200 },
                { new GearItem("Brass Helmet", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 2, 0, 0, 16f, 0, 0, 0f), 175 },
                { new GearItem("Brass Ring", GearItem.Rarity.Common, 0, 0, 0f, .05f, .05f, 0, 0, 0, 0, 0, 0, 0.1f, 0, 0, 0f), 350 },
                { new GearItem("Brass Amulet", GearItem.Rarity.Common, 10, 20, 0f, 0f, 0f, 0, 0, 0, 0, 0, 0, 5f, 0, 0, 0f), 350 }
            };
    }

    private void OnMouseDown()
{
    if (gameObject.name == "Tio Juancho (NPC)")
    {
        Debug.Log("NPC clicked!");

        // Verificar si se ha asignado la tarea y si el jugador ha completado el primer diálogo
        if (!taskAssigned && !firstClick)
        {
            // Obtener el componente "QuestSystem" del GameObject del jugador
            QuestSystem questSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestSystem>();
            if (questSystem != null)
            {
                AssignFirstTask(); // Agregar la primera misión al QuestSystem
            }
            else
            {
                Debug.LogError("QuestSystem component not found.");
            }

            taskAssigned = true;
        }
        else
        {
            StartDialog();
        }
    }
}

/*
    // Lógica de las tareas
    public void TaskCompleted(string enemyType)
    {
        // Verificar qué tipo de enemigo se ha derrotado y actualizar el contador correspondiente
        switch (enemyType)
        {
            case "Yitopo":
                yitopoCount++;
                break;
            case "Yotepo":
                yotepoCount++;
                break;
            case "Ranitoma":
                ranitomaCount++;
                break;
            case "Ranotema":
                ranotemaCount++;
                break;
        }

        // Verificar si se ha completado la primera tarea
        if (!firstTaskCompleted && yitopoCount >= 12 && yotepoCount >= 8 && ranitomaCount >= 10 && ranotemaCount >= 6)
        {
            firstTaskCompleted = true;
            StartSecondTask();
        }
    }
*/

    private void StartSecondTask()
    {
        // Encargar la segunda tarea (matar al jefe)
        Debug.Log("Well done! Now you have a new task. Defeat my brother 'Tio Juanete' to earn a reward.");

        // Aquí puedes agregar la lógica adicional para la segunda tarea, como activar al jefe en el juego.
        
        // Obtener el componente "QuestSystem" del GameObject del jugador
        QuestSystem questSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestSystem>();
        if (questSystem != null)
        {
            questSystem.AddSecondQuest();
        }
        else
        {
            Debug.LogError("QuestSystem component not found.");
        }
    }

    public void BossDefeated()
    {
        QuestSystem questSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestSystem>();
        // Verificar si se ha completado la segunda tarea
        if (!secondTaskCompleted && questSystem.secondQuestKillFinished)
        {
            secondTaskCompleted = true;
            GiveOldPendant();
            Debug.Log("Congratulations! You have completed all tasks.");

            // Aquí puedes agregar la lógica adicional para otorgar la recompensa al jugador.
        }
    }
    
    public void Update() {
        if(Input.GetKeyDown(KeyCode.R)) {
         OpenShop();
        }
        
    }
    // Lógica de la tienda
    public void OpenShop()
    {
        // Abrir o cerrar la tienda según su estado actual
        shopOpen = !shopOpen;

        if (shopOpen)
        {
            // Inicializar precios de los productos
            /*itemPrices = new Dictionary<GearItem, int>()
            {
                { new GearItem("Small Health Potion", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 0, 0, 0, 0.1f, 20, 0, .01f), 15 },
                { new GearItem("Small Mana Potion", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 0, 0, 0, 0.1f, 0, 20, .01f), 20 },
                { new GearItem("Backpack", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 0, 0, 0, 12f, 0, 0, 0f), 25 },
                { new GearItem("Small Dagger", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 2, 0, 0, 0, 0, 0, 6f, 0, 0, 0f), 50 },
                { new GearItem("Brass Shield", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 2, 0, 0, 32f, 0, 0, 0f), 75 },
                { new GearItem("Brass Armor", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 2, 0, 0, 99f, 0, 0, 0f), 120 },
                { new GearItem("Brass Legs", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 2, 0, 0, 45f, 0, 0, 0f), 200 },
                { new GearItem("Brass Helmet", GearItem.Rarity.Common, 0, 0, 0f, 0f, 0f, 0, 0, 0, 2, 0, 0, 16f, 0, 0, 0f), 175 },
                { new GearItem("Brass Ring", GearItem.Rarity.Common, 0, 0, 0f, .05f, .05f, 0, 0, 0, 0, 0, 0, 0.1f, 0, 0, 0f), 350 },
                { new GearItem("Brass Amulet", GearItem.Rarity.Common, 10, 20, 0f, 0f, 0f, 0, 0, 0, 0, 0, 0, 5f, 0, 0, 0f), 350 }
            };*/
            
            // Mostrar los productos disponibles en la tienda
            Debug.Log("Welcome to my shop! Here are my products:");

            if (itemPrices != null && itemPrices.Count > 0) // Verificar si el diccionario no es nulo y contiene elementos
            {
                foreach (KeyValuePair<GearItem, int> item in itemPrices)
                {
                    GearItem gearItem = item.Key;
                    int itemPrice = item.Value;

                    Debug.Log(gearItem.itemName + " (" + itemPrice + " Gold)");
                }
            }
            else
            {
                Debug.Log("No items available in the shop.");
            }
        }
        else
        {
            // Cerrar la tienda
            Debug.Log("Thanks for your visit! Come back soon.");
        }
    }

    public void BuyItem(GearItem gearItem)
    {
        // Verificar si la tienda está abierta
        if (!shopOpen)
        {
            Debug.Log("The shop is currently closed.");
            return;
        }

        // Verificar si el jugador tiene suficiente dinero para comprar el objeto
        if (CanAfford(gearItem))
        {
            // Reducir el dinero del jugador y agregar el objeto al inventario
            ReduceMoney(GetItemPrice(gearItem));
            //AddItemToInventory(gearItem);

            Debug.Log("Purchase successful!");

            // Aquí puedes agregar la lógica adicional para mostrar una confirmación de compra al jugador.
        }
        else
        {
            Debug.Log("You can't buy that item.");
        }
    }

    public bool CanAfford(GearItem gearItem)
    {
        int itemPrice = GetItemPrice(gearItem);

        // Obtener una referencia al componente PlayerInventory del jugador
        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();

        // Verificar si se encontró el componente PlayerInventory y si el jugador tiene suficiente dinero para comprar el objeto
        if (playerInventory != null)
        {
            return playerInventory.GetGoldAmount() >= itemPrice;
        }
        else
        {
            Debug.LogError("PlayerInventory component not found.");
            return false;
        }
    }

    private void ReduceMoney(int amount)
    {
        // Obtener una referencia al componente PlayerInventory del jugador
        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();

        // Verificar si se encontró el componente PlayerInventory y reducir la cantidad de dinero del jugador después de una compra
        if (playerInventory != null)
        {
            playerInventory.RemoveGold(amount);
        }
        else
        {
            Debug.LogError("PlayerInventory component not found.");
        }
    }

    private void AddItemToInventory(GearItem gearItem)
    {
        // Obtener una referencia al componente GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();

        // Verificar si se encontró el componente GameManager
        if (gameManager != null)
        {
            // Obtener la referencia al componente PlayerInventory a través del GameManager
            PlayerInventory playerInventory = gameManager.GetPlayerInventory();

            // Verificar si se encontró el componente PlayerInventory
            if (playerInventory != null)
            {
                // Agregar el objeto al inventario del jugador
                playerInventory.AddItemToInventory(gearItem);
            }
            else
            {
                Debug.LogError("PlayerInventory component not found.");
            }
        }
        else
        {
            Debug.LogError("GameManager component not found.");
        }
    }

    private int GetItemPrice(GearItem gearItem)
    {
        // Obtener el precio de un objeto de la tienda según su nombre
        if (itemPrices.ContainsKey(gearItem))
        {
            return itemPrices[gearItem];
        }

        return 0;
    }

    // Lógica de los regalos
    public void StartDialog()
    {
        QuestSystem questSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestSystem>();
        if (firstClick && isClickable)
        {
            Debug.Log("Tio Juancho: Hey there, adventurer! Welcome to my humble tavern. Looking for a drink or a quest, perhaps? Well, you're in luck! I have a special gift for you, just because you caught my attention. Take a look at what I've got:");

            // Aquí puedes agregar la lógica para mostrar los objetos que el Tio Juancho le regala al jugador en la consola.
            GiveGift();
            firstClick = false;
        }
        else
        {
            Debug.Log("Tio Juancho: Oh, it's you again!");
            if (!taskAssigned)
            {
                AssignFirstTask();
                taskAssigned = true;
            }
            else if (!secondTaskCompleted && questSystem.firstQuestKillFinished && questSystem.firstQuestItemFinished)
            {
                StartSecondTask();
            }
            else if (questSystem.secondQuestKillFinished && !secondTaskCompleted)
            {
                BossDefeated();
            }
            else
            {
                Debug.Log("Tio Juancho: Sorry, but I'm all out of tasks for now. Come back later!");
            }
        }
    }

    private void AssignFirstTask()
    {
        Debug.Log("Tio Juancho: I have a task for you, adventurer. Help me defeat the monsters that have been causing trouble around here. I need you to defeat 12 Yitopos, 8 Yotepos, 10 Ranitomas, and 6 Ranotemas. Once you complete this task, come back to me for a reward.");
        // Aquí puedes agregar la lógica adicional para iniciar la tarea en tu juego, como activar a los monstruos en el juego.
        
        // Obtener el componente "QuestSystem" del GameObject del jugador
        QuestSystem questSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestSystem>();
        if (questSystem != null)
        {
            questSystem.AddFirstQuest();
        }
        else
        {
            Debug.LogError("QuestSystem component not found.");
        }
    }

    private void GiveGift()
    {
        GetLoot();
        Debug.Log("- Backpack");
        Debug.Log("- Wooden Stick");
        Debug.Log("- Wooden Shield");
        Debug.Log("- Leather Boots");
        Debug.Log("- Leather Armor");
        Debug.Log("- Leather Legs");
        Debug.Log("- Leather Helmet");
        Debug.Log("- Wooden Ring");
        Debug.Log("- 12 Small Health Potions");
        Debug.Log("- 8 Small Mana Potions");
    }

    private void GiveOldPendant()
    {
        Debug.Log("- Old Pendant");
        // Obtener el objeto GearItem del GearItemManager por su nombre
        GearItem gearItem = GearItemManager.Instance.GetGearItemByName("Old Pendant");
        
        if (gearItem != null)
        {
            // Agregar el GearItem al inventario del jugador
            playerInventory.AddItemToInventory(gearItem);
        }
        else
        {
            Debug.LogError("El objeto GearItem 'Old Pendant' no se encontró en el GearItemManager.");
        }
    }

    private void GetLoot()
    {
        Debug.Log("Metodo GetLoot() llamado exitosamente.");
            LootSystem lootSystem = GetComponent<LootSystem>();
            if (lootSystem != null)
            {
                lootSystem.OnMonsterDeath(); // Llamar al método OnMonsterDeath del LootSystem
                isClickable = false;
                return;
            }
        Debug.Log("Obtén loot del objeto");
    }
}