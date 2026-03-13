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
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
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
