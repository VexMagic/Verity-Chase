using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using CodeMonkey.Utils;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;

public class ConspiracyManager : MonoBehaviour
{
    public static ConspiracyManager instance;

    [SerializeField] private Animator gainNoteAnimator;
    [SerializeField] private NoteDisplay gainDisplay;

    [SerializeField] private GameObject board;
    [SerializeField] private Transform noteArea;
    [SerializeField] private GameObject darken;

    [SerializeField] private RectTransform rope;
    [SerializeField] private GameObject pin1;
    [SerializeField] private GameObject pin2;

    [SerializeField] private GameObject noteObject;
    [SerializeField] private GameObject confirmButton;

    [SerializeField] private float confirmNoteDistance;
    [SerializeField] private int noteDistance;
    [SerializeField] private Vector2 edgeDistance;
    [SerializeField] private Vector2 noteSize;

    [SerializeField] private int spinFrames;
    [SerializeField] private int loopAmount;
    [SerializeField] private int spinExpandDistance;
    [SerializeField] private int slamFrames;
    [SerializeField] private int fallFrames;
    [SerializeField] private int fallAwayDistance;
    [SerializeField] private float fallRotation;
    [SerializeField] private AnimationCurve spinCurve;
    [SerializeField] private AnimationCurve fallDownCurve;
    [SerializeField] private AnimationCurve fallAwayCurve;

    [SerializeField] private List<ConspiracyNote> notes = new List<ConspiracyNote>();
    private List<GameObject> noteObjects = new List<GameObject>();
    private bool isAnimationFinished = true;
    private bool isConfirming;
    private Coroutine ropeCoroutine;

    private ConspiracyBoard currentBoard;
    private NoteDisplay firstNote;
    private NoteDisplay secondNote;

    public bool IsAnimationFinished => isAnimationFinished;

    private void Awake()
    {
        instance = this;
        board.SetActive(false);
    }

    public IEnumerator GainNote(ConspiracyNote note)
    {
        if (!notes.Contains(note))
        {
            notes.Add(note);

            isAnimationFinished = false;
            UpdateGainLocationDisplay(note);
        }

        if (!isAnimationFinished)
        {
            yield return new WaitUntil(() => isAnimationFinished);
            yield return new WaitForSeconds(0.4f);
        }
    }

    private void UpdateGainLocationDisplay(ConspiracyNote note)
    {
        if (note != null)
        {
            gainDisplay.SetValues(note);
            gainNoteAnimator.SetBool("Open", true);
        }
    }

    public void CloseGainNoteDisplay()
    {
        gainNoteAnimator.SetBool("Open", false);
        isAnimationFinished = true;
    }

    public void StartConspiracy(ConspiracyBoard conspiracy)
    {
        if (notes.Count == 0)
        {
            ResponseManager.instance.ResponseResult(conspiracy.complete);
            return;
        }

        ropeCoroutine = StartCoroutine(RopeSimulation());

        currentBoard = conspiracy;

        firstNote = null;
        secondNote = null;
        confirmButton.SetActive(false);
        darken.SetActive(false);
        isConfirming = false;

        foreach (GameObject item in noteObjects)
        {
            Destroy(item);
        }
        noteObjects.Clear();

        board.SetActive(true);

        Vector2 tempOffset = new Vector2((Screen.width - noteSize.x) / 2, (Screen.height - noteSize.y) / 2) - edgeDistance;
        Debug.Log(tempOffset);

        bool abandone = false;

        foreach (var item in notes)
        {
            GameObject tempObject = Instantiate(noteObject, noteArea);
            tempObject.GetComponent<NoteDisplay>().SetValues(item);

            bool tempValid;
            int attempts = 0;
            do
            {
                tempValid = true;
                tempObject.GetComponent<RectTransform>().localPosition = new Vector2(Random.Range(tempOffset.x, -tempOffset.x), Random.Range(tempOffset.y, -tempOffset.y));

                foreach (var objects in noteObjects)
                {
                    if (WithinDistace(tempObject.GetComponent<RectTransform>().localPosition, objects.GetComponent<RectTransform>().localPosition))
                    {
                        tempValid = false;
                    }
                }
                attempts++;

            } while (!tempValid && attempts < 20);


            noteObjects.Add(tempObject);

            if (attempts >= 20)
            {
                abandone = true;
                break;
            }
        }

        if (abandone)
        {
            StartConspiracy(conspiracy);
            return;
        }

        Vector2 middleOffset = Vector2.zero;

        foreach (var item in noteObjects)
            middleOffset += (Vector2)item.transform.localPosition;

        middleOffset = new Vector2(middleOffset.x / noteObjects.Count, middleOffset.y / noteObjects.Count);

        foreach (var item in noteObjects)
        {
            item.transform.localPosition -= (Vector3)middleOffset;
            if (Mathf.Abs(item.transform.localPosition.x) > tempOffset.x || Mathf.Abs(item.transform.localPosition.y) > tempOffset.y)
            {
                abandone = true;
            }
        }

        if (abandone)
        {
            StartConspiracy(conspiracy);
            return;
        }
    }

    private IEnumerator Conspiracy()
    {
        yield return DialogueManager.instance.ConspiracyAnimation(true);
    }

    public void SelectNote(NoteDisplay note)
    {
        if (firstNote != null && secondNote != null && isConfirming)
        {
            isConfirming = false;
            confirmButton.SetActive(false);
            darken.SetActive(false);

            firstNote.MoveBack();
            secondNote.MoveBack();
        }

        if (firstNote == note)
        {
            firstNote = null;
            note.Select(false);
        }
        else if (secondNote == note)
        {
            secondNote = null;
            note.Select(false);
        }
        else if (firstNote == null)
        {
            firstNote = note;
            note.Select(true);
        }
        else if (secondNote == null)
        {
            secondNote = note;
            note.Select(true);
        }

        if (firstNote != null && secondNote != null && !isConfirming)
        {
            confirmButton.SetActive(true);
            darken.SetActive(true);
            darken.transform.SetAsLastSibling();
            firstNote.transform.SetAsLastSibling();
            secondNote.transform.SetAsLastSibling();

            isConfirming = true;

            if (firstNote.transform.position.x > secondNote.transform.position.x)
            {
                firstNote.MoveToMiddle(new Vector3(confirmNoteDistance, 0));
                secondNote.MoveToMiddle(new Vector3(-confirmNoteDistance, 0));
            }
            else
            {
                firstNote.MoveToMiddle(new Vector3(-confirmNoteDistance, 0));
                secondNote.MoveToMiddle(new Vector3(confirmNoteDistance, 0));
            }
        }
    }

    private IEnumerator RopeSimulation()
    {
        while (true)
        {
            rope.gameObject.SetActive(firstNote != null || secondNote != null);
            pin1.SetActive(true);
            pin2.SetActive(false);

            if (firstNote != null && secondNote != null)
            {
                SetRope(firstNote.transform.position + new Vector3(0, 140), secondNote.transform.position + new Vector3(0, 140));
                pin2.SetActive(true);
            }
            else if (firstNote != null && secondNote == null)
            {
                SetRope(firstNote.transform.position + new Vector3(0, 140), Input.mousePosition);
            }
            else if (firstNote == null && secondNote != null)
            {
                SetRope(secondNote.transform.position + new Vector3(0, 140), Input.mousePosition);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void SetRope(Vector2 from, Vector2 to)
    {
        rope.position = from;
        float angle = UtilsClass.GetAngleFromVectorFloat(from - to);
        rope.eulerAngles = new Vector3(0, 0, angle + 90);
        float dist = Vector3.Distance(from, to);
        rope.sizeDelta = new Vector2(rope.sizeDelta.x, dist);
    }

    public void CombineNotes()
    {
        StartCoroutine(CombineAnimation());
    }

    private IEnumerator CombineAnimation()
    {
        firstNote.StopCoroutine();
        secondNote.StopCoroutine();

        bool firstOnRight = firstNote.transform.position.x > secondNote.transform.position.x;
        float tempDistance = confirmNoteDistance;
        float time = Time.time;

        AudioManager.instance.PlaySFX("Note Spin");

        for (int i = 0; i < spinFrames * loopAmount; i++)
        {
            yield return new WaitForFixedUpdate();

            float percentage = i / (((float)spinFrames - 1) * loopAmount);
            float tempAngle = spinCurve.Evaluate(percentage) * loopAmount * 360;
            Vector2 pos;

            pos.x = (tempDistance * Mathf.Cos(tempAngle / (180f / Mathf.PI)));
            pos.y = (tempDistance * Mathf.Sin(tempAngle / (180f / Mathf.PI)));

            if (firstOnRight)
            {
                firstNote.transform.localPosition = pos;
                secondNote.transform.localPosition = -pos;
            }
            else
            {
                firstNote.transform.localPosition = -pos;
                secondNote.transform.localPosition = pos;
            }

            tempDistance = Mathf.Lerp(confirmNoteDistance, confirmNoteDistance + spinExpandDistance, percentage);
        }

        Debug.Log(Time.time - time);

        yield return new WaitForSeconds(0.5f);

        AudioManager.instance.PlaySFX("Open Case");

        for (int i = 0; i < slamFrames; i++)
        {
            yield return new WaitForFixedUpdate();
            tempDistance = Mathf.Lerp(confirmNoteDistance + spinExpandDistance, 0, (float)i / (slamFrames - 1));

            Vector2 pos = new Vector2(tempDistance, 0);

            if (firstOnRight)
            {
                firstNote.transform.localPosition = pos;
                secondNote.transform.localPosition = -pos;
            }
            else
            {
                firstNote.transform.localPosition = -pos;
                secondNote.transform.localPosition = pos;
            }
        }

        bool correct = false;

        foreach (var item in currentBoard.connections)
        {
            if ((item.note1 == firstNote.currentNote && item.note2 == secondNote.currentNote)
                || (item.note1 == secondNote.currentNote && item.note2 == firstNote.currentNote))
            {
                yield return new WaitForFixedUpdate();
                notes.Remove(item.note1);
                notes.Remove(item.note2);
                ResponseManager.instance.ResponseResult(item.result);
                AudioManager.instance.PlaySFX("Correct");
                correct = true;
                board.SetActive(false);
                StopCoroutine(ropeCoroutine);
                break;
            }
        }

        if (!correct)
        {
            StopCoroutine(ropeCoroutine);
            AudioManager.instance.PlaySFX("Wrong");

            for (float i = 0; i < fallFrames; i++)
            {
                rope.gameObject.SetActive(false);
                yield return new WaitForFixedUpdate();

                float percentage = i / (fallFrames - 1);
                float tempX = fallAwayCurve.Evaluate(percentage) * fallAwayDistance;
                float tempY = fallDownCurve.Evaluate(percentage) * Screen.height;

                if (firstOnRight)
                {
                    firstNote.transform.localPosition = new Vector3(tempX, -tempY);
                    secondNote.transform.localPosition = new Vector3(-tempX, -tempY);
                    firstNote.transform.eulerAngles = new Vector3(0, 0, -fallRotation * percentage);
                    secondNote.transform.eulerAngles = new Vector3(0, 0, fallRotation * percentage);
                }
                else
                {
                    firstNote.transform.localPosition = new Vector3(-tempX, -tempY);
                    secondNote.transform.localPosition = new Vector3(tempX, -tempY);
                    firstNote.transform.eulerAngles = new Vector3(0, 0, fallRotation * percentage);
                    secondNote.transform.eulerAngles = new Vector3(0, 0, -fallRotation * percentage);
                }
            }

            board.SetActive(false);
            ResponseManager.instance.ResponseResult(currentBoard.wrongAnswer);
        }
    }

    private bool WithinDistace(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) < noteSize.x + noteDistance && Mathf.Abs(pos1.y - pos2.y) < noteSize.y + noteDistance;
    }
}
