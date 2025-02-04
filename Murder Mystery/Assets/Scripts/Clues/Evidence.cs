using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Evidence2 : Clue2
{
    [SerializeField] private List<CheckEvidence> Check;

    public List<CheckEvidence> check => Check;

    public bool hasCheck() => Check.Count != 0;
}

[CreateAssetMenu(fileName = "New Evidence", menuName = "Clue/Evidence")]
public class Evidence : Clue
{
    [SerializeField] private List<CheckEvidence> Check;

    public List<CheckEvidence> check => Check;

    public bool hasCheck() => Check.Count != 0;
}

[Serializable]
public class CheckEvidence
{
    [SerializeField] private CheckType CheckType;
    [SerializeField] [TextArea] private string Description;
    [SerializeField] private Sprite Image;
    [SerializeField] private AnimationClip Clip;

    public CheckType checkType => CheckType;
    public string description => Description;
    public Sprite image => Image;
    public AnimationClip clip => Clip;
}

public enum CheckType { Text, Image, Cutscene}
