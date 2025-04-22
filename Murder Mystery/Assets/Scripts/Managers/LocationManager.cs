using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LocationManager : MonoBehaviour
{
    public static LocationManager instance;

    [SerializeField] private RectTransform moveBox;
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    [SerializeField] private GameObject locationButton;

    [SerializeField] private GameObject backButton;
    [SerializeField] private Button talkButton;
    [SerializeField] private Button examineButton;
    [SerializeField] private Button moveButton;

    [SerializeField] private GameObject talkCheckmark;
    [SerializeField] private GameObject examineCheckmark;

    [SerializeField] private Image preview;
    [SerializeField] private GameObject unknownLocation;

    [SerializeField] private GameObject locationDisplayParent;

    public PersonSelectorSpawner personSpawner;

    [SerializeField] private Animator screenTransition;
    [SerializeField] private Animator gainLocationAnimator;
    [SerializeField] private TextMeshProUGUI gainedLocationName;

    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject moveScreen;

    [SerializeField] private List<ChapterLocations> chapterLocations = new List<ChapterLocations>();

    private List<Location> unlockedLocations = new List<Location>();
    private List<Location> enteredLocations = new List<Location>();

    [SerializeField] private int currentLocation;
    [SerializeField] private int selectedLocation;

    private List<GameObject> buttonObjects = new List<GameObject>();
    private List<UIButton> UIButtons = new List<UIButton>();
    private bool isMultiplePeople;
    private bool isAnimationFinished;

    public bool isExamining;
    public SpriteRenderer locationSprite;

    private int lastLocationAction = 0;

    public bool IsAnimationFinished => isAnimationFinished;
    public bool IsButtonsActive => buttons.activeSelf;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            moveScreen.SetActive(false);
            SetBackActive(false);
        }
    }

    private void Start()
    {
        unlockedLocations = chapterLocations[ChapterManager.instance.currentChapter].partLocations[ChapterManager.instance.currentPart].locations;
        selectedLocation = chapterLocations[ChapterManager.instance.currentChapter].partLocations[ChapterManager.instance.currentPart].currentLocation;

        foreach (var item in unlockedLocations)
            enteredLocations.Add(item);

        SpawnLocationButtons();
        SetOptionsValues();
        ChangeLocation(true);
    }

    public void Talk()
    {
        lastLocationAction = 1;
        SetBackActive(true);
        SetButtonsActive(false);
        LocationState tempPeople = unlockedLocations[currentLocation].CurrentLocationState();
        if (tempPeople.people.Length == 1)
        {
            personSpawner.SetActive(false);
            ResponseManager.instance.ResponseResult(tempPeople.people[0]);
        }
        else
        {
            personSpawner.SetButtonActive(true);
        }
    }

    public void Examine()
    {
        lastLocationAction = 2;
        isExamining = true;
        DialogueManager.instance.SetClickDetectionActive(false);
        SetBackActive(true);
        SetButtonsActive(false);
        personSpawner.SetActive(false);
        ControlManager.instance.DeselectButton();
        ExamineManager.instance.ResetMouseTracker();
        ExamineManager.instance.SetAimPosition(Vector2.zero, false);
        ExamineManager.instance.StartExamine();
    }

    public void Move()
    {
        lastLocationAction = 3;
        SetBackActive(true);
        UIButtons.Clear();
        for (int i = 0; i < buttonObjects.Count; i++)
        {
            bool tempActive = (buttonObjects.Count - 1 - i) != currentLocation;
            buttonObjects[i].SetActive(tempActive);
            if (tempActive)
            {
                UIButtons.Add(buttonObjects[i].GetComponent<UIButton>());
            }
        }

        SetButtonsActive(false);
        moveScreen.SetActive(true);
        SetUIButtonValues();

        UIButtons[^1].SelectButton(false);
    }

    private void SetUIButtonValues()
    {
        if (UIButtons.Count == 1)
            return;

        for (int i = 0; i < UIButtons.Count; i++)
        {
            UIButton tempBelow = null;
            UIButton tempAbove = null;

            if (i == 0)
                tempBelow = UIButtons[^1];
            else
                tempBelow = UIButtons[i - 1];

            if (i == UIButtons.Count - 1)
                tempAbove = UIButtons[0];
            else
                tempAbove = UIButtons[i + 1];

            UIButtons[i].SetAdjacent(tempAbove, tempBelow, null, null);
        }
    }

    public void SetMultiplePeople()
    {
        SetMultiplePeople(unlockedLocations[currentLocation].CurrentLocationState().people.Length > 1);
    }

    public void SetMultiplePeople(bool multiple)
    {
        isMultiplePeople = multiple;
    }

    public void SetButtonsActive(bool isActive)
    {
        buttons.SetActive(isActive);

        if (isActive)
        {
            SetCheckmarks();
            Invoke(nameof(StartMusic), 0.01f);
            Invoke(nameof(SelectLocationAction), 0.01f);
        }
    }

    private void StartMusic()
    {
        if ((buttons.activeSelf || backButton.activeSelf) && !ResponseManager.instance.IsResponsesActive())
        {
            AudioManager.instance.PlayMusic(unlockedLocations[currentLocation].CurrentLocationState().areaMusic);
        }
    }

    private void SelectLocationAction()
    {
        if ((!buttons.activeSelf && !backButton.activeSelf) || ResponseManager.instance.IsResponsesActive())
            return;

        UIButton tempAction = null;

        if (lastLocationAction == 1)
            tempAction = talkButton.gameObject.GetComponent<UIButton>();
        else if (lastLocationAction == 3)
            tempAction = moveButton.gameObject.GetComponent<UIButton>();
        else
            tempAction = examineButton.gameObject.GetComponent<UIButton>();

        ControlManager.instance.SetSelectedButton(tempAction);
    }

    public void SetBackActive(bool isActive)
    {
        backButton.SetActive(isActive);

        if (isActive)
            Invoke(nameof(StartMusic), 0.01f);
    }

    public bool GetBackButtonActive()
    {
        return backButton.activeSelf;
    }

    public void BackButton()
    {
        isExamining = false;
        DialogueManager.instance.SetClickDetectionActive(true);
        ResponseManager.instance.OnPickedResponse(new Response("", null));
        personSpawner.SetActive(true);
        moveScreen.SetActive(false);
        ExamineManager.instance.DeselectExaminable();
        ExamineManager.instance.StopExamine();

        if (isMultiplePeople)
        {
            personSpawner.SetButtonActive(true);
            SetBackActive(true);
            SetButtonsActive(false);
        }
        else
        {
            personSpawner.SetButtonActive(false);
            SetBackActive(false);
            SetButtonsActive(true);
        }

        SetMultiplePeople(false);
    }

    public IEnumerator GainLocation(Location location)
    {
        if (!unlockedLocations.Contains(location))
        {
            unlockedLocations.Add(location);
            UpdateLocationButtons();
            SetOptionsValues();

            isAnimationFinished = false;
            UpdateGainLocationDisplay(location);
        }

        if (!isAnimationFinished)
        {
            yield return new WaitUntil(() => isAnimationFinished);
            yield return new WaitForSeconds(0.4f);
        }
    }

    private void UpdateGainLocationDisplay(Location location)
    {
        if (location != null)
        {
            gainedLocationName.text = location.title;
            gainLocationAnimator.SetBool("Open", true);
        }
    }

    public void CloseGainLocationDisplay()
    {
        gainLocationAnimator.SetBool("Open", false);
        isAnimationFinished = true;
    }


    private void UpdateLocationButtons()
    {
        foreach (var item in buttonObjects)
        {
            Destroy(item);
        }
        buttonObjects.Clear();
        SpawnLocationButtons();
    }

    private void SpawnLocationButtons()
    {
        float boxHeight = -layoutGroup.spacing;

        foreach (Location location in unlockedLocations.ToArray().Reverse())
        {
            GameObject buttonObject = Instantiate(locationButton, moveBox);
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = location.title;
            buttonObject.GetComponent<UIButton>().onSelect.AddListener(() => SelectLocation(location));
            buttonObject.GetComponent<Button>().onClick.AddListener(() => OnMove());

            buttonObjects.Add(buttonObject);

            boxHeight += buttonObject.GetComponent<RectTransform>().sizeDelta.y + layoutGroup.spacing;
        }

        moveBox.sizeDelta = new Vector2(moveBox.sizeDelta.x, boxHeight);
        moveBox.gameObject.SetActive(true);
    }

    public void SelectLocation(Location location)
    {
        for (int i = 0; i < unlockedLocations.Count; i++)
        {
            if (location == unlockedLocations[i])
            {
                selectedLocation = i;
                if (enteredLocations.Contains(unlockedLocations[selectedLocation]))
                {
                    preview.sprite = location.preview;
                    unknownLocation.SetActive(false);
                }
                else
                    unknownLocation.SetActive(true);
                return;
            }
        }
    }

    public void OnMove() //When you move to a location
    {
        screenTransition.SetTrigger("Start");
        Invoke(nameof(MoveToLocation), 0.16f);
    }

    private void MoveToLocation()
    {
        ChangeLocation(false);
    }

    public void TurnOffLocations()
    {
        foreach (Transform location in locationDisplayParent.transform)
        {
            location.gameObject.SetActive(false);
        }
        ExamineManager.instance.ClearExaminables();
    }

    public void ChangeLocation(bool isStart)
    {
        preview.sprite = null;
        currentLocation = selectedLocation;

        if (!enteredLocations.Contains(unlockedLocations[currentLocation]))
            enteredLocations.Add(unlockedLocations[currentLocation]);

        SetCurrentLocationDisplay();
        personSpawner.DestroyPeople();
        SetOptionsValues();
        BackButton();
        if (isStart)
            DialogueManager.instance.StartChapter();
        else
            ActivateEnterInteraction();
        ExamineManager.instance.CalculateEdgeDistance();
        ExamineManager.instance.ResetCameraPosition();

        screenTransition.SetTrigger("End");
    }

    public void SetCurrentLocationDisplay()
    {
        TurnOffLocations();

        Transform tempLocation = GetCurrentLocationDisplay();

        locationSprite = tempLocation.GetComponent<SpriteRenderer>();
        tempLocation.gameObject.SetActive(true);
        ExamineManager.instance.GetExaminables(tempLocation);
        foreach (var examinable in tempLocation.GetComponentsInChildren<Examinable>())
        {
            examinable.EndHover();
        }
    }

    private Transform GetCurrentLocationDisplay()
    {
        foreach (Transform location in locationDisplayParent.transform)
        {
            if (location.name == unlockedLocations[currentLocation].CurrentLocationState().locationName)
            {
                return location;
            }
        }
        return null;
    }

    public void CheckEnterInteraction()
    {
        if (!unlockedLocations[currentLocation].CurrentLocationState().HasSeenEnterInteraction())
        {
            personSpawner.DestroyPeople();
            SetOptionsValues();
            BackButton();
            personSpawner.SetActive(false);
            ResponseManager.instance.IgnoreNextResponse = true;
            ResponseManager.instance.ResponseResult(unlockedLocations[currentLocation].CurrentLocationState().enterInteraction);
            personSpawner.SetSelectedPersonActive(false);
            SetCurrentLocationDisplay();
        }
    }

    private void ActivateEnterInteraction()
    {
        if (!unlockedLocations[currentLocation].CurrentLocationState().HasSeenEnterInteraction())
        {
            personSpawner.SetActive(false);
            ResponseManager.instance.ResponseResult(unlockedLocations[currentLocation].CurrentLocationState().enterInteraction);
        }
    }

    private void SetOptionsValues()
    {
        if (unlockedLocations[currentLocation].CurrentLocationState() == null)
        {
            talkButton.interactable = false;
            talkButton.GetComponent<UIButton>().SetOutlineInteractable();
        }
        else
        {
            talkButton.interactable = unlockedLocations[currentLocation].CurrentLocationState().people.Length != 0;
            talkButton.GetComponent<UIButton>().SetOutlineInteractable();

            personSpawner.SpawnPeople(unlockedLocations[currentLocation].CurrentLocationState());

            personSpawner.SetButtonActive(false);
        }

        examineButton.interactable = unlockedLocations[currentLocation].CurrentLocationState().examinable;
        examineButton.GetComponent<UIButton>().SetOutlineInteractable();

        SetCheckmarks();
        moveButton.interactable = unlockedLocations.Count > 1;
        moveButton.GetComponent<UIButton>().SetOutlineInteractable();
    }

    private void SetCheckmarks()
    {
        talkCheckmark.SetActive(false);
        examineCheckmark.SetActive(false);

        if (unlockedLocations == null)
            return;
        if (currentLocation >= unlockedLocations.Count)
            return;
        if (unlockedLocations[currentLocation] == null)
            return;

        if (talkButton.interactable)
            SetTalkCheckmark();
        if (examineButton.interactable)
            SetExamineCheckmark();
    }

    private void SetTalkCheckmark()
    {
        talkCheckmark.SetActive(true);
        foreach (var interview in unlockedLocations[currentLocation].CurrentLocationState().people)
        {
            if (!interview.DoneTalking())
            {
                talkCheckmark.SetActive(false);
                break;
            }
        }
    }

    private void SetExamineCheckmark()
    {
        Transform tempLocation = GetCurrentLocationDisplay();

        examineCheckmark.SetActive(true);
        foreach (var examinable in tempLocation.GetComponentsInChildren<Examinable>())
        {
            if (!examinable.HasBeenExamined())
            {
                examineCheckmark.SetActive(false);
                break;
            }
        }
    }
}
