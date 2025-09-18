using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDisplay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool startActive;

    private void Start()
    {
        SetActive(startActive);
    }

    public void SetActive(bool active)
    {
        animator.SetBool("Active", active);
    }
}
