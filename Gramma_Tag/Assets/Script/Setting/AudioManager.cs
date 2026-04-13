using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    void Awake()
    {
        // Singleton para walang duplicate
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Load saved values
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Apply volume
        musicSource.volume = musicVol;
        sfxSource.volume = sfxVol;

        // Apply sa sliders (IMPORTANT)
        if (musicSlider != null)
        {
            musicSlider.value = musicVol;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVol;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Ensure music is playing
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    // 🎵 MUSIC
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;

        // 🔥 FIX: pag galing 0 → kailangan siguraduhin nagpe-play ulit
        if (volume > 0 && !musicSource.isPlaying)
        {
            musicSource.Play();
        }

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // 🔊 SFX
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}
