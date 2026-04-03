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

    private bool isPrepared = false;
    private bool isPlaying = false;
    private bool isDragging = false; // 🔥 IMPORTANT
    private float updateTimer = 0f;

    void Start()
    {
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();

        timeText.text = "00:00 / 00:00";
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        isPrepared = true;

        slider.value = 0;
        timeText.text = "00:00 / " + FormatTime(videoPlayer.length);

        isPlaying = false;

        UpdatePlayPauseIcon(); // para default naka ▶️
    }

    void Update()
    {
        if (!isPrepared) return;

        // 🔥 UPDATE UI ONLY IF NOT DRAGGING
        if (isPlaying && !isDragging)
        {
            updateTimer += Time.deltaTime;

            if (updateTimer >= 0.2f)
            {
                updateTimer = 0f;

                if (videoPlayer.length > 0)
                {
                    slider.value = (float)(videoPlayer.time / videoPlayer.length);

                    timeText.text = FormatTime(videoPlayer.time) + " / " + FormatTime(videoPlayer.length);
                }
            }
        }
    }

    public void TogglePlayPause()
    {
        if (!isPrepared) return;

        if (!isPlaying)
        {
            videoPlayer.Play();
            isPlaying = true;
        }
        else
        {
            videoPlayer.Pause();
            isPlaying = false;
        }

        UpdatePlayPauseIcon(); // 🔥 ADD THIS
    }

    void UpdatePlayPauseIcon()
    {
        if (isPlaying)
        {
            playPauseIcon.sprite = pauseSprite; // ⏸️
        }
        else
        {
            playPauseIcon.sprite = playSprite; // ▶️
        }
    }


    // 👉 habang dinadrag
    public void OnSliderChanged()
    {
        if (!isPrepared) return;

        if (isDragging)
        {
            videoPlayer.time = slider.value * videoPlayer.length;
        }
    }

    // 👉 pag pinindot slider
    public void OnSliderDown()
    {
        if (!isPrepared) return;

        isDragging = true;
        videoPlayer.Pause();
        isPlaying = false;

        UpdatePlayPauseIcon();
    }

    // 👉 pag binitawan slider
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
}
