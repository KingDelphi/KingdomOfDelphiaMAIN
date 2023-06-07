using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShopSystem : MonoBehaviour
{
    public Dictionary<GearItem, int> currentShop;
    public GameObject shopSlot, shopCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L)) {
            if(shopCanvas.active) {
             shopCanvas.SetActive(false);
            } else {
             currentShop = FindObjectOfType<TioJuancho>().itemPrices;
             shopCanvas.SetActive(true);
             UpdateShop();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(shopCanvas.active) {
                shopCanvas.SetActive(false);
            }
        }
    }

    public void UpdateShop() {
        Transform[] ts = shopCanvas.GetComponentsInChildren<Transform>();
        foreach(Transform transform in ts) {
            if(transform.gameObject.name != "Background") {
             Destroy(transform.gameObject);
            }
            
        }
        foreach(KeyValuePair<GearItem, int> item in currentShop){
         GearItem gearItem = item.Key;
         int itemPrice = item.Value;
         GameObject obj = Instantiate(shopSlot, shopCanvas.transform);
         var itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
         itemName.text = gearItem.itemName + " x" + gearItem.quantity + " (Price:" + itemPrice + ")";
         shopSlot.GetComponent<ShopButton>().itemToPurchase = gearItem;
        }
    }
}
