using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using System;

public class Typewriter : MonoBehaviour
{
    public static Typewriter instance;

    [SerializeField] private float typewriterSpeed;

    public bool isRunning { get; private set; }
    public int currentChar { get; private set; }

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        //new Punctuation(new HashSet<char>(){'…'}, 0.8f),
        new Punctuation(new HashSet<char>(){'.', '!', '?'}, 0.4f),
        new Punctuation(new HashSet<char>(){',', ';', ':'}, 0.2f),
    };

    private readonly HashSet<char> vowels = new HashSet<char>{ 'a', 'e', 'i', 'o', 'u', 'y' };
    private readonly HashSet<string> abbreviations = new HashSet<string>{ "st.", "mr.", "ms.", "dr." };

    private readonly HashSet<char> commandStart = new HashSet<char>{ '<' };
    private readonly HashSet<char> commandEnd = new HashSet<char>{ '>' };

    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Run(string textToType, TextMeshProUGUI textLabel)
    {
        typingCoroutine = StartCoroutine(TypeText(textToType, textLabel));
    }

    public void Stop()
    {
        StopCoroutine(typingCoroutine);
        isRunning = false;
    }

    private IEnumerator TypeText(string textToType, TextMeshProUGUI textLabel)
    {
        textLabel.text = string.Empty;
        string text = TextEffectManager.instance.ConvertCommands(textToType);

        isRunning = true;
        float t = 1;
        int charIndex = 0;

        text = FormateLine(text, textLabel);

        while (charIndex < text.Length)
        {
            int lastCharIndex = charIndex;

            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);
            currentChar = charIndex;
            charIndex = Mathf.Clamp(charIndex, 0, text.Length);
            bool isCommand = false;

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= text.Length - 1;
                if (!isCommand && IsCommandStart(text[i]))
                {
                    isCommand = true;
                }
                
                if (isCommand)
                {
                    if (charIndex < text.Length)
                    {
                        charIndex++;
                        t++;
                    }
                    if (IsCommandEnd(text[i]))
                    {
                        isCommand = false;
                    }
                }

                textLabel.text = text.Substring(0, i + 1);

                if (IsPunctuation(text[i], out float waitTime) && !isLast && (text[i + 1] == ' ' || IsPunctuation(text[i + 1], out _)))
                {
                    bool isAbbreviation = false;
                    if (i - 2 == 0 && abbreviations.Contains(textLabel.text.ToLower()))
                    {
                        isAbbreviation = true;
                    }
                    else if (i - 2 > 0)
                    {
                        if (abbreviations.Contains(text.Substring(i - 2, 3).ToLower()))
                        {
                            isAbbreviation = true;
                        }
                    }

                    if (!isAbbreviation)
                    {
                        TextEffectManager.instance.ActivateEffects();
                        if (IsPunctuation(text[i + 1], out _))
                            yield return new WaitForSeconds(waitTime / 2);
                        else
                            yield return new WaitForSeconds(waitTime);
                    }
                }

                //sound effect
                if (!isLast)
                {
                    if (IsVowel(text[i + 1]) && !IsVowel(text[i]))
                    {
                        AudioManager.instance.PlayBlip();
                    }
                    //if ((text[i + 1] == ' ' && text[i] != ' ') || (!IsPunctuation(text[i], out _) && IsPunctuation(text[i + 1], out _)))
                    //{
                    //    AudioManager.instance.PlayBlip();
                    //}
                }
            }

            //TextEffectManager.instance.dialogueTextEffect.UpdateEffect();

            yield return null;
        }

        isRunning = false;
    }

    private string FormateLine(string textToType, TextMeshProUGUI textLabel)
    {
        string tempLine = string.Empty;
        string tempSegment = string.Empty;

        float size = textLabel.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta.x;

        float textBoxSize = /*textLabel.GetComponent<RectTransform>().sizeDelta.x*/ size - 440;

        foreach (var item in textToType.Split(' '))
        {
            if (item.Contains("\n"))
            {
                tempSegment = string.Empty;
                //tempLength = 0;
            }

            if (tempSegment == string.Empty)
            {
                //do nothing
            }
            else if (GetTextWidth(tempSegment + " " + item, textLabel) > textBoxSize)
            {
                tempSegment = string.Empty;
                //tempLength = 0;
                tempLine += "\n";
            }
            else if (tempLine != string.Empty)
            {
                tempSegment += " ";
                tempLine += " ";
                //tempLength++;
            }

            tempLine += item;
            tempSegment += item;
            //tempLength += item.Length;
        }

        return tempLine;
    }

    private float GetTextWidth(string text, TextMeshProUGUI textLabel)
    {
        string tempText = textLabel.text;
        textLabel.text = text;
        textLabel.ForceMeshUpdate();
        float textWidth = textLabel.preferredWidth;
        textLabel.text = tempText;
        textLabel.ForceMeshUpdate();
        return textWidth;
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.Punctuations.Contains(character))
            {
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }

        waitTime = default;
        return false;
    }

    private bool IsVowel(char character)
    {
        return vowels.Contains(char.ToLower(character));
    }

    private bool IsCommandStart(char character)
    {
        if (commandStart.Contains(character))
        {
            return true;
        }
        return false;
    }

    private bool IsCommandEnd(char character)
    {
        if (commandEnd.Contains(character))
        {
            return true;
        }
        return false;
    }

    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}
