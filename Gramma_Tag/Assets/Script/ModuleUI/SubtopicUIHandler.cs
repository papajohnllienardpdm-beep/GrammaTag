using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubtopicUIHandler : MonoBehaviour
{
    [System.Serializable]
    public class SubtopicData
    {
        public int moduleID;
        public TextMeshProUGUI moduleNameText;
        public TextMeshProUGUI difficultyText;
    }

    public SubtopicData[] subtopics;

    void Start()
    {
        LoadSubtopics();
    }

    void LoadSubtopics()
    {
        if (!DatabaseManager.Instance.IsDatabaseReady())
        {
            Debug.LogError("Database not ready!");
            return;
        }

        foreach (var sub in subtopics)
        {
            var data = DatabaseManager.Instance.GetModuleData(sub.moduleID);

            if (data != null)
            {
                sub.moduleNameText.text = data.ModuleName;
                sub.difficultyText.text = data.Difficulty;
            }
            else
            {
                Debug.LogWarning("No data for ModuleID: " + sub.moduleID);
            }
        }
    }
}
