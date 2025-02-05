using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEditor;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] private List<CharacterData> characterDisplayNames;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject nameBox;
    [SerializeField] private GameObject progressDetection;
    [SerializeField] private GameObject nextIcon;
    [SerializeField] private StatementDisplaySpawner testimonyDisplay;
    [SerializeField] private GameObject hasPressedCheckmark;
    [SerializeField] private GameObject testimonyArrows;
    [SerializeField] private GameObject leftTestimony;
    [SerializeField] private GameObject rightTestimony;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Animator testimonyAnimatior;
    [SerializeField] private TextMeshProUGUI topTestimonyText;
    [SerializeField] private TextMeshProUGUI bottomTestimonyText;
    [SerializeField] private List<PartStartData> chapterStart;
    //[SerializeField] private bool debugActive;

    private Dialogue currentDialogue;
    private int currentLine;
    private int currentTestimonyLine;
    private bool isHovering;
    private bool hasPresented;
    private bool hasPressed;
    private bool inTestimony;
    private bool isPressing;

    public bool newInteractionFinished;
    public bool interactionActive;

    private Coroutine lineCoroutine;

    [Serializable]
    public class CharacterData
    {
        [SerializeField] private string Title;
        [SerializeField] private BlipSound Sound;
        public string title => Title;
        public BlipSound sound => Sound;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        //backgroundImage.enabled = true;
        testimonyDisplay.gameObject.SetActive(false);
        EndDialogue();
    }

    public void StartChapter()
    {
        DialogueLine line = ResponseManager.instance.GetDialogueLine(chapterStart[ChapterManager.instance.currentChapter].starts[ChapterManager.instance.currentPart]);
        if (line != null)
        {
            if (line.backgrounds.Count > 0)
            {
                backgroundImage.sprite = line.backgrounds[^1].image;
                backgroundImage.color = new Color(1, 1, 1, 1);
                backgroundImage.enabled = true;
            }
        }
        ResponseManager.instance.ResponseResult(chapterStart[ChapterManager.instance.currentChapter].starts[ChapterManager.instance.currentPart]);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        interactionActive = true;
        StartCoroutine(StepThroughDialogue(dialogue));
    }

    public void StartPresent(PresentClue present)
    {
        interactionActive = true;
        StartCoroutine(RunPresentClue(present));
    }

    public void StartDeduction(Deduction deduction)
    {
        interactionActive = true;
        StartCoroutine(RunDeduction(deduction));
    }

    public void StartTestimony(Testimony testimony)
    {
        interactionActive = true;
        if (LogManager.instance.HasFinishedInteraction(testimony))
        {
            testimonyDisplay.CreateDisplays(testimony, currentTestimonyLine);
            StartCoroutine(RunTestimony(testimony));
        }
        else
        {
            inTestimony = false;
            StartCoroutine(RunTestimonyPreview(testimony));
        }
    }

    public void StartCutscene(Cutscene cutscene)
    {
        textBox.SetActive(false);
        CutsceneManager.instance.StartCutscene(cutscene);
    }

    private IEnumerator StepThroughDialogue(Dialogue dialogue)
    {
        dialogueText.text = string.Empty;

        for (int i = 0; i < dialogue.lines.Length; i++)
        {
            yield return RunDialogueLine(dialogue.lines[i]);

            if (i == dialogue.lines.Length - 1 && dialogue.HasResponses())
            {
                break;
            }

            yield return null;
            yield return new WaitUntil(() => Progress());
        }

        LogManager.instance.FinishInteraction(dialogue);

        if (dialogue.HasResponses())
        {
            ResponseManager.instance.ShowResponses(dialogue.responses);
        }
        else if (dialogue.AutoPickResponse())
        {
            ResponseManager.instance.OnPickedResponse(dialogue.responses[0]);
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator RunPresentClue(PresentClue present)
    {
        yield return RunDialogueLine(present.line);

        yield return null;
        ClueManager.instance.OpenMenu(true, false);

        yield return new WaitUntil(() => HasPresented());
        hasPresented = false;
        ClueManager.instance.CloseMenu();

        bool presentedCorrectly = false;
        var selectedClue = ClueManager.instance.GetSelectedClue(ClueManager.instance.currentMenuType);

        foreach (var item in present.correctAnswers)
        {
            if (selectedClue is GainEvidence)
            {
                if ((selectedClue as GainEvidence).gainedEvidence == item.gainedClue)
                {
                    presentedCorrectly = true;
                }
            }
            else if (selectedClue is GainProfile)
            {
                if ((selectedClue as GainProfile).gainedProfile == item.gainedClue)
                {
                    presentedCorrectly = true;
                }
            }
        }

        if (presentedCorrectly)
        {
            AudioManager.instance.PlaySFX("Correct");
            ResponseManager.instance.ResponseResult(present.correctAnswer);
            LogManager.instance.FinishInteraction(present);
        }
        else
            ResponseManager.instance.ResponseResult(present.wrongAnswer);
    }

    public IEnumerator RunDeduction(Deduction deduction)
    {
        SetClickDetectionActive(false);
        OpenDialogueBox(false);
        DeductionManager.instance.SetButtonActive(true);
        DeductionManager.instance.CreateDeductionDisplays(deduction);
        yield return new WaitUntil(() => HasPresented());
        hasPresented = false;

        SetClickDetectionActive(true);
        if (DeductionManager.instance.IsCorrectAnswer())
        {
            AudioManager.instance.PlaySFX("Correct");
            ResponseManager.instance.ResponseResult(deduction.correctAnswer);
            DeductionManager.instance.SetMenuActive(false);
            DeductionManager.instance.ResetDisplays();
            LogManager.instance.FinishInteraction(deduction);
        }
        else
        {
            ResponseManager.instance.ResponseResult(deduction.wrongAnswer);
            DeductionManager.instance.SetButtonActive(false);
        }
    }

    private IEnumerator RunTestimonyPreview(Testimony testimony)
    {
        yield return TestimonyAnimation(true, testimony.title);

        testimonyAnimatior.SetBool("Active", true);
        AudioManager.instance.PlayMusic(testimony.music);
        dialogueText.text = string.Empty;

        for (int i = 0; i < testimony.lines.Count; i++)
        {
            if (testimony.lines[i].IsAvailable())
            {
                yield return RunDialogueLine(testimony.lines[i].line);
                yield return null;
                yield return new WaitUntil(() => Progress());
            }
        }

        LogManager.instance.FinishInteraction(testimony);

        AudioManager.instance.PlayMusic(null);

        testimonyAnimatior.SetBool("Active", false);
        testimonyAnimatior.SetTrigger("End");
        textBox.SetActive(false);
        yield return new WaitForSeconds(1f);

        ResponseManager.instance.ResponseResult(testimony.previewInteraction);
    }

    private IEnumerator TestimonyAnimation(bool isPreview, string testimonyTitle)
    {
        textBox.SetActive(false);
        AudioManager.instance.PlayMusic(null);
        AudioManager.instance.PlaySFX("Testimony");
        if (isPreview)
        {
            topTestimonyText.text = "Witness";
            bottomTestimonyText.text = "Testimony";
        }
        else
        {
            topTestimonyText.text = "Counter";
            bottomTestimonyText.text = "Argument";
        }
        testimonyAnimatior.SetTrigger("Start");
        yield return new WaitForSeconds(2.5f);

        if (isPreview)
        {
            yield return RunDialogueLine(new DialogueLine(Character.None, testimonyTitle));
            yield return null;
            yield return new WaitUntil(() => Progress());
        }
    }

    private IEnumerator RunTestimony(Testimony testimony)
    {
        if (!inTestimony)
        {
            yield return TestimonyAnimation(false, testimony.title);
            inTestimony = true;
        }
        else if (isPressing)
        {
            //move to next testimony line
            for (int i = currentTestimonyLine + 1; i < testimony.lines.Count; i++)
            {
                if (testimony.lines[i].IsAvailable())
                {
                    currentTestimonyLine = i;
                    testimonyDisplay.UpdateDisplays(testimony, currentTestimonyLine);
                    hasPressedCheckmark.SetActive(LogManager.instance.HasFinishedInteraction(testimony.lines[currentTestimonyLine].pressResult));
                    break;
                }
            }
        }

        isPressing = false;

        AudioManager.instance.PlayMusic(testimony.music);
        ClueManager.instance.SetTestimony(true);
        testimonyDisplay.gameObject.SetActive(true);
        hasPressedCheckmark.SetActive(LogManager.instance.HasFinishedInteraction(testimony.lines[currentTestimonyLine].pressResult));

        while (!HasPresented() && !HasPressed())
        {
            dialogueText.text = string.Empty;
            yield return RunDialogueLine(testimony.lines[currentTestimonyLine].line, true);
            SetTestimonyArrows(testimony);
            TestimonyArrowsActive(true);
            yield return null;
            yield return new WaitUntil(() => TestimonyProgress(testimony) || HasPresented() || HasPressed());

            if (TestimonyProgress(testimony))
            {
                Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                if (mouse.x < Screen.width / 2 && currentTestimonyLine > 0) //Click Left side
                {
                    for (int i = currentTestimonyLine - 1; i >= 0; i--)
                    {
                        if (testimony.lines[i].IsAvailable())
                        {
                            currentTestimonyLine = i;
                            testimonyDisplay.UpdateDisplays(testimony, currentTestimonyLine);
                            hasPressedCheckmark.SetActive(LogManager.instance.HasFinishedInteraction(testimony.lines[currentTestimonyLine].pressResult));
                            break;
                        }
                    }
                }
                else if (mouse.x >= Screen.width / 2 && currentTestimonyLine < testimony.lines.Count - 1) //Click Right side
                {
                    for (int i = currentTestimonyLine + 1; i < testimony.lines.Count; i++)
                    {
                        if (testimony.lines[i].IsAvailable())
                        {
                            currentTestimonyLine = i;
                            testimonyDisplay.UpdateDisplays(testimony, currentTestimonyLine);
                            hasPressedCheckmark.SetActive(LogManager.instance.HasFinishedInteraction(testimony.lines[currentTestimonyLine].pressResult));
                            break;
                        }
                    }
                }
            }
        }

        ClueManager.instance.SetTestimony(false);
        testimonyDisplay.gameObject.SetActive(false);
        ClueManager.instance.CloseMenu();

        if (HasPresented())
        {
            hasPresented = false;

            bool isCorrect = false;
            TestimonyPresent tempPresent = null;
            foreach (var answer in testimony.lines[currentTestimonyLine].correctAnswers)
            {
                var selectedClue = ClueManager.instance.GetSelectedClue(ClueManager.instance.currentMenuType);
                if (selectedClue is GainEvidence)
                {
                    if ((selectedClue as GainEvidence).gainedEvidence == answer.correctClue.gainedClue)
                    {
                        isCorrect = true;
                        tempPresent = answer;
                    }
                }
                else if (selectedClue is GainProfile)
                {
                    if ((selectedClue as GainProfile).gainedProfile == answer.correctClue.gainedClue)
                    {
                        isCorrect = true;
                        tempPresent = answer;
                    }
                }
            }

            if (isCorrect)
            {
                currentTestimonyLine = 0;
                inTestimony = false;
                AudioManager.instance.PlaySFX("Correct");
                ResponseManager.instance.ResponseResult(tempPresent.result);
                LogManager.instance.FinishInteraction(testimony);
            }
            else
                ResponseManager.instance.ResponseResult(testimony.wrongAnswer);
        }
        else
        {
            hasPressed = false;
            isPressing = true;
            ResponseManager.instance.ResponseResult(testimony.lines[currentTestimonyLine].pressResult);
        }
    }

    public CharacterData GetCharacterData(Character character)
    {
        return characterDisplayNames[((int)character)];
    }

    private IEnumerator RunDialogueLine(DialogueLine line, bool isTestimony = false)
    {
        if (lineCoroutine != null)
            StopCoroutine(lineCoroutine);

        yield return lineCoroutine = StartCoroutine(StartDialogueLine(line, isTestimony));
    }

    private IEnumerator StartDialogueLine(DialogueLine line, bool isTestimony)
    {
        LogManager.instance.ReadLine(line);
        TextEffectManager.instance.DeactivateEffects(true);
        LocationManager.instance.personSpawner.SetActive(false);
        LocationManager.instance.SetButtonsActive(false);
        TestimonyArrowsActive(false);
        nextIcon.SetActive(false);
        OpenDialogueBox(true);
        nameBox.SetActive(line.talkingCharacter != Character.None);
        if (line.talkingCharacter != Character.None)
        {
            characterName.text = GetCharacterData(line.talkingCharacter).title;
            dialogueText.alignment = TextAlignmentOptions.TopLeft;
            AudioManager.instance.currentBlipSound = GetCharacterData(line.talkingCharacter).sound;
        }
        else
        {
            AudioManager.instance.currentBlipSound = BlipSound.Typewriter;
            dialogueText.alignment = TextAlignmentOptions.Top;
        }

        //move character displays
        foreach (CharacterMovement movement in line.movements)
        {
            CharacterManager.instance.MoveCharacter(movement);
        }

        foreach (var item in line.backgrounds)
        {
            StartCoroutine(BackgroundEffect(item));
        }

        if (line.music != null)
        {
            AudioManager.instance.PlayMusic(line.music);
        }

        //add testimony color
        string tempLine = line.text;
        if (isTestimony)
            tempLine = "<testimony>" + tempLine + "<>";

        yield return new WaitForSeconds(0.1f);

        CameraManager.instance.NewLine(line.talkingCharacter);
        if (CharacterManager.instance.GetAnimationDelayActive())
        {
            textBox.SetActive(false);
            yield return new WaitUntil(() => !CharacterManager.instance.GetAnimationDelayActive());
            textBox.SetActive(true);
        }

        yield return RunTypingEffect(tempLine);

        nextIcon.SetActive(true && !isTestimony);
        dialogueText.text = TextEffectManager.instance.ConvertCommands(tempLine);

        TextEffectManager.instance.ActivateEffects();

        foreach (var gainedClue in line.gainedClues)
        {
            yield return ClueManager.instance.GainClue(gainedClue);
        }

        foreach (Location gainedLocation in line.gainedLocations)
        {
            yield return LocationManager.instance.GainLocation(gainedLocation);
        }

        lineCoroutine = null;
    }

    private IEnumerator BackgroundEffect(Background background)
    {
        //wait until textbox is reset
        yield return new WaitUntil(() => Typewriter.instance.currentChar > 1);

        //wait until the delay has been reached
        yield return new WaitUntil(() => Typewriter.instance.currentChar >= background.delay || !Typewriter.instance.isRunning);

        if (background.fadeIn)
        {
            backgroundImage.sprite = background.image;
            backgroundImage.color = new Color(1, 1, 1, 0);
            backgroundImage.enabled = true;
        }
        else
        {
            backgroundImage.color = new Color(1, 1, 1, 1);
        }

        float startTime = Time.time;
        while (Time.time - startTime < background.fadeSpeed)
        {
            float progress = (Time.time - startTime) / background.fadeSpeed;
            backgroundImage.color = new Color(1, 1, 1, background.fadeIn ? progress : 1 - progress);
            yield return null;
        }

        if (!background.fadeIn)
        {
            backgroundImage.enabled = false;
        }
        else
        {
            backgroundImage.color = new Color(1, 1, 1, 1);
        }
    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        Typewriter.instance.Run(dialogue, dialogueText);

        while (Typewriter.instance.isRunning)
        {
            yield return null;

            if (Progress())
            {
                Typewriter.instance.Stop();
            }
        }
    } 

    public bool Progress()
    {
#if (UNITY_EDITOR)
        if (Input.GetKey(KeyCode.Space) && isHovering)
            return true;
#endif
        return Input.GetMouseButtonDown(0) && isHovering;
    }

    public bool TestimonyProgress(Testimony testimony)
    {
        bool isValid = false;
        Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

        if (mouse.x < Screen.width / 2 && currentTestimonyLine > 0) //Click Left side
        {
            for (int i = currentTestimonyLine - 1; i >= 0; i--)
            {
                if (testimony.lines[i].IsAvailable())
                {
                    isValid = true;
                    break;
                }
            }
        }
        else if (mouse.x >= Screen.width / 2 && currentTestimonyLine < testimony.lines.Count - 1) //Click Right side
        {
            for (int i = currentTestimonyLine + 1; i < testimony.lines.Count; i++)
            {
                if (testimony.lines[i].IsAvailable())
                {
                    isValid = true;
                    break;
                }
            }
        }

        return Input.GetMouseButtonDown(0) && isHovering && isValid;
    }

    private void SetTestimonyArrows(Testimony testimony)
    {
        leftTestimony.SetActive(false);
        rightTestimony.SetActive(false);
        for (int i = currentTestimonyLine - 1; i >= 0; i--)
        {
            if (testimony.lines[i].IsAvailable())
            {
                leftTestimony.SetActive(true);
                break;
            }
        }

        for (int i = currentTestimonyLine + 1; i < testimony.lines.Count; i++)
        {
            if (testimony.lines[i].IsAvailable())
            {
                rightTestimony.SetActive(true);
                break;
            }
        }
    }

    public void TestimonyArrowsActive(bool active)
    {
        testimonyArrows.SetActive(active);
    }

    public void EndDialogue(bool isInterviewing = false)
    {
        interactionActive = false;
        if (!isInterviewing)
        {
            LocationManager.instance.SetButtonsActive(true);
            LocationManager.instance.personSpawner.SetActive(true);
        }
        else
            LocationManager.instance.SetBackActive(true);

        OpenDialogueBox(false);
        TextEffectManager.instance.DeactivateEffects(false);
        CameraManager.instance.EndDialogue();
        CharacterManager.instance.DeleteCharacters();
        if (newInteractionFinished)
        {
            newInteractionFinished = false;
            LocationManager.instance.CheckEnterInteraction();
        }

        if (LocationManager.instance.isExamining)
        {
            LocationManager.instance.Examine();
            //SetClickDetectionActive(false);
        }
    }

    private void OpenDialogueBox(bool open)
    {
        dialogueText.text = string.Empty;
        textBox.SetActive(open);
    }

    public void SetClickDetectionActive(bool active)
    {
        progressDetection.SetActive(active);
    }

    public bool HasPresented()
    {
        return hasPresented;
    }

    public bool HasPressed()
    {
        return hasPressed;
    }

    public void Enter() { isHovering = true; }

    public void Exit() { isHovering = false; }

    public void Present(bool present) { hasPresented = present; ClueManager.instance.isTestimonyPresenting = false; }
    public void Press(bool press) { hasPressed = press; }
}
