using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Interaction/Dialogue")]
public class Dialogue : ResponseInteraction
{
    [SerializeField] protected bool ShowResponsesAfterDialogue;
    [SerializeField] private DialogueLine[] Lines;
    public bool showResponsesAfterDialogue => ShowResponsesAfterDialogue;
    public DialogueLine[] lines => Lines;

    public Dialogue(DialogueLine[] lines)
    {
        Lines = lines;
    }
}
