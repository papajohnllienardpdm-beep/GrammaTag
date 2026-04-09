using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementNavigation : MonoBehaviour
{
    [Header("Containers")]
    public GameObject achievementPanel;   // list ng modules
    public GameObject certificatePanel;   // lalabas na certificates

    [Header("Certificates")]
    public GameObject[] certificates;     // lahat ng certificate panels

    // 👉 OPEN CERTIFICATE (AUTO INDEX)
    public void OpenCertificate(GameObject panel)
    {
        int index = panel.transform.GetSiblingIndex();

        if (index < 0 || index >= certificates.Length) return;

        achievementPanel.SetActive(false);
        certificatePanel.SetActive(true);

        // hide all
        foreach (GameObject c in certificates)
        {
            c.SetActive(false);
        }

        // show selected
        certificates[index].SetActive(true);
    }

    // 👉 BACK
    public void BackToAchievements()
    {
        achievementPanel.SetActive(true);
        certificatePanel.SetActive(false);

        foreach (GameObject c in certificates)
        {
            c.SetActive(false);
        }
    }
}
