using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownBehaviour : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public GameManager gameManager;
    public TMP_Dropdown dropdown;
    public GameObject character;
    public TMP_Text selectedItem;
    public string previousItem;
    public GearItem gearItem, previousGearItem;
    public GearMenu gearMenu;
    public int index;
    // Start is called before the first frame update
    void Start()
    {
      previousItem = "None";
      selectedItem.text = "None";
      gameManager = FindObjectOfType<GameManager>();
      playerInventory = FindObjectOfType<PlayerInventory>();
      dropdown = this.gameObject.GetComponent<TMP_Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
      if(gearMenu.equippedGear[index].itemName == selectedItem.text) {        
         gearItem = gearMenu.equippedGear[index];
         } else {
            playerInventory.CheckForItem(selectedItem.text, index);
        }
        
        if(playerInventory.gameObject.name != character.GetComponent<Image>().sprite.name) {
         dropdown.ClearOptions();
          //characterSelectorButton.previousChar = character.GetComponent<Image>().sprite.name;
        }
     if(gearItem != previousGearItem) {
      if(previousGearItem.itemName != "") {
       playerInventory = FindObjectOfType<PlayerInventory>();
      playerInventory.inventory.Add(previousGearItem);
      }
      previousGearItem = gearItem;
     }
     if(selectedItem.text != previousItem) {
      if(selectedItem.text == "None") {
        Debug.Log("selected item was none");
      } else {
        Debug.Log("selected item was NOT none");
      }
      if(previousItem == "None") {
       Debug.Log("previous item was none");
      } else {
        Debug.Log("previous item was NOT none");
      
      }
      previousItem = selectedItem.text;
     }
    }

    public void UpdateInventory() {
      playerInventory = FindObjectOfType<PlayerInventory>();
    }
}
