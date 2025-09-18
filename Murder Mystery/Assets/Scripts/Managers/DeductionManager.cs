using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DeductionManager : MonoBehaviour
{
    public static DeductionManager instance;

    [SerializeField] private GameObject deductionMenu;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private GameObject selectorPrefab;
    [SerializeField] private GameObject button;
    [SerializeField] private TextMeshProUGUI question;

    private List<GameObject> selectors = new List<GameObject>();
    private List<DeductionDisplay> displays = new List<DeductionDisplay>();

    private Deduction currentDeduction;
    private DeductionDisplay selectedDeduction;

    public bool deductionMenuActive => deductionMenu.activeSelf;
    public bool confirmButtonActive => button.activeSelf;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SetMenuActive(false);
        }
    }

    public void SetSelectedButton(DeductionDisplay display)
    {
        if (display == null)
            return;

        if (selectedDeduction == display)
            return;

        if (selectedDeduction != null)
            selectedDeduction.SetOutline(false);

        selectedDeduction = display;
        selectedDeduction.SetOutline(true);
    }

    public void GoToAdjacentDisplay()
    {
        int index = GetSelectedDisplayIndex();

        if (ControlManager.instance.Navigate().x > 0)
        {
            index++;
            if (index >= displays.Count)
                index = 0;
        }
        else if (ControlManager.instance.Navigate().x < 0)
        {
            index--;
            if (index < 0)
                index = displays.Count - 1;
        }

        SetSelectedButton(displays[index]);
    }

    public void LockDisplay(int slider, int option)
    {
        displays[slider].Lock(option);
    }

    public void ChangeDisplay()
    {
        if (ControlManager.instance.Navigate().y > 0)
        {
            selectedDeduction.ClickUp();
        }
        else if (ControlManager.instance.Navigate().y < 0)
        {
            selectedDeduction.ClickDown();
        }
    }

    private int GetSelectedDisplayIndex()
    {
        for (int i = 0; i < displays.Count; i++)
        {
            if (displays[i] == selectedDeduction)
                return i;
        }
        return 0;
    }

    public void CreateDeductionDisplays(Deduction deduction)
    {
        if (selectors.Count > 0)
            return;

        currentDeduction = deduction;
        foreach (var item in deduction.options)
        {
            GameObject displayObject = Instantiate(selectorPrefab, parentObject.transform);
            DeductionDisplay deductionDisplay = displayObject.GetComponent<DeductionDisplay>();

            deductionDisplay.SetOptions(item);
            selectors.Add(displayObject);
            displays.Add(deductionDisplay);
        }

        question.text = deduction.question;

        ControlManager.instance.DeselectButton();
        SetSelectedButton(displays[0]);

        SetMenuActive(true);
    }

    public void ResetDisplays()
    {
        foreach (var item in selectors)
        {
            Destroy(item);
        }
        selectors.Clear();
        displays.Clear();
    }

    public bool IsCorrectAnswer()
    {
        List<string> answers = new List<string>();
        foreach (var answer in currentDeduction.answers)
        {
            string tempAnswer = "";
            foreach (var item in answer.answer)
            {
                if (tempAnswer == "")
                    tempAnswer += item;
                else
                    tempAnswer += ","+ item;
            }

            answers.Add(tempAnswer);
            //Debug.Log("Answer: " + tempAnswer);
        }

        string temp = "";
        foreach (var item in displays)
        {
            if (temp == "")
                temp += item.currentOption;
            else
                temp += "," + item.currentOption;
        }
        //Debug.Log("Selected: " + temp);

        return answers.Contains(temp);
    }

    public void SetButtonActive(bool isActive)
    {
        button.SetActive(isActive);
    }

    public void SetMenuActive(bool isActive)
    {
        deductionMenu.SetActive(isActive);
    }
}
