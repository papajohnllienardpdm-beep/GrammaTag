using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    public RectTransform panel;
    public RectTransform videoPlayerUI;

    public GameObject navigationBar;
    public GameObject upperNavigationBarPanel;

    [Header("Video Size (Landscape)")]
    public Vector2 videoAnchorMin;
    public Vector2 videoAnchorMax;
    public Vector2 videoOffsetMin;
    public Vector2 videoOffsetMax;

    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalOffsetMin;
    private Vector2 originalOffsetMax;

    private Vector2 videoOriginalAnchorMin;
    private Vector2 videoOriginalAnchorMax;
    private Vector2 videoOriginalOffsetMin;
    private Vector2 videoOriginalOffsetMax;

    void Start()
    {
        originalAnchorMin = panel.anchorMin;
        originalAnchorMax = panel.anchorMax;
        originalOffsetMin = panel.offsetMin;
        originalOffsetMax = panel.offsetMax;

        videoOriginalAnchorMin = videoPlayerUI.anchorMin;
        videoOriginalAnchorMax = videoPlayerUI.anchorMax;
        videoOriginalOffsetMin = videoPlayerUI.offsetMin;
        videoOriginalOffsetMax = videoPlayerUI.offsetMax;
    }

    public void EnterFullscreen()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        panel.anchorMin = Vector2.zero;
        panel.anchorMax = Vector2.one;
        panel.offsetMin = Vector2.zero;
        panel.offsetMax = Vector2.zero;

        videoPlayerUI.anchorMin = videoAnchorMin;
        videoPlayerUI.anchorMax = videoAnchorMax;
        videoPlayerUI.offsetMin = videoOffsetMin;
        videoPlayerUI.offsetMax = videoOffsetMax;

        navigationBar.SetActive(false);
        upperNavigationBarPanel.SetActive(false);
    }

    public void ExitFullscreen()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        panel.anchorMin = originalAnchorMin;
        panel.anchorMax = originalAnchorMax;
        panel.offsetMin = originalOffsetMin;
        panel.offsetMax = originalOffsetMax;

        videoPlayerUI.anchorMin = videoOriginalAnchorMin;
        videoPlayerUI.anchorMax = videoOriginalAnchorMax;
        videoPlayerUI.offsetMin = videoOriginalOffsetMin;
        videoPlayerUI.offsetMax = videoOriginalOffsetMax;

        navigationBar.SetActive(true);
        upperNavigationBarPanel.SetActive(true);
    }
}
