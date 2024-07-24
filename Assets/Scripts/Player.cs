// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System;
// using UnityEngine.SceneManagement;
// using Fusion;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;

// public class Player : NetworkBehaviour, ISaveable
// {
//     public GameObject characterPrefab; // Prefab del personaje
//     public GameObject character; // Instancia del personaje
    
//     public string ID => GetInstanceID().ToString(); //SAVELOAD
//     public Type SaveType => typeof(PlayerSaveData); // Implementación correcta de `SaveType`

    
//     public int maxHealth;
//     public int currentHealth;
//     public Image healthBar;
//     public int maxMana;
//     public int currentMana;
//     public Image manaBar;
//     public string playerName = "King Delphi";
//     private TextMeshPro textMeshPro;
//     public int experience;

//     // Stats iniciales del jugador
//     public int initialHp;
//     public int initialMana;
//     public float initialMovementSpeed;
//     public float initialCriticalChance;
//     public float initialCriticalBonus;
//     public int initialPhysicalAttack;
//     public int initialDemonAttack;
//     public int initialHollyAttack;
//     public int initialPhysicalDefense;
//     public int initialDemonDefense;
//     public int initialHollyDefense;
//     public float initialWeight;
//     public int initialHpRegen;
//     public int initialManaRegen;
//     public float initialEffectDuration;

//     // Stats actuales del jugador
//     public int hp;
//     public int mana;
//     public float movementSpeed;
//     public float criticalChance;
//     public float criticalBonus;
//     public int physicalAttack;
//     public int demonAttack;
//     public int hollyAttack;
//     public int physicalDefense;
//     public int demonDefense;
//     public int hollyDefense;
//     public float weight;
//     public int hpRegen;
//     public int manaRegen;
//     public float effectDuration;

//     // Gear del jugador
//     public PlayerGear gear;
//     public PlayerInventory inventory;
//     public event Action<int> OnAddExperience;
//     public GearItemManager gearItemManager;

//     [Networked]
//     public int sceneIndex {get; set;}
//     [Networked]
//     public NetworkId clientId {get; set;}
//     public Client client;

//     private bool isInitialized = false;

//     // Comienza Implementación de ISaveable en el script Player

// [Serializable]
// private class PlayerSaveData
// {
//     public int maxHealth;
//     public int currentHealth;
//     public int maxMana;
//     public int currentMana;
//     public string playerName;
//     public int experience;

//     // Stats iniciales del jugador
//     public int initialHp;
//     public int initialMana;
//     public float initialMovementSpeed;
//     public float initialCriticalChance;
//     public float initialCriticalBonus;
//     public int initialPhysicalAttack;
//     public int initialDemonAttack;
//     public int initialHollyAttack;
//     public int initialPhysicalDefense;
//     public int initialDemonDefense;
//     public int initialHollyDefense;
//     public float initialWeight;
//     public int initialHpRegen;
//     public int initialManaRegen;
//     public float initialEffectDuration;

//     // Stats actuales del jugador
//     public int hp;
//     public int mana;
//     public float movementSpeed;
//     public float criticalChance;
//     public float criticalBonus;
//     public int physicalAttack;
//     public int demonAttack;
//     public int hollyAttack;
//     public int physicalDefense;
//     public int demonDefense;
//     public int hollyDefense;
//     public float weight;
//     public int hpRegen;
//     public int manaRegen;
//     public float effectDuration;

//     // Gear del jugador
//     public GearItemSaveData holder;
//     public GearItemSaveData helmet;
//     public GearItemSaveData armor;
//     public GearItemSaveData legs;
//     public GearItemSaveData boots;
//     public GearItemSaveData weapon;
//     public GearItemSaveData shield;
//     public GearItemSaveData ring;
//     public GearItemSaveData amulet;

//     // Inventario del jugador
//     public List<GearItemSaveData> inventoryItems;
// }

// [Serializable]
// public class GearItemSaveData
// {
//     public string itemName;
//     public GearItem.Rarity itemRarity;
//     public int hp;
//     public int mana;
//     public float movementSpeed;
//     public float criticalChance;
//     public float criticalBonus;
//     public int physicalAttack;
//     public int demonAttack;
//     public int hollyAttack;
//     public int physicalDefense;
//     public int demonDefense;
//     public int hollyDefense;
//     public float weight;
//     public int hpRegen;
//     public int manaRegen;
//     public float effectDuration;
//     public GearItem.ItemType itemType;
// }

// public object SaveState()
// {
//     var saveData = new PlayerSaveData
//     {
//         maxHealth = this.maxHealth,
//         currentHealth = this.currentHealth,
//         maxMana = this.maxMana,
//         currentMana = this.currentMana,
//         playerName = this.playerName,
//         experience = this.experience,

//         initialHp = this.initialHp,
//         initialMana = this.initialMana,
//         initialMovementSpeed = this.initialMovementSpeed,
//         initialCriticalChance = this.initialCriticalChance,
//         initialCriticalBonus = this.initialCriticalBonus,
//         initialPhysicalAttack = this.initialPhysicalAttack,
//         initialDemonAttack = this.initialDemonAttack,
//         initialHollyAttack = this.initialHollyAttack,
//         initialPhysicalDefense = this.initialPhysicalDefense,
//         initialDemonDefense = this.initialDemonDefense,
//         initialHollyDefense = this.initialHollyDefense,
//         initialWeight = this.initialWeight,
//         initialHpRegen = this.initialHpRegen,
//         initialManaRegen = this.initialManaRegen,
//         initialEffectDuration = this.initialEffectDuration,

//         hp = this.hp,
//         mana = this.mana,
//         movementSpeed = this.movementSpeed,
//         criticalChance = this.criticalChance,
//         criticalBonus = this.criticalBonus,
//         physicalAttack = this.physicalAttack,
//         demonAttack = this.demonAttack,
//         hollyAttack = this.hollyAttack,
//         physicalDefense = this.physicalDefense,
//         demonDefense = this.demonDefense,
//         hollyDefense = this.hollyDefense,
//         weight = this.weight,
//         hpRegen = this.hpRegen,
//         manaRegen = this.manaRegen,
//         effectDuration = this.effectDuration,

//         holder = ConvertGearItemToSaveData(this.gear.holder),
//         helmet = ConvertGearItemToSaveData(this.gear.helmet),
//         armor = ConvertGearItemToSaveData(this.gear.armor),
//         legs = ConvertGearItemToSaveData(this.gear.legs),
//         boots = ConvertGearItemToSaveData(this.gear.boots),
//         weapon = ConvertGearItemToSaveData(this.gear.weapon),
//         shield = ConvertGearItemToSaveData(this.gear.shield),
//         ring = ConvertGearItemToSaveData(this.gear.ring),
//         amulet = ConvertGearItemToSaveData(this.gear.amulet),

//         inventoryItems = this.inventory.inventory.Select(ConvertGearItemToSaveData).ToList()
//     };

//     return saveData;
// }

// public void LoadState(object state)
//     {
//         if (state is PlayerSaveData saveData)
//         {
//             this.maxHealth = saveData.maxHealth;
//             this.currentHealth = saveData.currentHealth;
//             this.maxMana = saveData.maxMana;
//             this.currentMana = saveData.currentMana;
//             this.playerName = saveData.playerName;
//             this.experience = saveData.experience;

//             this.initialHp = saveData.initialHp;
//             this.initialMana = saveData.initialMana;
//             this.initialMovementSpeed = saveData.initialMovementSpeed;
//             this.initialCriticalChance = saveData.initialCriticalChance;
//             this.initialCriticalBonus = saveData.initialCriticalBonus;
//             this.initialPhysicalAttack = saveData.initialPhysicalAttack;
//             this.initialDemonAttack = saveData.initialDemonAttack;
//             this.initialHollyAttack = saveData.initialHollyAttack;
//             this.initialPhysicalDefense = saveData.initialPhysicalDefense;
//             this.initialDemonDefense = saveData.initialDemonDefense;
//             this.initialHollyDefense = saveData.initialHollyDefense;
//             this.initialWeight = saveData.initialWeight;
//             this.initialHpRegen = saveData.initialHpRegen;
//             this.initialManaRegen = saveData.initialManaRegen;
//             this.initialEffectDuration = saveData.initialEffectDuration;

//             this.hp = saveData.hp;
//             this.mana = saveData.mana;
//             this.movementSpeed = saveData.movementSpeed;
//             this.criticalChance = saveData.criticalChance;
//             this.criticalBonus = saveData.criticalBonus;
//             this.physicalAttack = saveData.physicalAttack;
//             this.demonAttack = saveData.demonAttack;
//             this.hollyAttack = saveData.hollyAttack;
//             this.physicalDefense = saveData.physicalDefense;
//             this.demonDefense = saveData.demonDefense;
//             this.hollyDefense = saveData.hollyDefense;
//             this.weight = saveData.weight;
//             this.hpRegen = saveData.hpRegen;
//             this.manaRegen = saveData.manaRegen;
//             this.effectDuration = saveData.effectDuration;

//             this.gear.holder = ConvertSaveDataToGearItem(saveData.holder);
//             this.gear.helmet = ConvertSaveDataToGearItem(saveData.helmet);
//             this.gear.armor = ConvertSaveDataToGearItem(saveData.armor);
//             this.gear.legs = ConvertSaveDataToGearItem(saveData.legs);
//             this.gear.boots = ConvertSaveDataToGearItem(saveData.boots);
//             this.gear.weapon = ConvertSaveDataToGearItem(saveData.weapon);
//             this.gear.shield = ConvertSaveDataToGearItem(saveData.shield);
//             this.gear.ring = ConvertSaveDataToGearItem(saveData.ring);
//             this.gear.amulet = ConvertSaveDataToGearItem(saveData.amulet);

//             // Clear current inventory before loading
//             this.inventory.inventory.Clear();
//             foreach (var itemData in saveData.inventoryItems)
//             {
//                 var item = ConvertSaveDataToGearItem(itemData);
//                 this.inventory.inventory.Add(item);
//             }

//             CalculatePlayerStats();
//         }
//         else
//         {
//             Debug.LogError("Invalid load state for Player.");
//         }
//     }

// private GearItemSaveData ConvertGearItemToSaveData(GearItem gearItem)
// {
//     if (gearItem == null) return null;

//     return new GearItemSaveData
//     {
//         itemName = gearItem.itemName,
//         itemRarity = gearItem.rarity,
//         hp = gearItem.hp,
//         mana = gearItem.mana,
//         movementSpeed = gearItem.movementSpeed,
//         criticalChance = gearItem.criticalChance,
//         criticalBonus = gearItem.criticalBonus,
//         physicalAttack = gearItem.physicalAttack,
//         demonAttack = gearItem.demonAttack,
//         hollyAttack = gearItem.hollyAttack,
//         physicalDefense = gearItem.physicalDefense,
//         demonDefense = gearItem.demonDefense,
//         hollyDefense = gearItem.hollyDefense,
//         weight = gearItem.weight,
//         hpRegen = gearItem.hpRegen,
//         manaRegen = gearItem.manaRegen,
//         effectDuration = gearItem.effectDuration,
//         itemType = gearItem.itemType
//     };
// }

// private GearItem ConvertSaveDataToGearItem(GearItemSaveData saveData)
// {
//     if (saveData == null) return null;

//     return new GearItem(
//         saveData.itemName,
//         saveData.itemRarity,
//         saveData.hp,
//         saveData.mana,
//         saveData.movementSpeed,
//         saveData.criticalChance,
//         saveData.criticalBonus,
//         saveData.physicalAttack,
//         saveData.demonAttack,
//         saveData.hollyAttack,
//         saveData.physicalDefense,
//         saveData.demonDefense,
//         saveData.hollyDefense,
//         saveData.weight,
//         saveData.hpRegen,
//         saveData.manaRegen,
//         saveData.effectDuration,
//         saveData.itemType
//     );
// }

//     // Termina Implementación de ISaveable en el script Player



//     private void Awake()
//     {
//         gear = new PlayerGear();
        
//         textMeshPro = GetComponentInChildren<TextMeshPro>();

//         experience = 0;

//         if (!isInitialized)
//         {
//             hp = maxHealth;
//             mana = maxMana;

//             currentHealth = hp;
//             currentMana = mana;

//             ResetPlayerEquipment();

//             isInitialized = true;

//             Debug.Log("Player(367).Awake() called. Inicialización de script Player. NO SE CUAL OBJETO INICIALIZA."); // OJOLOCUAZ...NO SE CUAL OBJETO INICIALIZA NI EN LA 1ra NI EN LA 2da Iteracion
//             Debug.Log("NEW (3rd Iteration)Player(368).Awake() called. this.gameObject: " + this.gameObject); // OJOLOCUAZ...Vamos a ver que sale en this.gameObject en la 3ra Iteracion.
//         }

//         healthBar.color = Color.green;
//         textMeshPro.color = Color.green;
//         manaBar.color = Color.blue;

//         inventory = FindObjectOfType<PlayerInventory>();
//         if (inventory == null)
//         {
//             Debug.LogError("PlayerInventory component not found.");
//         }
//         else
//         {
//             Debug.Log("Player(382) PlayerInventory found and assigned to variable inventory, notifying GameManager. inventory: " + inventory); // OJOLOCUAZ 3ra Iteracion - Ya se puso la variable inventory para rastrear su valor
        
//             // Verificar que la instancia del GameManager esté inicializada
//             if (GameManager.Instance != null)
//             {
//                 // Notificar al GameManager que el inventario está listo
//                 GameManager.Instance.PlayerInventoryInitialized(inventory);
//             }
//             else
//             {
//                 Debug.LogError("GameManager instance is null.");
//             }
//         }

//         // Suscribir el método CalculatePlayerStats al evento OnEquipmentChanged del gear
//         gear.OnEquipmentChanged += CalculatePlayerStats;
//     }

    

//     private void ResetPlayerEquipment()
//     {
//         if (gear == null)
//         {
//             gear = new PlayerGear();
//         }
//         else
//         {
//             // Asignar todos los slots de equipo a null o a un nuevo GearItem vacío
//             gear.holder = null;
//             gear.helmet = null;
//             gear.armor = null;
//             gear.legs = null;
//             gear.boots = null;
//             gear.weapon = null;
//             gear.shield = null;
//             gear.ring = null;
//             gear.amulet = null;

//             // Notificar cambios en el equipo (opcional, depende de la implementación de tu juego)
//             gear.EquipmentChanged();
//         }
//     }

//     private void Update()
//     {
//         // Actualiza el nombre del player en el TextMeshPro
//         textMeshPro.text = playerName;

//         // Consumir poción de salud al presionar la tecla "Q"
//         if (Input.GetKeyDown(KeyCode.Q))
//         {
//             ConsumeHealthPotion();
//         }

//         // Consumir poción de mana al presionar la tecla "E"
//         if (Input.GetKeyDown(KeyCode.E))
//         {
//             ConsumeManaPotion();
//         }
//     }

//     public void TakeDamage(int damageAmount)
//     {
//         currentHealth -= damageAmount;

//         healthBar.fillAmount = (float)currentHealth / hp;

//         if (currentHealth >= hp * 0.8)
//         {
//             healthBar.color = Color.green;
//             textMeshPro.color = Color.green;
//         }
//         else if (currentHealth >= hp * 0.3)
//         {
//             healthBar.color = Color.yellow;
//             textMeshPro.color = Color.yellow;
//         }
//         else if (currentHealth >= hp * 0.1)
//         {
//             healthBar.color = Color.red;
//             textMeshPro.color = Color.red;
//         }
//         else
//         {
//             healthBar.color = Color.black;
//             textMeshPro.color = Color.black;
//         }

//         currentHealth = Mathf.Max(currentHealth, 0);

//         if (currentHealth <= 0)
//         {
//             Die();
//         }
//     }

//     public void Heal(int healAmount)
//     {
//         currentHealth += healAmount;

//         currentHealth = Mathf.Min(currentHealth, hp);

//         healthBar.fillAmount = (float)currentHealth / hp;

//         if (currentHealth >= hp * 0.8)
//         {
//             healthBar.color = Color.green;
//             textMeshPro.color = Color.green;
//         }
//         else if (currentHealth >= hp * 0.3)
//         {
//             healthBar.color = Color.yellow;
//             textMeshPro.color = Color.yellow;
//         }
//         else if (currentHealth >= hp * 0.1)
//         {
//             healthBar.color = Color.red;
//             textMeshPro.color = Color.red;
//         }
//         else
//         {
//             healthBar.color = Color.black;
//             textMeshPro.color = Color.black;
//         }
//     }

//     public void HealMana(int healAmount)
//     {
//         currentMana += healAmount;

//         currentMana = Mathf.Min(currentMana, mana);

//         manaBar.fillAmount = (float)currentMana / mana;
//     }

//     public void ConsumeMana(int manaCost)
//     {
//         if (currentMana >= manaCost)
//         {
//             currentMana -= manaCost;

//             manaBar.fillAmount = (float)currentMana / mana;

//             Debug.Log("Mana consumido: " + manaCost);
//         }
//         else
//         {
//             Debug.Log("No tienes suficiente mana para lanzar el hechizo.");
//         }
//     }

//     private void Die()
//     {
//         Debug.Log("El personaje ha muerto.");
//     }

//     public void ReceiveExp(int expAmount)
//     {
//         experience += expAmount;
//         OnAddExperience?.Invoke(expAmount);

//         Debug.Log("El personaje ha recibido " + expAmount + " puntos de experiencia. Total de experiencia: " + experience);
//     }

//     public int GetExperience()
//     {
//         return experience;
//     }

//     public void UpdateGear(GearItem newGearItem)
// {
//     if (newGearItem == null)
//     {
//         // Remover el equipo del jugador
//         gear.holder = null;
//         gear.helmet = null;
//         gear.armor = null;
//         gear.legs = null;
//         gear.boots = null;
//         gear.weapon = null;
//         gear.shield = null;
//         gear.ring = null;
//         gear.amulet = null;

//         // Recalcular estadísticas del jugador
//         CalculatePlayerStats();
//         return;
//     }

//     // Determinar qué tipo de equipo es newGearItem y asignarlo apropiadamente
//     switch (newGearItem.itemType)
//     {
//         case GearItem.ItemType.Holder:
//             gear.holder = newGearItem;
//             break;
//         case GearItem.ItemType.Helmet:
//             gear.helmet = newGearItem;
//             break;
//         case GearItem.ItemType.Armor:
//             gear.armor = newGearItem;
//             break;
//         case GearItem.ItemType.Legs:
//             gear.legs = newGearItem;
//             break;
//         case GearItem.ItemType.Boots:
//             gear.boots = newGearItem;
//             break;
//         case GearItem.ItemType.Weapon:
//             gear.weapon = newGearItem;
//             break;
//         case GearItem.ItemType.Shield:
//             gear.shield = newGearItem;
//             break;
//         case GearItem.ItemType.Ring:
//             gear.ring = newGearItem;
//             break;
//         case GearItem.ItemType.Amulet:
//             gear.amulet = newGearItem;
//             break;
//         default:
//             Debug.LogError("Unsupported gear type: " + newGearItem.itemType);
//             return;
//     }

//     // Recalcular estadísticas del jugador
//     CalculatePlayerStats();
// }

//     private void CalculatePlayerStats()
//     {
//         ResetPlayerStats();

//         AddGearStats(gear.holder);
//         AddGearStats(gear.helmet);
//         AddGearStats(gear.armor);
//         AddGearStats(gear.legs);
//         AddGearStats(gear.boots);
//         AddGearStats(gear.weapon);
//         AddGearStats(gear.shield);
//         AddGearStats(gear.ring);
//         AddGearStats(gear.amulet);
//     }

//     private void ResetPlayerStats()
//     {
//         hp = initialHp;
//         mana = initialMana;
//         movementSpeed = initialMovementSpeed;
//         criticalChance = initialCriticalChance;
//         criticalBonus = initialCriticalBonus;
//         physicalAttack = initialPhysicalAttack;
//         demonAttack = initialDemonAttack;
//         hollyAttack = initialHollyAttack;
//         physicalDefense = initialPhysicalDefense;
//         demonDefense = initialDemonDefense;
//         hollyDefense = initialHollyDefense;
//         weight = initialWeight;
//         hpRegen = initialHpRegen;
//         manaRegen = initialManaRegen;
//         effectDuration = initialEffectDuration;
//     }

//     private void AddGearStats(GearItem gearItem)
//     {
//         if (gearItem != null)
//         {
//             hp += gearItem.hp;
//             mana += gearItem.mana;
//             movementSpeed += gearItem.movementSpeed;
//             criticalChance += gearItem.criticalChance;
//             criticalBonus += gearItem.criticalBonus;
//             physicalAttack += gearItem.physicalAttack;
//             demonAttack += gearItem.demonAttack;
//             hollyAttack += gearItem.hollyAttack;
//             physicalDefense += gearItem.physicalDefense;
//             demonDefense += gearItem.demonDefense;
//             hollyDefense += gearItem.hollyDefense;
//             weight += gearItem.weight;
//             hpRegen += gearItem.hpRegen;
//             manaRegen += gearItem.manaRegen;
//             effectDuration += gearItem.effectDuration;
//         }
//     }

//     private void ConsumeHealthPotion()
//     {
//         GearItem smallHealthPotion = inventory.GetGearItemByName("Small Health Potion");
//         GearItem mediumHealthPotion = inventory.GetGearItemByName("Medium Health Potion");
//         GearItem bigHealthPotion = inventory.GetGearItemByName("Big Health Potion");
//         GearItem greatHealthPotion = inventory.GetGearItemByName("Great Health Potion");
//         GearItem ultimateHealthPotion = inventory.GetGearItemByName("Ultimate Health Potion");

//         if (smallHealthPotion != null && inventory.HasItem(smallHealthPotion))
//         {
//             inventory.UseItem(smallHealthPotion);
//             Heal(20);
//         }
//         else if (mediumHealthPotion != null && inventory.HasItem(mediumHealthPotion))
//         {
//             inventory.UseItem(mediumHealthPotion);
//             Heal(75);
//         }
//         else if (bigHealthPotion != null && inventory.HasItem(bigHealthPotion))
//         {
//             inventory.UseItem(bigHealthPotion);
//             Heal(250);
//         }
//         else if (greatHealthPotion != null && inventory.HasItem(greatHealthPotion))
//         {
//             inventory.UseItem(greatHealthPotion);
//             Heal(800);
//         }
//         else if (ultimateHealthPotion != null && inventory.HasItem(ultimateHealthPotion))
//         {
//             inventory.UseItem(ultimateHealthPotion);
//             Heal(1500);
//         }
//         else
//         {
//             Debug.Log("You don't have any Health Potion");
//         }
//     }

//     private void ConsumeManaPotion()
//     {
//         GearItem smallManaPotion = inventory.GetGearItemByName("Small Mana Potion");
//         GearItem mediumManaPotion = inventory.GetGearItemByName("Medium Mana Potion");
//         GearItem bigManaPotion = inventory.GetGearItemByName("Big Mana Potion");
//         GearItem greatManaPotion = inventory.GetGearItemByName("Great Mana Potion");
//         GearItem ultimateManaPotion = inventory.GetGearItemByName("Ultimate Mana Potion");

//         if (smallManaPotion != null && inventory.HasItem(smallManaPotion))
//         {
//             inventory.UseItem(smallManaPotion);
//             HealMana(20);
//         }
//         else if (mediumManaPotion != null && inventory.HasItem(mediumManaPotion))
//         {
//             inventory.UseItem(mediumManaPotion);
//             HealMana(75);
//         }
//         else if (bigManaPotion != null && inventory.HasItem(bigManaPotion))
//         {
//             inventory.UseItem(bigManaPotion);
//             HealMana(250);
//         }
//         else if (greatManaPotion != null && inventory.HasItem(greatManaPotion))
//         {
//             inventory.UseItem(greatManaPotion);
//             HealMana(800);
//         }
//         else if (ultimateManaPotion != null && inventory.HasItem(ultimateManaPotion))
//         {
//             inventory.UseItem(ultimateManaPotion);
//             HealMana(1500);
//         }
//         else
//         {
//             Debug.Log("You don't have any Mana Potion");
//         }
//     }

//     public void SetupLocalPlayer()
//     {
//         if (GameManager.Instance != null)
//         {
//             GameManager.Instance.SetupSceneForPlayer(this, FindObjectOfType<NetworkRunner>());
//         }
//         else
//         {
//             Debug.LogError("GameManager.Instance is null in SetupLocalPlayer");
//         }

//         SceneManager.sceneLoaded += GameManager.Instance.OnSceneLoaded;

//         if (gear == null)
//         {
//             gear = new PlayerGear();
//         }
//         gearItemManager = FindObjectOfType<GearItemManager>();

//         CalculatePlayerStats();

//         gear.OnEquipmentChanged += CalculatePlayerStats;
//     }

//     public void SetupOtherPlayer()
//     {
//         GetComponent<CharacterController>().Initiate();
//         gameObject.tag = "Untagged";
//     }

//     public override void Spawned()
//     {
//         sceneIndex = 1;
//         DontDestroyOnLoad(gameObject);
//         transform.SetParent(GameManager.Instance.characterHolder);
//         gameObject.name = "Character " + GetComponent<NetworkObject>().Id.ToString();
//         StartCoroutine(FindClient());
//     }

//     IEnumerator FindClient()
//     {
//         while (true)
//         {
//             foreach (Client c in GameManager.Instance.clients)
//             {
//                 if(c.GetComponent<NetworkObject>().Id == clientId)
//                 {
//                     Debug.Log("FindClient Coroutine called > SetActivePlayer called (probably twice)");
//                     client = c;
//                     c.SetActivePlayer(this);
//                     RpcUpdatePlayers();
//                     yield break;
//                 }
//             }
//             yield return null;
//         }
//     }

//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RpcUpdatePlayers()
//     {
//         GameManager.Instance.ActivatePlayersInScene();
//     }
// }

// [System.Serializable]
// public class PlayerGear
// {
//     public GearItem holder;
//     public GearItem helmet;
//     public GearItem armor;
//     public GearItem legs;
//     public GearItem boots;
//     public GearItem weapon;
//     public GearItem shield;
//     public GearItem ring;
//     public GearItem amulet;

//     public PlayerGear()
//     {
//         // Inicializar todos los campos en null o en un estado vacío
//         holder = null;
//         helmet = null;
//         armor = null;
//         legs = null;
//         boots = null;
//         weapon = null;
//         shield = null;
//         ring = null;
//         amulet = null;
//     }

//     public event Action OnEquipmentChanged;

//     public void EquipmentChanged()
//     {
//         if (OnEquipmentChanged != null)
//         {
//             OnEquipmentChanged.Invoke();
//         }
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Fusion;
using System.Collections;

public class Player : NetworkBehaviour
{
    public int maxHealth = 0;
    public int currentHealth;

    public Image healthBar;

    public int maxMana = 0;
    public int currentMana;

    public Image manaBar;

    public string playerName = "King Delphi";
    private TextMeshPro textMeshPro;

    public int experience;

    // Stats iniciales del jugador
    public int initialHp;
    public int initialMana;
    public float initialMovementSpeed;
    public float initialCriticalChance;
    public float initialCriticalBonus;
    public int initialPhysicalAttack;
    public int initialDemonAttack;
    public int initialHollyAttack;
    public int initialPhysicalDefense;
    public int initialDemonDefense;
    public int initialHollyDefense;
    public float initialWeight;
    public int initialHpRegen;
    public int initialManaRegen;
    public float initialEffectDuration;

    // Stats actuales del jugador
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

    // Gear del jugador
    public PlayerGear gear;

    public PlayerInventory inventory;

    public event Action<int> OnAddExperience;

    public GearItemManager gearItemManager;

    [Networked]
    public int sceneIndex {get; set;}

    [Networked]
    public NetworkId clientId {get; set;}

    public Client client;

    public void SetClient(Client client)
    {
        this.client = client;
    }

    public Client GetClient()
    {
        return client;
    }

    private void Awake()
    {
        textMeshPro = GetComponentInChildren<TextMeshPro>();

        experience = 0;

        currentHealth = hp;
        healthBar.color = Color.green;
        textMeshPro.color = Color.green;

        currentMana = mana;
        manaBar.color = Color.blue;

        GameManager.Instance.PlayerInventoryInitialized(inventory);
    }

    private void Update()
    {
        textMeshPro.text = playerName;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ConsumeHealthPotion();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ConsumeManaPotion();
        }
    }

    

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        healthBar.fillAmount = (float)currentHealth / hp;

        if (currentHealth >= hp * 0.8)
        {
            healthBar.color = Color.green;
            textMeshPro.color = Color.green;
        }
        else if (currentHealth >= hp * 0.3)
        {
            healthBar.color = Color.yellow;
            textMeshPro.color = Color.yellow;
        }
        else if (currentHealth >= hp * 0.1)
        {
            healthBar.color = Color.red;
            textMeshPro.color = Color.red;
        }
        else
        {
            healthBar.color = Color.black;
            textMeshPro.color = Color.black;
        }

        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        currentHealth = Mathf.Min(currentHealth, hp);

        healthBar.fillAmount = (float)currentHealth / hp;

        if (currentHealth >= hp * 0.8)
        {
            healthBar.color = Color.green;
            textMeshPro.color = Color.green;
        }
        else if (currentHealth >= hp * 0.3)
        {
            healthBar.color = Color.yellow;
            textMeshPro.color = Color.yellow;
        }
        else if (currentHealth >= hp * 0.1)
        {
            healthBar.color = Color.red;
            textMeshPro.color = Color.red;
        }
        else
        {
            healthBar.color = Color.black;
            textMeshPro.color = Color.black;
        }
    }

    public void HealMana(int healAmount)
    {
        currentMana += healAmount;

        currentMana = Mathf.Min(currentMana, mana);

        manaBar.fillAmount = (float)currentMana / mana;
    }

    public void ConsumeMana(int manaCost)
    {
        if (currentMana >= manaCost)
        {
            currentMana -= manaCost;

            manaBar.fillAmount = (float)currentMana / mana;

            Debug.Log("Mana consumido: " + manaCost);
        }
        else
        {
            Debug.Log("No tienes suficiente mana para lanzar el hechizo.");
        }
    }

    private void Die()
    {
        Debug.Log("El personaje ha muerto.");
    }

    public void ReceiveExp(int expAmount)
    {
        experience += expAmount;
        OnAddExperience?.Invoke(expAmount);

        Debug.Log("El personaje ha recibido " + expAmount + " puntos de experiencia. Total de experiencia: " + experience);
    }

    public int GetExperience()
    {
        return experience;
    }

    public void UpdateGear(GearItem newGearItem)
    {
        if (newGearItem == null)
        {
            // Remover el equipo del jugador
            gear.holder = null;
            gear.helmet = null;
            gear.armor = null;
            gear.legs = null;
            gear.boots = null;
            gear.weapon = null;
            gear.shield = null;
            gear.ring = null;
            gear.amulet = null;

            // Recalcular estadísticas del jugador
            CalculatePlayerStats();
            return;
        }

        // Determinar qué tipo de equipo es newGearItem y asignarlo apropiadamente
        switch (newGearItem.itemType)
        {
            case GearItem.ItemType.Holder:
                gear.holder = newGearItem;
                break;
            case GearItem.ItemType.Helmet:
                gear.helmet = newGearItem;
                break;
            case GearItem.ItemType.Armor:
                gear.armor = newGearItem;
                break;
            case GearItem.ItemType.Legs:
                gear.legs = newGearItem;
                break;
            case GearItem.ItemType.Boots:
                gear.boots = newGearItem;
                break;
            case GearItem.ItemType.Weapon:
                gear.weapon = newGearItem;
                break;
            case GearItem.ItemType.Shield:
                gear.shield = newGearItem;
                break;
            case GearItem.ItemType.Ring:
                gear.ring = newGearItem;
                break;
            case GearItem.ItemType.Amulet:
                gear.amulet = newGearItem;
                break;
            default:
                Debug.LogError("Unsupported gear type: " + newGearItem.itemType);
                return;
        }

        // Recalcular estadísticas del jugador
        CalculatePlayerStats();
    }

    private void CalculatePlayerStats()
    {
        ResetPlayerStats();

        AddGearStats(gear.holder);
        AddGearStats(gear.helmet);
        AddGearStats(gear.armor);
        AddGearStats(gear.legs);
        AddGearStats(gear.boots);
        AddGearStats(gear.weapon);
        AddGearStats(gear.shield);
        AddGearStats(gear.ring);
        AddGearStats(gear.amulet);
    }

    private void ResetPlayerStats()
    {
        hp = initialHp;
        mana = initialMana;
        movementSpeed = initialMovementSpeed;
        criticalChance = initialCriticalChance;
        criticalBonus = initialCriticalBonus;
        physicalAttack = initialPhysicalAttack;
        demonAttack = initialDemonAttack;
        hollyAttack = initialHollyAttack;
        physicalDefense = initialPhysicalDefense;
        demonDefense = initialDemonDefense;
        hollyDefense = initialHollyDefense;
        weight = initialWeight;
        hpRegen = initialHpRegen;
        manaRegen = initialManaRegen;
        effectDuration = initialEffectDuration;
    }

    private void AddGearStats(GearItem gearItem)
    {
        if (gearItem != null)
        {
            hp += gearItem.hp;
            mana += gearItem.mana;
            movementSpeed += gearItem.movementSpeed;
            criticalChance += gearItem.criticalChance;
            criticalBonus += gearItem.criticalBonus;
            physicalAttack += gearItem.physicalAttack;
            demonAttack += gearItem.demonAttack;
            hollyAttack += gearItem.hollyAttack;
            physicalDefense += gearItem.physicalDefense;
            demonDefense += gearItem.demonDefense;
            hollyDefense += gearItem.hollyDefense;
            weight += gearItem.weight;
            hpRegen += gearItem.hpRegen;
            manaRegen += gearItem.manaRegen;
            effectDuration += gearItem.effectDuration;
        }
    }

    private void ConsumeHealthPotion()
    {
        GearItem smallHealthPotion = inventory.GetGearItemByName("Small Health Potion");
        GearItem mediumHealthPotion = inventory.GetGearItemByName("Medium Health Potion");
        GearItem bigHealthPotion = inventory.GetGearItemByName("Big Health Potion");
        GearItem greatHealthPotion = inventory.GetGearItemByName("Great Health Potion");
        GearItem ultimateHealthPotion = inventory.GetGearItemByName("Ultimate Health Potion");

        if (smallHealthPotion != null && inventory.HasItem(smallHealthPotion))
        {
            inventory.UseItem(smallHealthPotion);
            Heal(20);
        }
        else if (mediumHealthPotion != null && inventory.HasItem(mediumHealthPotion))
        {
            inventory.UseItem(mediumHealthPotion);
            Heal(75);
        }
        else if (bigHealthPotion != null && inventory.HasItem(bigHealthPotion))
        {
            inventory.UseItem(bigHealthPotion);
            Heal(250);
        }
        else if (greatHealthPotion != null && inventory.HasItem(greatHealthPotion))
        {
            inventory.UseItem(greatHealthPotion);
            Heal(800);
        }
        else if (ultimateHealthPotion != null && inventory.HasItem(ultimateHealthPotion))
        {
            inventory.UseItem(ultimateHealthPotion);
            Heal(1500);
        }
        else
        {
            Debug.Log("You don't have any Health Potion");
        }
    }

    private void ConsumeManaPotion()
    {
        GearItem smallManaPotion = inventory.GetGearItemByName("Small Mana Potion");
        GearItem mediumManaPotion = inventory.GetGearItemByName("Medium Mana Potion");
        GearItem bigManaPotion = inventory.GetGearItemByName("Big Mana Potion");
        GearItem greatManaPotion = inventory.GetGearItemByName("Great Mana Potion");
        GearItem ultimateManaPotion = inventory.GetGearItemByName("Ultimate Mana Potion");

        if (smallManaPotion != null && inventory.HasItem(smallManaPotion))
        {
            inventory.UseItem(smallManaPotion);
            HealMana(20);
        }
        else if (mediumManaPotion != null && inventory.HasItem(mediumManaPotion))
        {
            inventory.UseItem(mediumManaPotion);
            HealMana(75);
        }
        else if (bigManaPotion != null && inventory.HasItem(bigManaPotion))
        {
            inventory.UseItem(bigManaPotion);
            HealMana(250);
        }
        else if (greatManaPotion != null && inventory.HasItem(greatManaPotion))
        {
            inventory.UseItem(greatManaPotion);
            HealMana(800);
        }
        else if (ultimateManaPotion != null && inventory.HasItem(ultimateManaPotion))
        {
            inventory.UseItem(ultimateManaPotion);
            HealMana(1500);
        }
        else
        {
            Debug.Log("You don't have any Mana Potion");
        }
    }

    public void SetupLocalPlayer()
    {
        GameManager.Instance.SetupSceneForPlayer(this);
        SceneManager.sceneLoaded += GameManager.Instance.OnSceneLoaded;

        if (gear == null)
        {
            gear = new PlayerGear();
        }
        gearItemManager = FindObjectOfType<GearItemManager>();

        CalculatePlayerStats();

        gear.OnEquipmentChanged += CalculatePlayerStats;
    }

    public void SetupOtherPlayer()
    {
        GetComponent<CharacterController>().Initiate();
        gameObject.tag = "Untagged";
    }

    public override void Spawned()
    {
        sceneIndex = 1;
        DontDestroyOnLoad(gameObject);
        transform.SetParent(GameManager.Instance.characterHolder);
        
        // Usar el prefabName y clientId para el nombre del objeto
        PrefabInfo prefabInfo = GetComponent<PrefabInfo>();
        if (prefabInfo != null)
        {
            gameObject.name = $"{prefabInfo.prefabName} {clientId.ToString()}";
        }
        else
        {
            gameObject.name = $"Player {clientId.ToString()}";
        }

        StartCoroutine(FindClient());
    }

    IEnumerator FindClient()
    {
        while (true)
        {
            foreach (Client c in GameManager.Instance.clients)
            {
                if(c.GetComponent<NetworkObject>().Id == clientId)
                {
                    client = c;
                    c.SetActivePlayer(this);
                    RpcUpdatePlayers();
                    yield break;
                }
            }
            yield return null;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdatePlayers()
    {
        Debug.Log("RpcUpdatePlayers");
        GameManager.Instance.ActivatePlayersInScene();
    }
}

[System.Serializable]
public class PlayerGear
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

    public PlayerGear()
    {
        // Inicializar todos los campos en null o en un estado vacío
        holder = null;
        helmet = null;
        armor = null;
        legs = null;
        boots = null;
        weapon = null;
        shield = null;
        ring = null;
        amulet = null;
    }

    public event Action OnEquipmentChanged;

    public void EquipmentChanged()
    {
        if (OnEquipmentChanged != null)
        {
            OnEquipmentChanged.Invoke();
        }
    }
}