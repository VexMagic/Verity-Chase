using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class ResponseManager : MonoBehaviour
{
    public static ResponseManager instance;

    [SerializeField] private RectTransform responseBox;
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    [SerializeField] private GameObject responseButton;
    [SerializeField] private Vector2 responsePos;
    [SerializeField] private Vector2 interviewPos;

    private List<GameObject> buttonObjects = new List<GameObject>();
    private List<UIButton> UIButtons = new List<UIButton>();

    public bool IgnoreNextResponse;
    private AudioClip storedMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public bool IsResponsesActive() { return responseBox.gameObject.activeSelf; }

    public void ShowResponses(Response[] responses, bool isInterview = false)
    {
        if (IgnoreNextResponse)
        {
            IgnoreNextResponse = false;
            return;
        }

        if (isInterview)
            responseBox.localPosition = interviewPos;
        else
            responseBox.localPosition = responsePos;

        float boxHeight = -layoutGroup.spacing;

        foreach (Response response in responses)
        {
            GameObject buttonObject = Instantiate(responseButton, responseBox);
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = response.text;
            buttonObject.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));
            buttonObject.GetComponent<Button>().onClick.AddListener(() => AudioManager.instance.PlaySFX("Click"));

            //set check mark to active if you have read the response and is part of interview
            buttonObject.transform.GetChild(2).gameObject.SetActive(isInterview && LogManager.instance.HasFinishedInteraction(response.result));

            UIButton tempButton = buttonObject.GetComponent<UIButton>();

            buttonObjects.Add(buttonObject);
            UIButtons.Add(tempButton);

            boxHeight += buttonObject.GetComponent<RectTransform>().sizeDelta.y + layoutGroup.spacing;
        }

        SetUIButtonValues();
        UIButtons[0].SelectButton(false);

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, boxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void SetUIButtonValues()
    {
        if (UIButtons.Count == 1)
            return;

        for (int i = 0; i < UIButtons.Count; i++)
        {
            UIButton tempAbove = null;
            UIButton tempBelow = null;

            if (i == 0)
                tempAbove = UIButtons[^1];
            else
                tempAbove = UIButtons[i - 1];

            if (i == UIButtons.Count - 1)
                tempBelow = UIButtons[0];
            else
                tempBelow = UIButtons[i + 1];

            UIButtons[i].SetAdjacent(tempAbove, tempBelow, null, null);
        }
    }

    public void OnPickedResponse(Response response)
    {
        ExitResponseMenu();

        if (response.result != null)
        {
            ResponseResult(response.result);
            switch (response.sFX)
            {
                case ResponseSFX.Correct:
                    AudioManager.instance.PlaySFX("Correct");
                    break;
                case ResponseSFX.Wrong:
                    AudioManager.instance.PlaySFX("Wrong");
                    break;
            }
        }
        else
            DialogueManager.instance.EndDialogue();
    }

    public void ExitResponseMenu()
    {
        LocationManager.instance.SetBackActive(false);
        LocationManager.instance.personSpawner.SetSelectedPersonActive(false);
        responseBox.gameObject.SetActive(false);
        ControlManager.instance.DeselectButton();

        foreach (GameObject button in buttonObjects)
        {
            Destroy(button);
        }
        buttonObjects.Clear();
        UIButtons.Clear();
    }

    public DialogueLine GetDialogueLine(Interaction result)
    {
        if (result is Dialogue) //check what childclass the result is
        {
            return (result as Dialogue).lines[0];
        }
        else if (result is PresentClue)
        {
            return (result as PresentClue).line;
        }
        else if (result is Testimony)
        {
            return (result as Testimony).lines[0].line;
        }

        return null;
    }

    public void ResponseResult(Interaction result)
    {
        if (result == null)
            return;

        HintManager.instance.SetPresentAnswer(null, true);
        DialogueManager.instance.SetClickDetectionActive(true);
        ExitResponseMenu();

        if (result is Dialogue) //check what childclass the result is
        {
            DialogueManager.instance.StartDialogue(result as Dialogue);
        }
        else if (result is PresentClue)
        {
            DialogueManager.instance.StartPresent(result as PresentClue);
        }
        else if (result is Deduction)
        {
            DialogueManager.instance.StartDeduction(result as Deduction);
        }
        else if (result is Testimony)
        {
            DialogueManager.instance.StartTestimony(result as Testimony);
        }
        else if (result is Interview)
        {
            IgnoreNextResponse = false;
            LocationManager.instance.SetMultiplePeople();
            ShowResponses((result as Interview).responses(), true);
            storedMusic = (result as Interview).music;
            LocationManager.instance.personSpawner.SelectPerson((result as Interview));
            DialogueManager.instance.EndDialogue(true);
            Invoke(nameof(StartMusic), 0.01f);
        }
        else if (result is Cutscene) 
        {
            DialogueManager.instance.StartCutscene(result as Cutscene);
        }
        else if (result is ChapterTransition)
        {
            ChapterManager.instance.currentCase = (result as ChapterTransition).caseIndex;
            ChapterManager.instance.currentChapter = (result as ChapterTransition).chapterIndex;
            ChapterManager.instance.currentPart = 0;
            TransitionManager.instance.EnterScene(SceneManager.sceneCountInBuildSettings - 1);
        }
        else if (result is Memory)
        {
            DialogueManager.instance.SetClickDetectionActive(false);
            CharacterManager.instance.DeleteCharacters();
            DialogueManager.instance.StartMemory(result as Memory);
        }
        else if (result is SplitPath)
        {
            DialogueManager.instance.StartSplitPath(result as SplitPath);
        }
        else if (result is ConspiracyBoard)
        {
            DialogueManager.instance.StartConspiracyBoard(result as ConspiracyBoard);
        }
        else if (result is CameraZoom)
        {
            CameraManager.instance.StartZoom(result as CameraZoom);

            if ((result as CameraZoom).HasResponses())
            {
                instance.IgnoreNextResponse = false;
                instance.ShowResponses((result as CameraZoom).responses);
            }
            else if ((result as CameraZoom).AutoPickResponse())
            {
                OnPickedResponse((result as CameraZoom).responses[0]);
            }
        }

        HintManager.instance.SetInteraction(result);
    }

    private void StartMusic()
    {
        if (IsResponsesActive())
        {
            AudioManager.instance.PlayMusic(storedMusic);
        }
    }
}
