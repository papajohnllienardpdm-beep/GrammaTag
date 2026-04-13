using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public static PlayerPrefsManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔓 Unlock module
    public void UnlockModule(int moduleIndex)
    {
        PlayerPrefs.SetInt("Module_" + moduleIndex, 1);
        PlayerPrefs.Save();
    }

    // 🔒 Lock module
    public void LockModule(int moduleIndex)
    {
        PlayerPrefs.SetInt("Module_" + moduleIndex, 0);
        PlayerPrefs.Save();
    }

    // 🔍 Check if unlocked
    public bool IsModuleUnlocked(int moduleIndex)
    {
        // ✅ ALWAYS UNLOCK VIDEO PANEL
        if (moduleIndex == 0)
            return true;

        return PlayerPrefs.GetInt("Module_" + moduleIndex, 0) == 1;
    }


}
