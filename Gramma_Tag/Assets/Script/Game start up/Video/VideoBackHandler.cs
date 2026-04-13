using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class VideoBackHandler : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public FullscreenToggle fullscreenToggle;
    public ModuleNavigation moduleNavigation;

    public void OnBackPressed()
    {
        // ⏸ Pause video
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }

        // 📱 Exit fullscreen
        if (fullscreenToggle != null)
        {
            fullscreenToggle.ExitFullscreen();
        }

        // 🔙 Back to modules
        if (moduleNavigation != null)
        {
            moduleNavigation.BackToModules();
        }
    }
}
