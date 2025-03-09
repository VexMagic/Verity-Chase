using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu instance;

    [SerializeField] private Transform subMenuButtonArea;

    private UIButton[] UIButtons;
    private bool IsOpen;

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
    }

    private void GetSubMenuButtons()
    {
        UIButtons = subMenuButtonArea.GetComponentsInChildren<UIButton>();

        if (UIButtons.Length == 1)
            return;

        for (int i = 0; i < UIButtons.Length; i++)
        {
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
        }
    }

    public void OpenSettings(bool open)
    {
        IsOpen = open;
        transform.GetChild(0).gameObject.SetActive(open);
        if (open)
            UIButtons[0].SelectButton(false);
        else
        {
            ControlManager.instance.DeselectButton();
            if (KeyIconManager.instance != null)
                KeyIconManager.instance.UpdateKeyIcons();
        }
    }

    public void OpenSettings()
    {
        OpenSettings(!IsOpen);
    }
}
