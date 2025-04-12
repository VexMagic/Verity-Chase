using System;
using System.Collections.Generic;
using UnityEngine;

public class ClueDisplaySpawner : MonoBehaviour
{
    public static ClueDisplaySpawner instance;

    public float scrollSpeed;
    [SerializeField] private GameObject displayPrefab;
    [SerializeField] private GameObject[] displayParents;
    [SerializeField] private float displayWidth;
    [SerializeField] private float selectionAreaWidth;
    [SerializeField] private int DisplaysPerSection;
    [SerializeField] private int MinimumSections;
    public int displaysPerSection => DisplaysPerSection;

    private List<GameObject> displayObjects = new List<GameObject>();
    private List<ClueDisplay> evidenceDisplays = new List<ClueDisplay>();
    private List<ClueDisplay> profileDisplays = new List<ClueDisplay>();

    private float displaySpacing;
    public int evidenceSectionAmount = 0;
    public int profileSectionAmount = 0;
    public int currentEvidenceSection = 0;
    public int currentProfileSection = 0;
    public bool isScrolling = false;

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
        displaySpacing = (selectionAreaWidth - (displayWidth * displaysPerSection)) / displaysPerSection;

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

    public ClueDisplay GetAdjacentDisplay(int index, Clue.Type type, bool left)
    {
        int tempIndex = index;
        ClueDisplay tempDisplay = null;

        switch (type)
        {
            case Clue.Type.Evidence:
                if (tempIndex == 0 && left)
                    tempIndex = evidenceDisplays.Count - 1;
                else if (tempIndex == evidenceDisplays.Count - 1 && !left)
                    tempIndex = 0;
                else if (left)
                    tempIndex--;
                else
                    tempIndex++;

                tempDisplay = evidenceDisplays[tempIndex];
                break;
            case Clue.Type.Profile:
                if (tempIndex == 0 && left)
                    tempIndex = profileDisplays.Count - 1;
                else if (tempIndex == profileDisplays.Count - 1 && !left)
                    tempIndex = 0;
                else if (left)
                    tempIndex--;
                else
                    tempIndex++;

                tempDisplay = profileDisplays[tempIndex];
                break;
        }

        if (tempDisplay == null)
            return null;
        else
        {
            if (tempDisplay.IsEmpty())
            {
                return GetAdjacentDisplay(tempIndex, type, left);
            }
            return tempDisplay;
        }
    }

    public int GetSelectedDisplayIndex(Clue.Type type)
    {
        switch (type)
        {
            case Clue.Type.Evidence:
                for (int i = 0; i < evidenceDisplays.Count; i++)
                {
                    if (evidenceDisplays[i].isSelected)
                    {
                        return i;
                    }
                }
                break;
            case Clue.Type.Profile:
                for (int i = 0; i < profileDisplays.Count; i++)
                {
                    if (profileDisplays[i].isSelected)
                    {
                        return i;
                    }
                }
                break;
        }
        return 0;
    }

    public void SetCurrentMenu(bool type)
    {
        for (int i = 0; i < displayParents.Length; i++)
        {
            displayParents[i].SetActive((i == 0) == type);
        }
    }

    public void ScrollSection(int index, bool specificDirection, bool scrollLeft = false)
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

            if ((IsScrollForward(index) && !specificDirection) || (!scrollLeft && specificDirection))
            {
                currentSection++;
                if (currentSection >= Mathf.Clamp(sectionAmount, MinimumSections, 100))
                    currentSection = 0;
                MoveDisplay(true);
            }
            else
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
        isScrolling = true;
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
        Invoke(nameof(StopScroll), scrollSpeed);
    }

    private void StopScroll()
    {
        isScrolling = false;
    }

    public float MaxDispayDistance()
    {
        return SectionSize() / 2 * Mathf.Clamp(evidenceSectionAmount, MinimumSections, 100);
    }

    public float SectionSize()
    {
        return selectionAreaWidth;
    }

    public float MoveSectionDistance()
    {
        return GetIndexPosition(displaysPerSection) - GetIndexPosition(0);
    }

    public float GetIndexPosition(int index)
    {
        float offset = ((float)displaysPerSection / 2) - 0.5f;
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
