using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoCompletionUnlocker : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [Header("Unlock Target")]
    public int moduleToUnlock; // 🔥 inspector controlled
    public ModuleLocker targetLocker;

    private bool unlocked = false;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (unlocked) return;

        Debug.Log("VIDEO FINISHED → Unlock Module " + moduleToUnlock);

        // 🔓 unlock gamit index
        PlayerPrefsManager.Instance.UnlockModule(moduleToUnlock);

        // 🔄 update UI
        if (targetLocker != null)
        {
            targetLocker.UpdateLockState();
        }

        unlocked = true;
    }
}
