using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType { Master, Music, SFX }

    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI percent;
    [SerializeField] private VolumeType volumeType;

    private void Start()
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                slider.value = slider.maxValue * SettingsManager.instance.masterVolume;
                break;
            case VolumeType.Music:
                slider.value = slider.maxValue * SettingsManager.instance.musicVolume;
                break;
            case VolumeType.SFX:
                slider.value = slider.maxValue * SettingsManager.instance.sFXVolume;
                break;
        }

        OnVolumeChange(true);
    }

    public void OnVolumeChange(bool start)
    {
        if (!start) 
            AudioManager.instance.PlaySFX("Change Volume");
        float percentage = slider.value / slider.maxValue;

        SettingsManager.instance.ChangeVolume(percentage, volumeType);
        percent.text = (percentage * 100) + "%";
    }

    public void ChangeValue(bool increase)
    {
        if (increase)
            slider.value++;
        else 
            slider.value--;
    }
}
