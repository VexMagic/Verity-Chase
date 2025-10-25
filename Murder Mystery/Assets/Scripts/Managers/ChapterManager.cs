using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager instance;

    public Button startButton;
    public Button chapterButton;
    public int currentCase = 0;
    public int currentChapter = 0;
    public int currentPart = 0;

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
            instance.chapterButton = chapterButton;
            UpdateStartButton();
            instance.ReadInteractions();
            Destroy(this);
        }
    }

    private void UpdateStartButton()
    {
        if (startButton != null)
        {
            startButton.interactable = currentCase == 1 && currentChapter > 0 && currentPart > 0;
            startButton.GetComponent<UIButton>().SetOutlineInteractable();
        }

        if (chapterButton != null)
        {
            chapterButton.interactable = currentCase == 1 && currentChapter > 0 && currentPart > 0;
            chapterButton.GetComponent<UIButton>().SetOutlineInteractable();
        }
    }

    public void SelectCase(int Case)
    {
        currentCase = Case;
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
        if (currentCase == 0 || LogManager.instance == null)
            return;

        for (int i = 1; i < currentChapter; i++)
        {
            int counter = 1;
            while (true)
            {
                Interaction[] interactions = Resources.LoadAll<Interaction>("Interactions/Case " + currentCase + "/Chapter " + i + "/Part " + counter);
                if (interactions != null)
                {
                    if (interactions.Length > 0)
                    {
                        foreach (Interaction interaction in interactions)
                        {
                            LogManager.instance.FinishInteraction(interaction);
                        }
                        counter++;
                    }
                    else
                        break;
                }
                else
                    break;
            }
        }

        for (int i = 1; i < currentPart; i++)
        {
            Interaction[] interactions = Resources.LoadAll<Interaction>("Interactions/Case " + currentCase + "/Chapter " + currentChapter + "/Part " + i);
            foreach (Interaction interaction in interactions)
            {
                LogManager.instance.FinishInteraction(interaction);
            }
        }
    }
}
