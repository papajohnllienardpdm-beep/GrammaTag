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

    public TextMeshProUGUI timeText;

    public Image playButtonImage;
    public Sprite playIcon;
    public Sprite pauseIcon;

    private float updateTimer = 0f;
    private bool isDragging = false;

    private bool canSkip = true;

    void Start()
    {
        UpdateIcon();
    }

    void Update()
    {
        if (!videoPlayer.isPlaying || videoPlayer.length <= 0 || isDragging) return;

        updateTimer += Time.deltaTime;

        if (updateTimer >= 0.2f) // 🔥 LIMIT UPDATE (ANTI-LAG)
        {
            updateTimer = 0f;

            float currentTime = (float)videoPlayer.time;
            float totalTime = (float)videoPlayer.length;

            slider.value = currentTime / totalTime;
            timeText.text = FormatTime(currentTime) + " / " + FormatTime(totalTime);
        }
    }

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

    public void OnSliderDown()
    {
        // ❌ FIRST WATCH = NO SKIP
        if (!canSkip)
            return;

        isDragging = true;

        videoPlayer.Pause();
    }

    public void Seek()
    {
        // ❌ FIRST WATCH = NO SKIP
        if (!canSkip)
            return;

        if (!isDragging || videoPlayer.length <= 0)
            return;

        videoPlayer.time =
            slider.value * videoPlayer.length;
    }

    public void OnSliderRelease()
    {
        // ❌ FIRST WATCH = NO SKIP
        if (!canSkip)
            return;

        isDragging = false;

        videoPlayer.time =
            slider.value * videoPlayer.length;

        videoPlayer.Play();

        UpdateIcon();
    }

    public void SetCanSkip(bool value)
    {
        canSkip = value;

        // 🔥 ENABLE / DISABLE SLIDER
        if (slider != null)
        {
            slider.interactable = value;
        }
    }
}
