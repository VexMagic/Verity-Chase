using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CaseSelectorContinue : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject caseChapterButton;
    [SerializeField] private GameObject casePartButton;
    [SerializeField] private GameObject comingSoonText;
    [SerializeField] private GameObject wIPIndicator;
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private int caseIndex;
    [SerializeField] private bool workInProgress;
    //[SerializeField] private List<string> parts;
    [SerializeField] private List<ChapterData> chapters;

    private bool open = false;
    private List<RectTransform> chapterButtons = new List<RectTransform>();

    private void Start()
    {
        CaseSelectManager.instance.AddCase(this);
        //for (int i = 0; i < parts.Count; i++)
        //{
        //    GameObject gameObject = Instantiate(casePartButton, buttonParent);
        //    gameObject.GetComponent<CasePartButton>().index = i;

        //    TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        //    text.text = parts[i];
        //}

        for (int i = 0; i < chapters.Count; i++)
        {
            GameObject gameObject = Instantiate(caseChapterButton, buttonParent);

            chapterButtons.Add(gameObject.GetComponent<RectTransform>());

            CaseChapterButton tempButton = gameObject.GetComponent<CaseChapterButton>();
            tempButton.index = i;
            tempButton.source = this;
            tempButton.CreateParts(chapters[i].parts);

            TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.text = chapters[i].title;
        }

        comingSoonText.SetActive(chapters.Count == 0);

        if (workInProgress && chapters.Count > 0)
            Instantiate(wIPIndicator, buttonParent);

        UpdateScrollArea();
    }

    public void UpdateScrollArea()
    {
        float buttonAreaHeight = /*(partAmount * 64) + ((partAmount - 1) * 16) + 32*/ 16;

        foreach (var item in chapterButtons)
        {
            buttonAreaHeight += item.sizeDelta.y + 16;
        }

        if (workInProgress && chapters.Count > 0)
            buttonAreaHeight += 112;

        if (buttonAreaHeight < 692)
            buttonAreaHeight = 692;

        buttonParent.sizeDelta = new Vector2(buttonParent.sizeDelta.x, buttonAreaHeight);
    }

    public void Hover(bool hovering)
    {
        animator.SetBool("Hovering", hovering);
    }

    public void Click()
    {
        open = !open;

        bool tempOpen = open;

        CaseSelectManager.instance.CloseCases();
        if (tempOpen)
        {
            ChapterManager.instance.SelectCase(caseIndex);
            open = true;
        }
        animator.SetBool("Open", open);
    }

    public void Close()
    {
        open = false;
        animator.SetBool("Open", false);
    }

    public void OpenSound()
    {
        AudioManager.instance.PlaySFX("Open Case");
    }

    public void HoverSound()
    {
        AudioManager.instance.PlaySFX("Highlight Case");
    }
}
