using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public GearItemManager gearItemManager;
    public GameManager gameManager;
    public GearItem itemToPurchase;
    public PlayerInventory playerInventory;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gearItemManager = FindObjectOfType<GearItemManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickShopButton() {
        foreach(GearCategory category in gearItemManager.gearCategories) {
            Debug.Log("Step 1");
          if(itemToPurchase.itemType.ToString() == category.categoryName) {
            Debug.Log("Step 2");
            foreach(GearItem item in category.gearItems) {
                Debug.Log("Step 3");
                if(itemToPurchase.itemType.ToString() == item.itemType.ToString()) {
                 Debug.Log("Step 4");
                 TioJuancho tioJuancho = FindObjectOfType<TioJuancho>();
                 if(tioJuancho.CanAfford(item)) {
                    Debug.Log("Step 5");
                   tioJuancho.BuyItem(item);
                   playerInventory.ListItem(item, gameManager.slot, gameManager.inventoryCanvas.transform.GetChild(0).gameObject);
                 }
                 
                }
            }
          }
        }
        
    }
}
