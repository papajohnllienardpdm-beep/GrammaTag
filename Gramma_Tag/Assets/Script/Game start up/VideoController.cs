using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider timeline;
    public TextMeshProUGUI timeText;

    void Update()
    {
        if (videoPlayer.isPlaying)
        {
            timeline.value = (float)(videoPlayer.time / videoPlayer.length);

            int minutes = Mathf.FloorToInt((float)videoPlayer.time / 60);
            int seconds = Mathf.FloorToInt((float)videoPlayer.time % 60);

            int totalMin = Mathf.FloorToInt((float)videoPlayer.length / 60);
            int totalSec = Mathf.FloorToInt((float)videoPlayer.length % 60);

            timeText.text =
                minutes.ToString("00") + ":" + seconds.ToString("00")
                + " / " +
                totalMin.ToString("00") + ":" + totalSec.ToString("00");
        }
    }

    public void PlayPause()
    {
        Debug.Log("BUTTON CLICKED");

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("Paused");
        }
        else
        {
            videoPlayer.Play();
            Debug.Log("Playing");
        }
    }

    public void Seek()
    {
        videoPlayer.time = timeline.value * videoPlayer.length;
    }
}
