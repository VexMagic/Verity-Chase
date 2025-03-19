using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyIcon : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private InputActionReference action;

    public void SetButtonSprite()
    {
        buttonIcon.sprite = KeyIconManager.instance.GetIcon("Keyboard", action);
    }

    private void OnDisable()
    {
        if (highlight != null)
            highlight.SetActive(false);
    }
}
