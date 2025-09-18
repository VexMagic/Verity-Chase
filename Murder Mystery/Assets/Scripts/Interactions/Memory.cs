using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Memory", menuName = "Interaction/Memory")]
public class Memory : MinigameInteraction
{
    [SerializeField] private List<MemoryHint> Hints;
    [SerializeField] private List<MemoryChange> MemoryChanges;

    public MemoryHint GetHint()
    {
        for (int i = Hints.Count - 1; i >= 0; i--)
        {
            if (Hints[i].IsAvailable())
                return Hints[i];
        }
        return null;
    }

    public MemoryChange GetChange()
    {
        for (int i = MemoryChanges.Count - 1; i >= 0; i--)
        {
            if (MemoryChanges[i].IsFinished())
                return MemoryChanges[i];
        }
        return null;
    }

    public int GetVersion()
    {
        for (int i = MemoryChanges.Count - 1; i >= 0; i--)
        {
            if (MemoryChanges[i].IsFinished())
                return i + 1;
        }
        return 0;
    }
}


[Serializable]
public class MemoryHint : BaseHint
{
    [SerializeField] private string IndicatorName;

    public string indicatorName => IndicatorName;
}

[Serializable]
public class MemoryChange
{
    [SerializeField] private Sprite NewVisual;
    [SerializeField] private Interaction StartInteraction;
    [SerializeField] private Interaction[] Conditions;

    public Sprite newVisual => NewVisual;
    public Interaction startInteraction => StartInteraction;

    public bool IsFinished() => LogManager.instance.HasFinishedMultipleInteractions(Conditions) || Conditions == null;
}