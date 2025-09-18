using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConspiracyManager : MonoBehaviour
{
    public static ConspiracyManager instance;

    [SerializeField] private Animator gainNoteAnimator;
    [SerializeField] private NoteDisplay gainDisplay;

    [SerializeField] private GameObject board;
    [SerializeField] private Transform noteArea;
    [SerializeField] private GameObject darken;
    [SerializeField] private Transform notePlacements;
    [SerializeField] private GameObject connectButton;

    [SerializeField] private RectTransform rope;
    [SerializeField] private GameObject pin1;
    [SerializeField] private GameObject pin2;

    [SerializeField] private GameObject noteObject;
    [SerializeField] private GameObject noteCluePrefab;
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
    private List<NoteDisplay> noteDisplays = new List<NoteDisplay>();
    private GameObject noteClueObject;
    private NoteClueDisplay noteClueDisplay;
    private GainClue noteClueType;
    private bool isAnimationFinished = true;
    public bool isConfirming;
    private Coroutine ropeCoroutine;

    public ConspiracyBoard currentBoard;
    private NoteDisplay firstNote;
    private NoteDisplay secondNote;

    public bool isConspiracyActive;
    private Vector2 previousMousePos;
    [SerializeField] private GameObject mouseObject;
    [SerializeField] private float moveSpeed;
    private Vector2 screenMaxSize = new Vector2(960, 540);
    private Coroutine mouseCoroutine;
    private Coroutine clickCoroutine;
    private NoteDisplay hoverNote;
    private bool showMouse;
    private bool keyMovedMouseLast;

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

        if (conspiracy.canPresent)
            connectButton.SetActive(true);

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
        noteDisplays.Clear();

        board.SetActive(true);

        Vector2 flipModifier = new Vector2(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 2) == 0 ? 1 : -1);
        Debug.Log("Flip " + flipModifier);

        List<Vector3> placements = new List<Vector3>();
        foreach (Transform item in notePlacements.GetChild(notes.Count - 1))
        {
            placements.Add(item.localPosition * flipModifier);
        }

        foreach (var item in notes)
        {
            GameObject tempObject = Instantiate(noteObject, noteArea);
            NoteDisplay tempDisplay = tempObject.GetComponent<NoteDisplay>();
            tempDisplay.SetValues(item);

            int placementIndex = Random.Range(0, placements.Count);

            tempObject.GetComponent<RectTransform>().localPosition = placements[placementIndex];
            tempDisplay.SetLocalPos();
            placements.RemoveAt(placementIndex);

            noteObjects.Add(tempObject);
            noteDisplays.Add(tempDisplay);
        }

        HideHints();

        if (currentBoard.canPresent)
        {
            bool isDone = false;
            foreach (var connection in currentBoard.clueConnections)
            {
                foreach (var display in noteDisplays)
                {
                    if (connection.note == display.currentNote)
                    {
                        HintManager.instance.SetPresentAnswer(connection.clue);
                        isDone = true;
                        break;
                    }
                }
                if (isDone) 
                    break;
            }
            if (!isDone)
            {
                Debug.Log("can't present");
                HintManager.instance.SetPresentAnswer(new GainAnyClueType(), true);
            }
        }

        mouseObject.transform.SetAsLastSibling();
        isConspiracyActive = true;
        showMouse = true;

        if (mouseCoroutine == null)
            mouseCoroutine = StartCoroutine(Conspiracy());
        if (clickCoroutine == null)
            clickCoroutine = StartCoroutine(ClickDetection());
    }

    public void HideHints()
    {
        Debug.Log("hide hint");
        foreach (var item in noteDisplays)
        {
            item.HideHint();
        }
    }

    public void ShowHint(ConspiracyNote note, ConspiracyHintType type)
    {
        foreach (var item in noteDisplays)
        {
            if (item.currentNote == note)
            {
                item.ShowHint(type);
                break;
            }
        }
    }

    private void RemoveClueNote()
    {
        Destroy(noteClueObject);
        noteClueObject = null;
        noteClueDisplay = null;
        noteClueType = null;
    }

    public void AddNoteClue()
    {
        if (noteClueObject != null)
            RemoveClueNote();

        Vector2 flipModifier = new Vector2(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 2) == 0 ? 1 : -1);
        Debug.Log("Flip " + flipModifier);

        List<Vector3> placements = new List<Vector3>();
        foreach (Transform item in notePlacements.GetChild(notes.Count))
        {
            placements.Add(item.localPosition * flipModifier);
        }

        for (int i = 0; i < noteObjects.Count; i++)
        {
            int placementIndex = Random.Range(0, placements.Count);

            noteObjects[i].GetComponent<RectTransform>().localPosition = placements[placementIndex];
            noteDisplays[i].SetLocalPos();
            placements.RemoveAt(placementIndex);
        }

        noteClueObject = Instantiate(noteCluePrefab, noteArea);
        noteClueDisplay = noteClueObject.GetComponent<NoteClueDisplay>();

        noteClueType = ClueManager.instance.GetSelectedClue(ClueManager.instance.currentMenuType);
        if (noteClueType is GainEvidence)
            noteClueDisplay.SetValues((noteClueType as GainEvidence).gainedEvidence.title, (noteClueType as GainEvidence).gainedEvidence.versions[(noteClueType as GainEvidence).version].sprite);
        else if (noteClueType is GainProfile)
            noteClueDisplay.SetValues((noteClueType as GainProfile).gainedProfile.title, (noteClueType as GainProfile).gainedProfile.versions[(noteClueType as GainProfile).version].sprite);

        SelectNote(noteClueDisplay);

        noteClueObject.GetComponent<RectTransform>().localPosition = placements[0];
        noteClueDisplay.SetLocalPos();
    }

    private IEnumerator Conspiracy()
    {
        //yield return DialogueManager.instance.ConspiracyAnimation(true);
        while (isConspiracyActive)
        {
            if (previousMousePos != (Vector2)Input.mousePosition)
            {
                previousMousePos = (Vector2)Input.mousePosition;

                Vector2 screenSize = new Vector2(Screen.width / 2, Screen.height / 2);
                Vector2 mousePos = (Vector2)Input.mousePosition - screenSize;

                Vector2 posPercentage = mousePos / screenSize;
                SetAimPosition(posPercentage * screenMaxSize, true);
                mouseObject.SetActive(false);
                keyMovedMouseLast = false;
            }
            else if (ControlManager.instance.Movement != Vector2.zero && !ClueManager.instance.isOpen)
            {
                Vector2 newPos = (Vector2)mouseObject.transform.localPosition + (ControlManager.instance.Movement * moveSpeed);
                newPos = new Vector2(Mathf.Clamp(newPos.x, -screenMaxSize.x, screenMaxSize.x), Mathf.Clamp(newPos.y, -screenMaxSize.y, screenMaxSize.y));
                SetAimPosition(newPos, true);
                mouseObject.SetActive(showMouse);
                keyMovedMouseLast = true;
            }

            if (DialogueManager.instance.HasPresented())
            {
                DialogueManager.instance.SetPresented(false);
                Debug.Log("add clue note");
                AddNoteClue();
                ClueManager.instance.CloseMenu();
            }

            yield return new WaitForFixedUpdate();
        }
        mouseCoroutine = null;
    }


    private IEnumerator ClickDetection()
    {
        while (isConspiracyActive)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || ControlManager.instance.Progress());

            if (hoverNote != null && !ClueManager.instance.isOpen)
            {
                SelectNote(hoverNote);
            }
            yield return new WaitForFixedUpdate();
        }
        clickCoroutine = null;
    }

    public void SetAimPosition(Vector2 position, bool select)
    {
        ControlManager.instance.DeselectButton();
        mouseObject.transform.localPosition = position;
        if (noteDisplays.Count == 0)
            return;

        Vector2 tempPos = (position / screenMaxSize * screenMaxSize);

        if (hoverNote != null)
        {
            if (!hoverNote.IsPointInside(tempPos))
            {
                hoverNote = null;
            }
        }

        if (hoverNote == null && select)
        {
            if (isConfirming)
            {
                if (firstNote.IsPointInside(tempPos))
                    hoverNote = firstNote;
                else if (secondNote.IsPointInside(tempPos))
                    hoverNote = secondNote;
            }
            else
            {
                foreach (var item in noteDisplays)
                {
                    if (item.IsPointInside(tempPos))
                    {
                        hoverNote = item;
                        break;
                    }
                }

                if (noteClueDisplay != null)
                {
                    if (noteClueDisplay.IsPointInside(tempPos))
                        hoverNote = noteClueDisplay;
                }
            }
        }
    }

    public void SelectNote(NoteDisplay note)
    {
        if (!isConspiracyActive)
            return;

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
            mouseObject.transform.SetAsLastSibling();

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

        showMouse = ((firstNote != null) == (secondNote != null));
        if (keyMovedMouseLast) 
            mouseObject.SetActive(showMouse);
    }

    private IEnumerator RopeSimulation()
    {
        while (true)
        {
            rope.gameObject.SetActive(firstNote != null || secondNote != null);
            pin1.SetActive(true);
            pin2.SetActive(false);

            Vector3 offset1 = new Vector3(0, 140);
            Vector3 offset2 = new Vector3(0, 140);

            if (firstNote == noteClueDisplay)
                offset1 = new Vector3(0, 160);
            if (secondNote == noteClueDisplay)
                offset2 = new Vector3(0, 160);

            if (firstNote != null && secondNote != null)
            {
                SetRope(firstNote.transform.localPosition + offset1, secondNote.transform.localPosition + offset2);
                pin2.SetActive(true);
            }
            else if (firstNote != null && secondNote == null)
            {
                SetRope(firstNote.transform.localPosition + offset1, (Vector2)mouseObject.transform.localPosition);
            }
            else if (firstNote == null && secondNote != null)
            {
                SetRope(secondNote.transform.localPosition + offset2, (Vector2)mouseObject.transform.localPosition);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void SetRope(Vector2 from, Vector2 to)
    {
        //from += screenMaxSize;
        //to += screenMaxSize;

        rope.localPosition = from;
        float angle = UtilsClass.GetAngleFromVectorFloat(from - to);
        rope.eulerAngles = new Vector3(0, 0, angle + 90);
        float dist = Vector3.Distance(from, to);
        rope.sizeDelta = new Vector2(rope.sizeDelta.x, dist);
    }

    public void CombineNotes()
    {
        confirmButton.SetActive(false);
        StartCoroutine(CombineAnimation());
    }

    private IEnumerator CombineAnimation()
    {
        firstNote.StopCoroutine();
        secondNote.StopCoroutine();

        bool firstOnRight = firstNote.transform.position.x > secondNote.transform.position.x;
        float tempDistance = confirmNoteDistance;
        float time = Time.time;

        isConspiracyActive = false;

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

        ClueManager.instance.isConspiracyConnection = false;
        ClueManager.instance.CloseMenu();
        connectButton.SetActive(false);

        yield return new WaitForFixedUpdate();
        bool correct = IsConnectionCorrect();

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

    private bool IsConnectionCorrect()
    {
        bool correct = false;

        if (firstNote == noteClueDisplay || secondNote == noteClueDisplay)
        {
            foreach (var item in currentBoard.clueConnections)
            {
                if (item.note == firstNote.currentNote || item.note == secondNote.currentNote)
                {
                    if (noteClueType is GainEvidence)
                    {
                        if ((noteClueType as GainEvidence).gainedEvidence == item.clue.gainedClue)
                        {
                            notes.Remove(item.note);
                            RemoveClueNote();
                            ResponseManager.instance.ResponseResult(item.result);
                            AudioManager.instance.PlaySFX("Correct");
                            correct = true;
                            board.SetActive(false);
                            StopCoroutine(ropeCoroutine);
                            break;
                        }
                    }
                    else if (noteClueType is GainProfile)
                    {
                        if ((noteClueType as GainProfile).gainedProfile == item.clue.gainedClue)
                        {
                            notes.Remove(item.note);
                            RemoveClueNote();
                            ResponseManager.instance.ResponseResult(item.result);
                            AudioManager.instance.PlaySFX("Correct");
                            correct = true;
                            board.SetActive(false);
                            StopCoroutine(ropeCoroutine);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var item in currentBoard.noteConnections)
            {
                if ((item.note1 == firstNote.currentNote && item.note2 == secondNote.currentNote)
                    || (item.note1 == secondNote.currentNote && item.note2 == firstNote.currentNote))
                {
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
        }

        return correct;
    }

    private bool WithinDistace(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) < noteSize.x + noteDistance && Mathf.Abs(pos1.y - pos2.y) < noteSize.y + noteDistance;
    }
}
