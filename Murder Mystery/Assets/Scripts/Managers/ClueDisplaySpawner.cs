using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueDisplaySpawner : MonoBehaviour
{
    public static ClueDisplaySpawner instance;

    public float scrollSpeed;
    [SerializeField] private GameObject displayPrefab;
    [SerializeField] private GameObject[] displayParents;
    [SerializeField] private float displayWidth;
    [SerializeField] private float displaySpacing;
    [SerializeField] private int DisplaysPerSection;
    [SerializeField] private int MinimumSections;
    public int displaysPerSection => DisplaysPerSection;

    private List<GameObject> displayObjects = new List<GameObject>();
    private List<ClueDisplay> evidenceDisplays = new List<ClueDisplay>();
    private List<ClueDisplay> profileDisplays = new List<ClueDisplay>();

    public int evidenceSectionAmount = 0;
    public int profileSectionAmount = 0;
    public int currentEvidenceSection = 0;
    public int currentProfileSection = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        CreateClueDisplays();
        SetCurrentMenu(ClueManager.instance.currentMenuType == Clue.Type.Evidence);
    }

    public void CreateClueDisplays()
    {
        foreach (var item in displayObjects)
        {
            Destroy(item);
        }
        displayObjects.Clear();
        evidenceDisplays.Clear();
        profileDisplays.Clear();

        int evidenceSections = Mathf.FloorToInt(((float)ClueManager.instance.allEvidence.Count / displaysPerSection) + 0.9f);
        int profileSections = Mathf.FloorToInt(((float)ClueManager.instance.allProfiles.Count / displaysPerSection) + 0.9f);

        evidenceSectionAmount = evidenceSections;
        profileSectionAmount = profileSections;

        evidenceSections = Mathf.Clamp(evidenceSections, MinimumSections, 100);
        profileSections = Mathf.Clamp(evidenceSections, MinimumSections, 100);

        for (int i = 0; i < evidenceSections * displaysPerSection; i++)
        {
            CreateDisplay(displayParents[0].transform, i, Clue.Type.Evidence);
        }

        for (int i = 0; i < profileSections * displaysPerSection; i++)
        {
            CreateDisplay(displayParents[1].transform, i, Clue.Type.Profile);
        }

        for (int i = 0; i < currentEvidenceSection; i++)
        {

            foreach (var item in evidenceDisplays)
            {
                StartCoroutine(item.MoveToPosition(true, true));
            }
        }

        for (int i = 0; i < currentProfileSection; i++)
        {
            foreach (var item in profileDisplays)
            {
                StartCoroutine(item.MoveToPosition(true, true));
            }
        }
    }

    private void CreateDisplay(Transform parent, int index, Clue.Type type)
    {
        GameObject displayObject = Instantiate(displayPrefab, parent);
        ClueDisplay clueDisplay = displayObject.GetComponent<ClueDisplay>();

        displayObject.transform.localPosition = new Vector3(GetIndexPosition(index), displayObject.transform.localPosition.y);
        clueDisplay.FixPosition();

        switch (type)
        {
            case Clue.Type.Evidence:
                clueDisplay.SetSelector(index == ClueManager.instance.currentEvidence);
                break;
            case Clue.Type.Profile:
                clueDisplay.SetSelector(index == ClueManager.instance.currentProfile);
                break;
        }

        GainEvidence tempEvidence = null;
        GainProfile tempProfile = null;

        if (index < ClueManager.instance.allEvidence.Count)
            tempEvidence = ClueManager.instance.allEvidence[index];

        if (index < ClueManager.instance.allProfiles.Count)
            tempProfile = ClueManager.instance.allProfiles[index];

        clueDisplay.SetValues(index, tempEvidence, tempProfile, type);
        clueDisplay.UpdateShownClue(type);

        displayObjects.Add(displayObject);
        switch (type)
        {
            case Clue.Type.Evidence:
                evidenceDisplays.Add(clueDisplay);
                break;
            case Clue.Type.Profile:
                profileDisplays.Add(clueDisplay);
                break;
        }
    }

    public void SetCurrentMenu(bool type)
    {
        for (int i = 0; i < displayParents.Length; i++)
        {
            displayParents[i].SetActive((i == 0) == type);
        }
    }

    public void ScrollSection(int index)
    {
        int sectionAmount = 0;
        switch (ClueManager.instance.currentMenuType)
        {
            case Clue.Type.Evidence:
                sectionAmount = evidenceSectionAmount;
                break;
            case Clue.Type.Profile:
                sectionAmount = profileSectionAmount;
                break;
        }

        if (sectionAmount > 1)
        {
            int currentSection = CurrentSection();

            if (IsScrollForward(index))
            {
                currentSection++;
                if (currentSection >= Mathf.Clamp(sectionAmount, MinimumSections, 100))
                    currentSection = 0;
                MoveDisplay(true);
            }
            else if (IsScrollBackward(index))
            {
                currentSection--;
                if (currentSection < 0)
                    currentSection = Mathf.Clamp(sectionAmount, MinimumSections, 100) - 1;
                MoveDisplay(false);
            }

            switch (ClueManager.instance.currentMenuType)
            {
                case Clue.Type.Evidence:
                    currentEvidenceSection = currentSection;
                    break;
                case Clue.Type.Profile:
                    currentProfileSection = currentSection;
                    break;
            }
        }
    }

    private void MoveDisplay(bool moveForward)
    {
        //StartCoroutine(MoveToPosition(moveForward, ClueManager.instance.currentMenuType));
        switch (ClueManager.instance.currentMenuType)
        {
            case Clue.Type.Evidence:
                foreach (var item in evidenceDisplays)
                {
                    StartCoroutine(item.MoveToPosition(moveForward));
                }
                break;
            case Clue.Type.Profile:
                foreach (var item in profileDisplays)
                {
                    StartCoroutine(item.MoveToPosition(moveForward));
                }
                break;
        }
    }

    public float MaxDispayDistance()
    {
        return SectionSize() / 2 * Mathf.Clamp(evidenceSectionAmount, MinimumSections, 100);
    }

    public float SectionSize()
    {
        return (displayWidth + displaySpacing) * DisplaysPerSection;
    }

    public float GetIndexPosition(int index)
    {
        int offset = Mathf.FloorToInt((float)displaysPerSection / 2);

        return (displayWidth + displaySpacing) * (index - offset);
    }

    public bool IsMainClueDisplay(int index)
    {
        int indexStart = displaysPerSection * CurrentSection();
        bool returnValue = indexStart <= index && indexStart + displaysPerSection > index;
        return returnValue;
    }

    public bool IsScrollForward(int index)
    {
        int indexStart = displaysPerSection * CurrentSection();
        bool returnValue = indexStart + displaysPerSection == index;
        if (CurrentSection() == Mathf.Clamp(evidenceSectionAmount, MinimumSections, 100) - 1)
            returnValue = 0 == index;

        return returnValue;
    }

    public bool IsScrollBackward(int index)
    {
        int indexStart = displaysPerSection * CurrentSection();
        bool returnValue = indexStart - 1 == index;
        if (CurrentSection() == 0)
        {
            returnValue = DisplaysPerSection * Mathf.Clamp(evidenceSectionAmount, MinimumSections, 100) - 1 == index;
        }

        return returnValue;
    }


    public void ResetSelection(Clue.Type type)
    {
        switch (type)
        {
            case Clue.Type.Evidence:
                foreach (var item in evidenceDisplays)
                {
                    item.SetSelector(false);
                }
                break;
            case Clue.Type.Profile:
                foreach (var item in profileDisplays)
                {
                    item.SetSelector(false);
                }
                break;
        }
    }

    private int CurrentSection()
    {
        switch (ClueManager.instance.currentMenuType)
        {
            case Clue.Type.Evidence:
                return currentEvidenceSection;
            case Clue.Type.Profile:
                return currentProfileSection;
        }
        return 0;
    }
}
