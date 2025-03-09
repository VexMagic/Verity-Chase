using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : UIManager
{
    public static MainMenuManager instance;

    [SerializeField] private Animator animator;
    [SerializeField] private float openSpeed;

    [SerializeField] private GameObject[] menus;
    [SerializeField] private UIButton[] defaultButton;

    private Coroutine coroutine;
    private bool isOpen;
    private int menuIndex;

    public int MenusIndex => menuIndex;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        menuIndex = 0;
        ChangeMenu();
    }

    public void OpenMenu(int index)
    {
        animator.SetBool("Open", true);
        menuIndex = index;
        ControlManager.instance.DeselectButton();

        Invoke(nameof(ChangeMenu), openSpeed);
    }

    private void ChangeMenu()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(i == menuIndex);
            if (i == menuIndex)
            {
                if (defaultButton[i] != null)
                    ControlManager.instance.SetSelectedButton(defaultButton[i]);
            }
        }

        if (menuIndex == 0)
        {
            TitleCharacterSpawner.instance.StartCharacterDisplay();
        }
        else
        {
            TitleCharacterSpawner.instance.StopCharacterDisplay();
        }

        if (menuIndex == 1)
        {
            ChapterManager.instance.SelectCase(0);
            ChapterManager.instance.SelectChapter(0);
            ChapterManager.instance.SelectPart(0);
        }
        
        if (menuIndex == 2)
        {
            ChapterSelector.instance.SpawnObjects();
            ChapterManager.instance.SelectChapter(-1);
            ChapterManager.instance.SelectPart(-1);
        }
        else
        {
            ChapterSelector.instance.DestroyObjects();
        }

        animator.SetBool("Open", false);
    }
}
