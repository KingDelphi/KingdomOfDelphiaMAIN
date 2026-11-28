using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Fusion;
using System.Collections;

public class Player : NetworkBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public Image healthBar;

    public int maxMana;
    public int currentMana;

    public Image manaBar;

    public string playerName = "King Delphi";
    private TextMeshPro textMeshPro;

    public int experience;
    public int level;
    public TMP_Text playerNameText;

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
    public bool isDead = false;
    private GameObject bloodEffectPanel; // Panel de sangre en pantalla


    public void SetLevel(int newLevel)
    {
        level = newLevel;
    }
    
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
        playerName = GameManager.Instance.GetPlayerName();
        textMeshPro = GetComponentInChildren<TextMeshPro>();

        experience = 0;

        // Asegurar que hp tenga un valor válido
        if (hp <= 0)
        {
            hp = maxHealth; // Usar maxHealth como referencia
        }

        currentHealth = hp;
        healthBar.color = Color.green;
        textMeshPro.color = Color.green;

        currentMana = mana;
        manaBar.color = Color.blue;

                // Referencia al inventario (las pociones Q/E la usan)
        if (inventory == null) inventory = GetComponent<PlayerInventory>();

        // Panel de sangre en la UI. Si la UI aún no está activa (primer spawn), no falla:
        // se reintenta más tarde, al recibir daño.
        EnsureBloodEffectPanel();
    }

        // Crea el panel de sangre dentro del Canvas de la UI. Si todavía no hay Canvas, no hace nada.
    private void EnsureBloodEffectPanel()
    {
        if (bloodEffectPanel != null) return;

        GameObject prefab = Resources.Load<GameObject>("BloodEffectPanel");
        if (prefab == null)
        {
            Debug.Log("bloodEffectPanel prefab no encontrado...");
            return;
        }

        Transform canvasTransform = FindUICanvasTransform();
        if (canvasTransform == null) return; // se reintenta después

        bloodEffectPanel = Instantiate(prefab);
        bloodEffectPanel.transform.SetParent(canvasTransform, false);
    }

    private Transform FindUICanvasTransform()
    {
        // Busca incluyendo objetos inactivos: la UI puede estar apagada en el primer spawn
        if (GameManager.Instance != null && GameManager.Instance.UI != null)
        {
            Canvas canvas = GameManager.Instance.UI.GetComponentInChildren<Canvas>(true);
            if (canvas != null) return canvas.transform;
        }

        GameObject found = GameObject.Find("Canvas");
        return found != null ? found.transform : null;
    }


    private void Update()
    {
        textMeshPro.text = "LVL " + level + " " + playerName;

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
        EnsureBloodEffectPanel();   // <-- línea nueva
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

        // Activar efecto de sangre si está en vida negra (menos del 10% de HP)
        if (currentHealth <= hp * 0.1 && bloodEffectPanel != null)
        {
            Debug.Log("Activando BloodEffectPanel con parpadeo...");

            if (currentHealth <= hp * 0.1)
            {
                BloodEffectController bloodEffect = bloodEffectPanel.GetComponent<BloodEffectController>();
                if (bloodEffect != null)
                {
                    bloodEffect.ShowBloodEffect();
                }
            }
        }

        if (currentHealth <= 0 && bloodEffectPanel != null)
        {
            Debug.Log("Vida = 0. BloodEffectPanel se mantiene activado.");
            bloodEffectPanel.GetComponent<Image>().color = new Color(1, 0, 0, 0.8f); // Más opaco
            bloodEffectPanel.SetActive(true);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Método para ocultar el panel
    void HideBloodEffect()
    {
        if (bloodEffectPanel != null)
        {
            bloodEffectPanel.SetActive(false);
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
    if (isDead) return; // 🚫 Evita que muera varias veces

    // 🔥 🚀 Scroll of Life - Evita la muerte
    PlayerInventory inventory = GetComponent<PlayerInventory>();
    if (inventory.HasItem(inventory.GetGearItemByName("Life Scroll")))
    {
        inventory.UseItem(inventory.GetGearItemByName("Life Scroll"));

        Debug.Log("🔥 LifeScroll activado! El personaje revive.");

        // 🎵 Sonido de resurrección
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("lifeScrollAudio"), transform.position);

        // 🎬 Animación de resurrección
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Victory");
        }

        // 💖 Restaurar HP
        Heal(maxHealth);

        // 🛡️ Aplicar Buffs: Invulnerabilidad y Velocidad x2
        BuffSystem buffSystem = GetComponent<BuffSystem>();
        if (buffSystem != null)
        {
            buffSystem.ApplyBuff("Invulnerability", 10f, (state) =>
            {
                gameObject.layer = state ? LayerMask.NameToLayer("Invulnerable") : LayerMask.NameToLayer("Default");
            });

            buffSystem.ApplyBuff("SpeedBoost", 10f, (state) =>
            {
                movementSpeed = state ? movementSpeed * 2f : movementSpeed / 2f;
            });
        }

        // Instanciar el prefab del escudo protector sobre el personaje
        GameObject shieldPrefab = Resources.Load<GameObject>("ShieldPrefab"); // Asegúrate de tener el prefab en Resources
        if (shieldPrefab != null)
        {
            GameObject shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            shieldInstance.transform.SetParent(transform); // Hacer que el escudo esté en el mismo lugar que el personaje
            shieldInstance.transform.localPosition = Vector3.zero; // Ajustar la posición si es necesario

            // Destruir el escudo después de 10 segundos
            Destroy(shieldInstance, 10f);
        }

        return; // 🚫 No sigue la muerte
    }

    // ⚰️ Muerte Normal
    isDead = true;
    Debug.Log("El personaje ha muerto.");
    gameObject.layer = LayerMask.NameToLayer("Dead");

    // ❌ Desactiva el Collider
    Collider col = GetComponent<Collider>();
    if (col != null)
    {
        col.enabled = false;
    }

    PenalizeExp(0.10f);

    // 🌫️ Activar Nube de Muerte
    DeathFogEffect deathFog = FindObjectOfType<DeathFogEffect>();
    if (deathFog != null)
    {
        deathFog.StartFogEffect();
    }

    DeathSystem deathSystem = GetComponent<DeathSystem>();
    if (deathSystem != null)
    {
        deathSystem.Die(inventory);
    }
}

    

    public void ReceiveExp(int expAmount)
    {
        experience += expAmount;
        OnAddExperience?.Invoke(expAmount);

        Debug.Log("El personaje ha recibido " + expAmount + " puntos de experiencia. Total de experiencia: " + experience);
    }

    public void PenalizeExp(float percentage)
    {
        int expLost = Mathf.FloorToInt(experience * percentage);
        experience = Mathf.Max(0, experience - expLost); // Evita números negativos
        OnAddExperience?.Invoke(-expLost);

        Debug.Log($"El personaje ha perdido {expLost} puntos de experiencia. Experiencia previa: {experience + expLost}, Experiencia actual: {experience}");
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

    public override void Spawned()
    {
        Debug.Log("Player(445).Spawned() called. " + gameObject.name + " Spawned.");
        sceneIndex = 1;
        DontDestroyOnLoad(gameObject);
        transform.SetParent(GameManager.Instance.characterHolder);

        StartCoroutine(FindClient());
        
        RpcUpdatePlayers();
    }

    IEnumerator FindClient()
    {
        if (GetComponent<NetworkObject>().HasStateAuthority)
        {
            Debug.Log("Player(461).FindClient() called.");
        }

        while (true)
        {
            foreach (Client c in GameManager.Instance.clients)
            {
                if(c.GetComponent<NetworkObject>().Id == clientId)
                {
                    client = c;
                    c.SetActivePlayer(this);
                    //RpcUpdatePlayers();
                    yield break;
                }
            }
            yield return null;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdatePlayers()
    {
        if (GetComponent<NetworkObject>().HasStateAuthority)
        {
            Debug.Log("Player(485).RpcUpdatePlayers() called.");
        }
        
        GameManager.Instance.DeactivatePlayersInScene();
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