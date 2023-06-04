using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GearMenu : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public List<TMP_Dropdown> dropdownList;
    public Dropdown.OptionData m_NewData;
    // Start is called before the first frame update
    void Awake()
    {
         playerInventory = FindObjectOfType<PlayerInventory>();
         CheckGear(playerInventory, "helmet", 0);
         CheckGear(playerInventory, "armor", 1);
         CheckGear(playerInventory, "legs", 2);
         CheckGear(playerInventory, "boots", 3);
         CheckGear(playerInventory, "weapon", 4);
         CheckGear(playerInventory, "shield", 5);
         CheckGear(playerInventory, "ring", 6);
         CheckGear(playerInventory, "amulet", 7);
         CheckGear(playerInventory, "holder", 8);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)){
            CheckGear(playerInventory, "helmet", 0);
            }

    }

    void CheckGear(PlayerInventory inventory, string gearType, int index) {
        foreach(GearItem inventoryItem in inventory.inventory) {
            if(inventoryItem.itemName == gearType) {
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = gearType;
              dropdownList[index].options.Add(newData);
            }
        }
    }
}
