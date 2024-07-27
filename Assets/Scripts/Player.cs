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
        Debug.Log("Player(84).GetClient called.");
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
        Debug.Log("Player(423).SetupLocalPlayer() called.");
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
        Debug.Log("Player(445).Spawned() called.");
        sceneIndex = 1;
        DontDestroyOnLoad(gameObject);
        transform.SetParent(GameManager.Instance.characterHolder);
        
        // Usar el prefabName y clientId para el nombre del objeto
        // PrefabInfo prefabInfo = GetComponent<PrefabInfo>();
        // if (prefabInfo != null)
        // {
        //     gameObject.name = $"{prefabInfo.prefabName} {clientId.ToString()}";
        // }
        // else
        // {
        //     gameObject.name = $"Player {clientId.ToString()}";
        // }
        Debug.Log("Player(460) gameObject.name:" + gameObject.name + " Spawned. ");

        StartCoroutine(FindClient());
        
        // Debug.LogWarning("✔️✔️✔️✔️✔️ Player(466) - Game Initialized Successfully - Running GameManager.Instance.MaximumVerification().");
        // GameManager.Instance.MaximumVerification();
    }

    IEnumerator FindClient()
    {
        Debug.Log("Player(467).FindClient() called.");
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
        Debug.Log("Player(487).RpcUpdatePlayers() called.");
        GameManager.Instance.ActivatePlayersInScene();
    }

    public void PlayerVerification()
    {
        Debug.Log($"Player Verification: \n" +
                  $"gameObject.name: {gameObject.name}\n");
        Debug.Log("Player Verification: ✔️ Success");
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