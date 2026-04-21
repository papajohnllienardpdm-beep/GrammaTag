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

    private float musicVol;
    private float sfxVol;

    private bool isMusicMuted = false;

    private float prevMusicVolume;
    private float prevSFXVolume;


    void Awake()
    {
        // Singleton
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

        // Load saved values
        musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSource.volume = musicVol;
        sfxSource.volume = sfxVol;
    }

    void Start()
    {
        // Sync sliders WITHOUT triggering event
        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(musicVol);

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(sfxVol);

        // Always start playing music
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    // 🎵 MUSIC CONTROL (FIXED)
    public void SetMusicVolume(float volume)
    {
        musicVol = volume;

        // 🔥 HANDLE ZERO PROPERLY
        if (volume <= 0.0001f)
        {
            musicSource.volume = 0f;
            isMusicMuted = true;
        }
        else
        {
            musicSource.volume = volume;

            // 🔥 FORCE PLAY PAG GALING SA ZERO
            if (isMusicMuted || !musicSource.isPlaying)
            {
                musicSource.Play();
                isMusicMuted = false;
            }
        }

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // 🔊 SFX CONTROL
    public void SetSFXVolume(float volume)
    {
        sfxVol = volume;
        sfxSource.volume = volume;

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    // 🔇 MUTE ALL AUDIO (SAVE PREVIOUS STATE)
    public void MuteAll()
    {
        prevMusicVolume = musicSource.volume;
        prevSFXVolume = sfxSource.volume;

        musicSource.volume = 0f;
        sfxSource.volume = 0f;
    }

    // 🔊 RESTORE AUDIO
    public void RestoreAll()
    {
        musicSource.volume = prevMusicVolume;
        sfxSource.volume = prevSFXVolume;
    }

}
