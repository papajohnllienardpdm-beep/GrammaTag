using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioUIConnector : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        StartCoroutine(ConnectDelayed());
    }

    IEnumerator ConnectDelayed()
    {
        // hintayin AudioManager
        yield return new WaitUntil(() => AudioManager.Instance != null);

        // 🔥 kahit inactive panel, makukuha pa rin reference
        AudioManager.Instance.RegisterSliders(musicSlider, sfxSlider);

        musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume());
    }
}
