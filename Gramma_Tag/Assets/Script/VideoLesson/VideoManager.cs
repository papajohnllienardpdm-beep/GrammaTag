using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class VideoManager : MonoBehaviour
{
    public GameObject videoPanel;
    public GameObject gamePanel;

    // 🔥 NEW (ADD THIS)
    public GameObject mainMenu;

    public VideoPlayer videoPlayer;
    public VideoClip[] videos;

    public string[] nextScenes;

    public OrientationManager orientationManager;

    private int currentIndex = -1;

    public void PlayVideo(int index)
    {
        if (index < 0 || index >= videos.Length) return;

        currentIndex = index;

        // 👉 landscape mode
        orientationManager.SetLandscape();

        // 🔥 NEW (HIDE MAIN MENU)
        if (mainMenu != null)
            mainMenu.SetActive(false);

        gamePanel.SetActive(false);
        videoPanel.SetActive(true);

        videoPlayer.clip = videos[index];

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Prepare();
    }

    void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnVideoFinished;

        videoPlayer.Stop();

        // 👉 balik portrait
        orientationManager.SetPortrait();

        // 🔥 NEW (SHOW MAIN MENU AGAIN)
        if (mainMenu != null)
            mainMenu.SetActive(true);

        StartCoroutine(LoadSceneAfterOrientation());
    }

    IEnumerator LoadSceneAfterOrientation()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        if (currentIndex >= 0 && currentIndex < nextScenes.Length)
        {
            string sceneName = nextScenes[currentIndex];

            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }

    public void CloseVideo()
    {
        videoPlayer.Stop();

        // 👉 portrait
        orientationManager.SetPortrait();

        // 🔥 NEW (SHOW MAIN MENU AGAIN)
        if (mainMenu != null)
            mainMenu.SetActive(true);

        videoPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
}
