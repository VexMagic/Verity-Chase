using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHint
{
    [SerializeField] private List<Interaction> Conditions;
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
