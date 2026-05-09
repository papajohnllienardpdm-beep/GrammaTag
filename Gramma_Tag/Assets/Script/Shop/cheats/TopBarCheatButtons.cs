using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TopBarCheatButtons : MonoBehaviour
{
    [Header("References")]
    public CoinSystem coinSystem;

    [Header("Buttons")]
    public Button heartsButton;
    public Button coinsButton;

    void Start()
    {
        // ❤️ HEART CLICK
        if (heartsButton != null)
        {
            heartsButton.onClick.AddListener(AddHeart);
        }

        // 🪙 COIN CLICK
        if (coinsButton != null)
        {
            coinsButton.onClick.AddListener(AddCoins);
        }
    }

    void AddHeart()
    {
        if (HeartSystem.Instance != null)
        {
            HeartSystem.Instance.AddHeart(1);
        }
    }

    void AddCoins()
    {
        if (coinSystem != null)
        {
            coinSystem.AddCoins(100);
        }
    }
}
