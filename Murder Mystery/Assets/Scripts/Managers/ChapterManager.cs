using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager instance;

    public Button startButton;
    public int caseNumber = -1;
    public int currentChapter = -1;
    public int currentPart = -1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            UpdateStartButton();
        }
    }

    private void Start()
    {
        if (instance == this)
        {
            ReadInteractions();
        }
        else
        {
            instance.startButton = startButton;
            UpdateStartButton();
            instance.ReadInteractions();
            Destroy(this);
        }
    }

    private void UpdateStartButton()
    {
        if (startButton == null)
            return;

        startButton.interactable = caseNumber != -1 && currentChapter != -1 && currentPart != -1;
    }

    public void SelectCase(int Case)
    {
        caseNumber = Case;
        UpdateStartButton();
    }

    public void SelectChapter(int chapter)
    {
        currentChapter = chapter;
        UpdateStartButton();
    }

    public void SelectPart(int part)
    {
        currentPart = part;
        UpdateStartButton();
    }

    public void ReadInteractions()
    {
        if (caseNumber == -1 || LogManager.instance == null)
            return;
        
        for (int i = 1; i <= currentPart; i++)
        {
            Interaction[] interactions = Resources.LoadAll<Interaction>("Interactions/Case " + (caseNumber + 1) + "/Chapter " + (currentChapter + 1) + "/Part " + i);
            foreach (Interaction interaction in interactions)
            {
                LogManager.instance.FinishInteraction(interaction);
            }
        }
    }
}
