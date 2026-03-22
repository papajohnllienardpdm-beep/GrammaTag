using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;


public class VideoControlss : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playPauseButton;
    public Slider slider;
    public TextMeshProUGUI timeText;

    private bool isPrepared = false;
    private bool isPlaying = false;

    void Start()
    {
        videoPlayer.prepareCompleted += OnVideoPrepared;

        videoPlayer.Prepare();

        timeText.text = "00:00 / 00:00";
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("VIDEO PREPARED ✅");

        isPrepared = true;

        slider.value = 0;

        timeText.text = "00:00 / " + FormatTime(videoPlayer.length);
    }

    void Update()
    {
        if (!isPrepared) return;

        if (videoPlayer.length > 0)
        {
            slider.value = (float)(videoPlayer.time / videoPlayer.length);

            timeText.text = FormatTime(videoPlayer.time) + " / " + FormatTime(videoPlayer.length);
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
    }

    string FormatTime(double time)
    {
        int min = Mathf.FloorToInt((float)time / 60);
        int sec = Mathf.FloorToInt((float)time % 60);
        return min.ToString("00") + ":" + sec.ToString("00");
    }

    public void OnSliderChanged()
    {
        if (!isPrepared) return;

        videoPlayer.time = slider.value * videoPlayer.length;
    }
}
