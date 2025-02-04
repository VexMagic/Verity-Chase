using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakManager : MonoBehaviour
{
    [SerializeField] private float buttonDelay;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject wipButton;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private List<int> caseChapters;

    private void Start()
    {
        if (HasNextChapter())
        {
            continueButton.SetActive(true);
            continueButton.SetActive(false);
        }
        else
        {
            continueButton.SetActive(false);
            continueButton.SetActive(true);
        }

        Invoke(nameof(ShowButtons), buttonDelay);
    }

    private bool HasNextChapter()
    {
        return caseChapters[ChapterManager.instance.caseNumber] < ChapterManager.instance.currentChapter + 1;
    }

    private void ShowButtons()
    {
        animator.SetTrigger("Show");
    }

    public void Continue()
    {
        SceneManager.LoadScene(ChapterManager.instance.caseNumber + 1);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}
