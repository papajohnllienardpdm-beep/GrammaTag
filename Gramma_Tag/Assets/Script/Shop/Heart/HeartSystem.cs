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

        RecoverOfflineHearts();
        UpdateUI();
    }

    void RecoverOfflineHearts()
    {
        if (currentHearts >= maxHearts) return;

        TimeSpan timePassed = DateTime.Now - lastHeartTime;
        double secondsNeeded = cooldownMinutes * 60;

        int heartsToAdd = (int)(timePassed.TotalSeconds / secondsNeeded);

        if (heartsToAdd > 0)
        {
            currentHearts += heartsToAdd;

            if (currentHearts > maxHearts)
                currentHearts = maxHearts;

            double usedSeconds = heartsToAdd * secondsNeeded;
            lastHeartTime = lastHeartTime.AddSeconds(usedSeconds);

            DatabaseManager.Instance.UpdateHearts(currentHearts, lastHeartTime);

            OnHeartUpdated?.Invoke();
        }
    }

    IEnumerator HeartRegeneration()
    {
        while (true)
        {
            if (currentHearts < maxHearts)
            {
                TimeSpan timePassed = DateTime.Now - lastHeartTime;
                double secondsNeeded = cooldownMinutes * 60;

                int heartsToAdd = (int)(timePassed.TotalSeconds / secondsNeeded);

                if (heartsToAdd > 0)
                {
                    currentHearts += heartsToAdd;

                    if (currentHearts > maxHearts)
                        currentHearts = maxHearts;

                    double usedSeconds = heartsToAdd * secondsNeeded;
                    lastHeartTime = lastHeartTime.AddSeconds(usedSeconds);

                    DatabaseManager.Instance.UpdateHearts(currentHearts, lastHeartTime);

                    OnHeartUpdated?.Invoke();
                }
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
        OnHeartUpdated?.Invoke();
    }

    void UpdateUI()
    {
        if (heartsText != null)
            heartsText.text = currentHearts.ToString();

        if (timerText != null)
        {
            timerText.text = GetFormattedTime();
            timerText.gameObject.SetActive(true);
        }
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
        if (currentHearts >= maxHearts)
            return "Full";

        double remaining = GetRemainingTime();

        int min = Mathf.FloorToInt((float)remaining / 60);
        int sec = Mathf.FloorToInt((float)remaining % 60);

        return $"{min:D2}:{sec:D2}";
    }

    void OnApplicationPause(bool pause)
    {
        if (pause && DatabaseManager.Instance != null && DatabaseManager.Instance.IsDatabaseReady())
        {
            DatabaseManager.Instance.UpdateHearts(currentHearts, lastHeartTime);
        }
    }

    void OnApplicationQuit()
    {
        if (DatabaseManager.Instance != null && DatabaseManager.Instance.IsDatabaseReady())
        {
            DatabaseManager.Instance.UpdateHearts(currentHearts, lastHeartTime);
        }
    }

    public void ReloadFromDatabase()
    {
        if (DatabaseManager.Instance == null) return;

        currentHearts = DatabaseManager.Instance.GetHearts();
        lastHeartTime = DatabaseManager.Instance.GetLastHeartTime();

        RecoverOfflineHearts();
        UpdateUI();

        OnHeartUpdated?.Invoke();
    }
}
