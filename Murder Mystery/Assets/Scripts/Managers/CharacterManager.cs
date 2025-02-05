using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    [SerializeField] private GameObject characterDisplay;
    [SerializeField] private RectTransform basePosition;

    [SerializeField] private List<CharacterData> characterDatas = new List<CharacterData>();

    private List<GameObject> characters = new List<GameObject>();
    private List<CharacterDisplay> characterDisplays = new List<CharacterDisplay>();

    public const float YOffset = -540;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void DeleteCharacters()
    {
        foreach (var character in characters)
        {
            Destroy(character);
        }
        characters.Clear();
        characterDisplays.Clear();

        foreach (var item in characterDatas)
        {
            item.HasBeenUsed = false;
        }
    }

    public void CreateCharacter(CharacterMovement values)
    {
        GameObject tempDisplay = Instantiate(characterDisplay, basePosition);
        CharacterDisplay displayData = tempDisplay.GetComponent<CharacterDisplay>();

        if (values.facingLeft)
            tempDisplay.transform.localPosition = new Vector3(1310, YOffset);
        else
            tempDisplay.transform.localPosition = new Vector3(-1310, YOffset);

        displayData.SetID(values.displayID);
        displayData.SetValues(values, true);

        characters.Add(tempDisplay);
        characterDisplays.Add(displayData);
    }

    public CharacterDisplay GetCharacterDisplay(RuntimeAnimatorController controller)
    {
        foreach (var item in characterDisplays)
        {
            if (item.Animator.runtimeAnimatorController == controller)
            {
                return item;
            }
        }

        return null;
    }

    public RuntimeAnimatorController GetCharacterController(Character character)
    {
        if (character == Character.UnknownMale || character == Character.UnknownFemale)
        {
            foreach (var item in characterDisplays)
            {
                CharacterData tempData = GetCharacterData(item.Animator.runtimeAnimatorController);
                if (!tempData.HasBeenUsed)
                {
                    if ((tempData.IsMale && character == Character.UnknownMale) || (!tempData.IsMale && character == Character.UnknownFemale))
                    {
                        return tempData.Controller;
                    }
                }
            }
        }
        else
        {
            foreach (var item in characterDatas)
            {
                if (item.Character == character)
                {
                    item.HasBeenUsed = true;
                    return item.Controller;
                }
            }
        }

        return null;
    }

    private CharacterData GetCharacterData(RuntimeAnimatorController controller)
    {
        foreach (var item in characterDatas)
        {
            if (item.Controller == controller)
            {
                return item;
            }
        }
        return null;
    }

    public void MoveCharacter(CharacterMovement values)
    {
        foreach (var display in characterDisplays)
        {
            if (display.iD == values.displayID)
            {
                display.SetValues(values, false);
                return;
            }
        }

        CreateCharacter(values);
    }

    public bool GetAnimationDelayActive()
    {
        foreach (var display in characterDisplays)
        {
            if (display.delayDialogue)
            {
                return true;
            }
        }

        return false;
    }
}
