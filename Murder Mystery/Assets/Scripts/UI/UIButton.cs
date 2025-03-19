using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private GameObject outline;
    [SerializeField] private MaskableGraphic text;
    [SerializeField] private Color textNormal;
    [SerializeField] private Color textHighlight;

    [SerializeField] private UIButton upButton;
    [SerializeField] private UIButton downButton;
    [SerializeField] private UIButton leftButton;
    [SerializeField] private UIButton rightButton;

    [SerializeField] private UnityEvent OnSelect;
    [SerializeField] private UnityEvent OnKeyboardSelect;
    [SerializeField] private UnityEvent OnMouseSelect;
    [SerializeField] private UnityEvent OnDeselect;
    [SerializeField] private UnityEvent OnUp;
    [SerializeField] private UnityEvent OnDown;
    [SerializeField] private UnityEvent OnLeft;
    [SerializeField] private UnityEvent OnRight;

    public UnityEvent onSelect => OnSelect;
    public UnityEvent onKeyboardSelect => OnKeyboardSelect;

    private void Start()
    {
        if (button != null) 
            button.onClick.AddListener(() => AudioManager.instance.PlaySFX("Click"));
    }

    public void SelectButton(bool hoverSelect)
    { 
        if (outline != null)
        {
            if (hoverSelect && !outline.activeSelf)
                AudioManager.instance.PlaySFX("Select Button");
        }
        else if (hoverSelect)
            AudioManager.instance.PlaySFX("Select Button");

        ControlManager.instance.SetSelectedButton(this);
    }

    public bool IsInteractable => button.interactable;

    public void SetOutline(bool active, bool isKeyboard = false)
    {
        if (outline != null)
            outline.SetActive(active);

        if (buttonImage != null)
            buttonImage.enabled = !active;

        if (active)
        {
            OnSelect.Invoke();

            if (text != null)
                text.color = textHighlight;

            if (isKeyboard)
                OnKeyboardSelect.Invoke();
        }
        else
        {
            OnDeselect.Invoke();

            if (text != null)
                text.color = textNormal;
        }
    }

    public void SetOutlineInteractable()
    {
        if (button.interactable)
            SetOutlineColor(button.colors.normalColor);
        else
            SetOutlineColor(button.colors.disabledColor);
    }

    public void SetOutlineColor(Color color)
    {
        if (outline != null)
            outline.GetComponent<Image>().color = color;
    }

    public UIButton GetButtonInDirection(Vector2Int direction) 
    {
        if (direction == Vector2Int.up)
        {
            OnUp.Invoke();
            return upButton;
        }
        else if (direction == Vector2Int.down)
        {
            OnDown.Invoke();
            return downButton;
        }
        else if (direction == Vector2Int.left)
        {
            OnLeft.Invoke();
            return leftButton;
        }
        else if (direction == Vector2Int.right)
        {
            OnRight.Invoke();
            return rightButton;
        }

        return null;
    }

    public void SetButtonInDirection(UIButton button, Vector2Int direction)
    {
        if (button == null)
            return;

        if (direction == Vector2Int.up)
            upButton = button;
        else if (direction == Vector2Int.down)
            downButton = button;
        else if (direction == Vector2Int.left)
            leftButton = button;
        else if (direction == Vector2Int.right)
            rightButton = button;
    }

    public void SetAdjacent(UIButton up, UIButton down, UIButton left, UIButton right)
    {
        if (up != null)
            upButton = up;
        if (down != null)
            downButton = down;
        if (left != null)
            leftButton = left;
        if (right != null)
            rightButton = right;
    }

    public void Click()
    {
        if (button != null)
            button.onClick.Invoke();
    }

    public void DelayedClick()
    {
        Invoke(nameof(Click), 0.01f);
    }
}
