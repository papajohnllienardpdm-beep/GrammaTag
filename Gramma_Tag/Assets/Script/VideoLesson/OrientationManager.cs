using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    // 👉 landscape mode
    public void SetLandscape()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    // 👉 portrait mode
    public void SetPortrait()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
}
