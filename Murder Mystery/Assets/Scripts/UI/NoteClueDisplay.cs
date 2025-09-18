using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteClueDisplay : NoteDisplay
{
    [SerializeField] private Image icon;

    public void SetValues(string title, Sprite icon)
    {
        this.title.text = title;
        this.icon.sprite = icon;
    }

    public override bool IsPointInside(Vector2 pos)
    {
        return Mathf.Abs(transform.localPosition.x - pos.x) <= 160 && Mathf.Abs(transform.localPosition.y - pos.y) <= 180;
    }
}
