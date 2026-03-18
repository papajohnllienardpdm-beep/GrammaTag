using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleNavigation : MonoBehaviour
{
    [Header("Containers")]
    public GameObject moduleContainer;     // list ng modules (buttons)
    public GameObject contentPanel;        // EasyContentPanel

    [Header("Module Contents")]
    public GameObject[] modules;           // lahat ng content

    // 👉 OPEN MODULE
    public void OpenModule(int index)
    {
        if (index < 0 || index >= modules.Length) return;

        moduleContainer.SetActive(false);
        contentPanel.SetActive(true);

        // hide all modules
        foreach (GameObject m in modules)
        {
            m.SetActive(false);
        }

        // show selected
        modules[index].SetActive(true);
    }

    // 👉 BACK TO MODULE LIST
    public void BackToModules()
    {
        moduleContainer.SetActive(true);
        contentPanel.SetActive(false);

        foreach (GameObject m in modules)
        {
            m.SetActive(false);
        }
    }
}
