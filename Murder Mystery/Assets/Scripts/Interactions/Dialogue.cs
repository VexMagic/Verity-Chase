using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Interaction/Dialogue")]
public class Dialogue : ResponseInteraction
{
    [SerializeField] private DialogueLine[] Lines;
    public DialogueLine[] lines => Lines;

    public Dialogue(DialogueLine[] lines)
    {
        Lines = lines;
    }
}
