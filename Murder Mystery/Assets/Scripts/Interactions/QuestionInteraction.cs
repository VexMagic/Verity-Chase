using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionInteraction : Interaction
{
    [SerializeField] private Interaction CorrectAnswer;
    [SerializeField] private Interaction WrongAnswer;
    public Interaction correctAnswer => CorrectAnswer;
    public Interaction wrongAnswer => WrongAnswer;
}
