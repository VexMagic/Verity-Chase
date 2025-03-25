using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Conspiracy Board", menuName = "Interaction/Conspiracy Board")]
public class ConspiracyBoard : MinigameInteraction
{
    [SerializeField] private ConspiracyLogic[] Connections;
    public ConspiracyLogic[] connections => Connections;
}

[Serializable]
public class ConspiracyLogic
{
    [SerializeField] private ConspiracyNote Note1;
    [SerializeField] private ConspiracyNote Note2;
    [SerializeField] private Interaction Result;

    public ConspiracyNote note1 => Note1;
    public ConspiracyNote note2 => Note2;
    public Interaction result => Result;
}
