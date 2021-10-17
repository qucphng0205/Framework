using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// Manage sounds in game, and sound on/off state
/// </summary>
public class SoundManager : MonoSingleton<SoundManager>
{
    const string MUSIC_ON = "isMusicOn";
    const string SOUND_ON = "isSoundOn";

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource normalSource;

    [Header("Music")]
    public Sound music;
    bool isMusicOn = true;

    [Header("fxSound")]
    public Sound[] sounds;
    bool isSoundOn = true;
    int current = 0;

    public bool IsSoundOn
    {
        get => isSoundOn;
        private set
        {
            isSoundOn = value;
            PlayerPrefs.SetInt(SOUND_ON, value ? 1 : 0);
            SFXSource.mute = !value;
            normalSource.mute = !value;
        }
    }
    public bool IsMusicOn
    {
        get => isMusicOn;
        private set
        {
            isMusicOn = value;
            PlayerPrefs.SetInt(MUSIC_ON, value ? 1 : 0);
            musicSource.mute = !value;
        }
    }

    #region MUSIC_SOUND_OF_OFF

    public void _SetMusic(bool isOn)
    {
        IsMusicOn = isOn;
    }

    public static void SetMusic(bool isOn)
    {
        Instance?._SetMusic(isOn);
    }

    public void _SetSound(bool isOn)
    {
        IsSoundOn = isOn;
    }

    public static void SetSound(bool isOn)
    {
        Instance?._SetSound(isOn);
    }

    #endregion

    override protected void Init()
    {
        //init on off
        base.Init();
        IsMusicOn = PlayerPrefs.GetInt(MUSIC_ON, 1) == 1;
        IsSoundOn = PlayerPrefs.GetInt(SOUND_ON, 1) == 1;
    }

    void _PlayFx(SoundName name)
    {
        Sound fx = GetSound(name);
        if (fx != null)
            StartCoroutine(CR_PlayFX(fx));
    }

    public static void PlayFx(SoundName name)
    {
        if (Instance != null)
            Instance._PlayFx(name);
    }

    void _PlayMusic()
    {
        musicSource.clip = music.clip;
        musicSource.Play();
    }

    public static void PlayMusic()
    {
        if (Instance != null)
            Instance._PlayMusic();
    }

    Sound GetSound(SoundName clipName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clipName);
        return s;
    }

    IEnumerator CR_PlayFX(Sound sound)
    {
        if (sound.simultaneousSounds >= sound.maxSimultaneousSounds)
        {
            yield break;
        }

        ++sound.simultaneousSounds;
        if (sound.isNormal)
            normalSource.PlayOneShot(sound.clip);
        else
            SFXSource.PlayOneShot(sound.clip);
        float delay = sound.clip.length * 0.7f;

        yield return new WaitForSecondsRealtime(delay);

        --sound.simultaneousSounds;
    }
}
