using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager instance;

    [SerializeField] private GameObject memoryDisplayParent;
    [SerializeField] private GameObject clickPosition;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject deduceButton;
    [SerializeField] private Vector2 worldMaxSize;
    [SerializeField] private AudioClip music;
    [SerializeField] private Animator fadeEffect;

    public Memory currentMemory;
    private MemoryObject[] MemoryObjects;
    private Coroutine coroutine;
    private List<MemoryObject> selectedMemoryObjects = new List<MemoryObject>();
    private bool IsMemoryActive;
    private bool isHovering;

    public bool isMemoryActive => IsMemoryActive;

    private void Awake()
    {
        instance = this;
        EndMemorySequence(true);
    }

    IEnumerator ClickDetection()
    {
        deduceButton.SetActive(true);
        if (!IsMemoryActive)
        {
            fadeEffect.SetTrigger("Start");
            yield return new WaitForSeconds(0.16f);
            foreach (Transform memory in memoryDisplayParent.transform)
            {
                if (memory.name == currentMemory.memoryName)
                {
                    memory.gameObject.SetActive(true);
                    MemoryObjects = memory.GetComponentsInChildren<MemoryObject>();
                }
                else if (memory.gameObject == clickPosition)
                {
                    memory.gameObject.SetActive(true);
                }
                else
                    memory.gameObject.SetActive(false);
            }
            fadeEffect.SetTrigger("End");
            yield return new WaitForSeconds(0.16f);

            yield return DialogueManager.instance.MemoryAnimation(true);
            IsMemoryActive = true;
        }

        AudioManager.instance.PlayMusic(music);
        exitButton.SetActive(currentMemory.exitable);

        CameraManager.instance.EndDialogue();
        LocationManager.instance.TurnOffLocations();
        SetAimPosition(Vector2.zero);
        while (IsMemoryActive)
        {
            if (Draging() && !ClueManager.instance.isOpen)
            {
                clickPosition.SetActive(true);
                Vector2 screenSize = new Vector2(Screen.width / 2, Screen.height / 2);
                Vector2 mousePos = (Vector2)Input.mousePosition - screenSize;

                Vector2 posPercentage = mousePos / screenSize;
                SetAimPosition(posPercentage * worldMaxSize);
            }

            if (DialogueManager.instance.HasPresented())
            {
                DialogueManager.instance.SetPresented(false);

                MemoryObject tempObject = null;

                foreach (var item in selectedMemoryObjects)
                {
                    foreach (var answer in item.correctClues)
                    {
                        var selectedClue = ClueManager.instance.GetSelectedClue(ClueManager.instance.currentMenuType);
                        if (selectedClue is GainEvidence)
                        {
                            if ((selectedClue as GainEvidence).gainedEvidence == answer.gainedClue)
                            {
                                tempObject = item;
                            }
                        }
                        else if (selectedClue is GainProfile)
                        {
                            if ((selectedClue as GainProfile).gainedProfile == answer.gainedClue)
                            {
                                tempObject = item;
                            }
                        }
                    }
                }

                ClueManager.instance.CloseMenu();

                if (tempObject != null)
                {
                    AudioManager.instance.PlaySFX("Correct");
                    ResponseManager.instance.ResponseResult(tempObject.result);
                }
                else
                    ResponseManager.instance.ResponseResult(currentMemory.wrongAnswer);
            }

            yield return new WaitForEndOfFrame();
        }

        yield return DialogueManager.instance.MemoryAnimation(false);

        fadeEffect.SetTrigger("Start");
        yield return new WaitForSeconds(0.16f);

        ResponseManager.instance.ResponseResult(currentMemory.complete);
        EndMemorySequence(false);

        fadeEffect.SetTrigger("End");
        yield return new WaitForSeconds(0.16f);
    }

    private void SetAimPosition(Vector2 position)
    {
        clickPosition.transform.localPosition = position;
        selectedMemoryObjects.Clear();
        if (MemoryObjects == null)
            return;

        foreach (var item in MemoryObjects)
        {
            if (item.isHovering)
            {
                selectedMemoryObjects.Add(item);
            }
        }
    }

    public void Deduce()
    {
        ClueManager.instance.OpenMenu(true, true);
    }

    public void Exit()
    {
        ResponseManager.instance.ResponseResult(currentMemory.giveUp);
        StopMemory();
    }

    public bool CheckMemoryComplete()
    {
        if (currentMemory.IsFinished())
        {
            StopMemory();
            return true;
        }
        return false;
    }

    private void EndCoroutine()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = null;
    }

    public void SetCurrentMemory(Memory newMemory)
    {
        currentMemory = newMemory;
        if (CheckMemoryComplete())
        {
            return;
        }

        EndCoroutine();
        coroutine = StartCoroutine(ClickDetection());
    }

    private void StopMemory()
    {
        IsMemoryActive = false;
    }

    private void EndMemorySequence(bool isStart)
    {
        exitButton.SetActive(false);
        deduceButton.SetActive(false);
        EndCoroutine();
        foreach (Transform memory in memoryDisplayParent.transform)
        {
            memory.gameObject.SetActive(false);
        }
        DialogueManager.instance.SetClickDetectionActive(true);
        if (!isStart)
        {
            LocationManager.instance.SetCurrentLocationDisplay();
        }
    }

    public bool Draging()
    {
        return Input.GetMouseButton(0) && isHovering;
    }

    public void StartMemoryHover() { isHovering = true; }

    public void EndMemoryHover() { isHovering = false; }
}
