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
    [SerializeField] private Button confirmMoveButton;

    [SerializeField] private Image preview;
    [SerializeField] private Sprite unknownLocation;

    [SerializeField] private GameObject locationDisplayParent;

    public PersonSelectorSpawner personSpawner;

    [SerializeField] private Animator screenTransition;

    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject moveScreen;

    [SerializeField] private List<ChapterLocations> chapterLocations = new List<ChapterLocations>();

    private List<Location> unlockedLocations = new List<Location>();
    private List<Location> enteredLocations = new List<Location>();

    [SerializeField] private int currentLocation;
    [SerializeField] private int selectedLocation;

    private List<GameObject> buttonObjects = new List<GameObject>();
    private bool isMultiplePeople;

    public bool isExamining;
    public SpriteRenderer locationSprite;

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
        SpawnLocationButtons();
        SetOptionsValues();
        ChangeLocation(true);
    }

    public void Talk()
    {
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
        isExamining = true;
        DialogueManager.instance.SetClickDetectionActive(false);
        SetBackActive(true);
        SetButtonsActive(false);
        personSpawner.SetActive(false);
    }

    public void Move()
    {
        SetBackActive(true);
        for (int i = 0; i < buttonObjects.Count; i++)
        {
            buttonObjects[i].SetActive((buttonObjects.Count - 1 - i) != currentLocation);
        }

        SetButtonsActive(false);
        moveScreen.SetActive(true);
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
            Invoke(nameof(StartMusic), 0.01f);
    }

    private void StartMusic()
    {
        if ((buttons.activeSelf || backButton.activeSelf) && !ResponseManager.instance.IsResponsesActive())
        {
            AudioManager.instance.PlayMusic(unlockedLocations[currentLocation].CurrentLocationState().areaMusic);
        }
    }

    public void SetBackActive(bool isActive)
    {
        backButton.SetActive(isActive);

        if (isActive)
            Invoke(nameof(StartMusic), 0.01f);
    }

    public void BackButton()
    {
        isExamining = false;
        DialogueManager.instance.SetClickDetectionActive(true);
        ResponseManager.instance.OnPickedResponse(new Response("", null));
        personSpawner.SetActive(true);
        moveScreen.SetActive(false);

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
        unlockedLocations.Add(location);
        UpdateLocationButtons();
        SetOptionsValues();

        yield return null;
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
            buttonObject.GetComponent<Button>().onClick.AddListener(() => SelectLocation(location));

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
                confirmMoveButton.interactable = true;
                if (enteredLocations.Contains(unlockedLocations[selectedLocation]))
                    preview.sprite = location.preview;
                else
                    preview.sprite = unknownLocation;
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

    public void ChangeLocation(bool isStart)
    {
        preview.sprite = null;
        currentLocation = selectedLocation;
        confirmMoveButton.interactable = false;

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

    private void SetCurrentLocationDisplay()
    {
        foreach (Transform location in locationDisplayParent.transform)
        {
            if (location.name == unlockedLocations[currentLocation].CurrentLocationState().locationName)
            {
                locationSprite = location.GetComponent<SpriteRenderer>();
                location.gameObject.SetActive(true);
            }
            else
                location.gameObject.SetActive(false);
        }
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
            talkButton.interactable = false;
        else
        {
            talkButton.interactable = unlockedLocations[currentLocation].CurrentLocationState().people.Length != 0;
            personSpawner.SpawnPeople(unlockedLocations[currentLocation].CurrentLocationState());
            personSpawner.SetButtonActive(false);
        }

        moveButton.interactable = unlockedLocations.Count > 1;
    }
}
