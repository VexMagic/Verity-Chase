using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteDisplay : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject selected;
    [SerializeField] private Outline hintOutline;
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI connectionType;
    [SerializeField] private float moveSpeed;

    private ConspiracyNote CurrentNote;
    private Vector3 localPos;
    private bool isSelected;
    private Coroutine coroutine;

    public ConspiracyNote currentNote => CurrentNote;

    public void SetLocalPos()
    {
        localPos = transform.localPosition;
    }

    public void SetValues(ConspiracyNote note)
    {
        title.text = note.title;
        description.text = note.description;
        CurrentNote = note;
    }

    public void HideHint()
    {
        hintOutline.enabled = false;
        animator.SetBool("Active", false);
    }

    public void ShowHint(ConspiracyHintType type)
    {
        hintOutline.enabled = true;
        if (ConspiracyManager.instance.currentBoard.canPresent)
        {
            animator.SetBool("Active", true);
            connectionType.text = "+ " + type.ToString();
        }
        else
        {
            animator.SetBool("Active", false);
        }
    }

    public virtual bool IsPointInside(Vector2 pos)
    {
        return Mathf.Abs(transform.localPosition.x - pos.x) <= 160 && Mathf.Abs(transform.localPosition.y - pos.y) <= 160;
    }

    public void Select(bool select)
    {
        isSelected = select;
        selected.SetActive(isSelected);
    }

    public void MoveToMiddle(Vector3 newPos)
    {
        StartMove(newPos);
    }

    public void MoveBack()
    {
        StartMove(localPos);
    }

    public void StopCoroutine()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    private void StartMove(Vector3 endPos)
    {
        AudioManager.instance.PlaySFX("Open Case");
        StopCoroutine();
        coroutine = StartCoroutine(SmoothMovement(transform.localPosition, endPos));
    }

    private IEnumerator SmoothMovement(Vector3 start, Vector3 end)
    {
        bool active = true;
        float sinTime = 0;
        while (active)
        {
            sinTime += Time.deltaTime * moveSpeed;
            if (sinTime > Mathf.PI)
                active = false;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            transform.localPosition = Vector3.Lerp(new Vector3(start.x, start.y), new Vector3(end.x, end.y), t);
            yield return null;
        }
    }

    float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }
}
