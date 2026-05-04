using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HeartUIBinder : MonoBehaviour
{
    public TextMeshProUGUI heartsText;
    public TextMeshProUGUI timerText;

    void Start()
    {
        StartCoroutine(Setup());
    }

    System.Collections.IEnumerator Setup()
    {
        // wait HeartSystem
        yield return new WaitUntil(() => HeartSystem.Instance != null);

        // wait DB ready (important)
        yield return new WaitUntil(() => DatabaseManager.Instance != null && DatabaseManager.Instance.IsDatabaseReady());

        // 🔥 CONNECT UI ONLY (no reload)
        HeartSystem.Instance.heartsText = heartsText;
        HeartSystem.Instance.timerText = timerText;

        // 🔥 SUBSCRIBE
        HeartSystem.Instance.OnHeartUpdated += UpdateUI;

        // 🔥 INITIAL UPDATE
        UpdateUI();
    }

    void UpdateUI()
    {
        if (HeartSystem.Instance == null) return;

        heartsText.text = HeartSystem.Instance.currentHearts.ToString();

        if (timerText != null)
            timerText.text = HeartSystem.Instance.GetFormattedTime();
    }

    void OnDestroy()
    {
        if (HeartSystem.Instance != null)
        {
            HeartSystem.Instance.OnHeartUpdated -= UpdateUI;
        }
    }
}
