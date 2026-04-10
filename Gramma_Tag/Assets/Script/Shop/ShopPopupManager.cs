using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopPopupManager : MonoBehaviour
{
    public static ShopPopupManager Instance;

    [Header("Popup UI")]
    public GameObject popup;
    public Image itemIcon;
    public TMP_Text confirmText;

    private ShopItem currentItem;

    void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(ShopItem item)
    {
        currentItem = item;

        popup.SetActive(true);

        // set icon
        itemIcon.sprite = item.icon;

        // set text
        confirmText.text = "Buy " + item.amount + " " + item.itemName + "?";
    }

    public void ConfirmBuy()
    {
        ShopSystem.Instance.BuyItem(currentItem);

        popup.SetActive(false);
    }

    public void Cancel()
    {
        popup.SetActive(false);
    }
}
