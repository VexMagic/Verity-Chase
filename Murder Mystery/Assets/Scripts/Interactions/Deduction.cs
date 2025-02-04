using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deduction", menuName = "Interaction/Deduction")]
public class Deduction : QuestionInteraction
{
    [SerializeField] [TextArea] private string Question;
    [SerializeField] private List<DeductionOptions> Options;
    [SerializeField] private List<DeductionAnswers> Answers;
    public string question => Question;
    public List<DeductionOptions> options => Options;
    public List<DeductionAnswers> answers => Answers;
}

[Serializable]
public class DeductionOptions
{
    [SerializeField] private List<string> Options;
    public List<string> options => Options;
}

[Serializable]
public class DeductionAnswers
{
    [SerializeField] private List<int> Answer;
    public List<int> answer => Answer;
}
