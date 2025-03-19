using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
    public static ControlManager instance;

    [SerializeField] private InputActionReference navigateButton;
    [SerializeField] private InputActionReference progressButton;

    [SerializeField] private UIButton selectedButton;
    [SerializeField] private GameObject examineAim;
    [SerializeField] private GameObject UI;

    enum State { Normal, Examine, Deduction, ClueMenu, Testimony, Settings }

    private State currentState = State.Normal;
    private State previousState = State.Normal;

    private Vector2 movement;

    public Vector2 Movement => movement;
    public UIButton SelectedButton => selectedButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (selectedButton != null)
            selectedButton.SetOutline(true);
    }

    public bool SetSelectedButton(UIButton button, bool isKeyboard = false)
    {
        if (button == null)
            return false;

        if (selectedButton == button)
            return false;

        if (selectedButton != null)
            selectedButton.SetOutline(false, isKeyboard);

        selectedButton = button;
        selectedButton.SetOutline(true, isKeyboard);
        return true;
    }

    public void DeselectButton()
    {
        if (selectedButton != null)
            selectedButton.SetOutline(false);

        selectedButton = null;
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        Vector2Int movementInt = new Vector2Int((int)movement.x, (int)movement.y);

        if (!context.performed)
            return;

        if (ClueManager.instance != null)
        {
            if (ClueManager.instance.isOpen && movement.x != 0 && !ClueDisplaySpawner.instance.isScrolling)
            {
                ClueDisplay tempDisplay = ClueDisplaySpawner.instance.GetAdjacentDisplay(ClueDisplaySpawner.instance.GetSelectedDisplayIndex(ClueManager.instance.currentMenuType), ClueManager.instance.currentMenuType, movement.x < 0);
                ClueDisplaySpawner.instance.ResetSelection(ClueManager.instance.currentMenuType);

                AudioManager.instance.PlaySFX("Clue Click");
                ClueManager.instance.SetCurrentClue(ClueManager.instance.currentMenuType, tempDisplay.Index);
                tempDisplay.SetSelector(true);
                if (!ClueDisplaySpawner.instance.IsMainClueDisplay(tempDisplay.Index))
                {
                    AudioManager.instance.PlaySFX("Clue Slide");
                    ClueDisplaySpawner.instance.ScrollSection(tempDisplay.Index, true, movement.x < 0);
                }
                return;
            }
        }

        if (LogManager.instance != null)
        {
            if (LogManager.instance.isCheckingLogs)
            {
                return;
            }
        }

        if (DeductionManager.instance != null)
        {
            if (DeductionManager.instance.deductionMenuActive)
            {
                if (movement.x != 0)
                    DeductionManager.instance.GoToAdjacentDisplay();
                else if (movement.y != 0)
                    DeductionManager.instance.ChangeDisplay();
                return;
            }
        }

        if (selectedButton != null && movementInt != Vector2Int.zero)
        {
            if (SetSelectedButton(selectedButton.GetButtonInDirection(movementInt), true))
                AudioManager.instance.PlaySFX("Select Button");
            return;
        }
    }

    public void OnProgress(InputAction.CallbackContext context)
    {
        if (SettingsManager.instance != null)
        {
            if (CanPressButton(context, true, true) && SettingsMenu.instance.isOpen && selectedButton != null)
            {
                selectedButton.DelayedClick();
                return;
            }
        }

        if (!CanPressButton(context))
            return;

        if (ClueManager.instance != null)
        {
            if (!LocationManager.instance.IsAnimationFinished)
                LocationManager.instance.CloseGainLocationDisplay();
        }

        if (ClueManager.instance != null)
        {
            if (!ClueManager.instance.IsAnimationFinished)
                ClueManager.instance.CloseGainClueDisplay();
        }

        if (DeductionManager.instance != null)
        {
            if (DeductionManager.instance.confirmButtonActive && DeductionManager.instance.deductionMenuActive)
                DialogueManager.instance.Present(true);
        }

        if (selectedButton != null && selectedButton.IsInteractable)
            selectedButton.Click();
    }

    public void OnSettings(InputAction.CallbackContext context)
    {
        if (!CanPressButton(context, true, true) || SettingsManager.instance == null)
            return;

        SettingsMenu.instance.OpenSettings();
        if (SettingsMenu.instance.isOpen)
        {
            previousState = currentState;
            currentState = State.Settings;
        }
        else
        {
            currentState = previousState;
            previousState = State.Settings;
        }
        AudioManager.instance.PlaySFX("Click");
    }

    public void OnClue(InputAction.CallbackContext context)
    {
        if (!CanPressButton(context) || ClueManager.instance == null)
            return;

        if (!ClueManager.instance.isOpen)
        {
            if (MemoryManager.instance.isMemoryActive)
            {
                ClueManager.instance.DeduceButton();
            }
            else if (DialogueManager.instance.inTestimony)
            {
                ClueManager.instance.TestimonyPresentButton();
            }
            else
            {
                ClueManager.instance.ClueMenuButton();
            }
        }
        else
        {
            ClueManager.instance.SwapMenu();
        }

        AudioManager.instance.PlaySFX("Click");
    }

    public void OnPresent(InputAction.CallbackContext context)
    {
        if (!CanPressButton(context) || ClueManager.instance == null)
            return;

        if (ClueManager.instance.isOpen)
        {
            DialogueManager.instance.Present(true);
        }

        AudioManager.instance.PlaySFX("Click");
    }

    public void OnClosePress(InputAction.CallbackContext context)
    {
        if (!CanPressButton(context))
            return;

        if (ClueManager.instance != null)
        {
            if (ClueManager.instance.isOpen && ClueManager.instance.canGoBack)
            {
                AudioManager.instance.PlaySFX("Click");
                ClueManager.instance.CloseMenu();
                return;
            }
        }

        if (DialogueManager.instance != null)
        {
            if (DialogueManager.instance.inTestimony)
            {
                AudioManager.instance.PlaySFX("Click");
                DialogueManager.instance.Press(true);
                return;
            }
        }

        if (MemoryManager.instance != null)
        {
            if (MemoryManager.instance.isMemoryActive)
            {
                AudioManager.instance.PlaySFX("Click");
                MemoryManager.instance.Exit();
                return;
            }
        }

        if (LocationManager.instance != null)
        {
            if (LocationManager.instance.GetBackButtonActive())
            {
                AudioManager.instance.PlaySFX("Click");
                LocationManager.instance.BackButton();
                return;
            }
        }
    }

    public void OnLog(InputAction.CallbackContext context)
    {
        if (!CanPressButton(context, true) || LogManager.instance == null)
            return;

        LogManager.instance.OpenLog(!LogManager.instance.isCheckingLogs);
        AudioManager.instance.PlaySFX("Click");
    }

    public void OnCheck(InputAction.CallbackContext context)
    {
        if (!CanPressButton(context) || ClueManager.instance == null)
            return;

        if (ClueManager.instance.isOpen)
        {
            ClueManager.instance.CheckClue();
        }

        AudioManager.instance.PlaySFX("Click");
    }

    public void OnHideUI(InputAction.CallbackContext context)
    {
        if (UI == null)
            return;

        if (context.performed)
        {
            UI.SetActive(false);
        }
        else if (context.canceled)
        { 
            UI.SetActive(true); 
        }
    }

    private bool CanPressButton(InputAction.CallbackContext context, bool isLog = false, bool isSettings = false)
    {
        if (!context.performed)
            return false;

        if (SettingsManager.instance != null)
        {
            if (!isSettings && SettingsMenu.instance.isOpen)
                return false;
        }

        if (LogManager.instance != null)
        {
            if (!isLog && LogManager.instance.isCheckingLogs)
                return false;
        }

        return true;
    }

    public bool Progress()
    {
        if (!UI.activeSelf)
            return false;

        return progressButton.action.triggered && !SettingsMenu.instance.isOpen && !ClueManager.instance.isOpen;
    }

    public Vector2 Navigate()
    {
        if (!navigateButton.action.triggered)
            return Vector2.zero;

        return navigateButton.action.ReadValue<Vector2>();
    }

    public bool TestimonyNavigate()
    {
        return Navigate() != Vector2.zero && Navigate().y == 0 && !ClueManager.instance.isOpen;
    }
}
