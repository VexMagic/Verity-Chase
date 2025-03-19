using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ClueManager : MonoBehaviour
{
    public static ClueManager instance;

    [SerializeField] private GameObject clueMenu;
    [SerializeField] private GameObject[] swapButtons;
    [SerializeField] private GameObject presentButton;
    [SerializeField] private GameObject openButton;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject testimonyPresentButton;
    [SerializeField] private GameObject pressButton;
    [SerializeField] private GameObject exitMemoryButton;
    [SerializeField] private GameObject deduceMemoryButton;
    [SerializeField] private GameObject checkButton;
    [SerializeField] private Image background;
    [SerializeField] private Vector2 presentingOffset;

    [SerializeField] private ClueInfoDisplay menuInfoDisplay;
    [SerializeField] private ClueInfoDisplay gainInfoDisplay;

    //[SerializeField] private TextMeshProUGUI title;
    //[SerializeField] private TextMeshProUGUI description;
    //[SerializeField] private Image image;
    
    //[SerializeField] private TextMeshProUGUI gainTitle;
    //[SerializeField] private TextMeshProUGUI gainDescription;
    //[SerializeField] private Image gainImage;
    [SerializeField] private Animator gainClueAnimator;
    [SerializeField] private GameObject scrollArrows;

    public List<GainEvidence> allEvidence = new List<GainEvidence>();
    public List<GainProfile> allProfiles = new List<GainProfile>();

    public List<ChapterClues> chapters = new List<ChapterClues>();

    public int currentEvidence = 0;
    public int currentProfile = 0;

    public bool isOpen;
    public bool canGoBack;
    public bool isTestimonyPresenting;
    public bool isMemoryDeduction;
    public Clue.Type currentMenuType;
    public bool isEvidence;

    private bool isAnimationFinished;
    public bool IsAnimationFinished => isAnimationFinished;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            allEvidence = chapters[ChapterManager.instance.currentChapter].partClues[ChapterManager.instance.currentPart].allEvidence;
            allProfiles = chapters[ChapterManager.instance.currentChapter].partClues[ChapterManager.instance.currentPart].allProfiles;
            CloseMenu();
            UpdateDisplay(allEvidence[0]);
            scrollArrows.SetActive(false);
        }
    }

    public IEnumerator GainClue(GainClue clue)
    {
        bool isNew = false;
        bool isUpdated = false;
        bool exists = false;

        var tempData = (clue as GainAnyClueType).gainedClue;
        if (tempData is EvidenceData)
        {
            GainEvidence tempEvidence = new GainEvidence(tempData as EvidenceData, clue.version);

            foreach (var item in allEvidence)
            {
                if (item.gainedEvidence == tempEvidence.gainedEvidence)
                {
                    exists = true;
                    if (item.version < tempEvidence.version)
                    {
                        item.version = tempEvidence.version;
                        isUpdated = true;
                        break;
                    }
                }
            }

            if (!isUpdated && !exists)
            {
                allEvidence.Add(tempEvidence);
                isNew = true;
            }
        }
        else if (tempData is ProfileData)
        {
            GainProfile tempProfile = new GainProfile(tempData as ProfileData, clue.version);

            foreach (var item in allProfiles)
            {
                if (item.gainedProfile == tempProfile.gainedProfile)
                {
                    exists = true;
                    if (item.version < tempProfile.version)
                    {
                        item.version = tempProfile.version;
                        isUpdated = true;
                        break;
                    }
                }
            }

            if (!isUpdated && !exists)
            {
                allProfiles.Add(tempProfile);
                isNew = true;
            }
        }
        else if (tempData == null)
        {
            Debug.LogError("Clue is Null");
        }
        else
        {
            Debug.LogError("Unknown Clue Type");
        }

        if (isNew || isUpdated)
        {
            isAnimationFinished = false;
            ClueDisplaySpawner.instance.CreateClueDisplays();
            UpdateGainClueDisplay(clue);
        }

        if (!isAnimationFinished)
        {
            yield return new WaitUntil(() => isAnimationFinished);
            yield return new WaitForSeconds(0.4f); 
        }
    }

    public void CloseMenu()
    {
        isOpen = false;
        clueMenu.SetActive(false);
        foreach (var button in swapButtons)
        {
            button.SetActive(false);
        }

        openButton.SetActive(!isTestimonyPresenting && !isMemoryDeduction);
        testimonyPresentButton.SetActive(isTestimonyPresenting);
        pressButton.SetActive(isTestimonyPresenting);
        DialogueManager.instance.TestimonyArrowsActive(isTestimonyPresenting);

        deduceMemoryButton.SetActive(isMemoryDeduction);
        exitMemoryButton.SetActive(isMemoryDeduction && MemoryManager.instance.currentMemory.exitable);

        isTestimonyPresenting = false;
        isMemoryDeduction = false;

        closeButton.SetActive(false);
        presentButton.SetActive(false);
    }

    public void SetTestimony(bool isTestimony)
    {
        openButton.SetActive(!isTestimony);
        testimonyPresentButton.SetActive(isTestimony);
        pressButton.SetActive(isTestimony);
    }

    private void UpdateDisplay(GainClue clue)
    {
        if (clue != null)
        {
            menuInfoDisplay.SetValues(clue);
        }

        if (clue is GainEvidence)
        {
            checkButton.SetActive((clue as GainEvidence).gainedEvidence.versions[clue.version].hasCheck());
        }
        else
        {
            checkButton.SetActive(false);
        }
    }

    public void CheckClue()
    {
        if (currentMenuType != Clue.Type.Evidence)
            return;

        Evidence2 tempEvidence = (GetSelectedClue(Clue.Type.Evidence) as GainEvidence).gainedEvidence.versions[GetSelectedClue(Clue.Type.Evidence).version];
        if (tempEvidence.check.Count == 0)
            return;

        CheckEvidence evidence = tempEvidence.check[0];

        switch (evidence.checkType)
        {
            default:
                Debug.LogWarning("Check Type Doesn't exist");
                break;
            case CheckType.Text:
                break;
            case CheckType.Image:
                break;
            case CheckType.Cutscene:
                CutsceneManager.instance.StartCutscene(evidence.clip);
                break;
        }
    }

    private void UpdateGainClueDisplay(GainClue clue)
    {
        if (clue != null)
        {
            gainInfoDisplay.SetValues(clue);
            gainClueAnimator.SetBool("Open", true);
        }
    }

    public void CloseGainClueDisplay()
    {
        gainClueAnimator.SetBool("Open", false);
        isAnimationFinished = true;
    }

    public void ClueMenuButton()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            OpenMenu(false, true);
        }
        else
        {
            CloseMenu();
        }
    }

    public void TestimonyPresentButton()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            DialogueManager.instance.TestimonyArrowsActive(false);
            isTestimonyPresenting = true;
            OpenMenu(true, true);
        }
        else
        {
            CloseMenu();
        }
    }

    public void DeduceButton()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            exitMemoryButton.SetActive(false);
            isMemoryDeduction = true;
            OpenMenu(true, true);
        }
        else
        {
            CloseMenu();
        }
    }

    public void OpenMenu(bool isPresenting, bool canGoBack)
    {
        isOpen = true;
        this.canGoBack = canGoBack;
        clueMenu.SetActive(true);
        openButton.SetActive(false);
        testimonyPresentButton.SetActive(false);
        pressButton.SetActive(false);
        deduceMemoryButton.SetActive(false);
        exitMemoryButton.SetActive(false);
        SwapMenuButton(currentMenuType == Clue.Type.Evidence);
        closeButton.SetActive(canGoBack);

        if (isPresenting)
        {
            presentButton.SetActive(true);
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
            background.transform.localPosition = new Vector3(presentingOffset.x, presentingOffset.y);
        }
        else
        {
            presentButton.SetActive(false);
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.5f);
            background.transform.localPosition = Vector3.zero;
        }
    }

    public void SwapMenu()
    {
        SwapMenuButton(!isEvidence);
    }

    public void SwapMenuButton(bool type)
    {
        isEvidence = type;
        if (type)
        {
            currentMenuType = Clue.Type.Evidence;
            scrollArrows.SetActive(allEvidence.Count > 5);
        }
        else
        {
            currentMenuType = Clue.Type.Profile;
            scrollArrows.SetActive(allProfiles.Count > 5);
        }

        for (int i = 0; i < swapButtons.Length; i++)
        {
            swapButtons[i].SetActive((i == 0) == type);
        }

        UpdateDisplay(GetSelectedClue(currentMenuType));
        ClueDisplaySpawner.instance.SetCurrentMenu(type);
    }

    public void SetCurrentClue(Clue.Type clueType, int index)
    {
        switch (clueType)
        {
            case Clue.Type.Evidence:
                currentEvidence = index;
                break;
            case Clue.Type.Profile:
                currentProfile = index;
                break;
        }

        UpdateDisplay(GetSelectedClue(currentMenuType));
    }

    public GainClue GetSelectedClue(Clue.Type menuType)
    {
        int selectedIndex = 0;
        switch (menuType)
        {
            case Clue.Type.Evidence:
                selectedIndex = currentEvidence;
                break;
            case Clue.Type.Profile:
                selectedIndex = currentProfile;
                break;
        }

        int tempIndex = 0;

        foreach (GainClue clue in GetClueList(menuType))
        {
            if (tempIndex == selectedIndex)
            {
                return clue;
            }
            else
            {
                tempIndex++;
            }
        }

        return null;
    }
    public List<GainClue> GetClueList(Clue.Type type)
    {
        switch (type)
        {
            case Clue.Type.Evidence:
                return allEvidence.Cast<GainClue>().ToList();
            case Clue.Type.Profile:
                return allProfiles.Cast<GainClue>().ToList();
        }
        return null;
    }
}
