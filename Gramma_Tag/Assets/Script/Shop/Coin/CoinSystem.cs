using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinSystem : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI coinsText;

    [Header("TEST MODE")]
    public bool useInspectorValue = false;
    public int inspectorCoins = 0;

    [Header("Runtime")]
    public int currentCoins = 0;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => DatabaseManager.Instance != null);
        yield return new WaitUntil(() => DatabaseManager.Instance.IsDatabaseReady());

        LoadCoins();
    }

    void LoadCoins()
    {
        if (useInspectorValue)
        {
            currentCoins = inspectorCoins;

            // 🔥 IMPORTANT: save agad sa DB
            DatabaseManager.Instance.UpdateCoins(currentCoins);
        }
        else
        {
            currentCoins = DatabaseManager.Instance.GetCoins();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        coinsText.text = currentCoins.ToString();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;

        DatabaseManager.Instance.UpdateCoins(currentCoins);
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins < amount)
        {
            Debug.Log("Not enough coins!");
            return false;
        }

        currentCoins -= amount;

        DatabaseManager.Instance.UpdateCoins(currentCoins);
        UpdateUI();

        return true;
    }

    // 🔥 AUTO UPDATE PAG BINAGO SA INSPECTOR (WHILE PLAYING)
    void OnValidate()
    {
        if (Application.isPlaying && useInspectorValue)
        {
            currentCoins = inspectorCoins;

            if (DatabaseManager.Instance != null && DatabaseManager.Instance.IsDatabaseReady())
            {
                DatabaseManager.Instance.UpdateCoins(currentCoins);
                UpdateUI();
            }
        }
    }
}
