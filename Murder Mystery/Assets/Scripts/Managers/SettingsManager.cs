using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    [SerializeField] private List<TextSpeed> textSpeeds;
    [SerializeField] private List<TextWindoTransparency> textTransparency;

    private int selectedTextSpeed = 1;
    private int selectedTextTransparency = 1;

    private float MasterVolume = 0.5f;
    private float MusicVolume = 0.5f;
    private float SFXVolume = 0.5f;

    public float masterVolume => MasterVolume;
    public float musicVolume => MusicVolume;
    public float sFXVolume => SFXVolume;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeTextSpeed(bool increase)
    {
        if (increase)
        {
            selectedTextSpeed++;
            if (selectedTextSpeed >= textSpeeds.Count)
                selectedTextSpeed = 0;
        }
        else
        {
            selectedTextSpeed--;
            if (selectedTextSpeed < 0)
                selectedTextSpeed = textSpeeds.Count - 1;
        }
        //selectedTextSpeed = Mathf.Clamp(selectedTextSpeed, 0, TextSpeeds.Count - 1);
    }

    public void ChangeTextTransparency(bool increase)
    {
        if (increase)
        {
            selectedTextTransparency++;
            if (selectedTextTransparency >= textTransparency.Count)
                selectedTextTransparency = 0;
        }
        else
        {
            selectedTextTransparency--;
            if (selectedTextTransparency < 0)
                selectedTextTransparency = textTransparency.Count - 1;
        }
        //selectedTextSpeed = Mathf.Clamp(selectedTextSpeed, 0, TextSpeeds.Count - 1);
    }

    public void ChangeVolume(float value, VolumeSlider.VolumeType type)
    {
        switch (type)
        {
            case VolumeSlider.VolumeType.Master:
                MasterVolume = value;
                break;
            case VolumeSlider.VolumeType.Music:
                MusicVolume = value;
                break;
            case VolumeSlider.VolumeType.SFX:
                SFXVolume = value;
                break;
        }
        AudioManager.instance.UpdateVolume();
    }

    public TextSpeed GetTextSpeeds()
    {
        return textSpeeds[selectedTextSpeed];
    }

    public TextWindoTransparency GetTextTransparency()
    {
        return textTransparency[selectedTextTransparency];
    }
}

[Serializable]
public class TextSpeed
{
    [SerializeField] private float TypewriterSpeed;
    [SerializeField] private float PunctuationDelay;
    [SerializeField] private string DisplayName;

    public float typewriterSpeed => TypewriterSpeed;
    public float punctuationDelay => PunctuationDelay;
    public string displayName => DisplayName;
}


[Serializable]
public class TextWindoTransparency
{
    [SerializeField] private Color Transparency;
    [SerializeField] private string DisplayName;

    public Color transparency => Transparency;
    public string displayName => DisplayName;
}
