using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private Transform camera;
    [SerializeField] private float maxOffset;
    [SerializeField] private float maxRotation;
    [SerializeField] private float moveSpeed;
    private Character tempCharacter;
    private Coroutine coroutine;

    private void Awake()
    {
        instance = this;
    }

    public void NewLine(Character speaker)
    {
        if (speaker == Character.None)
            return;

        CharacterDisplay display = CharacterManager.instance.GetCharacterDisplay(CharacterManager.instance.GetCharacterController(speaker));

        if (display == null)
        {
            tempCharacter = speaker;
            Invoke(nameof(RetryNewLine), 0.1f);
            return;
        }

        SetCameraPos(display.EndPos);
    }

    private void SetCameraPos(float endPos)
    {
        float percentage = endPos / maxOffset;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(Rotation(percentage * maxRotation));
    }

    IEnumerator Rotation(float endPos)
    {
        float start = camera.eulerAngles.y;
        if (start > 180)
            start -= 360;

        bool active = start != endPos;
        float sinTime = 0;
        while (active)
        {
            sinTime += Time.deltaTime * moveSpeed;
            if (sinTime > Mathf.PI)
                active = false;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            camera.eulerAngles = Vector3.Lerp(new Vector3(0, start), new Vector3(0, endPos), t);
            yield return null;
        }
        coroutine = null;
    }

    float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }

    private void RetryNewLine()
    {
        NewLine(tempCharacter);
    }

    public void EndDialogue()
    {
        SetCameraPos(0);
    }
}
