using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryObject : MonoBehaviour
{
    [SerializeField] private Interaction Result;
    [SerializeField] private GainAnyClueType[] CorrectClues;
    [SerializeField] private Collider2D hitBox;

    public Interaction result => Result;
    public GainAnyClueType[] correctClues => CorrectClues;

    public bool IsPointInsideCollider(Vector2 point)
    {
        return hitBox.bounds.Contains(point);
    }
}
