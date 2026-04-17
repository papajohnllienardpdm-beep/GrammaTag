using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeartSystem : MonoBehaviour
{
    public static HeartSystem Instance;

    [Header("UI")]
    public TextMeshProUGUI heartsText;
    public TextMeshProUGUI timerText;

    [Header("Settings")]
    public int maxHearts = 5;
    public float cooldownMinutes = 3f;

    [Header("Runtime")]
    public int currentHearts = 5;

    private DateTime lastHeartTime;

    // 🔥 EVENT (ITO ANG FIX)
    public Action OnHeartUpdated;

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

    IEnumerator Start()
    {
        yield return new WaitUntil(() => DatabaseManager.Instance != null);
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

                    Debug.Log("❤️ Heart regenerated!");

                    // 🔥 TRIGGER EVENT
                    OnHeartUpdated?.Invoke();
                }

                double remaining = secondsNeeded - timePassed.TotalSeconds;
                if (remaining < 0) remaining = 0;

                if (timerText != null)
                {
                    timerText.text = GetFormattedTime();
                    timerText.gameObject.SetActive(true);
                }
            }
            else
            {
                if (timerText != null)
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

        // 🔥 TRIGGER EVENT
        OnHeartUpdated?.Invoke();
    }

    void UpdateUI()
    {
        if (heartsText != null)
            heartsText.text = currentHearts.ToString();
    }

    public double GetRemainingTime()
    {
        if (currentHearts >= maxHearts) return 0;

        TimeSpan timePassed = DateTime.Now - lastHeartTime;
        double secondsNeeded = cooldownMinutes * 60;

        double remaining = secondsNeeded - timePassed.TotalSeconds;

        if (remaining < 0) remaining = 0;

        return remaining;
    }

    public string GetFormattedTime()
    {
        double remaining = GetRemainingTime();

        int min = Mathf.FloorToInt((float)remaining / 60);
        int sec = Mathf.FloorToInt((float)remaining % 60);

        return $"{min:D2}:{sec:D2}";
    }

    string FormatTime(double seconds)
    {
        int min = Mathf.FloorToInt((float)seconds / 60);
        int sec = Mathf.FloorToInt((float)seconds % 60);

        return $"{min:D2}:{sec:D2}";
    }

}
