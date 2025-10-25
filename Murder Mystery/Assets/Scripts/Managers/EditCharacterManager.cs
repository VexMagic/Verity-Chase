using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class EditCharacterManager : MonoBehaviour
{
    public static EditCharacterManager instance;

    [SerializeField] private GameObject startEditorButton;
    [SerializeField] private GameObject editorHUD;
    [SerializeField] private GameObject characterEditor;
    [SerializeField] private GameObject characterSelector;
    [SerializeField] private GameObject movementDeleter;
    [SerializeField] private Transform movementDisplayArea;
    [SerializeField] private TMP_Dropdown characterSelect;
    [SerializeField] private TMP_Dropdown spriteSelect;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private GameObject selectedDisplayIndicator;
    [SerializeField] private GameObject movementDisplay;

    [SerializeField] private Sprite[] characterProfiles;

    private List<GameObject> movementDisplays = new List<GameObject>();
    private CharacterDisplay selectedDisplay;
    private bool selectorActive;
    private int selectorDisplayIndex;

    public bool hasMadeChanges = false;

    private void Awake()
    {
        instance = this;

        editorHUD.SetActive(false);

#if (!UNITY_EDITOR)
        startEditorButton.SetActive(false);
#endif

#if (UNITY_EDITOR)
        startEditorButton.SetActive(true);
        Setup();
#endif
    }

    private void Setup()
    {
        characterSelect.ClearOptions();
        List<TMP_Dropdown.OptionData> tempCharacters = new List<TMP_Dropdown.OptionData>();
        for (int i = 2; i < Enum.GetNames(typeof(Character)).Length - 1; i++)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            optionData.text = ((Character)i).ToString();
            optionData.image = GetCharacterIcon((Character)i);
            tempCharacters.Add(optionData);
        }
        characterSelect.AddOptions(tempCharacters);
        DeselectCharacter();
    }

    public Sprite GetCharacterIcon(Character character)
    {
        return characterProfiles[(int)character - 2];
    }

    public void SetDisplayCharacter()
    {
        if (selectedDisplay == null)
            return;

        SetSpriteOptions();
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        RuntimeAnimatorController animator = GetCurrentController();
        int spriteIndex = spriteSelect.value;

        selectedDisplay.SetCharacter(animator, animator.animationClips[spriteIndex]);

        CharacterMovement movement = DialogueManager.instance.currentLine.GetIDMovement(selectedDisplay.iD);
        movement.SetCharacter(animator);
        movement.SetAnimation(animator.animationClips[spriteIndex]);
        SetSOPosition();
    }

    private void SetSpriteOptions()
    {
        spriteSelect.ClearOptions();
        List<string> tempSprites = new List<string>();
        foreach (var item in GetCurrentController().animationClips)
        {
            tempSprites.Add(GetSpriteName(item));
        }
        spriteSelect.AddOptions(tempSprites);
        spriteSelect.value = 0;
    }

    public string GetSpriteName(AnimationClip animation)
    {
        string[] parts = animation.ToString().Split(' ');

        string spriteName = string.Empty;
        for (int i = 1; i < parts.Length - 1; i++)
        {
            spriteName += parts[i];
            if (i < parts.Length - 2)
                spriteName += " ";
        }

        return spriteName;
    }

    private Character GetCurrentCharacter()
    {
        return (Character)(characterSelect.value + 2);
    }

    private RuntimeAnimatorController GetCurrentController()
    {
        return CharacterManager.instance.GetCharacterController(GetCurrentCharacter());
    }

    public void AddCharacter()
    {
        CharacterManager.instance.CreateEditorCharacter();
        SetDisplayCharacter();
        CloseDeleteMenu();
    }

    public void StartSelector()
    {
        selectorActive = true;
        characterSelector.SetActive(true);
        if (characterEditor.activeSelf)
        {
            characterEditor.SetActive(false);
        }
        else
        {
            selectorDisplayIndex = 0;
        }

        SelectCharacter(CharacterManager.instance.GetDisplayFromIndex(selectorDisplayIndex));
        UpdateSelectedCharacter();
        CloseDeleteMenu();
    }

    public void EndSelector()
    {
        selectorActive = false;
        characterSelector.SetActive(false);
        characterEditor.SetActive(true);
        //SetSpriteOptions();
    }

    public void NextSelectorDisplay()
    {
        selectorDisplayIndex++;
        if (selectorDisplayIndex >= CharacterManager.instance.GetDisplayAmount())
            selectorDisplayIndex = 0;

        UpdateSelectedCharacter();
    }

    public void PreviousSelectorDisplay()
    {
        selectorDisplayIndex--;
        if (selectorDisplayIndex < 0)
            selectorDisplayIndex = CharacterManager.instance.GetDisplayAmount() - 1;

        UpdateSelectedCharacter();
    }

    private void UpdateSelectedCharacter()
    {
        SelectCharacter(CharacterManager.instance.GetDisplayFromIndex(selectorDisplayIndex));

        RuntimeAnimatorController animator = selectedDisplay.Animator.runtimeAnimatorController;
        characterSelect.SetValueWithoutNotify(((int)CharacterManager.instance.GetCharacterData(animator).Character) - 2);
        SetSpriteOptions();

        for (int i = 0; i < animator.animationClips.Length; i++)
        {
            foreach (var item in selectedDisplay.Animator.GetCurrentAnimatorClipInfo(0))
            {
                if (animator.animationClips[i] == item.clip)
                {
                    spriteSelect.SetValueWithoutNotify(i);
                    break;
                }
            }
        }
    }

    public void DeselectCharacter()
    {
        characterEditor.SetActive(false);
        characterSelector.SetActive(false);
        selectedDisplay = null;
        characterSelect.value = 0;
        selectedDisplayIndicator.SetActive(false);
        selectorActive = false;
    }

    public void SelectCharacter(CharacterDisplay display)
    {
        selectedDisplay = display;

        if (selectedDisplay != null)
        {
            if (!selectorActive)
            {
                characterEditor.SetActive(true);
            }

            selectedDisplayIndicator.SetActive(true);
            UpdatePosition();
        }
        else
        {
            characterEditor.SetActive(false);
            characterSelector.SetActive(false);
            selectorActive = false;
        }
    }

    private void UpdatePosition()
    {
        positionText.text = selectedDisplay.transform.localPosition.x.ToString();
        selectedDisplayIndicator.transform.localPosition = new Vector3(selectedDisplay.transform.localPosition.x, selectedDisplayIndicator.transform.localPosition.y);

        if (!selectorActive)
        {
            SetSOPosition();
        }
    }

    private void SetSOPosition()
    {
        CharacterMovement movement = DialogueManager.instance.currentLine.GetIDMovement(selectedDisplay.iD);
        movement.SetPosition((int)selectedDisplay.transform.localPosition.x);
        movement.SetFacingLeft(selectedDisplay.transform.localScale.x == 1);
        hasMadeChanges = true;
        UpdateDisplay();
    }

    public void MoveDisplayLeft()
    {
        selectedDisplay.MovePosition(-50);
        UpdatePosition();
    }

    public void MoveDisplayRight()
    {
        selectedDisplay.MovePosition(50);
        UpdatePosition();
    }

    public void ExitDisplayLeft()
    {
        selectedDisplay.SetPosition(-1500);
        UpdatePosition();

        CharacterManager.instance.RemoveCharacter(selectedDisplay);
        DeselectCharacter();
    }

    public void ExitDisplayRight()
    {
        selectedDisplay.SetPosition(1500);
        UpdatePosition();

        CharacterManager.instance.RemoveCharacter(selectedDisplay);
        DeselectCharacter();
    }

    public void FlipDisplay()
    {
        if (selectedDisplay.transform.localScale.x > 0)
        {
            selectedDisplay.transform.localScale = new Vector3(-1, 1);
        }
        else
        {
            selectedDisplay.transform.localScale = new Vector3(1, 1);
        }

        SetSOPosition();
    }

    public void DeleteMovement()
    {
        movementDeleter.SetActive(true);
        UpdateDeleteMenu();
    }

    public void UpdateDeleteMenu()
    {
        if (!movementDeleter.activeSelf)
        {
            return;
        }

        foreach (var item in movementDisplays)
        {
            Destroy(item);
        }
        movementDisplays.Clear();

        foreach (var item in DialogueManager.instance.currentLine.movements)
        {
            GameObject tempDisplay = Instantiate(movementDisplay, movementDisplayArea);
            tempDisplay.GetComponent<MovementDisplay>().SetValues(item);
            movementDisplays.Add(tempDisplay);
        }

        UpdateMovementOrderDisplays();
    }

    public void UpdateMovementOrderDisplays()
    {
        foreach (var item in movementDisplays)
        {
            item.GetComponent<MovementDisplay>().UpdateOrderButtons();
        }
    }

    public void SwapMovementPositions(int firstIndex, int secondIndex)
    {
        CharacterMovement tempMovement = DialogueManager.instance.currentLine.movements[firstIndex];
        DialogueManager.instance.currentLine.movements[firstIndex] = DialogueManager.instance.currentLine.movements[secondIndex];
        DialogueManager.instance.currentLine.movements[secondIndex] = tempMovement;
        UpdateMovementOrderDisplays();
    }

    public void CloseDeleteMenu()
    {
        movementDeleter.SetActive(false);
    }

    public void DeleteMovementDisplay(GameObject display)
    {
        movementDisplays.Remove(display);
        Destroy(display);
    }
}
