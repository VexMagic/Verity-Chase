using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SOCreator : MonoBehaviour
{
    public static SOCreator instance;

    const string path = "Assets/Resources/lol.asset";

    private void Awake()
    {
        instance = this;
    }

    public void CreateSO()
    {
        TextAsset mytxtData = (TextAsset)Resources.Load("New Text");
        string[] txt = mytxtData.text.Split("\n");

        DialogueLine[] tempLines = new DialogueLine[txt.Length];

        for (int i = 0; i < txt.Length; i++)
        {
            txt[i] = txt[i].Replace("�", "'");

            string[] tempText = txt[i].Split(" - ");

            if (tempText.Length != 1)
            {
                Character tempCharacter = Character.None;

                if (tempText[0] == "???M")
                    tempCharacter = Character.UnknownMale;
                else if (tempText[0] == "???F")
                    tempCharacter = Character.UnknownFemale;
                else
                    tempCharacter = (Character)System.Enum.Parse(typeof(Character), tempText[0]);

                tempLines[i] = new DialogueLine(tempCharacter, tempText[1]);
            }
            else
            {
                tempLines[i] = new DialogueLine(Character.None, tempText[0]);
            }
        }

        Dialogue tempSO = new Dialogue(tempLines);

        AssetDatabase.CreateAsset(tempSO, path);
        AssetDatabase.SaveAssets();
    }
}
