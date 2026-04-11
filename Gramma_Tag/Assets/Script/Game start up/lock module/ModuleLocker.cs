using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleLocker : MonoBehaviour
{
    public int moduleIndex; // 1 to 8
    public GameObject lockPanel;
    public Button moduleButton;

    void Start()
    {
        UpdateLockState();
    }

    public void UpdateLockState()
    {
        bool unlocked = PlayerPrefsManager.Instance.IsModuleUnlocked(moduleIndex);

        lockPanel.SetActive(!unlocked);
        moduleButton.interactable = unlocked;
    }
}
