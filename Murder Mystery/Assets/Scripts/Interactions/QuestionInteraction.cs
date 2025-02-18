using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionInteraction : Interaction
{
    [SerializeField] private Interaction CorrectAnswer;
    [SerializeField] private Interaction WrongAnswer;
    [SerializeField] private List<UniqueWrong> UniqueWrongs;
    public Interaction correctAnswer => CorrectAnswer;
    public Interaction wrongAnswer => WrongAnswer;

    public Interaction GetWrongAnswerResult(GainClue clue)
    {
        foreach (var item in UniqueWrongs)
        {
            foreach (var item1 in item.conditions)
            {
                if (clue is GainEvidence)
                {
                    if ((clue as GainEvidence).gainedEvidence == item1.gainedClue)
                    {
                        return item.result;
                    }
                }
                else if (clue is GainProfile)
                {
                    if ((clue as GainProfile).gainedProfile == item1.gainedClue)
                    {
                        return item.result;
                    }
                }
            }
        }

        return wrongAnswer;
    }
}

[Serializable]
public class UniqueWrong
{
    [SerializeField] private Interaction Result;
    [SerializeField] private List<GainAnyClueType> Conditions;

    public Interaction result => Result;
    public List<GainAnyClueType> conditions => Conditions;
}