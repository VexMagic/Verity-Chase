using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fade To Black", menuName = "Interaction/Extra/Fade To Black")]
public class FadeToBlack : ResponseInteraction
{
    [SerializeField] private float FadeInSpeed;
    [SerializeField] private float FadeOutSpeed;
    [SerializeField] private float FadeOutDelay;
    [SerializeField] private Color FadeColor;
    [SerializeField] private List<CharacterMovement> Movements;

    public float fadeInSpeed => FadeInSpeed;
    public float fadeOutSpeed => FadeOutSpeed;
    public float fadeOutDelay => FadeOutDelay;
    public Color fadeColor => FadeColor;
    public List<CharacterMovement> movements => Movements;
}
