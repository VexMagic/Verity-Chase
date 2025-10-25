using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public static FadeEffect instance;

    [SerializeField] private Image fade;
    [SerializeField] private AnimationCurve curve;

    private void Awake()
    {
        instance = this;
    }

    public void StartFadeEffect(FadeToBlack fadeToBlack)
    {
        StartCoroutine(FadeAnimation(fadeToBlack));
    }

    IEnumerator FadeAnimation(FadeToBlack fadeToBlack)
    {
        fade.color = new Color(fadeToBlack.fadeColor.r, fadeToBlack.fadeColor.g, fadeToBlack.fadeColor.b, 0);

        //fade in
        float fadeTime = 0;
        while (fadeTime < fadeToBlack.fadeInSpeed)
        {
            yield return new WaitForFixedUpdate();
            fadeTime += Time.fixedDeltaTime;

            float percentage = fadeTime / fadeToBlack.fadeInSpeed;
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, percentage);
        }

        //movement
        CharacterMovement(fadeToBlack);

        //delay
        fadeTime = 0;
        while (fadeTime < fadeToBlack.fadeOutDelay)
        {
            yield return new WaitForFixedUpdate();
            fadeTime += Time.fixedDeltaTime;
        }

        //fade out
        fadeTime = 0;
        while (fadeTime < fadeToBlack.fadeOutSpeed)
        {
            yield return new WaitForFixedUpdate();
            fadeTime += Time.fixedDeltaTime;

            float percentage = fadeTime / fadeToBlack.fadeOutSpeed;
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1 - percentage);
        }

        if (fadeToBlack.HasResponses())
        {
            Debug.Log("show response");
            ResponseManager.instance.IgnoreNextResponse = false;
            ResponseManager.instance.ShowResponses(fadeToBlack.responses);
        }
        else if (fadeToBlack.AutoPickResponse())
        {
            ResponseManager.instance.OnPickedResponse(fadeToBlack.responses[0]);
        }
    }

    private void CharacterMovement(FadeToBlack fadeToBlack)
    {
        //move character displays
        foreach (CharacterMovement movement in fadeToBlack.movements)
        {
            CharacterManager.instance.MoveCharacter(movement, true);
        }
    }
}
