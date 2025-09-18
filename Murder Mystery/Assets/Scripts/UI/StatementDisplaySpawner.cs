using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatementDisplaySpawner : MonoBehaviour
{
    [SerializeField] private GameObject displayObject;

    private List<GameObject> displays = new List<GameObject>();
    private List<StatementDisplay> displayData = new List<StatementDisplay>();

    public void DestroyDisplays()
    {
        foreach (var display in displays)
        {
            Destroy(display);
        }
        displays.Clear();
        displayData.Clear();
    }

    public void CreateDisplays(Testimony testimony, int current)
    {
        DestroyDisplays();

        for (int i = 0; i < testimony.lines.Count; i++)
        {
            GameObject tempDisplay = Instantiate(displayObject, transform);
            StatementDisplay tempData = tempDisplay.GetComponent<StatementDisplay>();

            displays.Add(tempDisplay);
            displayData.Add(tempData);
        }

        UpdateDisplays(testimony, current);
    }

    public void UpdateDisplays(Testimony testimony, int currentStatement)
    {
        HintManager.instance.HideTestimonyHintDisplay();
        for (int i = 0; i < displayData.Count; i++)
        {
            displays[i].SetActive(testimony.lines[i].IsAvailable());

            displayData[i].Highlight(i == currentStatement);
        }
    }

    public void ShowHint(int statement, TestimonyHintType hintType)
    {
        Debug.Log("show hint " + statement + " - " + hintType.ToString());
        displayData[statement].ShowHint(hintType);
    }

    public void HideHint()
    {
        foreach (var item in displayData)
        {
            item.HideHint();
        }
    }
}
