using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChapterSelector : MonoBehaviour
{
    public static ChapterSelector instance;

    [SerializeField] private TextMeshProUGUI caseTitle;
    [SerializeField] private TextMeshProUGUI caseNumber;
    [SerializeField] private GameObject caseChapterButton;
    [SerializeField] private GameObject casePartButton;
    [SerializeField] private GameObject wIPIndicator;
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private int caseIndex;
    [SerializeField] private bool workInProgress;
    public UIButton backButton;
    [SerializeField] private List<CaseData> cases;

    private List<RectTransform> chapterButtons = new List<RectTransform>();
    private List<UIButton> UIButtons = new List<UIButton>();
    private GameObject indicator;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnObjects()
    {
        List<ChapterData> chapters = cases[ChapterManager.instance.currentCase].chapters;

        caseTitle.text = cases[ChapterManager.instance.currentCase].title;
        caseNumber.text = "Case " + (ChapterManager.instance.currentCase + 1);

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
            UIButtons.Add(gameObject.GetComponentInChildren<UIButton>());
        }

        if (workInProgress && chapters.Count > 0)
            indicator = Instantiate(wIPIndicator, buttonParent);

        UpdateScrollArea();

        ControlManager.instance.SetSelectedButton(UIButtons[0]);
        SetUIButtonValues();
    }

    private void SetUIButtonValues()
    {
        if (UIButtons.Count == 1)
        {
            chapterButtons[0].GetComponent<CaseChapterButton>().SetUIButtonValues(UIButtons[0], backButton);
            UIButtons[0].SetAdjacent(backButton, backButton, null, null);
            return;
        }

        for (int i = 0; i < UIButtons.Count; i++)
        {
            UIButton tempAbove = null;
            UIButton tempBelow = null;

            if (i == 0)
                tempAbove = backButton;
            else
                tempAbove = UIButtons[i - 1];

            if (i == UIButtons.Count - 1)
                tempBelow = backButton;
            else
                tempBelow = UIButtons[i + 1];

            chapterButtons[i].GetComponent<CaseChapterButton>().SetUIButtonValues(UIButtons[i], tempBelow);
            UIButtons[i].SetAdjacent(tempAbove, tempBelow, null, null);
        }

        backButton.SetAdjacent(UIButtons[^1], UIButtons[0], null, null);
    }

    public void DestroyObjects()
    {
        foreach (var item in chapterButtons)
        {
            Destroy(item.gameObject);
        }
        chapterButtons.Clear();
        UIButtons.Clear();
        Destroy(indicator);
        indicator = null;
    }

    public void UpdateScrollArea()
    {
        float buttonAreaHeight = /*(partAmount * 64) + ((partAmount - 1) * 16) + 32*/ 16;

        foreach (var item in chapterButtons)
        {
            buttonAreaHeight += item.sizeDelta.y + 16;
        }

        if (workInProgress && cases[ChapterManager.instance.currentCase].chapters.Count > 0)
            buttonAreaHeight += 112;

        if (buttonAreaHeight < 692)
            buttonAreaHeight = 692;

        buttonParent.sizeDelta = new Vector2(buttonParent.sizeDelta.x, buttonAreaHeight);
    }
}
