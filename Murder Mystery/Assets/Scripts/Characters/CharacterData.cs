using System;
using UnityEngine.Animations;
using UnityEngine;

[Serializable]
public class CharacterData
{
    [SerializeField] private RuntimeAnimatorController controller;
    [SerializeField] private Character character;
    [SerializeField] private bool isMale;

    public RuntimeAnimatorController Controller { get { return controller; } }
    public Character Character { get { return character; } }
    public bool IsMale { get { return isMale; } }

    [HideInInspector] public bool HasBeenUsed;
}
