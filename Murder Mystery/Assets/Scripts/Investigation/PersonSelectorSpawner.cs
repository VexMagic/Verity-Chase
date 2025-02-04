using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonSelectorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject selector;
    [SerializeField] private RectTransform spawnParent;
    [SerializeField] private CharacterDisplay talkSelected;
    [SerializeField] private float moveSpeed;

    private List<GameObject> selectorObjects = new List<GameObject>();
    private List<Button> selectorButtons = new List<Button>();

    private Coroutine movementCoroutine;
    private Vector2 selectedPersonStart;

    private void Start()
    {
        talkSelected.gameObject.SetActive(false);
        selectedPersonStart = talkSelected.transform.position;
    }

    public void SpawnPeople(LocationState people)
    {
        DestroyPeople();

        foreach (Interview person in people.people)
        {
            GameObject buttonObject = Instantiate(selector, spawnParent);
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = DialogueManager.instance.GetCharacterData(person.characterName).title;
            buttonObject.GetComponentInChildren<CharacterDisplay>().SetCharacter(person.character, person.animation);
            Button tempButton = buttonObject.GetComponentInChildren<Button>();

            tempButton.onClick.AddListener(() => LocationManager.instance.SetMultiplePeople(true));
            tempButton.onClick.AddListener(() => ResponseManager.instance.ResponseResult(person));
            tempButton.onClick.AddListener(() => SetActive(false));

            selectorObjects.Add(buttonObject);
            selectorButtons.Add(tempButton);
        }

        gameObject.SetActive(true);
    }

    public void SelectPerson(Interview person)
    {
        talkSelected.SetCharacter(person.character, person.animation);
        talkSelected.gameObject.SetActive(true);
    }

    public void SetSelectedPersonActive(bool active)
    {
        talkSelected.gameObject.SetActive(active);
    }

    public void DestroyPeople()
    {
        foreach (var item in selectorObjects)
        {
            Destroy(item);
        }
        selectorObjects.Clear();
        selectorButtons.Clear();
    }

    public void SetActive(bool isActive)
    {
        spawnParent.gameObject.SetActive(isActive);

        if (isActive)
            talkSelected.gameObject.SetActive(false);
    }

    public void SetButtonActive(bool isActive)
    {
        foreach (var item in selectorButtons)
        {
            item.gameObject.SetActive(isActive);
        }
    }
}
