using UnityEngine;
using UnityEngine.UI;

public class CasePartButton : MonoBehaviour
{
    [SerializeField] private Image outline;
    [SerializeField] private int chapter;
    [SerializeField] private int index;

    private void Start()
    {
        CaseSelectManager.instance.AddPart(this);
        GetComponent<UIButton>().onKeyboardSelect.AddListener(() => ScrollViewSnap.instance.SnapTo(GetComponent<RectTransform>(), false));
    }

    public void Click()
    {
        ChapterManager.instance.SelectChapter(chapter);
        ChapterManager.instance.SelectPart(index);
        AudioManager.instance.PlaySFX("Click");
        TransitionManager.instance.EnterScene(ChapterManager.instance.currentCase + 1);
    }

    public void SetValues(int chapter, int part)
    {
        this.chapter = chapter;
        index = part;
    }

    public void CloseDropdown()
    {
        if (outline.gameObject.activeSelf)
        {
            ControlManager.instance.SetSelectedButton(transform.parent.GetComponent<UIButton>());
            ChapterManager.instance.SelectPart(-1);
            ChapterManager.instance.SelectChapter(-1);
        }
    }
}
