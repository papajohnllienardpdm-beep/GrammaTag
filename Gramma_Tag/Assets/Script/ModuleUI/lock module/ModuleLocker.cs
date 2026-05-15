using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleLocker : MonoBehaviour
{
    public int moduleIndex;

    [Header("MAIN UI")]
    public GameObject lockPanel;
    public Button moduleButton;

    [Header("ACHIEVEMENT UI")]
    public GameObject achievementLockPanel; // 🔥 bagong idinagdag

    [Header("UNLOCK SETTINGS")]
    public bool useVideoUnlock;        // para sa Module 1
    public int requiredSubtopicID;     // para sa Module 2+

    IEnumerator Start()
    {
        // 🔥 wait for systems
        while (PlayerPrefsManager.Instance == null ||
               DatabaseManager.Instance == null ||
               !DatabaseManager.Instance.IsDatabaseReady())
        {
            yield return null;
        }

        UpdateLockState();
    }

    void OnEnable()
    {
        if (PlayerPrefsManager.Instance != null &&
            DatabaseManager.Instance != null &&
            DatabaseManager.Instance.IsDatabaseReady())
        {
            UpdateLockState();
        }
    }

    public void UpdateLockState()
    {
        bool unlocked = false;

        // 🔥 BACKDOOR = UNLOCK ALL
        if (BackdoorManager.backdoorEnabled)
        {
            unlocked = true;
        }
        else
        {
            // 🎬 VIDEO BASED (Module 1)
            if (useVideoUnlock)
            {
                unlocked =
                    PlayerPrefsManager.Instance
                    .IsModuleUnlocked(moduleIndex);
            }
            else
            {
                // 📚 DATABASE BASED (Module 2 pataas)
                unlocked =
                    DatabaseManager.Instance
                    .IsSubtopicPassed(requiredSubtopicID);
            }
        }

        // 🔒 MAIN LOCK PANEL
        if (lockPanel != null)
            lockPanel.SetActive(!unlocked);

        // 🔘 BUTTON INTERACTABLE
        if (moduleButton != null)
            moduleButton.interactable = unlocked;

        // 🏆 ACHIEVEMENT LOCK PANEL
        if (achievementLockPanel != null)
            achievementLockPanel.SetActive(!unlocked);

        Debug.Log(
            $"[Module {moduleIndex}] unlocked: {unlocked} | RequiredSubtopic: {requiredSubtopicID}"
        );
    }
}
