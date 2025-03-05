using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButton : MonoBehaviour
{
    public void PlayClickSFX()
    {
        AudioManager.instance.PlaySFX("Click");
    }

    public void RetutnToTitle()
    {
        SceneManager.LoadScene(0);
    }
}
