using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class CasePartButton : MonoBehaviour
{
    [SerializeField] private Image outline;
    [SerializeField] private int chapter;
    [SerializeField] private int index;
    private bool selected;
    private bool highlighted;

    private void Start()
    {
        CaseSelectManager.instance.AddPart(this);
    }

    public void Click()
    {
        CaseSelectManager.instance.DeselectPartsAndChapters();
        ChapterManager.instance.SelectChapter(chapter);
        ChapterManager.instance.SelectPart(index);
        selected = true;
        outline.gameObject.SetActive(true);
        outline.color = CaseSelectManager.instance.selectedColor;
        AudioManager.instance.PlaySFX("Click");
    }

    public void SetValues(int chapter, int part)
    {
        this.chapter = chapter;
        index = part;
    }

    public void Hover(bool hovering)
    {
        highlighted = hovering;
        if (!selected)
        {
            outline.gameObject.SetActive(highlighted);
        }
    }

    public void CloseDropdown()
    {
        if (outline.color != Color.white)
        {
            Deselect();
            ChapterManager.instance.SelectPart(-1);
            ChapterManager.instance.SelectChapter(-1);
        }
    }

    public void Deselect()
    {
        selected = false;
        outline.gameObject.SetActive(highlighted);
        outline.color = Color.white;
    }
}
