using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonDisplay : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private RectTransform display;
    [SerializeField] private GameObject checkmark;
    [SerializeField] private Button Button;
    [SerializeField] private float basePos, hoverPos;

    public Button button => Button;
    public Interview interview;

    private Coroutine movementCoroutine;

    private void Start()
    {
        SetBasePosition();
        checkmark.SetActive(interview.DoneTalking());
    }

    public void SetBasePosition()
    {
        display.localPosition = new Vector3(0, basePos);
    }

    public void StartHover()
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(SmoothMovement(display.localPosition.y, hoverPos));
    }

    public void EndHover()
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(SmoothMovement(display.localPosition.y, basePos));
    }

    public void SetButtonActive(bool isActive)
    {
        Button.gameObject.SetActive(isActive);
        if (isActive)
        {
            checkmark.SetActive(interview.DoneTalking());
        }
    }

    private IEnumerator SmoothMovement(float start, float end)
    {
        float sinTime = 0;
        while (sinTime != Mathf.PI)
        {
            sinTime += Time.deltaTime * moveSpeed;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            display.localPosition = Vector3.Lerp(new Vector3(0, start), new Vector3(0, end), t);
            yield return null;
        }
    }

    float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }
}
