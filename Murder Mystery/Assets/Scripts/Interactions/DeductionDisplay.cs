using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeductionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI topOption;
    [SerializeField] private TextMeshProUGUI mainOption;
    [SerializeField] private TextMeshProUGUI bottomOption;
    [SerializeField] private DeductionOptions options;

    [SerializeField] private int CurrentOption;
    public int currentOption => CurrentOption;

    public void SetOptions(DeductionOptions options)
    {
        this.options = options;
        CurrentOption = Random.Range(0, options.options.Count);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int topIndex = CurrentOption - 1;
        if (topIndex < 0)
            topIndex = options.options.Count - 1;

        int bottomIndex = CurrentOption + 1;
        if (bottomIndex >= options.options.Count)
            bottomIndex = 0;

        topOption.text = options.options[topIndex];
        mainOption.text = options.options[CurrentOption];
        bottomOption.text = options.options[bottomIndex];
    }

    public void ClickUp()
    {
        AudioManager.instance.PlaySFX("Clue Click");
        CurrentOption--;
        if (CurrentOption < 0)
            CurrentOption = options.options.Count - 1;

        UpdateDisplay();
    }

    public void ClickDown()
    {
        AudioManager.instance.PlaySFX("Clue Click");
        CurrentOption++;
        if (CurrentOption >= options.options.Count)
            CurrentOption = 0;

        UpdateDisplay();
    }
}
