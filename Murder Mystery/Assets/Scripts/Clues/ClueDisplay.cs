using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClueDisplay : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject selector;
    [SerializeField] private int index;
    [SerializeField] private GainEvidence myEvidence;
    [SerializeField] private GainProfile myProfile;
    [SerializeField] private Clue.Type myType;

    public bool isSelected => selector.activeSelf;
    public int Index => index;

    public void SetSelector(bool isSelected)
    {
        selector.SetActive(isSelected);
    }

    public bool IsEmpty()
    {
        switch (myType)
        {
            case Clue.Type.Evidence:
                return myEvidence == null;
            case Clue.Type.Profile:
                return myProfile == null;
        }
        return true;
    }

    public void Click()
    {
        if (ClueDisplaySpawner.instance.isScrolling)
            return;

        if (ClueDisplaySpawner.instance.IsMainClueDisplay(index))
        {
            if ((myEvidence != null && myType == Clue.Type.Evidence) || (myProfile != null && myType == Clue.Type.Profile))
            {
                AudioManager.instance.PlaySFX("Clue Click");
                ClueManager.instance.SetCurrentClue(ClueManager.instance.currentMenuType, index);
                ClueDisplaySpawner.instance.ResetSelection(myType);
                SetSelector(true);
                return;
            }
        }
        else if ((ClueManager.instance.currentMenuType == Clue.Type.Evidence && ClueDisplaySpawner.instance.evidenceSectionAmount > 1)
            || (ClueManager.instance.currentMenuType == Clue.Type.Profile && ClueDisplaySpawner.instance.profileSectionAmount > 1))
        {
            AudioManager.instance.PlaySFX("Clue Slide");
            ClueDisplaySpawner.instance.ScrollSection(index, false);
        }
    }

    public void SetValues(int index, GainEvidence evidence, GainProfile profile, Clue.Type type)
    {
        this.index = index;
        myEvidence = evidence;
        myProfile = profile;
        myType = type;
    }

    public void UpdateShownClue(Clue.Type type)
    {
        switch (type)
        {
            case Clue.Type.Evidence:
                UpdateDisplay(myEvidence);
                break;
            case Clue.Type.Profile:
                UpdateDisplay(myProfile);
                break;
        }
    }

    private void UpdateDisplay(GainClue clue)
    {
        image.sprite = null;

        if (clue != null)
        {
            if (clue is GainEvidence)
                image.sprite = (clue as GainEvidence).gainedEvidence.versions[clue.version].sprite;
            else if (clue is GainProfile)
                image.sprite = (clue as GainProfile).gainedProfile.versions[clue.version].sprite;
            else
                Debug.LogError("Clue Type Not Found");
            image.enabled = true;
        }
        else
        {
            image.enabled = false;
        }
    }

    public IEnumerator MoveToPosition(bool moveForward, bool moveFast = false)
    {
        float moveTime = ClueDisplaySpawner.instance.scrollSpeed;
        if (moveFast)
            moveTime = 0.1f;

        float timeSinceLast = 0;
        float totalTime = 0;

        while (totalTime <= moveTime)
        {
            totalTime += Time.deltaTime;
            timeSinceLast = Time.deltaTime;
            if (totalTime > moveTime)
            {
                timeSinceLast -= totalTime - moveTime;
                totalTime = moveTime;
            }

            float movement = ClueDisplaySpawner.instance.MoveSectionDistance() * (timeSinceLast / moveTime);
            if (moveForward)
                movement *= -1;

            transform.localPosition += new Vector3(movement, 0);
            FixPosition();

            yield return null;
        }

        float widthAndDistance = (ClueDisplaySpawner.instance.SectionSize() / ClueDisplaySpawner.instance.displaysPerSection);
        int finalPos = (int)(Mathf.RoundToInt(transform.localPosition.x / widthAndDistance) * widthAndDistance);
        transform.localPosition = new Vector3(finalPos, 0);
    }

    public void FixPosition()
    {
        if (transform.localPosition.x > ClueDisplaySpawner.instance.MaxDispayDistance())
        {
            transform.localPosition -= new Vector3(ClueDisplaySpawner.instance.MaxDispayDistance() * 2, 0);
        }
        else if (transform.localPosition.x < -ClueDisplaySpawner.instance.MaxDispayDistance())
        {
            transform.localPosition += new Vector3(ClueDisplaySpawner.instance.MaxDispayDistance() * 2, 0);
        }
    }
}
