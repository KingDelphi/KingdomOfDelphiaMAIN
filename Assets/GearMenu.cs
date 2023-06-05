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
    public TMP_Text moveSpeed, critChance, critBonus, physAtt, demAtt, holAtt, physDef, demDef, holDef;
    // Start is called before the first frame update
    void Awake()
    {
          
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)){
            //playerInventory = FindObjectOfType<PlayerInventory>();    
            CheckEquipped();
            CheckInvGear();
            }
            moveSpeed.text = "Movement Speed: " + equippedGear[0].movementSpeed.ToString();
            critChance.text = "Critical Chance: " + equippedGear[0].criticalChance.ToString();
            critBonus.text = "Critical Bonus: " + equippedGear[0].criticalBonus.ToString();
            physAtt.text = "Physical Attack: " + equippedGear[0].physicalAttack.ToString();
            demAtt.text = "Demon Attack: " + equippedGear[0].demonAttack.ToString();
            holAtt.text = "Holy Attack: " + equippedGear[0].hollyAttack.ToString();
            physDef.text = "Physical Def: " + equippedGear[0].physicalAttack.ToString();
            demDef.text = "Demon Def: " + equippedGear[0].demonDefense.ToString();
            holDef.text = "Holy Def: " + equippedGear[0].hollyDefense.ToString();
    }

    void CheckGear(PlayerInventory inventory, Gear gear, GearItem.ArmorType newArmorType, string gearType, int index) {
        dropdownList[index].ClearOptions();
        TMP_Dropdown.OptionData oldData = new TMP_Dropdown.OptionData();
        oldData.text = "None";
        dropdownList[index].options.Add(oldData);

         if(equippedGear[index].quantity == 0) {
            
         } else {
        if(newArmorType == equippedGear[index].armorType) {
            TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
            newData.text = equippedGear[index].itemName;
            dropdownList[index].options.Add(newData);
        }
         }
        
        foreach(GearItem inventoryItem in inventory.inventory) {
            if(inventoryItem.armorType.ToString() == gearType) {
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = inventoryItem.itemName;
              dropdownList[index].options.Add(newData); 
            }
        }
    }

    void CheckEquipped() {
         if(playerInventory.gear.helmet != null) {
          equippedGear[0] = playerInventory.gear.helmet;
         }
         if(playerInventory.gear.armor != null) {
          equippedGear[1] = playerInventory.gear.armor;
         }
         if(playerInventory.gear.legs != null) {
          equippedGear[2] = playerInventory.gear.legs;
         }
         if(playerInventory.gear.boots != null) {
          equippedGear[3] = playerInventory.gear.boots;
         }
         if(playerInventory.gear.weapon != null) {
          equippedGear[4] = playerInventory.gear.weapon;
         }
         if(playerInventory.gear.shield != null) {
          equippedGear[5] = playerInventory.gear.shield;
         }
         if(playerInventory.gear.ring != null) {
          equippedGear[6] = playerInventory.gear.ring;
         }
         if(playerInventory.gear.amulet != null) {
          equippedGear[7] = playerInventory.gear.amulet;
         }
         if(playerInventory.gear.holder != null) {
          equippedGear[8] = playerInventory.gear.holder;
         }
    }

    void CheckInvGear() {
        CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Helmet, "Helmet", 0);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Armor, "armor", 1);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Legs, "legs",2);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Boots, "boots", 3);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Weapon, "weapon", 4);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Shield, "shield", 5);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Ring, "ring", 6);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Amulet, "amulet", 7);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ArmorType.Holder, "holder", 8);
    }
}
