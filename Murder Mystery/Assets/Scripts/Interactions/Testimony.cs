using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Testimony", menuName = "Interaction/Testimony")]
public class Testimony : QuestionInteraction
{
    [SerializeField] private Interaction PreviewInteraction;
    [SerializeField] private AudioClip Music;
    [SerializeField] private string Title;
    [SerializeField] private List<TestimonyLine> Lines;
    public Interaction previewInteraction => PreviewInteraction;
    public AudioClip music => Music;
    public string title => Title;
    public List<TestimonyLine> lines => Lines;
}

[Serializable]
public class TestimonyLine
{ 
    [SerializeField] private DialogueLine Line;
    [SerializeField] private Interaction Condition;
    [SerializeField] private Interaction RemoveCondition;
    [SerializeField] private Interaction PressResult;
    [SerializeField] private List<TestimonyPresent> CorrectAnswers;
    public DialogueLine line => Line;
    public Interaction condition => Condition;
    public Interaction removeCondition => RemoveCondition;
    public Interaction pressResult => PressResult;
    public List<TestimonyPresent> correctAnswers => CorrectAnswers;

    public bool IsAvailable() => (LogManager.instance.HasFinishedInteraction(Condition) || Condition == null) 
        && (!LogManager.instance.HasFinishedInteraction(removeCondition) || removeCondition == null);
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
