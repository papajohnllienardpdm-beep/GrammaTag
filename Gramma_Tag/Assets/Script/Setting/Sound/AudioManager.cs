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

    private float musicVol;
    private float sfxVol;

    private bool isMusicMuted = false;

    private float prevMusicVolume;
    private float prevSFXVolume;

    // 🔥 MULTIPLE SLIDERS SUPPORT
    private List<Slider> musicSliders = new List<Slider>();
    private List<Slider> sfxSliders = new List<Slider>();

    void Awake()
    {
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

        musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSource.volume = musicVol;
        sfxSource.volume = sfxVol;
    }

    void Start()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    // 🔥 REGISTER SLIDERS (ITO ANG MAGIC)
    public void RegisterSliders(Slider music, Slider sfx)
    {
        if (music != null)
        {
            if (!musicSliders.Contains(music))
            {
                musicSliders.Add(music);

                music.SetValueWithoutNotify(musicVol);
                music.onValueChanged.AddListener(SetMusicVolume);
            }
        }

        if (sfx != null)
        {
            if (!sfxSliders.Contains(sfx))
            {
                sfxSliders.Add(sfx);

                sfx.SetValueWithoutNotify(sfxVol);
                sfx.onValueChanged.AddListener(SetSFXVolume);
            }
        }
    }

    // 🎵 MUSIC
    public void SetMusicVolume(float volume)
    {
        musicVol = volume;

        if (volume <= 0.0001f)
        {
            musicSource.volume = 0f;
            isMusicMuted = true;
        }
        else
        {
            musicSource.volume = volume;

            if (isMusicMuted || !musicSource.isPlaying)
            {
                musicSource.Play();
                isMusicMuted = false;
            }
        }

        // 🔥 SYNC ALL SLIDERS
        foreach (var slider in musicSliders)
        {
            if (slider != null)
                slider.SetValueWithoutNotify(volume);
        }

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // 🔊 SFX
    public void SetSFXVolume(float volume)
    {
        sfxVol = volume;
        sfxSource.volume = volume;

        foreach (var slider in sfxSliders)
        {
            if (slider != null)
                slider.SetValueWithoutNotify(volume);
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void MuteAll()
    {
        prevMusicVolume = musicSource.volume;
        prevSFXVolume = sfxSource.volume;

        musicSource.volume = 0f;
        sfxSource.volume = 0f;
    }

    public void RestoreAll()
    {
        musicSource.volume = prevMusicVolume;
        sfxSource.volume = prevSFXVolume;
    }

    public float GetMusicVolume()
    {
        return musicVol;
    }

    public float GetSFXVolume()
    {
        return sfxVol;
    }

}
