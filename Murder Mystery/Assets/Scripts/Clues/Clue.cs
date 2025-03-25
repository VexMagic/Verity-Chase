using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clue
{
    [SerializeField][TextArea] private string Description;
    [SerializeField] private Sprite Sprite;

    public enum Type { Evidence, Profile }

    public string description => Description;
    public Sprite sprite => Sprite;
}