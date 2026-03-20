using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public GameObject videoPanel;
    public GameObject gamePanel;

    public VideoPlayer videoPlayer;

    public VideoClip[] videos;

    public OrientationManager orientationManager; // 👈 add

    public void PlayVideo(int index)
    {
        if (index < 0 || index >= videos.Length) return;

        // 👉 rotate to landscape
        orientationManager.SetLandscape();

        gamePanel.SetActive(false);
        videoPanel.SetActive(true);

        videoPlayer.clip = videos[index];
        videoPlayer.Play();
    }

    public void CloseVideo()
    {
        videoPlayer.Stop();

        // 👉 balik portrait
        orientationManager.SetPortrait();

        videoPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
}
