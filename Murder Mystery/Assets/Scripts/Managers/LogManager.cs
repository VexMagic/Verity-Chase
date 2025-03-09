using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager instance;

    [SerializeField] private GameObject logMenu;

    [SerializeField] private GameObject display;
    [SerializeField] private GameObject exitDialogueDivider;
    [SerializeField] private RectTransform area;
    [SerializeField] private Scrollbar bar;
    [SerializeField] private float displayDistance;
    [SerializeField] private int duplicateLimit;
    [SerializeField] private int totalLimit;

    private List<DialogueLine> readLines = new List<DialogueLine>();
    private List<Interaction> finishedInteractions = new List<Interaction>();
    private List<GameObject> displayObjects = new List<GameObject>();
    private List<LogDisplay> displayComponents = new List<LogDisplay>();

    public bool isCheckingLogs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            logMenu.gameObject.SetActive(false);
            OpenLog(false);
        }
    }

    public void OpenLog(bool open)
    {
        //bar.value = 0;
        isCheckingLogs = open;
        logMenu.SetActive(open);
        StartCoroutine(WaitUntilNextFrame());
    }

    private IEnumerator WaitUntilNextFrame()
    {
        yield return new WaitForEndOfFrame();
        bar.value = 0;
    }

    private void SpawnLineDisplay(DialogueLine line)
    {
        GameObject tempDisplay = Instantiate(display, area.transform);
        LogDisplay tempComponent = tempDisplay.GetComponent<LogDisplay>();

        bool isFromSameChar = false;
        if (displayComponents.Count != 0)
            isFromSameChar = line.talkingCharacter == displayComponents[^1].GetLine().talkingCharacter;

        tempComponent.SetValues(line, isFromSameChar);
        displayObjects.Add(tempDisplay);
        displayComponents.Add(tempComponent);
        UpdateDisplayArea();
    }

    public void SpawnDivider()
    {
        GameObject tempDisplay = Instantiate(exitDialogueDivider, area.transform);
        displayObjects.Add(tempDisplay);
    }

    private bool CheckForDuplicates(DialogueLine line)
    {
        int listSize = displayComponents.Count;
        int start = Mathf.Clamp(listSize - duplicateLimit, 0, listSize);

        for (int i = start; i < listSize; i++)
        {
            if (displayComponents[i].GetLine() == line)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateDisplayArea()
    {
        float height = 0;

        while (displayComponents.Count > totalLimit) 
        {
            Destroy(displayObjects[0].gameObject);
            displayObjects.RemoveAt(0);
            displayComponents.RemoveAt(0);
        }

        foreach (var display in displayComponents)
            height += display.GetHeight();

        height += (displayComponents.Count - 1) * displayDistance;

        area.sizeDelta = new Vector2(area.sizeDelta.x, height);
    }

    public void ReadLine(DialogueLine line)
    {
        if (!CheckForDuplicates(line))
            SpawnLineDisplay(line);

        if (!HasReadLine(line))
        {
            readLines.Add(line);
        }
    }

    public bool HasReadLine(DialogueLine line)
    {
        return readLines.Contains(line);
    }

    public void FinishInteraction(Interaction interaction)
    {
        if (!HasFinishedInteraction(interaction))
        {
            finishedInteractions.Add(interaction);
            DialogueManager.instance.newInteractionFinished = true;
        }
    }

    public bool HasFinishedMultipleInteractions(Interaction[] interactions)
    {
        foreach (var item in interactions)
        {
            if (!HasFinishedInteraction(item))
            {
                return false;
            }
        }

        return true;
    }

    public bool HasFinishedInteraction(Interaction interaction)
    {
        return finishedInteractions.Contains(interaction);
    }
}
