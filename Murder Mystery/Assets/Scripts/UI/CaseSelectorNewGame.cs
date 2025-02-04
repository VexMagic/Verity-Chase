using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseSelectorNewGame : MonoBehaviour
{
    [SerializeField] private Outline outline;
    [SerializeField] private GameObject comingSoonText;
    [SerializeField] private bool comingSoon;
    [SerializeField] private int index;
    private bool isSelected;
    private bool isHovering;

    private void Start()
    {
        CaseSelectManager.instance.AddCase(this);
        comingSoonText.SetActive(comingSoon);
        outline.enabled = false;
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

        if (comingSoon)
            isSelected = false;

        if (isSelected)
        {
            ChapterManager.instance.SelectCase(index);
            ChapterManager.instance.SelectPart(0);
            outline.effectColor = CaseSelectManager.instance.selectedColor;
            //outline.enabled = true;
        }
        else
        {
            outline.effectColor = Color.white;
            Hover(isHovering);
        }

        ChapterManager.instance.SelectCase(index);
        ChapterManager.instance.SelectPart(0);
    }

    public void Hover(bool hovering)
    {
        isHovering = hovering;
        if (isSelected || comingSoon)
            return;

        //outline.enabled = isHovering;
    }
}
