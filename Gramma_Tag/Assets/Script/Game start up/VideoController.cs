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

    public Image playButtonImage;
    public Sprite playIcon;
    public Sprite pauseIcon;

    private bool isDragging = false;

    void Update()
    {
        // update slider only kapag hindi dini-drag
        if (videoPlayer.isPlaying && !isDragging)
        {
            timeline.value = (float)(videoPlayer.time / videoPlayer.length);
        }

        // update time text
        int minutes = Mathf.FloorToInt((float)videoPlayer.time / 60);
        int seconds = Mathf.FloorToInt((float)videoPlayer.time % 60);

        int totalMin = Mathf.FloorToInt((float)videoPlayer.length / 60);
        int totalSec = Mathf.FloorToInt((float)videoPlayer.length % 60);

        timeText.text =
            minutes.ToString("00") + ":" + seconds.ToString("00")
            + " / " +
            totalMin.ToString("00") + ":" + totalSec.ToString("00");
    }

    public void PlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            playButtonImage.sprite = playIcon;
        }
        else
        {
            videoPlayer.Play();
            playButtonImage.sprite = pauseIcon;
        }
    }

    // habang hinihila slider
    public void OnSliderDrag()
    {
        isDragging = true;
    }

    // kapag binitawan slider
    public void OnSliderRelease()
    {
        isDragging = false;
        videoPlayer.time = timeline.value * videoPlayer.length;
    }
}
