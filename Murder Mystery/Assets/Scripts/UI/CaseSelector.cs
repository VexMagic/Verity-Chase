using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseSelectorNewGame : MonoBehaviour
{
    [SerializeField] private GameObject comingSoonDisplay;
    [SerializeField] private bool isComingSoon;
    [SerializeField] private int index;
    private bool isSelected;
    private bool isHovering;

    private void Start()
    {
        CaseSelectManager.instance.AddCase(this);
        comingSoonDisplay.SetActive(isComingSoon);
    }

    public int GetIndex() { return index; }

    public bool IsSelected() { return isSelected; }

    public void Click()
    {
        return;
        AudioManager.instance.PlaySFX("Click");
        CaseSelectManager.instance.ClickCase(this);
    }

    public void Select(bool selected)
    {
        isSelected = selected;

        if (isComingSoon)
            isSelected = false;

        if (isSelected)
        {
            ChapterManager.instance.SelectCase(index);
            ChapterManager.instance.SelectPart(0);
            //outline.enabled = true;
        }
        else
        {
            Hover(isHovering);
        }

        ChapterManager.instance.SelectCase(index);
        ChapterManager.instance.SelectPart(0);
    }

    public void Hover(bool hovering)
    {
        isHovering = hovering;
        if (isSelected || isComingSoon)
            return;

        //outline.enabled = isHovering;
    }
}
