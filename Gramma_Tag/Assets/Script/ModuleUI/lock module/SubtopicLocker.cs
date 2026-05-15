using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtopicLocker : MonoBehaviour
{
    public int moduleID; // current subtopic ID
    public int previousModuleID; // 🔥 manual assign
    public Button button;

    IEnumerator Start()
    {
        while (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            yield return null;

        UpdateLock();
    }

    public void UpdateLock()
    {
        bool unlocked = false;

        // 🔥 BACKDOOR = UNLOCK ALL
        if (BackdoorManager.backdoorEnabled)
        {
            unlocked = true;
        }
        else
        {
            // 🔥 FIRST SUBTOPIC
            if (previousModuleID == 0)
            {
                unlocked = true;
            }
            else
            {
                unlocked =
                    DatabaseManager.Instance
                    .IsSubtopicPassed(previousModuleID);
            }
        }

        button.interactable = unlocked;

        Debug.Log(
            $"Subtopic {moduleID} unlocked: {unlocked}"
        );
    }
}
