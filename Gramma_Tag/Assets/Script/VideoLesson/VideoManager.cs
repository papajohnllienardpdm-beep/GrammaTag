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

    [Header("Video Controls")]
    public VideoControls videoControls;

    public OrientationManager orientationManager;

    [Header("No Lives UI")]
    public GameObject noLivesPanel;
    public TextMeshProUGUI timerText;

    [Header("No Lives Animation")]
    public float popupDuration = 0.25f;
    public AnimationCurve popupCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Begin Popup")]
    public GameObject popupBeginPanel;
    public float beginPopupWaitTime = 2f;


    private Coroutine popupAnim;

    private int currentIndex = -1;

    void Start()
    {
        if (noLivesPanel != null)
            noLivesPanel.SetActive(false);

        if (popupBeginPanel != null)
            popupBeginPanel.SetActive(false);

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
        {
            int selectedModule = moduleIDs[index];

            PlayerPrefs.SetInt("SelectedModuleID", selectedModule);
            PlayerPrefs.Save(); // 🔥 IMPORTANT

            // 🔥 CHECK IF VIDEO ALREADY WATCHED
            string watchKey =
                "VIDEO_WATCHED_" + selectedModule;

            bool alreadyWatched =
                PlayerPrefs.HasKey(watchKey);

            // 🔥 ENABLE / DISABLE SKIP
            if (videoControls != null)
            {
                videoControls.SetCanSkip(alreadyWatched);
            }

            Debug.Log("🔥 SELECTED MODULE ID: " + selectedModule);
        }
        else
        {
            Debug.LogError("❌ INVALID INDEX FOR MODULE ID: " + index);
        }

        orientationManager.SetLandscape();

        // 🔥 MUTE AUDIO
        if (AudioManager.Instance != null)
            AudioManager.Instance.MuteAll();

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

        // 🔥 MARK VIDEO AS WATCHED
        if (currentIndex >= 0 &&
            currentIndex < moduleIDs.Length)
        {
            int watchedModuleID =
                moduleIDs[currentIndex];

            string watchKey =
                "VIDEO_WATCHED_" + watchedModuleID;

            PlayerPrefs.SetInt(watchKey, 1);
            PlayerPrefs.Save();
        }

        // 🔊 RESTORE AUDIO
        if (AudioManager.Instance != null)
            AudioManager.Instance.RestoreAll();

        if (HeartSystem.Instance.currentHearts <= 0)
        {
            ShowNoLivesPopup();
        }
        else
        {

            StartCoroutine(ShowBeginPopupAndLoad());
        }
    }

    void ShowNoLivesPopup()
    {
        noLivesPanel.SetActive(true);

        if (popupAnim != null) StopCoroutine(popupAnim);
        popupAnim = StartCoroutine(ScalePopup(Vector3.zero, Vector3.one));

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

            if (popupAnim != null) StopCoroutine(popupAnim);
            popupAnim = StartCoroutine(ClosePopupAnim());

            
            StartCoroutine(LoadSceneAfterOrientation());
        }
    }

    public void CloseNoLivesPopup()
    {
        if (popupAnim != null) StopCoroutine(popupAnim);
        popupAnim = StartCoroutine(ClosePopupAnim());

        if (AudioManager.Instance != null)
            AudioManager.Instance.RestoreAll();

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

    public void BackToMenu()
    {
        // ❌ tanggalin listener para di mag auto next scene
        videoPlayer.loopPointReached -= OnVideoFinished;

        // ⏹ stop video
        videoPlayer.Stop();

        // 🔊 restore audio
        if (AudioManager.Instance != null)
            AudioManager.Instance.RestoreAll();

        // 🔄 portrait mode
        orientationManager.SetPortrait();

        // 🧹 reset current index
        currentIndex = -1;

        // 📺 balik UI
        videoPanel.SetActive(false);
        mainMenu?.SetActive(true);
        gamePanel.SetActive(true);
    }


    IEnumerator ScalePopup(Vector3 from, Vector3 to)
    {
        float time = 0f;

        while (time < popupDuration)
        {
            float t = time / popupDuration;
            float curveValue = popupCurve.Evaluate(t);

            noLivesPanel.transform.localScale = Vector3.LerpUnclamped(from, to, curveValue);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        noLivesPanel.transform.localScale = to;
    }

    IEnumerator ClosePopupAnim()
    {
        yield return ScalePopup(Vector3.one, Vector3.zero);
        noLivesPanel.SetActive(false);
    }

    IEnumerator ScaleSpecificPopup(GameObject targetPopup, Vector3 from, Vector3 to)
    {
        float time = 0f;

        RectTransform popupRect =
            targetPopup.GetComponent<RectTransform>();

        while (time < popupDuration)
        {
            float t = time / popupDuration;

            float curveValue = popupCurve.Evaluate(t);

            popupRect.localScale =
                Vector3.LerpUnclamped(from, to, curveValue);

            time += Time.unscaledDeltaTime;

            yield return null;
        }

        popupRect.localScale = to;
    }

    IEnumerator ShowBeginPopupAndLoad()
    {
        // 🔥 SHOW PANEL
        popupBeginPanel.SetActive(true);

        // 🔥 PLAY POPUP ANIMATION
        yield return StartCoroutine(
            ScaleSpecificPopup(
                popupBeginPanel,
                Vector3.zero,
                Vector3.one
            )
        );

        // 🔥 WAIT
        yield return new WaitForSeconds(beginPopupWaitTime);

        // 🔥 LOAD NEXT SCENE
        StartCoroutine(LoadSceneAfterOrientation());
    }

}
