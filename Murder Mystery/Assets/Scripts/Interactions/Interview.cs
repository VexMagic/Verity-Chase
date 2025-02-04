using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Interview", menuName = "Interaction/Interview")]
public class Interview : Interaction
{
    [SerializeField] private Character CharacterName;
    [SerializeField] private RuntimeAnimatorController Character;
    [SerializeField] private AnimationClip Animation;
    [SerializeField] private AudioClip Music;
    [SerializeField] protected Question[] Questions;

    public Character characterName => CharacterName;
    public RuntimeAnimatorController character => Character;
    public AnimationClip animation => Animation;
    public AudioClip music => Music;
    public Question[] questions => Questions;

    public Response[] responses()
    {
        List<Response> responses = new List<Response>();

        foreach (var question in questions)
        {
            if (question.IsAvailable())
            {
                responses.Add(question.response);
            }
        }

        return responses.ToArray();
    }
}

[Serializable]
public class Question
{
    [SerializeField] private Response Response;
    [SerializeField] private Interaction Condition;
    public Response response => Response;
    public Interaction condition => Condition;
    public bool IsAvailable() => LogManager.instance.HasFinishedInteraction(Condition) || Condition == null;
}