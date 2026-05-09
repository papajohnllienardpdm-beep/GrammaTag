using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultPopupUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI messageText;

    Coroutine currentCoroutine;

    void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void Show(string message)
    {
        if (panel == null) return;

        panel.SetActive(true);

        if (messageText != null)
            messageText.text = message;

        // 🔥 STOP OLD ANIMATION
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(OpenPopupAnimation());
    }

    public void Close()
    {
        if (panel == null) return;

        // 🔥 STOP OLD ANIMATION
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(ClosePopupAnimation());
    }

    IEnumerator OpenPopupAnimation()
    {
        RectTransform panelRect = panel.GetComponent<RectTransform>();

        panelRect.localScale = Vector3.zero;

        float timer = 0f;
        float duration = 0.15f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float scale = Mathf.SmoothStep(0f, 1f, timer / duration);

            panelRect.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        panelRect.localScale = Vector3.one;
    }

    IEnumerator ClosePopupAnimation()
    {
        RectTransform panelRect = panel.GetComponent<RectTransform>();

        float timer = 0f;
        float duration = 0.12f;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, timer / duration);

            panelRect.localScale =
                Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        panelRect.localScale = Vector3.zero;

        panel.SetActive(false);
    }
}
