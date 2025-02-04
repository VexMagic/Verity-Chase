using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PartStartData
{
    [SerializeField] private List<Interaction> Starts;

    public List<Interaction> starts => Starts;
}
