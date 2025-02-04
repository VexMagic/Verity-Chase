using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Threading;

public class TextEffectManager : MonoBehaviour
{
    public static TextEffectManager instance;

    public TextEffect dialogueTextEffect;
    [SerializeField] private List<TextColor> colorValues;
    [SerializeField] private List<string> effectCommands = new List<string>();

    public float floatSpeed;
    public float floatWaviness;
    public float floatHeight;

    public float shakeSpeed;
    public float shakeMaxOffset;

    private Coroutine effectCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    struct Command
    {
        public string name;
        public int start;
    }

    public string ConvertCommands(string text, TextEffect textEffect = null)
    {
        //Clear();
        List<Command> commands = new List<Command>();

        if (textEffect == null)
            textEffect = dialogueTextEffect;

        textEffect.ClearStartAndEnd();

        int commandStart = 0;
        int lineChar = 1;
        bool isCommand = false;

        //convert the "triple dots" char into 3 separate periods
        if (text.Contains('…'))
        {
            string[] tempLines = text.Split('…');

            string tempNewText = tempLines[0];
            for (int i = 1; i < tempLines.Length; i++)
            {
                tempNewText += "..." + tempLines[i];
            }

            text = tempNewText;
        }

        for (int i = 0; i < text.Length; i++)
        {

            if (text[i] == '<') //check for command start
            {
                isCommand = true;
                //lineChar++;
                commandStart = i;
            }
            else if (text[i] == '>') //check for command end
            {
                isCommand = false;
                Command command = new Command();
                command.name = text.Substring(commandStart, i - commandStart + 1);
                lineChar--;
                command.start = lineChar;
                commands.Add(command);
            }

            if (!isCommand)
            {
                lineChar++;
            }
        }

        List<string> endCommands = new List<string>();
        TextEffect.startAndEnd startAndEnd = new TextEffect.startAndEnd();

        foreach (var item in commands)
        {
            if (item.name == "<>")
            {
                bool isEffectEnd = false;
                for (int i = 0; i < effectCommands.Count; i++)
                {
                    if (endCommands[^1] == ("</" + effectCommands[i] + ">"))
                    {
                        text = ReplaceText(text, "<>", string.Empty);
                        startAndEnd.end = item.start;
                        startAndEnd.type = (TextEffect.EffectType)i;
                        textEffect.AddStartAndEnd(startAndEnd);

                        //Debug.Log(startAndEnd.start + "-" + startAndEnd.end);
                        isEffectEnd = true;
                        break;
                    }
                }
                if (!isEffectEnd)
                {
                    text = ReplaceText(text, "<>", endCommands[^1]);
                }
                endCommands.Remove(endCommands[^1]);
            }

            //find color command
            foreach (var value in colorValues)
            {
                if (item.name == ("<" + value.command + ">"))
                {
                    text = ReplaceText(text, item.name, "<color=#" + ColorUtility.ToHtmlStringRGB(value.color) + ">");
                    endCommands.Add("</color>");
                    break;
                }
            }

            //find effect command
            for (int i = 0; i < effectCommands.Count; i++)
            {
                if (item.name == ("<" + effectCommands[i] + ">"))
                {
                    text = ReplaceText(text, item.name, string.Empty);
                    startAndEnd.start = item.start;
                    endCommands.Add("</" + effectCommands[i] + ">");
                    break;
                }
            }
        }

        return text;
    }

    public void ActivateEffects()
    {
        //DeactivateEffects(false);
        //if (dialogueTextEffect.startAndEnds.Count != 0)
        //{
        //    effectCoroutine = StartCoroutine(dialogueTextEffect.EffectCoroutine());
        //}
    }

    public void DeactivateEffects(bool clear)
    {
        if (effectCoroutine!= null)
            StopCoroutine(effectCoroutine);

        if (clear)
            Clear();
    }

    public void Clear()
    {
        dialogueTextEffect.ClearStartAndEnd();
    }

    private string ReplaceText(string text, string search, string replace)
    {
        var regex = new Regex(Regex.Escape(search));
        return regex.Replace(text, replace, 1);
    }

    [Serializable]
    public class TextColor
    {
        [SerializeField] private string Command;
        [SerializeField] private Color Color;
        public string command => Command;
        public Color color => Color;
    }
}
