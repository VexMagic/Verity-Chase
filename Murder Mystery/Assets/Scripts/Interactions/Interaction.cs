using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : ScriptableObject
{
}

public enum Character { None = -1, UnknownMale, UnknownFemale, Verity, Feyme, Claire, Arthur, Ace, Ramsey, Rook, Wren, Amara, Mari, Minerva, Wilma }

[Serializable]
public class DialogueLine
{
    [SerializeField] private Character TalkingCharacter;
    [SerializeField] [TextArea] private string Text;
    [SerializeField] private List<Background> Backgrounds;
    [SerializeField] private List<GainAnyClueType> GainedClues;
    [SerializeField] private List<Location> GainedLocations;
    [SerializeField] private List<ConspiracyNote> GainedNotes;
    [SerializeField] private List<CharacterMovement> Movements;
    [SerializeField] private AudioClip Music;

    public Character talkingCharacter => TalkingCharacter;
    public string text => Text;
    public List<Background> backgrounds => Backgrounds;
    public List<GainAnyClueType> gainedClues => GainedClues;
    public List<Location> gainedLocations => GainedLocations;
    public List<ConspiracyNote> gainedNotes => GainedNotes;
    public List<CharacterMovement> movements => Movements;
    public AudioClip music => Music;

    public DialogueLine(Character talkingCharacter, string text)
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

    public CharacterMovement GetIDMovement(int ID)
    {
        foreach (var item in Movements)
        {
            if (item.displayID == ID)
            {
                return item;
            }
        }

        Movements.Add(new CharacterMovement(ID));

        return Movements[^1];
    }

    public void DeleteMovement(CharacterMovement movement)
    {
        Movements.Remove(movement);
    }
}

[Serializable]
public class CharacterMovement
{
    [SerializeField] private int DisplayID;
    [SerializeField] private RuntimeAnimatorController Character;
    [SerializeField] private AnimationClip Animation;
    [SerializeField] private int XPos;
    [SerializeField] private bool FacingLeft;
    [SerializeField] private int Delay;

    public int displayID => DisplayID;
    public RuntimeAnimatorController character => Character;
    public AnimationClip animation => Animation;
    public int xPos => XPos;
    public bool facingLeft => FacingLeft;
    public int delay => Delay;

    public CharacterMovement(int ID)
    {
        DisplayID = ID;
        Character = null;
        Animation = null;
        XPos = 0;
        FacingLeft = true;
        Delay = 0;
    }

    public CharacterMovement(int ID, RuntimeAnimatorController controller, AnimationClip animation)
    {
        DisplayID = ID;
        Character = controller;
        Animation = animation;
        XPos = 0;
        FacingLeft = true;
        Delay = 0;
    }

    public void SetDisplayID(int ID) { DisplayID = ID; }
    public void SetCharacter(RuntimeAnimatorController controller) { Character = controller; }
    public void SetAnimation(AnimationClip animation) { Animation = animation; }
    public void SetPosition(int xPos) { XPos = xPos; }
    public void SetFacingLeft(bool facingLeft) { FacingLeft = facingLeft; }
    public void SetDelay(int delay) { Delay = delay; }
}

[Serializable]
public class Background
{
    [SerializeField] private Sprite Image;
    [SerializeField] private bool IsBackground = true;
    [SerializeField] private float FadeSpeed;
    [SerializeField] private bool FadeIn;
    [SerializeField] private int Delay;
    public Sprite image => Image;
    public bool isBackground => IsBackground;
    public float fadeSpeed => FadeSpeed;
    public bool fadeIn => FadeIn;
    public int delay => Delay;
}
