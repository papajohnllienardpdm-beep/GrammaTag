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
        public TextMeshProUGUI videoLengthLabelText; // UI text

        [TextArea]
        public string videoLengthValue; // input sa Inspector
    }

    public SubtopicData[] subtopics;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => DatabaseManager.Instance != null);
        yield return new WaitUntil(() => DatabaseManager.Instance.IsDatabaseReady());

        LoadSubtopics(); // or LoadOverview()
    }

    void LoadSubtopics()
    {
        if (DatabaseManager.Instance == null)
        {
            Debug.LogError("DatabaseManager is NULL!");
            return;
        }

        if (!DatabaseManager.Instance.IsDatabaseReady())
        {
            Debug.LogError("Database not ready!");
            return;
        }

        foreach (var sub in subtopics)
        {
            if (sub == null)
            {
                Debug.LogError("Subtopic is NULL!");
                continue;
            }

            var data = DatabaseManager.Instance.GetModuleData(sub.moduleID);

            if (data != null)
            {
                if (sub.moduleNameText != null)
                    sub.moduleNameText.text = data.ModuleName;

                if (sub.videoLengthLabelText != null)
                    sub.videoLengthLabelText.text = sub.videoLengthValue;
            }
            else
            {
                Debug.LogWarning("No data for ModuleID: " + sub.moduleID);
            }
        }
    }
}
