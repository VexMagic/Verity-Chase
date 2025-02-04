using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class LogDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform totalArea, textArea;
    [SerializeField] private TextMeshProUGUI title, line;
    [SerializeField] private TextEffect textEffect;
    [SerializeField] private float titleHeight, lineHeight;

    private DialogueLine dialogue;
    private int numberOfLines = 0;
    private float height;

    public void SetValues(DialogueLine dialogueLine, bool sameChar)
    {
        dialogue = dialogueLine;

        string tempText = TextEffectManager.instance.ConvertCommands(dialogueLine.text, textEffect);

        line.text = tempText;

        numberOfLines = line.GetTextInfo(line.text).lineCount;
        numberOfLines = Mathf.Clamp(numberOfLines, 1, 100);

        string characterName = string.Empty;
        if (dialogueLine.talkingCharacter != Character.None)
            characterName = DialogueManager.instance.GetCharacterData(dialogueLine.talkingCharacter).title;

        if (characterName == string.Empty || sameChar)
        {
            title.gameObject.SetActive(false);

            if (sameChar && characterName != string.Empty)
                line.alignment = TextAlignmentOptions.TopLeft;
            else
                line.alignment = TextAlignmentOptions.Center;

            height = lineHeight * numberOfLines;
        }
        else
        {
            title.gameObject.SetActive(true);
            line.alignment = TextAlignmentOptions.TopLeft;

            height = (lineHeight * numberOfLines) + titleHeight;
        }

        title.text = characterName;
        totalArea.sizeDelta = new Vector2(totalArea.sizeDelta.x, height);
        textArea.sizeDelta = new Vector2(textArea.sizeDelta.x, lineHeight * numberOfLines);
    }

    public DialogueLine GetLine()
    {
        return dialogue;
    }

    public float GetHeight()
    {
        return height;
    }
}
