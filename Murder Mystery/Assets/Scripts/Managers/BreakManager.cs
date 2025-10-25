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
    [SerializeField] private UIButton mainMenuButton;
    [SerializeField] private TextMeshProUGUI chapterDisplay;
    [SerializeField] private List<int> caseChapters;

    private void Start()
    {
        chapterDisplay.text = "Chapter " + (ChapterManager.instance.currentChapter - 1) + " Complete";
        if (HasNextChapter())
        {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            continueButton.GetComponent<Button>().interactable = true;
            continueButton.GetComponent<UIButton>().SetOutlineInteractable();
            ControlManager.instance.SetSelectedButton(continueButton.GetComponent<UIButton>());
        }
        else
        {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue (WIP)";
            continueButton.GetComponent<Button>().interactable = false;
            continueButton.GetComponent<UIButton>().SetOutlineInteractable();
            ControlManager.instance.SetSelectedButton(mainMenuButton);
        }

        Invoke(nameof(ShowButtons), buttonDelay);
    }

    private bool HasNextChapter()
    {
        return caseChapters[ChapterManager.instance.currentCase - 1] >= ChapterManager.instance.currentChapter;
    }

    private void ShowButtons()
    {
        animator.SetTrigger("Show");
    }

    public void Continue()
    {
        TransitionManager.instance.EnterScene(ChapterManager.instance.currentCase);
    }

    public void Menu()
    {
        ChapterManager.instance.currentCase = -1;
        ChapterManager.instance.currentChapter = -1;
        ChapterManager.instance.currentPart = -1;
        TransitionManager.instance.EnterScene(0);
    }
}
