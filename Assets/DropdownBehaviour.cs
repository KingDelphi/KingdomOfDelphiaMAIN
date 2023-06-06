using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownBehaviour : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public TMP_Dropdown dropdown;
    public GameObject character;
    public string previousInv;
    public TMP_Text selectedItem;
    public GearMenu gearMenu;
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = this.gameObject.GetComponent<TMP_Dropdown>();
        previousInv = character.GetComponent<Image>().sprite.name;
    }

    // Update is called once per frame
    void Update()
    {
      if(gearMenu.equippedGear[index].itemName == selectedItem.text) {        
         } else {
            playerInventory.CheckForItem(selectedItem.text, index);
        }
        
        if(playerInventory.gameObject.name != character.GetComponent<Image>().sprite.name) {
         dropdown.ClearOptions();
          //characterSelectorButton.previousChar = character.GetComponent<Image>().sprite.name;
        }
    }

    public void UpdateInventory() {
      playerInventory = FindObjectOfType<PlayerInventory>();
    }
}
