using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ModuleOverviewUI : MonoBehaviour
{
    public int moduleID;
    public TextMeshProUGUI overviewText;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => DatabaseManager.Instance != null);
        yield return new WaitUntil(() => DatabaseManager.Instance.IsDatabaseReady());

        LoadOverview();
    }

    void LoadOverview()
    {
        string overview = DatabaseManager.Instance.GetModuleOverview(moduleID);

        if (overviewText != null)
        {
            overviewText.text = overview;
        }
        else
        {
            Debug.LogError("Overview Text is not assigned!");
        }
    }
}
