using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [Header("Videos")]
    public VideoClip girlVideo;
    public VideoClip boyVideo;

    public string nextScene = "MainMenu";

    IEnumerator Start()
    {
        // 👉 FORCE LANDSCAPE
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // 👉 WAIT DATABASE
        yield return new WaitUntil(() =>
            DatabaseManager.Instance != null &&
            DatabaseManager.Instance.IsDatabaseReady()
        );

        // 👉 GET GENDER FROM SQLITE
        string gender = DatabaseManager.Instance.GetUserGender();

        Debug.Log("🎮 PLAYER GENDER: " + gender);

        // 👉 SELECT VIDEO
        if (gender == "Boy")
        {
            videoPlayer.clip = boyVideo;
            Debug.Log("▶️ Playing Boy Cutscene");
        }
        else
        {
            videoPlayer.clip = girlVideo;
            Debug.Log("▶️ Playing Girl Cutscene");
        }

        // 👉 PLAY VIDEO
        videoPlayer.Play();

        // 👉 VIDEO END EVENT
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextScene);
    }
}