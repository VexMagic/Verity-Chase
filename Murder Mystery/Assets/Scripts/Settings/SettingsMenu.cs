using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu instance;

    [SerializeField] private SubMenuManager subMenuManager;
    [SerializeField] private Transform subMenuButtonArea;
    [SerializeField] private TextMeshProUGUI textSpeedDisplay;
    [SerializeField] private TextMeshProUGUI textTransparencyDisplay;
    [SerializeField] private Animator animator;

    private UIButton[] UIButtons;
    private List<SettingsButton> settingsButtons = new List<SettingsButton>();
    private bool IsOpen;
    private UIButton previousButton;

    public bool isOpen => IsOpen;

    private void Awake()
    {
        instance = this;
        IsOpen = transform.GetChild(0).gameObject.activeSelf;
        GetSubMenuButtons();
    }

    private void Start()
    {
        OpenSettings(false);
        SetAnimator(false);
        textSpeedDisplay.text = SettingsManager.instance.GetTextSpeeds().displayName;
        textTransparencyDisplay.text = SettingsManager.instance.GetTextTransparency().displayName;
    }

    private void GetSubMenuButtons()
    {
        UIButtons = subMenuButtonArea.GetComponentsInChildren<UIButton>();

        if (UIButtons.Length == 1)
            return;

        Color tempColor = Color.HSVToRGB(0, 0.7f, 1);

        for (int i = 0; i < UIButtons.Length; i++)
        {
            UIButtons[i].GetComponent<Image>().color = tempColor;

            Color.RGBToHSV(tempColor, out float H, out float S, out float V);
            H += 0.167f;
            tempColor = Color.HSVToRGB(H, S, V);

            UIButton tempAbove = null;
            UIButton tempBelow = null;

            if (i == 0)
                tempAbove = UIButtons[^1];
            else
                tempAbove = UIButtons[i - 1];

            if (i == UIButtons.Length - 1)
                tempBelow = UIButtons[0];
            else
                tempBelow = UIButtons[i + 1];

            UIButtons[i].SetAdjacent(tempAbove, tempBelow, null, null);
            settingsButtons.Add(UIButtons[i].GetComponent<SettingsButton>());
        }
    }

    public void OpenSettings(bool open)
    {
        IsOpen = open;
        transform.GetChild(0).gameObject.SetActive(open);
        if (open)
        {
            previousButton = ControlManager.instance.SelectedButton;
            UIButtons[0].SelectButton(false);
        }
        else
        {
            subMenuManager.CloseAllSubMenus();
            if (DialogueManager.instance != null)
                DialogueManager.instance.SetTextWindowTransparency();
            ControlManager.instance.DeselectButton();
            if (KeyIconManager.instance != null)
                KeyIconManager.instance.UpdateKeyIcons();

            if (previousButton != null)
            {
                previousButton.SelectButton(true);
                previousButton = null;
            }
        }
    }

    public void SetAnimator(bool submenu)
    {
        animator.SetBool("Sub Menu", submenu);
    }

    public void OpenSettings()
    {
        OpenSettings(!IsOpen);
    }

    public void ChangeTextSpeed(bool increase)
    {
        SettingsManager.instance.ChangeTextSpeed(increase);
        textSpeedDisplay.text = SettingsManager.instance.GetTextSpeeds().displayName;
    }

    public void ChangeTextTransparency(bool increase)
    {
        SettingsManager.instance.ChangeTextTransparency(increase);
        textTransparencyDisplay.text = SettingsManager.instance.GetTextTransparency().displayName;
    }
}
