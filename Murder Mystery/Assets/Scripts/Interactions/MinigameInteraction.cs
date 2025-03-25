using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameInteraction : Interaction
{
    [SerializeField] private string MemoryName;
    [SerializeField] private bool Exitable;
    [SerializeField] private Interaction Complete;
    [SerializeField] private Interaction GiveUp;
    [SerializeField] private Interaction WrongAnswer;
    [SerializeField] private Interaction[] Conditions;

    public string memoryName => MemoryName;
    public bool exitable => Exitable;
    public Interaction complete => Complete;
    public Interaction giveUp => GiveUp;
    public Interaction wrongAnswer => WrongAnswer;
    public bool IsFinished() => LogManager.instance.HasFinishedMultipleInteractions(Conditions) || Conditions == null;
}
