using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoControls : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider slider;

    [Header("Time UI")]
    public TextMeshProUGUI timeText;

    [Header("Play Button")]
    public Image playButtonImage;   // 👈 button icon
    public Sprite playIcon;         // ▶️
    public Sprite pauseIcon;

    void Start()
    {
        UpdateIcon();
    }

    void Update()
    {
        if (videoPlayer.isPlaying && videoPlayer.length > 0)
        {
            float currentTime = (float)videoPlayer.time;
            float totalTime = (float)videoPlayer.length;

            // update slider
            slider.value = currentTime / totalTime;

            // update text
            timeText.text = FormatTime(currentTime) + " / " + FormatTime(totalTime);
        }
    }

    // 👉 FORMAT TIME (important)
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void PlayPause()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
        else
            videoPlayer.Play();

        UpdateIcon();
    }

    void UpdateIcon()
    {
        if (videoPlayer.isPlaying)
            playButtonImage.sprite = pauseIcon;
        else
            playButtonImage.sprite = playIcon;
    }

    public void Seek()
    {
        if (videoPlayer.length > 0)
        {
            videoPlayer.time = slider.value * videoPlayer.length;
        }
    }

    public void OnSliderDrag()
    {
        videoPlayer.Pause();
    }

    public void OnSliderRelease()
    {
        Seek();
        videoPlayer.Play();
    }
}
