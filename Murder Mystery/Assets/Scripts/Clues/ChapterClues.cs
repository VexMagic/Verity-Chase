using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChapterClues
{
    [SerializeField] private List<PartClues> PartClues = new List<PartClues>();

    public List<PartClues> partClues => PartClues;
}

[Serializable]
public class PartClues
{
    [SerializeField] private List<GainEvidence> AllEvidence = new List<GainEvidence>();
    [SerializeField] private List<GainProfile> AllProfiles = new List<GainProfile>();

    public List<GainEvidence> allEvidence => AllEvidence;
    public List<GainProfile> allProfiles => AllProfiles;
}