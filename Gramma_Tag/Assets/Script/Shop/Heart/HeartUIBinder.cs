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
        // hintayin HeartSystem
        yield return new WaitUntil(() => HeartSystem.Instance != null);

        // 🔥 CONNECT UI
        HeartSystem.Instance.heartsText = heartsText;
        HeartSystem.Instance.timerText = timerText;

        // 🔥 FORCE RELOAD FROM DB
        HeartSystem.Instance.ReloadFromDatabase();

        // 🔥 SUBSCRIBE SA UPDATE
        HeartSystem.Instance.OnHeartUpdated += UpdateUI;

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
