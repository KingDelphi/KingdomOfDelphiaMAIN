using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Fusion;

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
    public Gear gear;

    public PlayerInventory inventory;

    public event Action<int> OnAddExperience;

    public GearItemManager gearItemManager;

    public string sceneName;

    private void Awake()
    {
        //GameObject.FindObjectOfType<LoginButton>().characters.Add(gameObject);
        DontDestroyOnLoad(gameObject);
        sceneName = "1. (0,0,0) La Taberna del Tio Juancho";

        textMeshPro = GetComponentInChildren<TextMeshPro>();

        experience = 0;

        if (gear == null)
        {
            gear = new Gear();
        }
        gearItemManager = FindObjectOfType<GearItemManager>();
        
        // Calcular las estadísticas iniciales del jugador
        CalculatePlayerStats();

        // Escuchar los eventos de cambio de equipo
        gear.OnEquipmentChanged += CalculatePlayerStats;

        currentHealth = hp;
        healthBar.color = Color.green;
        textMeshPro.color = Color.green;

        currentMana = mana;
        manaBar.color = Color.blue;
    
    /*
        // Buscar todos los objetos de personaje en la escena
        GameObject[] characterObjects = GameObject.FindGameObjectsWithTag("Player");

        // Desactivar los personajes que no sean el actual
        foreach (GameObject characterObject in characterObjects)
        {
            if (characterObject != gameObject)
            {
                characterObject.SetActive(false);
            }
        }

        // Verificar si ya existe una instancia del personaje en la escena
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length > 1)
        {
            // Ya existe otra instancia del personaje, destruir esta instancia
            Destroy(gameObject);
        }
        else
        {
            // No existe otra instancia del personaje, mantenerlo al cargar nuevas escenas
            DontDestroyOnLoad(gameObject);
        }
    */
    }
    
    // private void Start()
    // {
    //     if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "0. CharacterLoader")
    //     {
    //         // Buscar todos los objetos de personaje en la escena
    //         GameObject[] characterObjects = GameObject.FindGameObjectsWithTag("Player");

    //         // Desactivar los personajes que no sean el actual
    //         // foreach (GameObject characterObject in characterObjects)
    //         // {
    //         //     GameManager.Instance.characters.Add(characterObject);
    //         //     if (characterObject != gameObject)
    //         //     {
    //         //         characterObject.SetActive(false);
    //         //     }
    //         // }
            
    //         //GameManager.Instance.character = gameObject;
    //         //GameManager.Instance.player = GetComponent<Player>();
    //         //SceneManager.LoadScene("1. (0,0,0) La Taberna del Tio Juancho");
    //         // GameManager.Instance.ChangeScene("1. (0,0,0) La Taberna del Tio Juancho", new Vector3 (-36.3f, -28.4f, 0f));
    //     }
    // }

    private void Update()
    {
        // Actualiza el nombre del player en el TextMeshPro
        textMeshPro.text = playerName;

        // Consumir poción de salud al presionar la tecla "Q"
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ConsumeHealthPotion();
        }

        // Consumir poción de mana al presionar la tecla "E"
        if (Input.GetKeyDown(KeyCode.E))
        {
            ConsumeManaPotion();
        }
    }

    public override void Spawned()
    {
        if(HasStateAuthority)
        {
            GameManager.Instance.SetupSceneForPlayer(this);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        healthBar.fillAmount = (float)currentHealth / hp;

        // Cambiar el color del fill según el rango de vida
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

        // Asegurarse de que la vida no sea menor que 0
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        // Asegurarse de que la vida no exceda el máximo
        currentHealth = Mathf.Min(currentHealth, hp);

        healthBar.fillAmount = (float)currentHealth / hp;

        // Cambiar el color del fill según el rango de vida
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

        // Asegurarse de que la vida no exceda el máximo
        currentMana = Mathf.Min(currentMana, mana);

        manaBar.fillAmount = (float)currentMana / mana;
    }

    public void ConsumeMana(int manaCost)
    {
        // Verificar si el personaje tiene suficiente mana para el costo
        if (currentMana >= manaCost)
        {
            // Restar el costo del mana actual
            currentMana -= manaCost;

            // Actualizar la barra de mana
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
        // Lógica para cuando el personaje muere
        Debug.Log("El personaje ha muerto.");
    }

    public void ReceiveExp(int expAmount)
    {
        experience += expAmount;
        OnAddExperience?.Invoke(expAmount);

        // Aquí puedes agregar cualquier acción relacionada con la experiencia recibida
        Debug.Log("El personaje ha recibido " + expAmount + " puntos de experiencia. Total de experiencia: " + experience);
    }

    public int GetExperience()
    {
        return experience;
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
            // Sumar las estadísticas del elemento del equipo a las estadísticas del jugador
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
}
