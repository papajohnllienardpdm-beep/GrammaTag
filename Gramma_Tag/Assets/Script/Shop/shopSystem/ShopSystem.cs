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

        // 🔥 CHECK IF POWERUP ITEM
        if (item.itemName == "Double Coin")
        {
            HandlePowerUp(item);
            return;
        }

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


    void HandlePowerUp(ShopItem item)
    {
        string type = "DoubleCoin";

        // ❌ already active
        if (DatabaseManager.Instance.IsPowerUpActive(type))
        {
            resultPopup.Show("Power-up is already active!");
            return;
        }

        int playerCoins = coinSystem.currentCoins;

        // ❌ not enough coins
        if (playerCoins < item.price)
        {
            resultPopup.Show(notEnoughCoinsMessage);
            return;
        }

        // ✅ BUY
        coinSystem.SpendCoins(item.price);

        DatabaseManager.Instance.ActivatePowerUp(type);

        resultPopup.Show("Double Coins Activated!");
    }

}
