using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CertificateContentManager : MonoBehaviour
{
    [Header("Containers")]
    public GameObject certificateContainer;
    public GameObject certificateContentContainer;

    [Header("Difficulty Content Panels")]
    public GameObject[] difficultyContentPanels;
    // 0 = Easy, 1 = Medium, 2 = Hard

    [Header("Modules per Difficulty (AUTO DETECT)")]
    public Transform[] moduleParents;
    // Drag:
    // Easy Content Panel
    // Medium Content Panel
    // Hard Content Panel

    private List<List<GameObject>> modulePanels = new List<List<GameObject>>();

    void Start()
    {
        SetupModules();
    }

    void SetupModules()
    {
        modulePanels.Clear();

        foreach (Transform parent in moduleParents)
        {
            List<GameObject> modules = new List<GameObject>();

            foreach (Transform child in parent)
            {
                modules.Add(child.gameObject);
                child.gameObject.SetActive(false); // hide lahat
            }

            modulePanels.Add(modules);
        }
    }

    // ✅ BUTTON FRIENDLY (1 PARAMETER LANG)
    public void OpenModule(int encodedValue)
    {
        int difficultyIndex = encodedValue / 100;
        int moduleIndex = encodedValue % 100;

        // Hide certificate list
        certificateContainer.SetActive(false);
        certificateContentContainer.SetActive(true);

        // Hide all difficulty panels
        for (int i = 0; i < difficultyContentPanels.Length; i++)
        {
            difficultyContentPanels[i].SetActive(false);
        }

        // Show correct difficulty
        difficultyContentPanels[difficultyIndex].SetActive(true);

        // Hide all modules in that difficulty
        for (int i = 0; i < modulePanels[difficultyIndex].Count; i++)
        {
            modulePanels[difficultyIndex][i].SetActive(false);
        }

        // Show selected module
        modulePanels[difficultyIndex][moduleIndex].SetActive(true);
    }

    public void BackToCertificateList()
    {
        certificateContentContainer.SetActive(false);
        certificateContainer.SetActive(true);
    }
}
