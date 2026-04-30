using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SettingsPanelController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;

    [Header("Scene Name")]
    public string mainMenuSceneName = "MainMenu"; // pwede mong baguhin sa inspector


    [Header("Animation")]
    public float animationDuration = 0.25f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine currentAnim;


    // 👉 OPEN SETTINGS
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);

        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(ScalePanel(Vector3.zero, Vector3.one));
    }

    // 👉 CLOSE SETTINGS
    public void CloseSettings()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(CloseAnim());
    }

    // 👉 GO TO MAIN MENU
    public void GoToMainMenu()
    {
        // 🔥 SAVE CURRENT HEART STATE
        if (HeartSystem.Instance != null && DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.UpdateHearts(
                HeartSystem.Instance.currentHearts,
                DateTime.Now
            );
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    IEnumerator ScalePanel(Vector3 from, Vector3 to)
    {
        float time = 0f;

        while (time < animationDuration)
        {
            float t = time / animationDuration;
            float curveValue = scaleCurve.Evaluate(t);

            settingsPanel.transform.localScale = Vector3.LerpUnclamped(from, to, curveValue);

            time += Time.deltaTime;
            yield return null;
        }

        settingsPanel.transform.localScale = to;
    }

    IEnumerator CloseAnim()
    {
        yield return ScalePanel(Vector3.one, Vector3.zero);
        settingsPanel.SetActive(false);
    }
}
