using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Split Path", menuName = "Interaction/Split Path")]
public class SplitPath : Interaction
{
    [SerializeField] private List<Path> Paths;

    public Path GetPathResult()
    {
        for (int i = Paths.Count - 1; i >= 0; i--)
        {
            if (Paths[i].IsAvailable())
                return Paths[i];
        }
        return null;
    }
}

[Serializable]
public class Path
{
    [SerializeField] private Interaction Result;
    [SerializeField] private List<Interaction> Conditions;
    [SerializeField] private bool ScreenFade;

    public Interaction result => Result;
    public bool screenFade => ScreenFade;

    public bool IsAvailable()
    {
        bool finishedConditons = true;
        foreach (var item in Conditions)
        {
            if (!LogManager.instance.HasFinishedInteraction(item))
                finishedConditons = false;
        }

        return finishedConditons;
    }
}