using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BreakManager : MonoBehaviour
{
    [SerializeField] private float buttonDelay;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private TextMeshProUGUI chapterDisplay;
    [SerializeField] private List<int> caseChapters;

    private void Start()
    {
        chapterDisplay.text = "Chapter " + ChapterManager.instance.currentChapter + " Complete";
        ControlManager.instance.SetSelectedButton(continueButton.GetComponent<UIButton>());
        if (HasNextChapter())
        {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            continueButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue (WIP)";
            continueButton.GetComponent<Button>().interactable = false;
        }

        Invoke(nameof(ShowButtons), buttonDelay);
    }

    private bool HasNextChapter()
    {
        return caseChapters[ChapterManager.instance.currentCase] >= ChapterManager.instance.currentChapter + 1;
    }

    private void ShowButtons()
    {
        animator.SetTrigger("Show");
    }

    public void Continue()
    {
        SceneManager.LoadScene(ChapterManager.instance.currentCase + 1);
    }

    public void Menu()
    {
        ChapterManager.instance.currentCase = -1;
        ChapterManager.instance.currentChapter = -1;
        ChapterManager.instance.currentPart = -1;
        SceneManager.LoadScene(0);
    }
}
