using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "Interaction/Cutscene")]
public class Cutscene : ResponseInteraction
{
    [SerializeField] private AnimationClip Clip;
    public AnimationClip clip => Clip;
}
