using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource otherMusicSource;
    [SerializeField] private AudioSource sFXSource;
    [SerializeField] private AudioSource blipSource;

    [SerializeField] private float pitchShiftAmount;
    [SerializeField] private float musicTransitionSpeed;

    [SerializeField] private List<SoundEffect> effects = new List<SoundEffect>();

    private Dictionary<string, SoundEffect> effectsMap = new Dictionary<string, SoundEffect>();

    public BlipSound currentBlipSound;

    public bool usingFirstMusic;
    private List<AudioClip> musicQueue = new List<AudioClip>();
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        instance = this;
        CreateEffectsMap();
        UpdateVolume();
    }

    private void CreateEffectsMap()
    {
        effectsMap.Clear();
        foreach (var effect in effects)
        {
            effectsMap.Add(effect.code, effect);
        }
    }

    public void UpdateVolume()
    {
        musicSource.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.musicVolume;
        otherMusicSource.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.musicVolume;
        sFXSource.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.sFXVolume;
        blipSource.volume = SettingsManager.instance.masterVolume * SettingsManager.instance.sFXVolume;

        //GetMusicSource(usingFirstMusic).Play();
        GetMusicSource(!usingFirstMusic).volume = 0;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (GetMusicSource(usingFirstMusic).clip == clip)
            return;

        musicQueue.Add(clip);
        if (transitionCoroutine == null)
        {
            transitionCoroutine = StartCoroutine(MusicTransition());
        }

        musicSource.clip = clip;
    }

    private IEnumerator MusicTransition()
    {
        while (musicQueue.Count > 0)
        {
            usingFirstMusic = !usingFirstMusic;
            GetMusicSource(usingFirstMusic).clip = musicQueue[0];
            GetMusicSource(usingFirstMusic).Play();

            float time = 0;
            while (musicTransitionSpeed > time)
            {
                time += Time.fixedDeltaTime;

                GetMusicSource(usingFirstMusic).volume = Mathf.Lerp(0, SettingsManager.instance.masterVolume * SettingsManager.instance.musicVolume, time / musicTransitionSpeed);
                GetMusicSource(!usingFirstMusic).volume = Mathf.Lerp(SettingsManager.instance.masterVolume * SettingsManager.instance.musicVolume, 0, time / musicTransitionSpeed);

                yield return new WaitForFixedUpdate();
            }

            musicQueue.RemoveAt(0);
        }
        transitionCoroutine = null;
    }

    private AudioSource GetMusicSource(bool isFirst)
    {
        if (isFirst)
            return musicSource;
        else
            return otherMusicSource;
    }

    public void PlaySFX(string code)
    {
        if (effectsMap[code].pitchShift)
        {
            float pitchShift = UnityEngine.Random.Range(-pitchShiftAmount, pitchShiftAmount);
            blipSource.pitch = 1 + pitchShift;
            blipSource.PlayOneShot(effectsMap[code].sound);
        }
        else
        {
            sFXSource.PlayOneShot(effectsMap[code].sound);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sFXSource.PlayOneShot(clip);
    }

    public void PlayBlip()
    {
        switch (currentBlipSound)
        {
            default:
                PlaySFX("Typewriter Blip");
                break;
            case BlipSound.Male:
                PlaySFX("Male Blip");
                break;
            case BlipSound.Female:
                PlaySFX("Female Blip");
                break;
        }
    }


    [Serializable]
    public class SoundEffect
    {
        [SerializeField] private string Code;
        [SerializeField] private AudioClip Sound;
        [SerializeField] private bool PitchShift;
        public string code => Code;
        public AudioClip sound => Sound;
        public bool pitchShift => PitchShift;
    }
}

public enum BlipSound { Typewriter, Male, Female }
