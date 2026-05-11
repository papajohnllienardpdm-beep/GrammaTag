using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject achievementPanel;
    public GameObject shopPanel;
    public GameObject settingsPanel;

    public TextMeshProUGUI nameText;


    IEnumerator Start()
    {
        yield return new WaitUntil(() => DatabaseManager.Instance != null);

        ShowHome();
        LoadPlayerData();
    }

    public void LoadPlayerData()
    {
        if (DatabaseManager.Instance == null)
        {
            Debug.LogError("DatabaseManager is NULL!");
            return;
        }

        string playerName = DatabaseManager.Instance.GetPlayerName();
        nameText.text = $"Hi, {playerName}!";
    }

    public void ShowHome()
    {
        homePanel.SetActive(true);
        achievementPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ShowAchievement()
    {
        homePanel.SetActive(false);
        achievementPanel.SetActive(true);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ShowShop()
    {
        homePanel.SetActive(false);
        achievementPanel.SetActive(false);
        shopPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        homePanel.SetActive(false);
        achievementPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
}
