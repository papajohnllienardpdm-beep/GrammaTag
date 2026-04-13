using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleNavigation : MonoBehaviour
{
    [Header("Containers")]
    public GameObject moduleContainer;
    public GameObject contentPanel;

    [Header("Module Contents")]
    public GameObject[] modules;

    // 👉 OPEN MODULE
    public void OpenModuleByIndex(int index)
    {
        if (index < 0 || index >= modules.Length) return;

        moduleContainer.SetActive(false);
        contentPanel.SetActive(true);

        foreach (GameObject m in modules)
            m.SetActive(false);

        modules[index].SetActive(true);
    }

    // 👉 BACK
    public void BackToModules()
    {
        moduleContainer.SetActive(true);
        contentPanel.SetActive(false);

        foreach (GameObject m in modules)
            m.SetActive(false);
    }
}
