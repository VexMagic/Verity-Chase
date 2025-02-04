using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChapterData
{
    [SerializeField] private string Title;
    [SerializeField] private List<string> Parts;

    public string title => Title;
    public List<string> parts => Parts;
}
