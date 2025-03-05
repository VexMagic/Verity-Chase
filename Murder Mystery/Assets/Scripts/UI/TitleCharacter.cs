using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleCharacter : MonoBehaviour
{
    [SerializeField] private Image character;

    public void Delete() 
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        TitleCharacterSpawner.instance.ReturnSpriteToList(character.sprite);
    }

    public void SetCharacter(Sprite sprite)
    {
        character.sprite = sprite;
    }
}
