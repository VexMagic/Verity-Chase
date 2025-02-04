using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clue2
{
    [SerializeField][TextArea] private string Description;
    [SerializeField] private Sprite Sprite;

    public enum Type { Evidence, Profile }

    public string description => Description;
    public Sprite sprite => Sprite;
}

public class Clue : ScriptableObject
{
    [SerializeField] private string Title;
    [SerializeField] [TextArea] private string Description;
    [SerializeField] private Sprite Sprite;

    public enum Type { Evidence, Profile }

    public string title => Title;
    public string description => Description;
    public Sprite sprite => Sprite;
}