using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryObject : MonoBehaviour
{
    [SerializeField] private Interaction Result;
    [SerializeField] private GainAnyClueType[] CorrectClues;
    [SerializeField] private Collider2D hitBox;
    private bool IsHovering;

    public Interaction result => Result;
    public GainAnyClueType[] correctClues => CorrectClues;
    public bool isHovering => IsHovering;

    public bool IsPointInsideCollider(Vector2 point)
    {
        return hitBox.bounds.Contains(point);
    }

    public void StartHover()
    {
        IsHovering = true;
    }

    public void EndHover()
    {
        IsHovering = false;
    }
}
