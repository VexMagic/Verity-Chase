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

        if (LogManager.instance.HasFinishedInteraction(examineResult))
        {
            renderer.material = ExamineManager.instance.examined;
        }
        else
        {
            renderer.material = ExamineManager.instance.unExamined;
        }
    }

    public void EndHover()
    {
        renderer.sortingOrder -= 100;
        renderer.material = ExamineManager.instance.normal;
    }

    public void Examine()
    {
        ResponseManager.instance.ResponseResult(examineResult);
    }
}
