using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingManager : MonoBehaviour
{
    public Slider loadingBar;
    public Toggle termsToggle;
    public GameObject termsPanel;

    public GameObject cardImage;

    public float loadingSpeed = 0.5f;

    private bool hasUser = false;

    public void OpenTermsCard()
    {
        if (cardImage != null)
        {
            cardImage.SetActive(true);

            StopCoroutine("OpenCardAnimation");
            StartCoroutine("OpenCardAnimation");
        }
    }

    IEnumerator OpenCardAnimation()
    {
        RectTransform cardRect = cardImage.GetComponent<RectTransform>();

        cardRect.localScale = Vector3.zero;

        float timer = 0f;
        float duration = 0.15f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float scale = Mathf.SmoothStep(0f, 1f, timer / duration);

            cardRect.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        cardRect.localScale = Vector3.one;
    }

    public void CloseTermsCard()
    {
        if (cardImage != null)
        {
            StopCoroutine("OpenCardAnimation");
            StartCoroutine("CloseCardAnimation");
        }
    }

    IEnumerator CloseCardAnimation()
    {
        RectTransform cardRect = cardImage.GetComponent<RectTransform>();

        float timer = 0f;
        float duration = 0.12f;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, timer / duration);

            cardRect.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        cardRect.localScale = Vector3.zero;

        cardImage.SetActive(false);
    }

    IEnumerator Start()
    {
        Debug.Log("⏳ Waiting for DB...");

        if (cardImage != null)
        {
            cardImage.SetActive(false);
        }

        float timer = 0f;
        float timeout = 5f;

        while (
            (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
            && timer < timeout
        )
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (DatabaseManager.Instance == null || !DatabaseManager.Instance.IsDatabaseReady())
        {
            Debug.LogError("❌ DB FAILED TO LOAD (TIMEOUT)");
        }
        else
        {
            Debug.Log("✅ DATABASE READY SA LOADING");
        }

        bool hasDBUser =
            DatabaseManager.Instance != null &&
            DatabaseManager.Instance.HasUser();

        if (hasDBUser)
        {
            Debug.Log("✅ EXISTING USER → SKIP TERMS");

            hasUser = true;

            termsPanel.SetActive(false);
        }
        else
        {
            Debug.Log("🆕 NEW USER → SHOW TERMS");

            hasUser = false;

            // 🔥 SHOW TERMS AGAD
            termsPanel.SetActive(true);

            // 🔥 RESET TOGGLE
            termsToggle.isOn = false;

            // 🔥 PWEDENG I-CHECK
            termsToggle.interactable = true;
        }

        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.5f);

        string targetScene;

        if (hasUser)
            targetScene = "MainMenu";
        else
            targetScene = "LoginScene";

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        operation.allowSceneActivation = false;

        float progress = 0f;
        bool accepted = false;

        while (true)
        {
            // 🔥 FOR NEW USER
            if (!hasUser)
            {
                // 🔥 WAIT HANGGANG MA-CHECK
                if (!accepted)
                {
                    if (termsToggle.isOn)
                    {
                        accepted = true;

                        // 🔥 LOCK CHECKBOX
                        termsToggle.interactable = false;
                    }
                }
                else
                {
                    // 🔥 TULOY LOADING
                    progress += Time.deltaTime * loadingSpeed;
                }
            }
            else
            {
                // 🔥 EXISTING USER
                progress += Time.deltaTime * loadingSpeed;
            }

            loadingBar.value = Mathf.Clamp01(progress);

            if (operation.progress >= 0.9f && progress >= 1f)
            {
                yield return new WaitForSeconds(0.3f);
                operation.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }
    }
}
