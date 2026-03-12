using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoFadeIn : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 1f;
   
    void Update()
    {
        if (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
        }
    }
}
