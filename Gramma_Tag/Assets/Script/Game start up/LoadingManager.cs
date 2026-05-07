using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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

        bool hasDBUser = DatabaseManager.Instance != null && DatabaseManager.Instance.HasUser();
        bool hasLogged = PlayerPrefs.GetInt("HAS_LOGGED_IN", 0) == 1;

        if (hasDBUser && hasLogged)
        {
            Debug.Log("✅ EXISTING USER → SKIP TERMS");

            hasUser = true;

            // ❌ siguradong walang terms pag may user
            termsPanel.SetActive(false);
        }
        else
        {
            Debug.Log("🆕 NEW USER → SHOW TERMS LATER");

            hasUser = false;

            // ❗ wag muna ipakita agad
            // lalabas lang sa 50% (LoadScene logic mo)
            termsPanel.SetActive(false);
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
        bool stoppedAtHalf = false;
        bool accepted = false;

        while (true)
        {
            // 🔥 PHASE 1
            if (!stoppedAtHalf)
            {
                progress += Time.deltaTime * loadingSpeed;

                // 👉 ONLY STOP kung WALANG USER
                if (!hasUser && progress >= 0.5f)
                {
                    progress = 0.5f;
                    stoppedAtHalf = true;

                    termsPanel.SetActive(true); // SHOW TERMS
                }
            }
            // 🔥 WAIT LANG KUNG WALANG USER
            else if (!accepted)
            {
                if (termsToggle.isOn)
                {
                    accepted = true;
                    // panel stays (ayon sa gusto mo)
                }
            }
            else
            {
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
