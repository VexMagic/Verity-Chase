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
    private List<PersonDisplay> displays = new List<PersonDisplay>();

    private Coroutine movementCoroutine;
    private Vector2 selectedPersonStart;

    private void Start()
    {
        SetSelectedPersonActive(false);
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
            PersonDisplay tempDisplay = buttonObject.GetComponent<PersonDisplay>();
            tempDisplay.interview = person;

            Button tempButton = tempDisplay.button;

            tempButton.onClick.AddListener(() => LocationManager.instance.SetMultiplePeople(true));
            tempButton.onClick.AddListener(() => ResponseManager.instance.ResponseResult(person));
            tempButton.onClick.AddListener(() => SetActive(false));

            selectorObjects.Add(buttonObject);
            displays.Add(tempDisplay);
        }

        gameObject.SetActive(true);
    }

    public void SelectPerson(Interview person)
    {
        talkSelected.SetCharacter(person.character, person.animation);
        SetSelectedPersonActive(true);
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
        displays.Clear();
    }

    public void SetActive(bool isActive)
    {
        spawnParent.gameObject.SetActive(isActive);

        if (isActive)
            talkSelected.gameObject.SetActive(false);
    }

    public void SetButtonActive(bool isActive)
    {
        foreach (var item in displays)
        {
            item.SetButtonActive(isActive);
        }
    }
}
