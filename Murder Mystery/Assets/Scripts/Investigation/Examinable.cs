using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examinable : MonoBehaviour
{
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private new Collider2D collider;
    [SerializeField] private Interaction examineResult;

    private void Start()
    {
        renderer.material = ExamineManager.instance.normal;
        if (examineResult == null)
        {
            collider.enabled = false;
        }
    }

    public void StartHover()
    {
        if (examineResult == null)
            return;

        renderer.sortingOrder += 100;

        if (HasBeenExamined())
        {
            renderer.material = ExamineManager.instance.examined;
        }
        else
        {
            renderer.material = ExamineManager.instance.unExamined;
        }
    }

    public bool HasBeenExamined()
    {
        return LogManager.instance.HasFinishedInteraction(examineResult);
    }

    public void EndHover()
    {
        renderer.sortingOrder -= 100;
        renderer.material = ExamineManager.instance.normal;
    }

    public void Examine()
    {
        if (!MemoryManager.instance.isMemoryActive)
            ResponseManager.instance.ResponseResult(examineResult);
    }
}
