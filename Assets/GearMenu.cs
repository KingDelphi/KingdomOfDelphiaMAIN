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
          playerInventory = FindObjectOfType<PlayerInventory>();    
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)){
            playerInventory = FindObjectOfType<PlayerInventory>();    
            CheckEquipped();
            CheckInvGear();
            }
            moveSpeed.text = "Movement Speed: " + equippedGear[0].movementSpeed.ToString();
            critChance.text = "Critical Chance: " + equippedGear[0].criticalChance.ToString();
            critBonus.text = "Critical Bonus: " + equippedGear[0].criticalBonus.ToString();
            physAtt.text = "Physical Attack: " + equippedGear[0].physicalAttack.ToString();
            demAtt.text = "Demon Attack: " + equippedGear[0].demonAttack.ToString();
            holAtt.text = "Holy Attack: " + equippedGear[0].hollyAttack.ToString();
            physDef.text = "Physical Def: " + equippedGear[0].physicalDefense.ToString();
            demDef.text = "Demon Def: " + equippedGear[0].demonDefense.ToString();
            holDef.text = "Holy Def: " + equippedGear[0].hollyDefense.ToString();
    }

    public void UpdateGearMenu() {
        playerInventory = FindObjectOfType<PlayerInventory>();    
        CheckEquipped();
        CheckInvGear();
    }

    void CheckGear(PlayerInventory inventory, Gear gear, GearItem.ItemType newArmorType, string gearType, int index) {
        
        dropdownList[index].ClearOptions();
        TMP_Dropdown.OptionData oldData = new TMP_Dropdown.OptionData();
        TMP_Dropdown.OptionData noneData = new TMP_Dropdown.OptionData();
        noneData.text = "None";
        oldData.text = equippedGear[index].itemName;
        dropdownList[index].gameObject.GetComponent<DropdownBehaviour>().selectedItem.text = equippedGear[index].itemName;
        dropdownList[index].options.Add(noneData);
        if(oldData.text != ""){
            dropdownList[index].options.Add(oldData);
        }
        
        dropdownList[index].value = 0;
        if(equippedGear[index].quantity == 0) {
            
         } else {
         if(newArmorType == equippedGear[index].itemType) {
            //TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
            //newData.text = equippedGear[index].itemName;
            //dropdownList[index].options.Add(newData);
         }
        }   

        foreach(GearItem inventoryItem in inventory.inventory) {
            if(inventoryItem.itemType.ToString() == gearType && inventoryItem.itemName != equippedGear[index].itemName) {
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = inventoryItem.itemName;
              dropdownList[index].options.Add(newData); 
            }
        }
    }

    void CheckEquipped() {
         if(equippedGear[0].itemName == null) {
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
        CheckGear(playerInventory, playerInventory.gear, GearItem.ItemType.Helmet, "Helmet", 0);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ItemType.Armor, "Armor", 1);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ItemType.Legs, "Legs",2);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ItemType.Boots, "Boots", 3);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ItemType.Weapon, "Weapon", 4);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ItemType.Shield, "Shield", 5);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ItemType.Ring, "Ring", 6);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ItemType.Amulet, "Amulet", 7);
        CheckGear(playerInventory, playerInventory.gear, GearItem.ItemType.Holder, "Holder", 8);
    }

   /* void Placeholder() {
        if(dropdownList[index].value.ToString() == "None") {
         //dropdownList[index].ClearOptions();
        } else {
         int itemValue = dropdownList[index].value;
         string currentItem = dropdownList[index].gameObject.GetComponent<DropdownBehaviour>().selectedItem.text;   
         List<TMP_Dropdown.OptionData> dataList = dropdownList[index].options;
         int newIndex = new int();
         Debug.Log(dropdownList[index].options);
         foreach(TMP_Dropdown.OptionData name in dataList) {
            
            
            if(name.text == currentItem) {
             Debug.Log("idk");
            } else {
                
                dropdownList[index].options.RemoveAt(newIndex);
                //dropdownList[index].value = 0;
                
            }
            newIndex += 1;
         }
        }
    }*/
}
