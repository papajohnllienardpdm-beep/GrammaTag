using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoControls : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider slider;

    void Update()
    {
        if (videoPlayer.isPlaying && videoPlayer.length > 0)
        {
            slider.value = (float)(videoPlayer.time / videoPlayer.length);
        }
    }

    public void PlayPause()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
        else
            videoPlayer.Play();
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
