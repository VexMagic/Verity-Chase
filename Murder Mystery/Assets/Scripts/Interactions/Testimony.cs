using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Testimony", menuName = "Interaction/Testimony")]
public class Testimony : QuestionInteraction
{
    [SerializeField] private Interaction PreviewInteraction;
    [SerializeField] private AudioClip Music;
    [SerializeField] private string Title;
    [SerializeField] private List<TestimonyLine> Lines;
    [SerializeField] private List<TestimonyHint> Hints;
    public Interaction previewInteraction => PreviewInteraction;
    public AudioClip music => Music;
    public string title => Title;
    public List<TestimonyLine> lines => Lines;

    public TestimonyHint GetHint()
    {
        for (int i = Hints.Count - 1; i >= 0; i--)
        {
            if (Hints[i].IsAvailable())
                return Hints[i];
        }
        return null;
    }
}

[Serializable]
public class TestimonyLine
{ 
    [SerializeField] private DialogueLine Line;
    [SerializeField] private Interaction Condition;
    [SerializeField] private Interaction RemoveCondition;
    [SerializeField] private Interaction PressResult;
    [SerializeField] private Interaction FinishPressCondition;
    [SerializeField] private List<TestimonyPresent> CorrectAnswers;
    public DialogueLine line => Line;
    public Interaction removeCondition => RemoveCondition;
    public Interaction pressResult => PressResult;
    public List<TestimonyPresent> correctAnswers => CorrectAnswers;

    public bool IsAvailable() => (LogManager.instance.HasFinishedInteraction(Condition) || Condition == null) 
        && (!LogManager.instance.HasFinishedInteraction(removeCondition) || removeCondition == null);
    public Interaction finishPressCondition()
    {
        if (FinishPressCondition == null)
            return pressResult;
        else
            return FinishPressCondition;
    }
}

[Serializable]
public class TestimonyPresent
{
    [SerializeField] private GainAnyClueType CorrectClue;
    [SerializeField] private Interaction Result;
    [SerializeField] private bool ContinueTestimony;
    public GainAnyClueType correctClue => CorrectClue;
    public Interaction result => Result;
    public bool continueTestimony => ContinueTestimony;
}

[Serializable]
public class TestimonyHint : BaseHint
{
    [SerializeField] private TestimonyHintStatement[] Hints;
    public TestimonyHintStatement[] hint => Hints;
}

[Serializable]
public class TestimonyHintStatement
{
    [SerializeField] private TestimonyHintType HintType;
    [SerializeField] private int Statement;
    public TestimonyHintType hintType => HintType;
    public int statement => Statement;
}

public enum TestimonyHintType { Press, Present }