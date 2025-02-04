using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueData : ScriptableObject
{
    [SerializeField] private string Title;
    
    public enum Type { Evidence, Profile }

    public string title => Title;
}

public class GainClue
{
    [SerializeField] protected int Version;

    public int version { get { return Version; } set { Version = value; } }
}

[Serializable]
public class GainAnyClueType : GainClue
{
    [SerializeField] private ClueData GainedClue;

    public ClueData gainedClue => GainedClue;
}
