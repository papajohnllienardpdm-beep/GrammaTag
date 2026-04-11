using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoCompletionUnlocker : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public ModuleLocker module1Locker;

    private bool unlocked = false;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (unlocked) return;

        Debug.Log("VIDEO FINISHED → Unlock Module 1");

        PlayerPrefsManager.Instance.UnlockModule(1);

        module1Locker.UpdateLockState();

        unlocked = true;
    }
}
