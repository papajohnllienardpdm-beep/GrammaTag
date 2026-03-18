using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gamePanel;

    public GameObject easyPanel;
    public GameObject mediumPanel;
    public GameObject hardPanel;

    public void PlayEasy()
    {
        mainMenu.SetActive(false);
        gamePanel.SetActive(true);

        easyPanel.SetActive(true);
        mediumPanel.SetActive(false);
        hardPanel.SetActive(false);
    }

    public void PlayMedium()
    {
        mainMenu.SetActive(false);
        gamePanel.SetActive(true);

        easyPanel.SetActive(false);
        mediumPanel.SetActive(true);
        hardPanel.SetActive(false);
    }

    public void PlayHard()
    {
        mainMenu.SetActive(false);
        gamePanel.SetActive(true);

        easyPanel.SetActive(false);
        mediumPanel.SetActive(false);
        hardPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        mainMenu.SetActive(true);
        gamePanel.SetActive(false);
    }
}
