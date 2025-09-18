using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public static HintManager instance;

    [SerializeField] private KeyDisplay hintButton;
    [SerializeField] private GameObject testimonyHintType;
    [SerializeField] private TextMeshProUGUI testimonyHintText;
    [SerializeField] private Animator testimonyAnimator;
    
    private BaseHint currentHint;
    private GainAnyClueType clueAnswer;
    private bool activated;
    private bool presentAnswerEmpty;
    private Coroutine coroutine;

    private void Awake()
    {
        instance = this;
    }

    public void SetInteraction(Interaction interaction)
    {
        BaseHint tempHint = null;

        if (interaction is Testimony)
        {
            tempHint = (interaction as Testimony).GetHint();
        }
        else if (interaction is Deduction)
        {
            tempHint = (interaction as Deduction).hint;
        }
        else if (interaction is Memory)
        {
            tempHint = (interaction as Memory).GetHint();
        }
        else if (interaction is ConspiracyBoard)
        {
            tempHint = (interaction as ConspiracyBoard).GetHint();
        }

        if (tempHint != null && (tempHint != currentHint || currentHint == null))
        {
            currentHint = tempHint;
            RemoveHintDisplays();
            ShowHintButton();
            activated = false;
        }
        else if (tempHint == null)
        {
            HideHintButton();
        }
        else if (tempHint == currentHint)
        {
            if (activated)
            {
                Invoke(nameof(ActivateHint), 0.1f);
            }
            else
            {
                ShowHintButton();
            }
        }
    }

    public void SetPresentAnswer(GainAnyClueType clue, bool isEmpty = false)
    {
        if (!isEmpty)
        {
            GainClue gainClue = ClueManager.instance.GetStoredClue(clue);

            if (gainClue == null)
                presentAnswerEmpty = true;
            else if (gainClue.version < clue.version)
                presentAnswerEmpty = true;
            else 
                presentAnswerEmpty = false;
        }
        else
            presentAnswerEmpty = true;

        if (clue != null)
        {
            if (clueAnswer != clue)
            {
                Debug.Log("get clue");
                clueAnswer = clue;
                ClueDisplaySpawner.instance.DisableHintLocks();
            }
        }
        else if (clueAnswer != null)
        {
            Debug.Log("no clue");
            clueAnswer = null;
            ClueDisplaySpawner.instance.DisableHintLocks();
        }
    }

    public void ActivateHint()
    {
        if (currentHint == null)
            return;

        HideHintButton();
        bool tempActivated = activated;
        activated = true;

        if (currentHint is TestimonyHint)
        {
            TestimonyHint();
        }
        else if (currentHint is DeductionHint)
        {
            DeductionHint();
        }
        else if (currentHint is MemoryHint)
        {
            MemoryHint();
        }
        else if (currentHint is ConspiracyHint)
        {
            ConspiracyHint();
        }
        else
        {
            Debug.Log("No hint");
            activated = tempActivated;
            ShowHintButton();
        }
    }

    private void TestimonyHint()
    {
        Debug.Log("testimony");
        foreach (var item in (currentHint as TestimonyHint).hint)
        {
            DialogueManager.instance.testimonyDisplay.ShowHint(item.statement, item.hintType);
        }
    }

    private void DeductionHint()
    {
        foreach (var item in (currentHint as DeductionHint).hints)
        {
            DeductionManager.instance.LockDisplay(item.slider, item.option);
        }
    }

    private void MemoryHint()
    {
        MemoryManager.instance.SetHintIndicator((currentHint as MemoryHint).indicatorName);
    }

    private void ConspiracyHint()
    {
        ConspiracyManager.instance.ShowHint((currentHint as ConspiracyHint).note, (currentHint as ConspiracyHint).hintType);
    }

    public void ActivateClueHint()
    {
        if (clueAnswer == null)
            return;

        if (presentAnswerEmpty)
            ClueDisplaySpawner.instance.EnableFullHintLocks();
        else
            ClueDisplaySpawner.instance.EnablePartialHintLocks(clueAnswer);
    }

    public void ShowTestimonyHintDisplay(TestimonyHintType hintType)
    {
        testimonyAnimator.SetBool("Active", true);
        testimonyHintText.text = hintType.ToString();
        //testimonyHintType.SetActive(true);
    }
    
    public void HideTestimonyHintDisplay()
    {
        testimonyAnimator.SetBool("Active", false);
        //testimonyHintType.SetActive(false);
    }

    private void RemoveHintDisplays()
    {
        DialogueManager.instance.testimonyDisplay.HideHint();
        MemoryManager.instance.SetHintIndicator("");
        ConspiracyManager.instance.HideHints();
    }

    private void ShowHintButton()
    {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(WaitToShowHintButton());
        }
    }

    private IEnumerator WaitToShowHintButton()
    {
        while (true)
        {
            yield return new WaitUntil(CanShowHintButton);
            yield return new WaitForFixedUpdate();
            if (CanShowHintButton())
                break;
        }

        hintButton.SetActive(true);
        coroutine = null;
    }
    
    private bool CanShowHintButton()
    {
        if (currentHint is TestimonyHint)
            return !DialogueManager.instance.IsAnimationActive() && !DialogueManager.instance.IsInPreview();
        else if (currentHint is MemoryHint)
            return !DialogueManager.instance.IsAnimationActive() && MemoryManager.instance.isMemoryActive;
        else 
            return true;
    }

    private void HideHintButton()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        hintButton.SetActive(false);
    }
}
