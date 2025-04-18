using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CharacterDisplay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int ID;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float flipSpeed;
    [SerializeField] private bool DelayDialogue;

    private List<CharacterMovementBacklog> backlogList = new List<CharacterMovementBacklog>();

    private Coroutine backlogCoroutine;
    private Coroutine animateCoroutine;
    private Coroutine moveCoroutine;
    private Coroutine flipCoroutine;
    private float endPos;

    public int iD => ID;
    public bool delayDialogue => DelayDialogue;
    public Animator Animator { get { return animator; } }
    public float EndPos { get { return endPos; } }

    public void SetID(int newID)
    {
        ID = newID;
    }

    public void SetValues(CharacterMovement values, bool spawn)
    {
        backlogList.Add(new CharacterMovementBacklog(values, spawn));

        if (backlogCoroutine == null)
            backlogCoroutine = StartCoroutine(Backlog());
    }

    private IEnumerator Backlog()
    {
        while (backlogList.Count > 0)
        {
            animateCoroutine = StartCoroutine(AnimateDisplay(backlogList[0].movement, backlogList[0].spawn));
            yield return new WaitUntil(() => animateCoroutine == null);
            backlogList.RemoveAt(0);
        }
        backlogCoroutine = null;
    }

    private IEnumerator AnimateDisplay(CharacterMovement values, bool spawn)
    {
        endPos = values.xPos;

        if (values.delay > 0)
        {
            //wait until textbox is reset
            //yield return new WaitUntil(() => Typewriter.instance.currentChar <= 1);

            //wait until the delay has been reached
            yield return new WaitUntil(() => Typewriter.instance.currentChar >= values.delay || !Typewriter.instance.isRunning);
        }

        SetCharacter(values.character, values.animation);

        //move character
        if (transform.localPosition.x != values.xPos)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(SmoothMovement(transform.localPosition.x, values.xPos));
        }
        else
            moveCoroutine = null;

        //flip character
        if (spawn)
        {
            if (values.facingLeft)
                transform.localScale = new Vector3(1, 1);
            else
                transform.localScale = new Vector3(-1, 1);
        }
        else if ((values.facingLeft && transform.localScale.x != 1) || (!values.facingLeft && transform.localScale.x == 1))
        {
            if (flipCoroutine != null)
                StopCoroutine(flipCoroutine);

            if (values.facingLeft)
                flipCoroutine = StartCoroutine(FlipAnimation(transform.localScale.x, 1));
            else
                flipCoroutine = StartCoroutine(FlipAnimation(transform.localScale.x, -1));
        }
        else
            flipCoroutine = null;

        yield return new WaitUntil(() => flipCoroutine == null);
        yield return new WaitUntil(() => moveCoroutine == null);
        animateCoroutine = null;
    }

    private IEnumerator SmoothMovement(float start, float end)
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
            transform.localPosition = Vector3.Lerp(new Vector3(start, 0), new Vector3(end, 0), t);
            yield return null;
        }
        moveCoroutine = null;
    }

    private IEnumerator FlipAnimation(float start, float end)
    {
        AudioManager.instance.PlaySFX("Swoosh");
        bool active = true;
        float sinTime = 0;
        while (active)
        {
            sinTime += Time.deltaTime * flipSpeed;
            if (sinTime > Mathf.PI)
                active = false;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            transform.localScale = Vector3.Lerp(new Vector3(start, 1), new Vector3(end, 1), t);
            yield return null;
        }
        flipCoroutine = null;
    }

    float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }

    public void SetCharacter(RuntimeAnimatorController character, AnimationClip animation)
    {
        if (character != null)
        {
            if (animator.runtimeAnimatorController != character)
                animator.runtimeAnimatorController = character;
        }

        SetAnimation(animation);
    }

    private void SetAnimation(AnimationClip animation)
    {
        if (animation != null)
            animator.Play(animation.name);
    }
}
