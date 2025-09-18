using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Conspiracy Board", menuName = "Interaction/Conspiracy Board")]
public class ConspiracyBoard : MinigameInteraction
{
    [SerializeField] private ConspiracyLogic[] NoteConnections;
    [SerializeField] private bool CanPresent;
    [SerializeField] private ConspiracyPresent[] ClueConnections;
    [SerializeField] private List<ConspiracyHint> Hints;
    public ConspiracyLogic[] noteConnections => NoteConnections;
    public bool canPresent => CanPresent;
    public ConspiracyPresent[] clueConnections => ClueConnections;

    public ConspiracyHint GetHint()
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
public class ConspiracyLogic
{
    [SerializeField] private ConspiracyNote Note1;
    [SerializeField] private ConspiracyNote Note2;
    [SerializeField] private Interaction Result;

    public ConspiracyNote note1 => Note1;
    public ConspiracyNote note2 => Note2;
    public Interaction result => Result;
}

[Serializable]
public class ConspiracyPresent
{
    [SerializeField] private ConspiracyNote Note;
    [SerializeField] private GainAnyClueType Clue;
    [SerializeField] private Interaction Result;

    public ConspiracyNote note => Note;
    public GainAnyClueType clue => Clue;
    public Interaction result => Result;
}

[Serializable]
public class ConspiracyHint : BaseHint
{
    [SerializeField] private ConspiracyHintType HintType;
    [SerializeField] private ConspiracyNote Note;
    public ConspiracyHintType hintType => HintType;
    public ConspiracyNote note => Note;
}

public enum ConspiracyHintType { Note, Clue }