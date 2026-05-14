using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackdoorManager : MonoBehaviour
{
    public static bool backdoorEnabled = false;

    public void EnableBackdoor()
    {
        backdoorEnabled = true;

        Debug.Log("🔥 BACKDOOR ENABLED");

        // 🔥 UNLOCK ALL VIDEOS
        for (int i = 1; i <= 100; i++)
        {
            PlayerPrefs.SetInt("VIDEO_WATCHED_" + i, 1);
        }

        PlayerPrefs.Save();

        // 🔥 REFRESH MODULE LOCKERS
        ModuleLocker[] modules =
            FindObjectsOfType<ModuleLocker>(true);

        foreach (ModuleLocker module in modules)
        {
            module.UpdateLockState();
        }

        // 🔥 REFRESH SUBTOPIC LOCKERS
        SubtopicLocker[] subtopics =
            FindObjectsOfType<SubtopicLocker>(true);

        foreach (SubtopicLocker subtopic in subtopics)
        {
            subtopic.UpdateLock();
        }
    }
}
