using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class Monsters : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public int physicalDefense;
    public int demonDefense;
    public int hollyDefense;

    public Image healthBar;

    public string monsterName = "";
    private TextMeshPro textMeshPro;

    public int expReward = 10; // Experiencia que otorga el monstruo

    private GameObject damageTextObject;
    private MonsterAttack monsterAttack;
    private LootSystem lootSystem;

    private bool isDead = false; // Bandera para indicar si el monstruo ha muerto

    private Dictionary<Player, int> playerDamage;

    public Counter counter;

    private void Start()
    {
        monsterAttack = GetComponent<MonsterAttack>();
        lootSystem = GetComponent<LootSystem>();
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        currentHealth = maxHealth;
        healthBar.color = Color.green;
        textMeshPro.color = Color.green;

        playerDamage = new Dictionary<Player, int>();
        counter = FindObjectOfType<Player>().GetComponent<Counter>();
    }

    private void Update()
    {
        // Actualiza el nombre del monstruo en el TextMeshPro
        textMeshPro.text = monsterName;
    }

    public void TakeDamage(Player player, int damageAmount)
    {
        if (isDead)
            return;

        if (!playerDamage.ContainsKey(player))
            playerDamage[player] = 0;

        playerDamage[player] += damageAmount;

        currentHealth -= damageAmount;

        healthBar.fillAmount = currentHealth / maxHealth;

        // Cambiar el color del fill según el rango de vida
        if (currentHealth >= maxHealth * 0.8f)
        {
            healthBar.color = Color.green;
            textMeshPro.color = Color.green;
        }
        else if (currentHealth >= maxHealth * 0.3f)
        {
            healthBar.color = Color.yellow;
            textMeshPro.color = Color.yellow;
        }
        else if (currentHealth >= maxHealth * 0.1f)
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
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (damageTextObject != null)
        {
            Destroy(damageTextObject);
        }
        Destroy(gameObject);
    }

    public void Die()
    {
        if (isDead)
            return; // Si el monstruo ya está muerto, no hagas nada

        isDead = true; // Marcar al monstruo como muerto
        OnDeath();
        counter.MonsterKilled(monsterName);

        if (playerDamage.Count == 0)
            return;

        int totalDamage = 0;
        foreach (var damage in playerDamage.Values)
        {
            totalDamage += damage;
        }

        foreach (var kvp in playerDamage)
        {
            Player player = kvp.Key;
            int damage = kvp.Value;

            float expRatio = (float)damage / totalDamage;
            int expReward = Mathf.RoundToInt(this.expReward * expRatio);

            player.ReceiveExp(expReward);
        }

        // Lógica para cuando el monstruo muere
        Debug.Log("El monstruo ha muerto.");

        // Destruir el objeto de texto del script MonsterAttack
        monsterAttack.DestroyDamageText();

        lootSystem.OnMonsterDeath(); // Llamar a la función en LootSystem cuando el monstruo muere
        StartCoroutine(DestroyAfterDelay(1.2f)); // Destruir el objeto del monstruo después de un tiempo de espera
    }

    // Evento de muerte
    public event Action OnDeath;
}
