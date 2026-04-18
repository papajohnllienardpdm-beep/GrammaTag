using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleLocker : MonoBehaviour
{
    public int moduleIndex;

    [Header("UI")]
    public GameObject lockPanel;
    public Button moduleButton;

    [Header("Unlock Settings")]
    public bool useVideoUnlock; // ✅ para sa Module 1 lang
    public int requiredSubtopicID; // ✅ database based

    IEnumerator Start()
    {
        // wait for systems
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
        if (PlayerPrefsManager.Instance != null && DatabaseManager.Instance != null)
        {
            UpdateLockState();
        }
    }

    public void UpdateLockState()
    {
        bool unlocked = false;

        // 🎬 CASE 1: VIDEO BASED (Module 1)
        if (useVideoUnlock)
        {
            unlocked = PlayerPrefsManager.Instance.IsModuleUnlocked(moduleIndex);
        }
        else
        {
            // 📚 CASE 2: DATABASE BASED
            unlocked = DatabaseManager.Instance.IsSubtopicPassed(requiredSubtopicID);
        }

        if (lockPanel != null)
            lockPanel.SetActive(!unlocked);

        if (moduleButton != null)
            moduleButton.interactable = unlocked;

        Debug.Log($"Module {moduleIndex} unlocked: {unlocked}");
    }
}
