using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoCompletionUnlocker : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public int moduleToUnlock = 1;
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

        PlayerPrefsManager.Instance.UnlockModule(moduleToUnlock);

        StartCoroutine(RefreshUI());

        unlocked = true;
    }

    IEnumerator RefreshUI()
    {
        yield return new WaitForSeconds(0.1f);

        if (targetLocker != null)
        {
            targetLocker.UpdateLockState();
        }
    }
}
