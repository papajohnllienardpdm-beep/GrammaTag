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

    public float loadingSpeed = 0.5f;

    private bool hasUser = false;

    IEnumerator Start()
    {
        Debug.Log("⏳ Waiting for DB...");

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
