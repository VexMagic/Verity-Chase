using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Camera Zoom", menuName = "Interaction/Camera Zoom")]
public class CameraZoom : ResponseInteraction
{
    [SerializeField] private float CameraSize;
    [SerializeField] private Vector2 CameraOffset;
    [SerializeField] private float TransitionDuration; 

    public float cameraSize => CameraSize;
    public Vector2 cameraOffset => CameraOffset;
    public float transitionDuration => TransitionDuration;
}
