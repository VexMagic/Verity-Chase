using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatementDisplay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip highlighted;
    [SerializeField] private GameObject hintDisplay;
    private bool isHighlighted;
    private TestimonyHintType hintType;
    private bool hasHint = false;

    public void ShowHint(TestimonyHintType hintType)
    {
        this.hintType = hintType;
        hasHint = true;
        hintDisplay.SetActive(true);
        if (isHighlighted)
            HintManager.instance.ShowTestimonyHintDisplay(hintType);
    }

    public void HideHint()
    {
        hasHint = false;
        hintDisplay.SetActive(false);
    }

    public void Highlight(bool isHighlighted)
    {
        this.isHighlighted = isHighlighted;
        if (animator.enabled)
            animator.SetBool("Selected", isHighlighted);

        if (isHighlighted && hasHint)
            HintManager.instance.ShowTestimonyHintDisplay(hintType);
    }

    private void OnEnable()
    {
        animator.SetBool("Selected", isHighlighted);
    }
}
