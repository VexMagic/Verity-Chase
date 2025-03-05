using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCharacterSpawner : MonoBehaviour
{
    public static TitleCharacterSpawner instance;

    [SerializeField] private GameObject titleCharacter;
    [SerializeField] private float offsetValue;
    [SerializeField] private float timeBetween;
    [SerializeField] private float animationTime;
    [SerializeField] private Transform leftSide;
    [SerializeField] private Transform rightSide;
    [SerializeField] private AnimationClip animationName;
    [SerializeField] private Sprite[] characters;

    private bool isLeftOffset;
    private bool isLeftSide;
    private List<Sprite> spriteList = new List<Sprite>();
    private List<GameObject> characterObjects = new List<GameObject>();
    private Coroutine coroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (var item in characters)
        {
            spriteList.Add(item);
        }
        //StartCharacterDisplay();
    }

    public void StartCharacterDisplay()
    {
        coroutine = StartCoroutine(SpawnCharacters());
    }

    public void StopCharacterDisplay()
    {
        StopCoroutine(coroutine);
        foreach (var item in characterObjects)
        {
            Destroy(item);
        }
        characterObjects.Clear();
    }

    IEnumerator SpawnCharacters()
    {
        SpawnStartingCharacters();
        while (true)
        {
            yield return new WaitForSeconds(timeBetween / 2);

            if (isLeftSide)
                SpawnCharacter(leftSide, 0);
            else
            {
                SpawnCharacter(rightSide, 0);
                isLeftOffset = !isLeftOffset;
            }

            isLeftSide = !isLeftSide;
        }
    }

    private void SpawnStartingCharacters()
    {
        for (float i = animationTime - timeBetween; i >= 0; i -= timeBetween / 2)
        {
            if (isLeftSide)
                SpawnCharacter(leftSide, i);
            else
            {
                SpawnCharacter(rightSide, i);
                isLeftOffset = !isLeftOffset;
            }

            isLeftSide = !isLeftSide;
        }
    }

    private void SpawnCharacter(Transform transform, float startingFrame)
    {
        GameObject tempObject = Instantiate(titleCharacter, transform);

        Vector3 offset = new Vector3(offsetValue, 0);
        if (isLeftOffset)
            offset = new Vector3(-offsetValue, 0);

        tempObject.transform.localPosition = tempObject.transform.localPosition + offset;
        tempObject.GetComponent<TitleCharacter>().SetCharacter(GetRandomCharacter());
        tempObject.GetComponent<Animator>().Play(animationName.name, 0, startingFrame / animationTime);

        characterObjects.Add(tempObject);
    }

    private Sprite GetRandomCharacter()
    {
        int index = Random.Range(0, spriteList.Count);
        Sprite tempSprite = spriteList[index];
        spriteList.RemoveAt(index);

        return tempSprite;
    }

    public void ReturnSpriteToList(Sprite sprite)
    {
        spriteList.Add(sprite);
    }
}
