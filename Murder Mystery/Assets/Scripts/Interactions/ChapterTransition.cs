using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chapter Transition", menuName = "Interaction/Chapter Transition")]
public class ChapterTransition : Interaction
{
    [SerializeField] private int CaseIndex;
    [SerializeField] private int ChapterIndex;

    public int caseIndex => CaseIndex;
    public int chapterIndex => ChapterIndex;
}
