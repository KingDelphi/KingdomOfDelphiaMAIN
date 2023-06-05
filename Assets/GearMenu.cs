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
    public List<GearItem> equippedGear;
    // Start is called before the first frame update
    void Awake()
    {
         playerInventory = FindObjectOfType<PlayerInventory>();
         
         if(playerInventory.gear.helmet != null) {
          equippedGear[0] = playerInventory.gear.helmet;
         }
         
         equippedGear[1] = playerInventory.gear.armor;
         equippedGear[2] = playerInventory.gear.legs;
         equippedGear[3] = playerInventory.gear.boots;
         equippedGear[4] = playerInventory.gear.weapon;
         equippedGear[5] = playerInventory.gear.shield;
         equippedGear[6] = playerInventory.gear.ring;
         equippedGear[7] = playerInventory.gear.amulet;
         equippedGear[8] = playerInventory.gear.holder;
         CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Helmet, "helmet", 0);
         CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Armor, "armor", 1);
         CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Legs, "legs",2);
         CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Boots, "boots", 3);
         CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Weapon, "weapon", 4);
         CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Shield, "shield", 5);
         CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Ring, "ring", 6);
         CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Amulet, "amulet", 7);
         CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Holder, "holder", 8);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)){
            CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Helmet, "helmet", 0);
            }

    }

    void CheckGear(PlayerInventory inventory, Gear gear, GearItem.ArmorType newArmorType, string gearType, int index) {
        dropdownList[index].ClearOptions();
        
        TMP_Dropdown.OptionData oldData = new TMP_Dropdown.OptionData();
        oldData.text = "None";
        dropdownList[index].options.Add(oldData);
         if(equippedGear[0].quantity == 0) {
            
         } else {
        if(newArmorType == equippedGear[index].armorType) {
            TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = gearType;
              dropdownList[index].options.Add(newData);
        }
         }
        

        foreach(GearItem inventoryItem in inventory.inventory) {
            if(inventoryItem.armorType.ToString() == gearType) {
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = gearType;
              dropdownList[index].options.Add(newData);
            }
        }
    }
}
