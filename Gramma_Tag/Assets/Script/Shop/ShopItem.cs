using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopItem : MonoBehaviour
{
    public string itemName;
    public int amount;
    public int price;
    public Sprite icon;

    public void OnClickBuy()
    {
        ShopPopupManager.Instance.ShowPopup(this);
    }
}
