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

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
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
