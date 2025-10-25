using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovementDisplay : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI position;
    [SerializeField] private TextMeshProUGUI delay;
    [SerializeField] private TextMeshProUGUI sprite;
    [SerializeField] private TextMeshProUGUI facing;
    [SerializeField] private TextMeshProUGUI id;
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;

    private CharacterMovement movement;

    public void SetValues(CharacterMovement movement)
    {
        this.movement = movement;

        icon.sprite = EditCharacterManager.instance.GetCharacterIcon(CharacterManager.instance.GetCharacterData(movement.character).Character);
        position.text = movement.xPos.ToString();
        delay.text = movement.delay.ToString();
        sprite.text = EditCharacterManager.instance.GetSpriteName(movement.animation);
        facing.text = movement.facingLeft ? "Left" : "Right";
        id.text = movement.displayID.ToString();
    }

    public void UpdateOrderButtons()
    {
        upButton.interactable = transform.GetSiblingIndex() != 0;
        downButton.interactable = transform.GetSiblingIndex() != transform.parent.childCount - 1;
    }

    public void UpOrderButton()
    {
        int index = transform.GetSiblingIndex();
        transform.SetSiblingIndex(index - 1);
        EditCharacterManager.instance.SwapMovementPositions(index, index - 1);
    }

    public void DownOrderButton()
    {
        int index = transform.GetSiblingIndex();
        transform.SetSiblingIndex(index + 1);
        EditCharacterManager.instance.SwapMovementPositions(index, index + 1);
    }

    public void Delete()
    {
        DialogueManager.instance.currentLine.DeleteMovement(movement);
        EditCharacterManager.instance.DeleteMovementDisplay(gameObject);
    }
}
