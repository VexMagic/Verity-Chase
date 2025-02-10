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

    private Memory currentMemory;
    private MemoryObject[] MemoryObjects;
    private Coroutine coroutine;
    private MemoryObject selectedObject;

    private void Awake()
    {
        instance = this;
        EndMemorySequence();
    }

    IEnumerator ClickDetection()
    {
        SetAimPosition(Vector2.zero);
        while (true)
        {
            if (DialogueManager.instance.Progress(true))
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

                bool isCorrect = false;

                if (selectedObject != null)
                {
                    foreach (var answer in selectedObject.correctClues)
                    {
                        var selectedClue = ClueManager.instance.GetSelectedClue(ClueManager.instance.currentMenuType);
                        if (selectedClue is GainEvidence)
                        {
                            if ((selectedClue as GainEvidence).gainedEvidence == answer.gainedClue)
                            {
                                isCorrect = true;
                            }
                        }
                        else if (selectedClue is GainProfile)
                        {
                            if ((selectedClue as GainProfile).gainedProfile == answer.gainedClue)
                            {
                                isCorrect = true;
                            }
                        }
                    }
                }

                ClueManager.instance.CloseMenu();

                if (isCorrect)
                {
                    AudioManager.instance.PlaySFX("Correct");
                    ResponseManager.instance.ResponseResult(selectedObject.result);
                }
                else
                    ResponseManager.instance.ResponseResult(currentMemory.wrongAnswer);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void SetAimPosition(Vector2 position)
    {
        clickPosition.transform.localPosition = position;
        selectedObject = GetSelectedMemoryObject(position);
    }

    private MemoryObject GetSelectedMemoryObject(Vector2 point)
    {
        if (MemoryObjects == null)
            return null;

        foreach (var item in MemoryObjects)
        {
            if (item.IsPointInsideCollider(point))
            {
                return item;
            }
        }

        return null;
    }

    public void Deduce()
    {
        ClueManager.instance.OpenMenu(true, true);
    }

    public void Exit()
    {
        ResponseManager.instance.ResponseResult(currentMemory.giveUp);
        EndMemorySequence();
    }

    public bool CheckMemoryComplete()
    {
        if (currentMemory.IsFinished())
        {
            ResponseManager.instance.ResponseResult(currentMemory.complete);
            EndMemorySequence();
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

        exitButton.SetActive(newMemory.exitable);
        deduceButton.SetActive(true);
        EndCoroutine();
        coroutine = StartCoroutine(ClickDetection());
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
    }

    private void EndMemorySequence()
    {
        exitButton.SetActive(false);
        deduceButton.SetActive(false);
        EndCoroutine();
        foreach (Transform memory in memoryDisplayParent.transform)
        {
            memory.gameObject.SetActive(false);
        }
    }
}
