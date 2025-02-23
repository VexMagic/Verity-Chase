using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public void Click()
    {
        AudioManager.instance.PlaySFX("Click");
    }
}
