
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bad End", menuName = "Interaction/Extra/Bad Ending")]
public class BadEnding : ResponseInteraction
{
    [SerializeField] private List<CharacterMovement> Movements;
    [SerializeField] private AudioClip SoundEffect;
    [SerializeField] private float ScreenEffectDelay;
    [SerializeField] private float SoundEffectDelay;
    [SerializeField] private float StartDialogueDelay;
    [SerializeField] private Background Background;

    public List<CharacterMovement> movements => Movements;
    public AudioClip soundEffect => SoundEffect;
    public float screenEffectDelay => ScreenEffectDelay;
    public float soundEffectDelay => SoundEffectDelay;
    public float startDialogueDelay => StartDialogueDelay;
    public Background background => Background;
}
