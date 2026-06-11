using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSystem : MonoBehaviour
{
    private bool isEating = false;
    private PlayerInventory inventory;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Consumir comida al presionar la tecla "F"
        if (Input.GetKeyDown(KeyCode.F))
        {
            ConsumeFood();
        }
    }

    public void ConsumeFood()
    {
        GearItem chickenLeg = inventory.GetGearItemByName("Chicken Leg");
        GearItem porkHam = inventory.GetGearItemByName("Pork Ham");
        GearItem bacon = inventory.GetGearItemByName("Bacon");
        GearItem honeyInfusedBacon = inventory.GetGearItemByName("Honey-Infused Bacon");

        if (chickenLeg != null && inventory.HasItem(chickenLeg))
        {
            StartCoroutine(ConsumeFoodCoroutine(chickenLeg, 12, 1, 1));
        }
        else if (porkHam != null && inventory.HasItem(porkHam))
        {
            StartCoroutine(ConsumeFoodCoroutine(porkHam, 36, 1, 1));
        }
        else if (bacon != null && inventory.HasItem(bacon))
        {
            StartCoroutine(ConsumeFoodCoroutine(bacon, 12, 2, 2));
        }
        else if (honeyInfusedBacon != null && inventory.HasItem(honeyInfusedBacon))
        {
            StartCoroutine(ConsumeFoodCoroutine(honeyInfusedBacon, 36, 2, 2));
        }
        else
        {
            Debug.Log("You don't have any Food");
        }
    }

    private IEnumerator ConsumeFoodCoroutine(GearItem foodItem, int durationSeconds, int healAmount, int healManaAmount)
    {
        isEating = true;

        inventory.UseItem(foodItem);
        Debug.Log("You have consumed " + foodItem.itemName + ". You will heal " + healAmount + " HP and " + healManaAmount + " Mana every second for " + durationSeconds + ".");

        for (int i = 0; i < durationSeconds; i++)
        {
            player.Heal(healAmount);
            player.HealMana(healManaAmount);
            yield return new WaitForSeconds(1f);
        }

        isEating = false;
        Debug.Log(foodItem.itemName + " has finished. You are hungry!");
    }
}