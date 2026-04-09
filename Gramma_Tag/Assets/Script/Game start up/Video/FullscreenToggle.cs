using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FullscreenToggle : MonoBehaviour
{
    public RectTransform panel; // VideoConPanel
    public RectTransform videoPlayerUI;

    public GameObject navigationBar;
    public GameObject upperNavigationBarPanel;

    [Header("Assign THIS → Canvas")]
    public Transform fullscreenParent;

    [Header("Video Size (Landscape)")]
    public Vector2 videoAnchorMin;
    public Vector2 videoAnchorMax;
    public Vector2 videoOffsetMin;
    public Vector2 videoOffsetMax;

    private Transform originalParent;
    private int originalSiblingIndex; // 🔥 IMPORTANT

    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalOffsetMin;
    private Vector2 originalOffsetMax;

    private Vector2 videoOriginalAnchorMin;
    private Vector2 videoOriginalAnchorMax;
    private Vector2 videoOriginalOffsetMin;
    private Vector2 videoOriginalOffsetMax;

    private LayoutElement layoutElement;

    void Start()
    {
        // SAVE ORIGINAL PARENT + POSITION
        originalParent = panel.parent;
        originalSiblingIndex = panel.GetSiblingIndex(); // 🔥 SAVE ORDER

        // SAVE PANEL VALUES
        originalAnchorMin = panel.anchorMin;
        originalAnchorMax = panel.anchorMax;
        originalOffsetMin = panel.offsetMin;
        originalOffsetMax = panel.offsetMax;

        // SAVE VIDEO UI VALUES
        videoOriginalAnchorMin = videoPlayerUI.anchorMin;
        videoOriginalAnchorMax = videoPlayerUI.anchorMax;
        videoOriginalOffsetMin = videoPlayerUI.offsetMin;
        videoOriginalOffsetMax = videoPlayerUI.offsetMax;

        layoutElement = panel.GetComponent<LayoutElement>();
    }

    public void EnterFullscreen()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // 🔥 MOVE TO CANVAS
        panel.SetParent(fullscreenParent);
        panel.SetAsLastSibling(); // OK dito (overlay)

        // 🔥 DISABLE LAYOUT
        if (layoutElement != null)
            layoutElement.ignoreLayout = true;

        // FULLSCREEN SIZE
        panel.anchorMin = Vector2.zero;
        panel.anchorMax = Vector2.one;
        panel.offsetMin = Vector2.zero;
        panel.offsetMax = Vector2.zero;

        // VIDEO SIZE
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

        // 🔥 BALIK SA SCROLL VIEW
        panel.SetParent(originalParent);

        // 🔥 IBALIK SA ORIGINAL POSITION
        panel.SetSiblingIndex(originalSiblingIndex);

        // 🔥 ENABLE LAYOUT
        if (layoutElement != null)
            layoutElement.ignoreLayout = false;

        // RESTORE SIZE
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
