using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{

    public Slider loadingBar;
    public string nextScene = "LoginScene";

    public float loadingSpeed = 1f; // ikaw ang magdikta ng bilis

    void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        // 🔥 WAIT FOR DB READY
        yield return new WaitForSeconds(0.5f);

        string targetScene;

        if (DatabaseManager.Instance != null && DatabaseManager.Instance.HasUser())
        {
            Debug.Log("Auto login → MainMenu");
            targetScene = "MainMenu";
        }
        else
        {
            Debug.Log("No user → Login");
            targetScene = "LoginScene";
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        operation.allowSceneActivation = false;

        float progress = 0f;

        while (!operation.isDone)
        {
            progress += Time.deltaTime * loadingSpeed;
            loadingBar.value = Mathf.Clamp01(progress);

            if (loadingBar.value >= 1f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
