using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeartSystem : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI heartsText;
    public TextMeshProUGUI timerText;

    [Header("Settings")]
    public int maxHearts = 5;
    public int currentHearts = 5;
    public float cooldownMinutes = 3f;

    private DateTime lastHeartTime;

    IEnumerator Start()
    {
        // WAIT hanggang may instance
        yield return new WaitUntil(() => DatabaseManager.Instance != null);

        // WAIT hanggang ready ang DB
        yield return new WaitUntil(() => DatabaseManager.Instance.IsDatabaseReady());

        LoadData();
        StartCoroutine(HeartRegeneration());
    }

    void LoadData()
    {
        currentHearts = DatabaseManager.Instance.GetHearts();
        lastHeartTime = DatabaseManager.Instance.GetLastHeartTime();

        UpdateUI();
    }

    IEnumerator HeartRegeneration()
    {
        while (true)
        {
            if (currentHearts < maxHearts)
            {
                TimeSpan timePassed = DateTime.Now - lastHeartTime;
                double secondsNeeded = cooldownMinutes * 60;

                if (timePassed.TotalSeconds >= secondsNeeded)
                {
                    currentHearts++;
                    lastHeartTime = DateTime.Now;

                    DatabaseManager.Instance.UpdateHearts(currentHearts, lastHeartTime);
                }
                else
                {
                    double remaining = secondsNeeded - timePassed.TotalSeconds;
                    timerText.text = FormatTime(remaining);
                    timerText.gameObject.SetActive(true);
                }
            }
            else
            {
                timerText.gameObject.SetActive(false);
            }

            UpdateUI();
            yield return new WaitForSeconds(1f);
        }
    }

    public void UseHeart(int amount = 1)
    {
        if (currentHearts <= 0) return;

        currentHearts -= amount;

        if (currentHearts < 0)
            currentHearts = 0;

        lastHeartTime = DateTime.Now;

        DatabaseManager.Instance.UpdateHearts(currentHearts, lastHeartTime);

        UpdateUI();
    }

    void UpdateUI()
    {
        heartsText.text = currentHearts.ToString();
    }

    string FormatTime(double seconds)
    {
        int min = Mathf.FloorToInt((float)seconds / 60);
        int sec = Mathf.FloorToInt((float)seconds % 60);

        return $"{min:D2}:{sec:D2}";
    }
}
