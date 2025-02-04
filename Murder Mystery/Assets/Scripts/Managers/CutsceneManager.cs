using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

    [SerializeField] private Animator cutsceneAnimator;
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private AnimationClip baseCutscene;

    private Cutscene cutscene;
    private AnimationClip clip;

    [SerializeField] private float transitionSpeed = 0.16f;

    private void Awake()
    {
        instance = this;
    }

    public void StartCutscene(AnimationClip clip)
    {
        this.clip = clip;
        cutscene = null;
        fadeAnimator.SetTrigger("Start");
        Invoke(nameof(FinishTransition), 0.16f);
    }

    public void StartCutscene(Cutscene cutscene)
    {
        this.cutscene = cutscene;
        clip = null;
        fadeAnimator.SetTrigger("Start");
        Invoke(nameof(FinishTransition), 0.16f);
    }

    public void FinishTransition()
    {
        fadeAnimator.SetTrigger("End");
        if (cutscene != null)
            cutsceneAnimator.Play(cutscene.clip.name);
        else
            cutsceneAnimator.Play(clip.name);

        StartCoroutine(EndCutscene());
    }

    IEnumerator EndCutscene()
    {
        if (cutscene != null)
            yield return new WaitForSeconds(cutscene.clip.length - transitionSpeed);
        else
            yield return new WaitForSeconds(clip.length - transitionSpeed);

        fadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionSpeed);
        cutsceneAnimator.Play(baseCutscene.name);
        fadeAnimator.SetTrigger("End");
        yield return new WaitForSeconds(transitionSpeed);

        if (cutscene != null)
        {
            if (cutscene.HasResponses())
            {
                ResponseManager.instance.ShowResponses(cutscene.responses);
            }
            else if (cutscene.AutoPickResponse())
            {
                ResponseManager.instance.OnPickedResponse(cutscene.responses[0]);
            }
        }
    }
}
