using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject achievementPanel;
    public GameObject shopPanel;
    public GameObject settingsPanel;

    void Start()
    {
        ShowHome();
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
