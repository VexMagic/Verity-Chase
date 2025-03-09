using UnityEngine;
using UnityEngine.UI;

public class StatementDisplay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip highlighted;
    private bool isHighlighted;

    public void Highlight(bool isHighlighted)
    {
        this.isHighlighted = isHighlighted;
        if (animator.enabled)
            animator.SetBool("Selected", isHighlighted);
    }

    private void OnEnable()
    {
        animator.SetBool("Selected", isHighlighted);
    }
}
