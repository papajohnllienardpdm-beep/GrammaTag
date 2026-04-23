using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SettingsPanelController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;

    [Header("Scene Name")]
    public string mainMenuSceneName = "MainMenu"; // pwede mong baguhin sa inspector

    // 👉 OPEN SETTINGS
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    // 👉 CLOSE SETTINGS
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // 👉 GO TO MAIN MENU
    public void GoToMainMenu()
    {
        // 🔥 SAVE CURRENT HEART STATE
        if (HeartSystem.Instance != null && DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.UpdateHearts(
                HeartSystem.Instance.currentHearts,
                DateTime.Now
            );
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
