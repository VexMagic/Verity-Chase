using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResponseManager : MonoBehaviour
{
    public static ResponseManager instance;

    [SerializeField] private RectTransform responseBox;
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    [SerializeField] private GameObject responseButton;
    [SerializeField] private Vector2 responsePos;
    [SerializeField] private Vector2 interviewPos;

    private List<GameObject> buttonObjects = new List<GameObject>();

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
            buttonObject.transform.GetChild(1).gameObject.SetActive(isInterview && LogManager.instance.HasFinishedInteraction(response.result));

            buttonObjects.Add(buttonObject);

            boxHeight += buttonObject.GetComponent<RectTransform>().sizeDelta.y + layoutGroup.spacing;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, boxHeight);
        responseBox.gameObject.SetActive(true);
    }

    public void OnPickedResponse(Response response)
    {
        ExitResponseMenu();

        if (response.result != null)
            ResponseResult(response.result);
        else
            DialogueManager.instance.EndDialogue();
    }

    public void ExitResponseMenu()
    {
        LocationManager.instance.SetBackActive(false);
        LocationManager.instance.personSpawner.SetSelectedPersonActive(false);
        responseBox.gameObject.SetActive(false);

        foreach (GameObject button in buttonObjects)
        {
            Destroy(button);
        }
        buttonObjects.Clear();
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
            LocationManager.instance.SetMultiplePeople();
            DialogueManager.instance.EndDialogue(true);
            ShowResponses((result as Interview).responses(), true);
            storedMusic = (result as Interview).music;
            LocationManager.instance.personSpawner.SelectPerson((result as Interview));
            Invoke(nameof(StartMusic), 0.01f);
        }
        else if (result is Cutscene) 
        {
            DialogueManager.instance.StartCutscene(result as Cutscene);
        }
        else if (result is ChapterTransition)
        {
            ChapterManager.instance.caseNumber = (result as ChapterTransition).caseIndex;
            ChapterManager.instance.currentChapter = (result as ChapterTransition).chapterIndex;
            ChapterManager.instance.currentPart = 0;
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
        }
    }

    private void StartMusic()
    {
        if (IsResponsesActive())
        {
            AudioManager.instance.PlayMusic(storedMusic);
        }
    }
}
