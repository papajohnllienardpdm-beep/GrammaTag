using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class VideoManager : MonoBehaviour
{
    public GameObject videoPanel;
    public GameObject gamePanel;

    public VideoPlayer videoPlayer;
    public VideoClip[] videos;

    // 👉 scene per video
    public string[] nextScenes;

    public OrientationManager orientationManager;

    private int currentIndex = -1;

    public void PlayVideo(int index)
    {
        if (index < 0 || index >= videos.Length) return;

        currentIndex = index;

        // 👉 landscape mode
        orientationManager.SetLandscape();

        gamePanel.SetActive(false);
        videoPanel.SetActive(true);

        videoPlayer.clip = videos[index];

        // 🔥 PREPARE FIRST (NO LAG START)
        videoPlayer.prepareCompleted += OnPrepared;

        // 🔥 detect video end
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

        // 👉 stop video
        videoPlayer.Stop();

        // 👉 balik portrait bago mag scene
        orientationManager.SetPortrait();

        // 👉 load scene after delay
        StartCoroutine(LoadSceneAfterOrientation());
    }

    IEnumerator LoadSceneAfterOrientation()
    {
        yield return new WaitForSecondsRealtime(0.2f); // 🔥 important delay

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

        // 👉 balik portrait
        orientationManager.SetPortrait();

        videoPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
}
