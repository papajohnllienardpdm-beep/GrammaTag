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

    void Start()
    {
        // 👉 GET SELECTED GENDER
        string gender = PlayerPrefs.GetString("SelectedGender", "Girl");

        // 👉 SELECT VIDEO BASED ON GENDER
        if (gender == "Boy")
        {
            videoPlayer.clip = boyVideo;
            Debug.Log("Playing Boy Cutscene");
        }
        else
        {
            videoPlayer.clip = girlVideo;
            Debug.Log("Playing Girl Cutscene");
        }

        // 👉 PLAY VIDEO
        videoPlayer.Play();

        // 👉 WHEN FINISHED
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextScene);
    }
}
