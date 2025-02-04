using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Profile2 : Clue2
{
    [SerializeField] private string Gender;
    [SerializeField] private string Age;
    public string gender => Gender;
    public string age => Age;
}

[CreateAssetMenu(fileName = "New Profile", menuName = "Clue/Profile")]
public class Profile : Clue
{
    [SerializeField] private string Gender;
    [SerializeField] private string Age;
    public string gender => Gender;
    public string age => Age;
}
