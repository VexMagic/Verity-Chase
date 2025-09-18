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
    [SerializeField] private KeyDisplay hintButton;
    [SerializeField] private GameObject noPresentDisplay;
    [SerializeField] private GameObject openButton;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject testimonyPresentButton;
    [SerializeField] private GameObject pressButton;
    [SerializeField] private GameObject exitMemoryButton;
    [SerializeField] private GameObject deduceMemoryButton;
    [SerializeField] private GameObject connectConspiracyButton;
    [SerializeField] private Image background;
    [SerializeField] private GameObject checkTextScreen;
    [SerializeField] private TextMeshProUGUI checkTextBox;
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
    [SerializeField] private Animator updateAnimator;
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
    public bool isConspiracyConnection;
    public Clue.Type currentMenuType;
    public bool isEvidence;
    public bool isChecking;
    private bool isUpdating;

    private bool isAnimationFinished = true;
    private bool presentActive;
    public bool IsAnimationFinished => isAnimationFinished;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            allEvidence = chapters[ChapterManager.instance.currentChapter - 1].partClues[ChapterManager.instance.currentPart - 1].allEvidence;
            allProfiles = chapters[ChapterManager.instance.currentChapter - 1].partClues[ChapterManager.instance.currentPart - 1].allProfiles;
            CloseMenu();
            UpdateDisplay(allEvidence[0], false);
            scrollArrows.SetActive(false);
            checkTextScreen.SetActive(false);
            noPresentDisplay.SetActive(false);
        }
    }

    public IEnumerator GainClue(GainClue clue)
    {
        bool isNew = false;
        bool isUpdated = false;
        bool exists = false;

        var tempData = (clue as GainAnyClueType).gainedClue;
        int oldVersion = 0;
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
                        oldVersion = item.version;
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
                        oldVersion = item.version;
                        item.version = tempProfile.version;
                        isUpdated = true;
                        Debug.Log("update");
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
            if (isNew)
                SetGainClueDisplay(clue);
            else
                SetUpdateClueDisplay(clue, oldVersion);
        }

        if (!isAnimationFinished)
        {
            yield return new WaitUntil(() => isAnimationFinished);
            yield return new WaitForSeconds(0.4f);
        }
    }

    public int GetClueIndex(GainAnyClueType clue)
    {
        var tempData = clue.gainedClue;
        if (tempData is EvidenceData)
        {
            GainEvidence tempEvidence = new GainEvidence(tempData as EvidenceData, clue.version);

            for (int i = 0; i < allEvidence.Count; i++)
            {
                if (allEvidence[i].gainedEvidence == tempEvidence.gainedEvidence)
                {
                    return i;
                }
            }
        }
        else if (tempData is ProfileData)
        {
            GainProfile tempProfile = new GainProfile(tempData as ProfileData, clue.version);

            for (int i = 0; i < allProfiles.Count; i++)
            {
                if (allProfiles[i].gainedProfile == tempProfile.gainedProfile)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public GainClue GetStoredClue(GainAnyClueType clue)
    {
        var tempData = clue.gainedClue;
        if (tempData is EvidenceData)
        {
            GainEvidence tempEvidence = new GainEvidence(tempData as EvidenceData, clue.version);

            for (int i = 0; i < allEvidence.Count; i++)
            {
                if (allEvidence[i].gainedEvidence == tempEvidence.gainedEvidence)
                {
                    return allEvidence[i];
                }
            }
        }
        else if (tempData is ProfileData)
        {
            GainProfile tempProfile = new GainProfile(tempData as ProfileData, clue.version);

            for (int i = 0; i < allProfiles.Count; i++)
            {
                if (allProfiles[i].gainedProfile == tempProfile.gainedProfile)
                {
                    return allProfiles[i];
                }
            }
        }
        return null;
    }

    public void ShowHintButton(bool show)
    {
        hintButton.SetActive(show);
    }

    public void ShowNoPresentDisplay(bool show)
    {
        noPresentDisplay.SetActive(show);
    }

    public void CloseMenu()
    {
        if (isChecking)
        {
            checkTextScreen.SetActive(false);
            isChecking = false;
            SwapMenuButton(isEvidence);
            closeButton.SetActive(canGoBack);
            return;
        }

        isOpen = false;
        clueMenu.SetActive(false);
        foreach (var button in swapButtons)
        {
            button.SetActive(false);
        }

        openButton.SetActive(!isTestimonyPresenting && !isMemoryDeduction && !isConspiracyConnection);
        testimonyPresentButton.SetActive(isTestimonyPresenting);
        pressButton.SetActive(isTestimonyPresenting);
        DialogueManager.instance.TestimonyArrowsActive(isTestimonyPresenting);

        deduceMemoryButton.SetActive(isMemoryDeduction);
        exitMemoryButton.SetActive(isMemoryDeduction && MemoryManager.instance.currentMemory.exitable);

        connectConspiracyButton.SetActive(isConspiracyConnection);
        exitMemoryButton.SetActive(isConspiracyConnection && ConspiracyManager.instance.currentBoard.exitable);

        isTestimonyPresenting = false;
        isMemoryDeduction = false;
        isConspiracyConnection = false;

        closeButton.SetActive(false);
        presentButton.SetActive(false);
        hintButton.SetActive(false);
        presentActive = false;
    }

    public void SetTestimony(bool isTestimony)
    {
        openButton.SetActive(!isTestimony);
        testimonyPresentButton.SetActive(isTestimony);
        pressButton.SetActive(isTestimony);
    }

    public void UpdateDisplay(GainClue clue, bool locked)
    {
        if (clue != null)
        {
            menuInfoDisplay.SetValues(clue, locked);
            if (presentActive) 
                presentButton.SetActive(!locked);
        }
    }

    public void CheckClue()
    {
        if (currentMenuType != Clue.Type.Evidence)
            return;

        Evidence tempEvidence = (GetSelectedClue(Clue.Type.Evidence) as GainEvidence).gainedEvidence.versions[GetSelectedClue(Clue.Type.Evidence).version];
        if (tempEvidence.check.Count == 0)
            return;

        CheckEvidence evidence = tempEvidence.check[0];

        OpenCheckMenu(evidence);
    }

    public void OpenCheckMenu(CheckEvidence evidence)
    {
        switch (evidence.checkType)
        {
            default:
                Debug.LogError("Check Type Doesn't exist");
                break;
            case CheckType.Text:
                SetTextCheckValues(evidence);
                break;
            case CheckType.Image:
                break;
            case CheckType.Cutscene:
                CutsceneManager.instance.StartCutscene(evidence.clip);
                break;
        }
    }

    private void SetTextCheckValues(CheckEvidence evidence)
    {
        checkTextScreen.SetActive(true);
        checkTextBox.text = evidence.description;
        isChecking = true;
        closeButton.SetActive(true);
        foreach (var button in swapButtons)
        {
            button.SetActive(false);
        }
    }

    private void SetGainClueDisplay(GainClue clue)
    {
        if (clue != null)
        {
            gainInfoDisplay.SetValues(clue, false);
            gainClueAnimator.SetBool("Open", true);
        }
    }

    private void SetUpdateClueDisplay(GainClue newClue, int oldVersion)
    {
        Debug.Log("update");
        if (newClue != null)
        {
            gainInfoDisplay.SetValues(newClue, oldVersion, false);
            gainClueAnimator.SetBool("Open", true);
            StartCoroutine(UpdateAnimation(newClue));
        }
    }

    private IEnumerator UpdateAnimation(GainClue newClue)
    {
        isUpdating = true;
        yield return new WaitForSeconds(0.7f);
        updateAnimator.SetBool("Active", true);
        yield return new WaitForSeconds(0.25f);
        gainInfoDisplay.SetValues(newClue, false);
        updateAnimator.SetBool("Active", false);
        yield return new WaitForSeconds(0.1f);
        isUpdating = false;
    }

    public void CloseGainClueDisplay()
    {
        if (isUpdating)
            return;

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

    public void ConnectButton()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            exitMemoryButton.SetActive(false);
            isConspiracyConnection = true;
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
        connectConspiracyButton.SetActive(false);
        SwapMenuButton(currentMenuType == Clue.Type.Evidence);
        closeButton.SetActive(canGoBack);

        if (isPresenting)
        {
            presentButton.SetActive(true);
            hintButton.SetActive(true);
            presentActive = true;
            if (isConspiracyConnection)
            {
                background.color = new Color(background.color.r, background.color.g, background.color.b, 0.5f);
                background.transform.localPosition = Vector3.zero;
            }
            else
            {
                background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
                background.transform.localPosition = new Vector3(presentingOffset.x, presentingOffset.y);
            }
        }
        else
        {
            presentButton.SetActive(false);
            hintButton.SetActive(false);
            presentActive = false;
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
            scrollArrows.SetActive(allEvidence.Count > ClueDisplaySpawner.instance.displaysPerSection);
        }
        else
        {
            currentMenuType = Clue.Type.Profile;
            scrollArrows.SetActive(allProfiles.Count > ClueDisplaySpawner.instance.displaysPerSection);
        }

        for (int i = 0; i < swapButtons.Length; i++)
        {
            swapButtons[i].SetActive((i == 0) == type);
        }

        UpdateDisplay(GetSelectedClue(currentMenuType), LockedClueSelected(currentMenuType));
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

        UpdateDisplay(GetSelectedClue(currentMenuType), LockedClueSelected(currentMenuType));
    }

    public bool LockedClueSelected(Clue.Type menuType)
    {
        return ClueDisplaySpawner.instance.GetLock(GetSelectedClue(menuType));
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
