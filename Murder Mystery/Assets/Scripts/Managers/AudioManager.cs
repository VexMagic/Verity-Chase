using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sFXSource;
    [SerializeField] private AudioSource blipSource;

    [SerializeField] private float pitchShiftAmount;

    [SerializeField] private List<SoundEffect> effects = new List<SoundEffect>();

    private Dictionary<string, SoundEffect> effectsMap = new Dictionary<string, SoundEffect>();

    public BlipSound currentBlipSound;

    private float masterVolume;
    private float musicVolume;
    private float SFXVolume;

    private void Awake()
    {
        instance = this;
        CreateEffectsMap();
    }

    private void CreateEffectsMap()
    {
        effectsMap.Clear();
        foreach (var effect in effects)
        {
            effectsMap.Add(effect.code, effect);
        }
    }

    public void ChangeVolume(float value, VolumeSlider.VolumeType type)
    {
        switch (type)
        {
            case VolumeSlider.VolumeType.Master:
                masterVolume = value;
                musicSource.volume = masterVolume * musicVolume;
                sFXSource.volume = masterVolume * SFXVolume;
                blipSource.volume = masterVolume * SFXVolume;
                break;
            case VolumeSlider.VolumeType.Music:
                musicVolume = value;
                musicSource.volume = masterVolume * musicVolume;
                break;
            case VolumeSlider.VolumeType.SFX:
                SFXVolume = value;
                sFXSource.volume = masterVolume * SFXVolume;
                blipSource.volume = masterVolume * SFXVolume;
                break;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip)
            return;

        musicSource.clip = clip;
        musicSource.Play();
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
