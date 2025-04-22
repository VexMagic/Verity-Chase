using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private Transform cameraPos;
    [SerializeField] private Camera cameraObject;
    [SerializeField] private float offsetIgnore;
    [SerializeField] private Vector2 sizeConversions;
    [SerializeField] private float moveSpeed;
    private Character tempCharacter;
    private Coroutine rotateCoroutine;
    private Coroutine zoomCoroutine;

    private float CameraOffset;
    public float cameraOffset => CameraOffset;

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
        float offset = Mathf.Clamp(Mathf.Abs(endPos) - offsetIgnore, 0, 10000);

        if (endPos < 0)
            offset *= -1;

        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;
        }

        CameraOffset = offset;

        rotateCoroutine = StartCoroutine(SmoothMovement(cameraPos.position.x, offset / sizeConversions.x * sizeConversions.y));
    }

    public void StartZoom(CameraZoom cameraZoom)
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
            zoomCoroutine = null;
        }
        zoomCoroutine = StartCoroutine(ZoomCamera(cameraZoom));
    }

    IEnumerator ZoomCamera(CameraZoom cameraZoom)
    {
        float startSize = cameraObject.orthographicSize;
        Vector3 startOffset = cameraObject.transform.position;
        Vector3 endOffset = new Vector3(cameraZoom.cameraOffset.x, cameraZoom.cameraOffset.y, cameraObject.transform.position.z);

        float timer = 0;
        while (timer < cameraZoom.transitionDuration)
        {
            timer += Time.fixedDeltaTime;
            timer = Mathf.Clamp(timer, 0, cameraZoom.transitionDuration);

            float percentage = timer / cameraZoom.transitionDuration;


            cameraObject.orthographicSize = Mathf.Lerp(startSize, cameraZoom.cameraSize, percentage);
            cameraObject.transform.position = Vector3.Lerp(startOffset, endOffset, percentage);

            yield return new WaitForFixedUpdate();
        }
        zoomCoroutine = null;
    }

    IEnumerator Rotation(float endPos)
    {
        float start = cameraPos.eulerAngles.y;
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
            cameraPos.eulerAngles = Vector3.Lerp(new Vector3(0, start), new Vector3(0, endPos), t);
            yield return null;
        }
        rotateCoroutine = null;
    }

    private IEnumerator SmoothMovement(float start, float end)
    {
        bool active = true;
        float sinTime = 0;
        while (active)
        {
            sinTime += Time.deltaTime * moveSpeed;
            if (sinTime > Mathf.PI)
                active = false;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            cameraPos.localPosition = Vector3.Lerp(new Vector3(start, cameraPos.localPosition.y, cameraPos.localPosition.z), 
                new Vector3(end, cameraPos.localPosition.y, cameraPos.localPosition.z), t);
            yield return null;
        }
        rotateCoroutine = null;
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
