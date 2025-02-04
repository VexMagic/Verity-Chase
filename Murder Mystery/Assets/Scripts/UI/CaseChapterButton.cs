using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CaseChapterButton : MonoBehaviour
{
    [SerializeField] private Image outline;
    [SerializeField] private GameObject casePartButton;
    [SerializeField] private RectTransform partParent;
    [SerializeField] private RectTransform mainObject;
    [SerializeField] private float openSpeed;
    public int index;
    public CaseSelectorContinue source;
    private bool selected;
    private bool highlighted;
    private bool droppedDown;
    private float extraSize;
    private Coroutine coroutine;
    private List<CasePartButton> partButtons = new List<CasePartButton>();

    private void Start()
    {
        CaseSelectManager.instance.AddChapter(this);
    }

    public void Click()
    {
        CaseSelectManager.instance.DeselectPartsAndChapters();
        ChapterManager.instance.SelectChapter(index);
        ChapterManager.instance.SelectPart(0);
        selected = true;
        outline.gameObject.SetActive(true);
        outline.color = CaseSelectManager.instance.selectedColor;
        AudioManager.instance.PlaySFX("Click");
    }
    
    public void CreateParts(List<string> parts)
    {
        extraSize = (parts.Count * 64) + ((parts.Count + 1) * 16);

        for (int i = 0; i < parts.Count; i++)
        {
            GameObject gameObject = Instantiate(casePartButton, partParent);

            CasePartButton tempButton = gameObject.GetComponent<CasePartButton>();
            tempButton.SetValues(index, i);
            partButtons.Add(tempButton);

            TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.text = parts[i];
        }
    }

    public void Dropdown()
    {
        droppedDown = !droppedDown;

        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(OpenParts());
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
            mainObject.sizeDelta = Vector2.Lerp(new Vector2(mainObject.sizeDelta.x, 96 + start), new Vector2(mainObject.sizeDelta.x, 96 + end), t);
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

    //private IEnumerator FlipAnimation(float start, float end)
    //{
    //    AudioManager.instance.PlaySFX("Swoosh");
    //    float sinTime = 0;
    //    while (start != end)
    //    {
    //        sinTime += Time.deltaTime * openSpeed;
    //        sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
    //        float t = Evaluate(sinTime);
    //        transform.localScale = Vector3.Lerp(new Vector3(start, 1), new Vector3(end, 1), t);
    //        yield return null;
    //    }
    //}

    float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }

    public void Hover(bool hovering)
    {
        highlighted = hovering;
        if (!selected)
        {
            outline.gameObject.SetActive(highlighted);
        }
    }

    public void Deselect()
    {
        selected = false;
        outline.gameObject.SetActive(highlighted);
        outline.color = Color.white;
    }
}
