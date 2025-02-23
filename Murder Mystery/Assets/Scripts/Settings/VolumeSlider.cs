using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType { Master, Music, SFX }

    [SerializeField] private Slider slider;
    [SerializeField] private VolumeType volumeType;

    private void Start()
    {
        OnVolumeChange();
    }

    public void OnVolumeChange()
    {
        AudioManager.instance.ChangeVolume(slider.value / slider.maxValue, volumeType);
    }
}
