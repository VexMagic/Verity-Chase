using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : UIManager
{
    [SerializeField] private RectTransform titleScreen;
    [SerializeField] private Animator animator;
    [SerializeField] private float openSpeed;

    private Coroutine coroutine;
    private bool isOpen;

    private void Start()
    {
        titleScreen.localPosition = Vector3.zero;
    }

    //public override void OnScreenSizeChange()
    //{
    //    StopCoroutine(coroutine);
    //    coroutine = null;
    //    if (isOpen)
    //        titleScreen.localPosition = new Vector3(0, 0);
    //    else
    //        titleScreen.localPosition = new Vector3(0, -Screen.height);
    //}

    public void OpenTitleScreen(bool open)
    {
        animator.SetBool("Open", open);
        //if (coroutine != null)
        //    return;

        //isOpen = open;

        //if (open)
        //    coroutine = StartCoroutine(SmoothMovement(-Screen.height, 0));
        //else
        //    coroutine = StartCoroutine(SmoothMovement(0, -Screen.height));

        if (open)
        {
            Invoke(nameof(ResetSlectors), 0.7f);
        }
    }

    private IEnumerator SmoothMovement(float start, float end)
    {
        bool active = true;
        float sinTime = 0;
        while (active)
        {
            sinTime += Time.deltaTime * Mathf.PI * openSpeed;
            if (sinTime > Mathf.PI)
                active = false;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            titleScreen.localPosition = Vector3.Lerp(new Vector3(0, start), new Vector3(0, end), t);
            yield return null;
        }
        coroutine = null;
    }

    float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }

    private void ResetSlectors()
    {
        CaseSelectManager.instance.ResetSelections();
    }

    public void StartCase()
    {
        SceneManager.LoadScene(ChapterManager.instance.caseNumber + 1);
    }
}
