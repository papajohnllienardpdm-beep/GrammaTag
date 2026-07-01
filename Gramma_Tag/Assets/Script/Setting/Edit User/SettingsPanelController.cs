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
    public string mainMenuSceneName = "MainMenu";

    [Header("Heart Deduction")]
    public bool deductHeartOnExit = true;

    [Header("Animation")]
    public float animationDuration = 0.25f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine currentAnim;

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);

        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(ScalePanel(Vector3.zero, Vector3.one));
    }

    public void CloseSettings()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(CloseAnim());
    }

    public void GoToMainMenu()
    {
        // 🔥 BAWAS HEART ONLY IF THIS SCENE IS AN ACTUAL GAME
        if (deductHeartOnExit && HeartSystem.Instance != null)
        {
            HeartSystem.Instance.UseHeartSafe(1);
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
