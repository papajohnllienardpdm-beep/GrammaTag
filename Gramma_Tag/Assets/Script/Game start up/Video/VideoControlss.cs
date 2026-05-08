using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;


public class VideoControlss : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public Button playPauseButton;
    public Image playPauseIcon;
    public Sprite playSprite;
    public Sprite pauseSprite;
    public Slider slider;
    public TextMeshProUGUI timeText;
    public FullscreenToggle fullscreenToggle;

    private bool isPrepared = false;
    private bool isPlaying = false;
    private bool isDragging = false;
    private float updateTimer = 0f;

    // 🔥 INITIAL SETUP
    void Start()
    {
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.prepareCompleted += OnVideoPrepared;

        // 🔥 ADD MO ITO
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Prepare();

        timeText.text = "00:00 / 00:00";
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        isPlaying = false;
        UpdatePlayPauseIcon();

        HandleMusic(false); // 🔥 resume music
    }

    // 🔥 AUTO PLAY PAG BUKAS NG PANEL
    void OnEnable()
    {
        TryAutoPlay();
    }

    void TryAutoPlay()
    {
        // kung ready na agad
        if (isPrepared)
        {
            StartPlayback();
        }
        else
        {
            // hintayin matapos prepare
            videoPlayer.prepareCompleted -= OnPreparedAutoPlay;
            videoPlayer.prepareCompleted += OnPreparedAutoPlay;
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        isPrepared = true;
        slider.value = 0;

        timeText.text = "00:00 / " + FormatTime(videoPlayer.length);

        isPlaying = false;
        UpdatePlayPauseIcon();
    }

    void Update()
    {
        if (!isPrepared) return;

        // 🔥 USE ACTUAL VIDEOPLAYER STATE
        if (videoPlayer.isPlaying && !isDragging)
        {
            updateTimer += Time.deltaTime;

            // 🔥 MAS RESPONSIVE UPDATE
            if (updateTimer >= 0.05f)
            {
                updateTimer = 0f;

                if (videoPlayer.length > 0)
                {
                    // 🔥 UPDATE SLIDER REALTIME
                    slider.value = (float)(videoPlayer.time / videoPlayer.length);

                    // 🔥 UPDATE TIME TEXT
                    timeText.text =
                        FormatTime(videoPlayer.time) +
                        " / " +
                        FormatTime(videoPlayer.length);
                }
            }
        }
    }

    // ▶️ PLAY / PAUSE BUTTON
    public void TogglePlayPause()
    {
        if (!isPrepared) return;

        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            fullscreenToggle.EnterFullscreen();
        }

        if (!isPlaying)
        {
            videoPlayer.Play();
            isPlaying = true;
            HandleMusic(true); // 🔥 pause music
        }
        else
        {
            videoPlayer.Pause();
            isPlaying = false;
            HandleMusic(false); // 🔥 play music
        }

        UpdatePlayPauseIcon();
    }

    void UpdatePlayPauseIcon()
    {
        playPauseIcon.sprite = isPlaying ? pauseSprite : playSprite;
    }

    // 🎚 SLIDER
    public void OnSliderChanged()
    {
        if (!isPrepared) return;

        if (isDragging)
        {
            videoPlayer.time = slider.value * videoPlayer.length;
        }
    }

    public void OnSliderDown()
    {
        if (!isPrepared) return;

        isDragging = true;
        videoPlayer.Pause();
        isPlaying = false;
        UpdatePlayPauseIcon();
    }

    public void OnSliderRelease()
    {
        if (!isPrepared) return;

        isDragging = false;
        videoPlayer.time = slider.value * videoPlayer.length;
        videoPlayer.Play();
        isPlaying = true;
        UpdatePlayPauseIcon();
    }

    string FormatTime(double time)
    {
        int min = Mathf.FloorToInt((float)time / 60);
        int sec = Mathf.FloorToInt((float)time % 60);
        return min.ToString("00") + ":" + sec.ToString("00");
    }

    // 🎥 AUTO PLAY LOGIC
    public void PlayVideoAuto()
    {
        if (!gameObject.activeInHierarchy) return;

        if (!isPrepared)
        {
            videoPlayer.prepareCompleted -= OnPreparedAutoPlay;
            videoPlayer.prepareCompleted += OnPreparedAutoPlay;
            return;
        }

        StartPlayback();
    }

    void OnPreparedAutoPlay(VideoPlayer vp)
    {
        videoPlayer.prepareCompleted -= OnPreparedAutoPlay;
        StartPlayback();
    }

    void StartPlayback()
    {
        fullscreenToggle.EnterFullscreen();

        videoPlayer.Play();
        isPlaying = true;

        HandleMusic(true); // 🔥 pause music

        UpdatePlayPauseIcon();
    }

    void HandleMusic(bool videoIsPlaying)
    {
        if (AudioManager.Instance == null) return;

        if (videoIsPlaying)
        {
            // ⏸ Pause music
            AudioManager.Instance.musicSource.Pause();
        }
        else
        {
            // ▶ Resume music
            if (!AudioManager.Instance.musicSource.isPlaying)
            {
                AudioManager.Instance.musicSource.Play();
            }
        }
    }
}
