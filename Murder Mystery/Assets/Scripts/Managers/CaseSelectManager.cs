using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CaseSelectManager : MonoBehaviour
{
    public static CaseSelectManager instance;

    public Color selectedColor;
    [SerializeField] private RectTransform slider;
    [SerializeField] private float slideAmount;
    [SerializeField] private float slideSpeed;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    private List<CaseSelectorNewGame> casesNewGame = new List<CaseSelectorNewGame>();
    private List<CaseChapterButton> chapters = new List<CaseChapterButton>();
    private List<CasePartButton> parts = new List<CasePartButton>();

    private Coroutine coroutine;
    private int currentCase = 1;
    private int caseAmount = 3;
    private bool newGame = false;

    private void Awake()
    {
        instance = this;

        leftArrow.SetActive(currentCase > 1);
        rightArrow.SetActive(currentCase < caseAmount);
    }

    public void SetNewGame(bool isNew)
    {
        newGame = isNew;
        if (newGame)
            Invoke(nameof(SetCaseAndPartToDefault), 0.4f);
    }

    private void SetCaseAndPartToDefault()
    {
        ChapterManager.instance.SelectCase(1);
        ChapterManager.instance.SelectChapter(1);
        ChapterManager.instance.SelectPart(1);
    }

    public void AddCase(CaseSelectorNewGame selector)
    {
        if (!casesNewGame.Contains(selector))
            casesNewGame.Add(selector);
    }

    public void AddChapter(CaseChapterButton chapter)
    {
        if (!chapters.Contains(chapter))
            chapters.Add(chapter);
    }

    public void AddPart(CasePartButton part)
    {
        if (!parts.Contains(part))
            parts.Add(part);
    }

    public void SlideCases(bool left)
    {
        if ((left && currentCase == 1) || (!left && currentCase == caseAmount))
            return;

        if (coroutine != null)
            return;

        //AudioManager.instance.PlaySFX("Click");
        AudioManager.instance.PlaySFX("Open Case");

        if (left)
            currentCase--;
        else 
            currentCase++;

        leftArrow.SetActive(false);
        rightArrow.SetActive(false);

        coroutine = StartCoroutine(Slide(slider.localPosition.x, GetCasePos(currentCase)));
    }

    private float GetCasePos(int caseIndex)
    {
        float temp = caseIndex * slideAmount;

        float temp2 = (caseAmount + 1) * slideAmount / 2;

        return temp2 - temp;
    }

    private IEnumerator Slide(float start, float end)
    {
        bool active = true;
        float sinTime = 0;
        while (active)
        {
            sinTime += Time.deltaTime * slideSpeed;
            if (sinTime > Mathf.PI)
                active = false;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            slider.localPosition = Vector3.Lerp(new Vector3(start, 0), new Vector3(end, 0), t);
            yield return null;
        }
        coroutine = null;

        leftArrow.SetActive(currentCase > 1);
        rightArrow.SetActive(currentCase < caseAmount);
        ChapterManager.instance.SelectCase(currentCase);
    }

    float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }

    public void ResetSlider() 
    {
        Invoke(nameof(SlideToDefault), 0.6f);
    }

    private void SlideToDefault()
    {
        currentCase = 1;
        slider.localPosition = new Vector3(slideAmount, 0);
    }

    public void ClickCase(CaseSelectorNewGame selector)
    {
        ChapterManager.instance.SelectCase(0);
        foreach (var item in casesNewGame)
        {
            if (item != selector)
            {
                item.Select(false);
            }
            else
            {
                item.Select(!item.IsSelected());
            }
        }
    }

    public void DeselectPartsAndChapters()
    {
        ChapterManager.instance.SelectPart(0);
        ChapterManager.instance.SelectChapter(0);
    }
}
