using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CaseChapterButton : MonoBehaviour
{
    [SerializeField] private GameObject casePartButton;
    [SerializeField] private RectTransform partParent;
    [SerializeField] private RectTransform mainObject;
    [SerializeField] private GameObject dropdownButton;
    [SerializeField] private GameObject dropupButton;
    [SerializeField] private float openSpeed;
    [SerializeField] private float partButtonHeight;
    [SerializeField] private UIButton button;
    public int index;
    public ChapterSelector source;
    private bool droppedDown;
    private float extraSize;
    private Coroutine coroutine;
    private List<CasePartButton> partButtons = new List<CasePartButton>();
    private List<UIButton> UIButtons = new List<UIButton>();
    private UIButton storedButton;
    private UIButton aboveStoredButton;

    private void Start()
    {
        CaseSelectManager.instance.AddChapter(this);
        button.onKeyboardSelect.AddListener(() => ScrollViewSnap.instance.SnapTo(mainObject, droppedDown));
    }

    public void Click()
    {
        ChapterManager.instance.SelectChapter(index);
        ChapterManager.instance.SelectPart(0);
        AudioManager.instance.PlaySFX("Click");
        TransitionManager.instance.EnterScene(ChapterManager.instance.currentCase + 1);
    }
    
    public void CreateParts(List<string> parts)
    {
        extraSize = (parts.Count * 48) + ((parts.Count + 1) * 8);

        for (int i = 0; i < parts.Count; i++)
        {
            GameObject gameObject = Instantiate(casePartButton, partParent);

            CasePartButton tempButton = gameObject.GetComponent<CasePartButton>();
            tempButton.SetValues(index, i);
            partButtons.Add(tempButton);

            TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.text = parts[i];

            UIButtons.Add(tempButton.GetComponent<UIButton>());
        }
    }

    public void SetUIButtonValues(UIButton above, UIButton below)
    {
        if (UIButtons.Count == 1)
        {
            UIButtons[0].SetAdjacent(above, below, null, null);
            return;
        }

        for (int i = 0; i < UIButtons.Count; i++)
        {
            UIButton tempAbove = null;
            UIButton tempBelow = null;

            if (i == 0)
                tempAbove = above;
            else
                tempAbove = UIButtons[i - 1];

            if (i == UIButtons.Count - 1)
                tempBelow = below;
            else
                tempBelow = UIButtons[i + 1];

            UIButtons[i].SetAdjacent(tempAbove, tempBelow, null, null);
        }
    }

    public void Dropdown()
    {
        droppedDown = !droppedDown;

        dropdownButton.SetActive(!droppedDown);
        dropupButton.SetActive(droppedDown);

        if (droppedDown)
        {
            storedButton = button.GetButtonInDirection(Vector2Int.down);
            button.SetButtonInDirection(UIButtons[0], Vector2Int.down);

            SetBelowButtonAdjacent(UIButtons[^1]);
        }
        else
        {
            button.SetButtonInDirection(storedButton, Vector2Int.down);
            SetBelowButtonAdjacent(button);
        }

        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(OpenParts());
    }

    private void SetBelowButtonAdjacent(UIButton upButton)
    {
        CaseChapterButton tempButton = storedButton.GetComponentInParent<CaseChapterButton>();
        if (tempButton != null)
            tempButton.SetAboveButton(upButton);
        else
            ChapterSelector.instance.backButton.SetButtonInDirection(upButton, Vector2Int.up);
    }

    public void SetAboveButton(UIButton upButton)
    {
        button.SetButtonInDirection(upButton, Vector2Int.up);
    }

    IEnumerator OpenParts()
    {
        AudioManager.instance.PlaySFX("Open Case");

        float start = 0;
        float end = 0;

        if (droppedDown)
            end = extraSize;
        else
            start = extraSize;

        float sinTime = 0;
        while (sinTime != Mathf.PI)
        {
            sinTime += Time.deltaTime * openSpeed;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            mainObject.sizeDelta = Vector2.Lerp(new Vector2(mainObject.sizeDelta.x, 64 + start), new Vector2(mainObject.sizeDelta.x, 64 + end), t);
            partParent.sizeDelta = Vector2.Lerp(new Vector2(partParent.sizeDelta.x, start), new Vector2(partParent.sizeDelta.x, end), t);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
            source.UpdateScrollArea();

            yield return null;
        }

        if (!droppedDown)
        {
            foreach (var item in partButtons)
            {
                item.CloseDropdown();
            }
        }

        coroutine = null;
    }

    float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }
}
