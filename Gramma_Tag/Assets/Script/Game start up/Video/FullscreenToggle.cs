using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    public RectTransform panel;
    public GameObject navigationBar; // 👈 ADD THIS

    private bool isFullscreen = false;

    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalOffsetMin;
    private Vector2 originalOffsetMax;

    void Start()
    {
        originalAnchorMin = panel.anchorMin;
        originalAnchorMax = panel.anchorMax;
        originalOffsetMin = panel.offsetMin;
        originalOffsetMax = panel.offsetMax;
    }

    public void Toggle()
    {
        if (!isFullscreen)
        {
            // 👉 FULLSCREEN
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            panel.anchorMin = Vector2.zero;
            panel.anchorMax = Vector2.one;
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;

            navigationBar.SetActive(false); // 🔥 HIDE
        }
        else
        {
            // 👉 BACK TO NORMAL
            Screen.orientation = ScreenOrientation.Portrait;

            panel.anchorMin = originalAnchorMin;
            panel.anchorMax = originalAnchorMax;
            panel.offsetMin = originalOffsetMin;
            panel.offsetMax = originalOffsetMax;

            navigationBar.SetActive(true); // 🔥 SHOW
        }

        isFullscreen = !isFullscreen;
    }
}
