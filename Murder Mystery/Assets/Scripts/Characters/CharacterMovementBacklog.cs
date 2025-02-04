using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovementBacklog 
{
    private CharacterMovement Movement;
    private bool Spawn;

    public CharacterMovement movement => Movement;
    public bool spawn => Spawn;

    public CharacterMovementBacklog(CharacterMovement values, bool spawn) 
    {
        Movement = values;
        Spawn = spawn;
    }
}
