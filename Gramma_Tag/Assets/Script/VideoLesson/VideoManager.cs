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

    public OrientationManager orientationManager;

    public void PlayVideo(int index)
    {
        if (index < 0 || index >= videos.Length) return;

        orientationManager.SetLandscape();

        gamePanel.SetActive(false);
        videoPanel.SetActive(true);

        videoPlayer.clip = videos[index];

        // 🔥 PREPARE FIRST (NO LAG START)
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();
    }

    void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
    }

    public void CloseVideo()
    {
        videoPlayer.Stop();

        orientationManager.SetPortrait();

        videoPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
}
