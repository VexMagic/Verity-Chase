using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Conspiracy Board", menuName = "Interaction/Conspiracy Board")]
public class ConspiracyBoard : MinigameInteraction
{
    [SerializeField] private ConspiracyLine[] Lines;
    public ConspiracyLine[] lines => Lines;
}

[Serializable]
public class ConspiracyLine : DialogueLine
{
    [SerializeField] private List<ConspiracyNote> GainedNotes;

    public ConspiracyLine(Character talkingCharacter, string text) : base(talkingCharacter, text)
    {
        TalkingCharacter = talkingCharacter;
        Text = text;
        Backgrounds = new List<Background>();
        GainedClues = new List<GainAnyClueType>();
        GainedLocations = new List<Location>();
        GainedNotes = new List<ConspiracyNote>();
        Movements = new List<CharacterMovement>();
        Music = null;
    }
}

[CreateAssetMenu(fileName = "New Conspiracy Note", menuName = "Clue/Conspiracy Note")]
public class ConspiracyNote : ScriptableObject
{

}
