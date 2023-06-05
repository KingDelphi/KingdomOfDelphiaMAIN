using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropdownBehaviour : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public TMP_Text selectedItem;
    public GearMenu gearMenu;
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gearMenu.equippedGear[index].itemName == selectedItem.text) {
            
        } else {
            playerInventory.CheckForItem(selectedItem.text, index);
        }
    }
}
