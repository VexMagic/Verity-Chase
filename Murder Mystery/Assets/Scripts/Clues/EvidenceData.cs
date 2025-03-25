using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Evidence", menuName = "Clue/Evidence")]
public class EvidenceData : ClueData
{
    [SerializeField] private Evidence[] Versions;

    public Evidence[] versions => Versions;
}

[Serializable]
public class GainEvidence : GainClue
{
    [SerializeField] private EvidenceData GainedEvidence;

    public EvidenceData gainedEvidence => GainedEvidence;

    public GainEvidence(EvidenceData data, int version)
    {
        GainedEvidence = data;
        Version = version;
    }
}