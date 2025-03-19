using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void PlayClickSFX()
    {
        AudioManager.instance.PlaySFX("Click");
    }

    public void RetutnToTitle()
    {
        TransitionManager.instance.EnterScene(0);
    }

    public void Hover(bool hovering)
    {
        animator.SetBool("Hover", hovering);
    }

    public void Select(bool selecting)
    {
        animator.SetBool("Select", selecting);
    }
}
