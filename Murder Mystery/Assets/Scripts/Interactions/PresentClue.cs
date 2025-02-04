using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Present", menuName = "Interaction/Present")]
public class PresentClue : QuestionInteraction
{
    [SerializeField] private DialogueLine Line;
    [SerializeField] private List<GainAnyClueType> CorrectAnswers;

    public DialogueLine line => Line;
    public List<GainAnyClueType> correctAnswers => CorrectAnswers;
}
