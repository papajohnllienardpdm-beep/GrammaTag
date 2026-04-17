using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;


public class VideoManager : MonoBehaviour
{
    public GameObject videoPanel;
    public GameObject gamePanel;
    public GameObject mainMenu;

    public VideoPlayer videoPlayer;
    public VideoClip[] videos;

    public string[] nextScenes;
    public int[] moduleIDs;

    public OrientationManager orientationManager;

    [Header("No Lives UI")]
    public GameObject noLivesPanel;
    public TextMeshProUGUI timerText;

    private int currentIndex = -1;

    void Start()
    {
        if (noLivesPanel != null)
            noLivesPanel.SetActive(false);

        // 🔥 LISTEN SA HEART SYSTEM
        HeartSystem.Instance.OnHeartUpdated += HandleHeartUpdate;
    }

    void OnDestroy()
    {
        if (HeartSystem.Instance != null)
            HeartSystem.Instance.OnHeartUpdated -= HandleHeartUpdate;
    }

    public void PlayVideo(int index)
    {
        if (index < 0 || index >= videos.Length) return;

        currentIndex = index;

        if (index < moduleIDs.Length)
            PlayerPrefs.SetInt("SelectedModuleID", moduleIDs[index]);

        orientationManager.SetLandscape();

        mainMenu?.SetActive(false);
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

        if (HeartSystem.Instance.currentHearts <= 0)
        {
            ShowNoLivesPopup();
        }
        else
        {
            HeartSystem.Instance.UseHeart(1);
            StartCoroutine(LoadSceneAfterOrientation());
        }
    }

    void ShowNoLivesPopup()
    {
        noLivesPanel.SetActive(true);
        StartCoroutine(UpdateTimerUI());
    }

    IEnumerator UpdateTimerUI()
    {
        while (noLivesPanel.activeSelf)
        {
            timerText.text = HeartSystem.Instance.GetFormattedTime();
            yield return new WaitForSeconds(1f);
        }
    }

    // 🔥 DITO NA MAG CLOSE (EVENT BASED)
    void HandleHeartUpdate()
    {
        if (!noLivesPanel.activeSelf) return;

        if (HeartSystem.Instance.currentHearts > 0)
        {
            Debug.Log("❤️ Heart detected → auto continue");

            noLivesPanel.SetActive(false);

            HeartSystem.Instance.UseHeart(1);
            StartCoroutine(LoadSceneAfterOrientation());
        }
    }

    public void CloseNoLivesPopup()
    {
        noLivesPanel.SetActive(false);

        orientationManager.SetPortrait();

        mainMenu?.SetActive(true);
        videoPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    IEnumerator LoadSceneAfterOrientation()
    {
        orientationManager.SetPortrait();

        yield return new WaitForSecondsRealtime(0.2f);

        if (currentIndex >= 0 && currentIndex < nextScenes.Length)
        {
            string sceneName = nextScenes[currentIndex];

            if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName);
        }
    }
}
