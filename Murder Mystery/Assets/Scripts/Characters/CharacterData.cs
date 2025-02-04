using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public class CharacterData
{
    [SerializeField] private AnimatorController controller;
    [SerializeField] private Character character;
    [SerializeField] private bool isMale;

    public AnimatorController Controller { get { return controller; } }
    public Character Character { get { return character; } }
    public bool IsMale { get { return isMale; } }
    public bool HasBeenUsed;
}
