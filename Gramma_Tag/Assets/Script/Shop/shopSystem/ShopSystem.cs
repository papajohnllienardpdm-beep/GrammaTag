using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance;

    private CoinSystem coinSystem;
    private HeartSystem heartSystem;

    [Header("Result Popup")]
    public ResultPopupUI resultPopup;

    [Header("MESSAGES (EDIT IN INSPECTOR)")]
    public string notEnoughCoinsMessage = "Not enough coins!";
    public string fullHeartsMessage = "Your hearts are already full!";
    public string successMessage = "Purchase successful!";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        coinSystem = FindObjectOfType<CoinSystem>();
        heartSystem = FindObjectOfType<HeartSystem>();
    }

    public void BuyItem(ShopItem item)
    {
        int playerCoins = coinSystem.currentCoins;
        int currentHearts = heartSystem.currentHearts;

        // ❌ FULL HEARTS
        if (currentHearts >= heartSystem.maxHearts)
        {
            resultPopup.Show(fullHeartsMessage);
            return;
        }

        // ❌ NOT ENOUGH COINS
        if (playerCoins < item.price)
        {
            resultPopup.Show(notEnoughCoinsMessage);
            return;
        }

        // ✅ SUCCESS
        coinSystem.SpendCoins(item.price);

        int newHearts = currentHearts + item.amount;

        if (newHearts > heartSystem.maxHearts)
            newHearts = heartSystem.maxHearts;

        heartSystem.currentHearts = newHearts;

        DatabaseManager.Instance.UpdateHearts(
            heartSystem.currentHearts,
            System.DateTime.Now
        );

        heartSystem.SendMessage("UpdateUI");

        resultPopup.Show(successMessage);
    }
}
