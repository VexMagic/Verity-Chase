using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    [SerializeField] private GameObject characterDisplay;
    [SerializeField] private RectTransform basePosition;
    [SerializeField] private RectTransform nameArea;
    [SerializeField] private float moveSpeed;

    [SerializeField] private List<CharacterData> characterDatas = new List<CharacterData>();

    private List<GameObject> characters = new List<GameObject>();
    private List<CharacterDisplay> characterDisplays = new List<CharacterDisplay>();

    public const float YOffset = -540;

    private Coroutine coroutine;

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

    private int GetLowestFreeID()
    {
        int id = 0;
        bool isFree = true;
        while (true)
        {
            isFree = true;
            foreach (var item in characterDisplays)
            {
                if (item.iD == id)
                {
                    isFree = false;
                }
            }

            if (isFree)
            {
                return id;
            }
            else 
            {
                id++;
            }
        }
    }

    public void CreateEditorCharacter()
    {
        GameObject tempDisplay = Instantiate(characterDisplay, basePosition);
        CharacterDisplay displayData = tempDisplay.GetComponent<CharacterDisplay>();

        tempDisplay.transform.localPosition = new Vector3(0, 0);

        int id = GetLowestFreeID();
        displayData.SetID(id);
        DialogueManager.instance.currentLine.movements.Add(new CharacterMovement(id));

        characters.Add(tempDisplay);
        characterDisplays.Add(displayData);

        EditCharacterManager.instance.SelectCharacter(displayData);
    }

    public void CreateCharacter(CharacterMovement values, bool instant)
    {
        if (Mathf.Abs(values.xPos) >= 1500)
            return;

        GameObject tempDisplay = Instantiate(characterDisplay, basePosition);
        CharacterDisplay displayData = tempDisplay.GetComponent<CharacterDisplay>();

        if (instant)
            tempDisplay.transform.localPosition = new Vector3(values.xPos, 0);
        else if (values.facingLeft)
            tempDisplay.transform.localPosition = new Vector3(1500, YOffset);
        else
            tempDisplay.transform.localPosition = new Vector3(-1500, YOffset);

        displayData.SetID(values.displayID);
        displayData.SetValues(values, true, instant);

        characters.Add(tempDisplay);
        characterDisplays.Add(displayData);
    }

    public void RemoveCharacter(CharacterDisplay display)
    {
        characters.Remove(display.gameObject);
        characterDisplays.Remove(display);
        Destroy(display.gameObject);
    }

    public void SetSpeaking(Character speaker)
    {
        bool speakerOnScreen = false;
        foreach (var character in characterDisplays)
        {
            if (GetCharacterController(speaker) == character.Animator.runtimeAnimatorController)
            {
                character.SetSpeaking(true);
                speakerOnScreen = true;
            }
            else
                character.SetSpeaking(false);
        }

        if (!speakerOnScreen)
            SetNameAreaPos(0, false);
    }

    public void SetNameAreaPos(float characterPos, bool moving)
    {
        float offset = nameArea.sizeDelta.x / 2;

        if (moving && coroutine != null)
            return;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        if (DialogueManager.instance.moveNameBoxInstantly)
            nameArea.localPosition = new Vector3(characterPos - offset - (CameraManager.instance.cameraOffset / 2), nameArea.localPosition.y);
        else
            coroutine = StartCoroutine(SmoothMovement(nameArea.localPosition.x, characterPos - offset - (CameraManager.instance.cameraOffset / 2)));
    }

    private IEnumerator SmoothMovement(float start, float end)
    {
        bool active = true;
        float sinTime = 0;
        while (active)
        {
            sinTime += Time.deltaTime * moveSpeed;
            if (sinTime > Mathf.PI)
                active = false;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            nameArea.localPosition = Vector3.Lerp(new Vector3(start, nameArea.localPosition.y), new Vector3(end, nameArea.localPosition.y), t);
            yield return null;
        }
        coroutine = null;
    }

    float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
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

    public int GetDisplayAmount()
    {
        return characterDisplays.Count;
    }

    public CharacterDisplay GetDisplayFromIndex(int index)
    {
        return characterDisplays[index];
    }

    public CharacterData GetCharacterData(RuntimeAnimatorController controller)
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

    public void MoveCharacter(CharacterMovement values, bool instant = false)
    {
        foreach (var display in characterDisplays)
        {
            if (display.iD == values.displayID)
            {
                display.SetValues(values, false, instant);
                return;
            }
        }

        CreateCharacter(values, instant);
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
