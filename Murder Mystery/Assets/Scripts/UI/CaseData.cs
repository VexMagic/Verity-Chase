using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CaseData
{
    [SerializeField] private string Title;
    [SerializeField] private List<ChapterData> Chapters;

    public string title => Title;
    public List<ChapterData> chapters => Chapters;
}
