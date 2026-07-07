using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    // 👉 landscape mode
    public void SetLandscape()
    {
        Debug.Log("Landscape Orientation");
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    // 👉 portrait mode
    public void SetPortrait()
    {
        Debug.Log("Portrait Orientation");
        Screen.orientation = ScreenOrientation.Portrait;
    }
}
