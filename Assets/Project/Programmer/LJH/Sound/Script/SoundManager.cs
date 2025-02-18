using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : BaseBinder
{

    public static SoundManager Instance;
    public static SoundData Data { get { return Instance._data; } }

     public static AudioMixer Mixer { get { return Instance._audioMixer; } }
    public static AudioSource Master { get { return Instance._master; } }
    public static AudioSource BGM { get { return Instance._bgm; } }
    public static AudioSource SFX { get { return Instance._sfx; } }
    public static AudioSource LoopSFX { get { return Instance._loopSfx; } }


    [SerializeField] private AudioMixer _audioMixer;
    private AudioSource _master;
    private AudioSource _bgm;
    private AudioSource _sfx;
    private AudioSource _loopSfx;
   

    [SerializeField] private SoundData _data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Bind();
        _master = GetObject<AudioSource>("Master");
        _bgm = GetObject<AudioSource>("BGM");
        _sfx = GetObject<AudioSource>("SFX");
        _loopSfx = GetObject<AudioSource>("LoopSFX");

    }

    public static void SetVolumeMaster(float volume)
    {
        if (volume <= 0)
            volume = 0.001f;

        Mixer.SetFloat("Master",  Mathf.Log10(volume) * 20);
    }
    public static void SetVolumeBGM(float volume)
    {
        if (volume <= 0)
            volume = 0.001f;
        Mixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }
    public static void SetVolumeSFX(float volume)
    {
        if (volume <= 0)
            volume = 0.001f;
        Mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
    public static void SetVolumeLoopSFX(float volume)
    {
        if (volume <= 0)
            volume = 0.001f;
        Mixer.SetFloat("LoopSFX", Mathf.Log10(volume) * 20);
    }
    public static float GetVolumeMaster()
    {
        Mixer.GetFloat("Master", out float volume);
        return Mathf.Pow(10, volume / 20);
    }
    public static float GetVolumeBGM()
    {
        Mixer.GetFloat("BGM", out float volume);
        return Mathf.Pow(10, volume/20);
    }
    public static float GetVolumeSFX()
    {
        Mixer.GetFloat("SFX", out float volume);
        return Mathf.Pow(10, volume / 20);
    }
    public static float GetVolumeLoopSFX()
    {
        Mixer.GetFloat("LoopSFX", out float volume);
        return Mathf.Pow(10, volume / 20);
    }

    public static void PlayBGM(AudioClip clip)
    {
        if (clip == null)
            return;

        BGM.clip = clip;
        BGM.Play();
    }
    public static void PauseBGM()
    {
        BGM.Pause();
    }
    public static void StopBGM()
    {
        BGM.Stop();
    }

    public static void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        SFX.PlayOneShot(clip);
    }

    public static void PlayLoopSFX(AudioClip clip)
    {
        if(clip == null)
            return;

        LoopSFX.PlayOneShot(clip);
    }
}
