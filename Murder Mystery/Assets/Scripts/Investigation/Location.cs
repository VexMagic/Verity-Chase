using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Location", menuName = "Location")]
public class Location : ScriptableObject
{
    [SerializeField] private string Title;
    [SerializeField] private Sprite Preview;
    [SerializeField] private LocationState[] PeopleAtLocations;

    public string title => Title;
    public Sprite preview => Preview;
    public LocationState[] peopleAtLocations => PeopleAtLocations;

    public LocationState CurrentLocationState() 
	{
        if (peopleAtLocations.Length == 0)
            return null;

        for (int i = peopleAtLocations.Length - 1; i >= 0; i--)
        {
            if (peopleAtLocations[i].IsAvailable())
            {
                return peopleAtLocations[i];
            }
        }

        return null;
    }
}

[Serializable]
public class LocationState
{
    [SerializeField] private string LocationName;
    [SerializeField] private Interaction EnterInteraction;
    [SerializeField] private AudioClip AreaMusic; 
    [SerializeField] private Interview[] People;
    [SerializeField] private Interaction[] Conditions;

    public string locationName => LocationName;
    public Interaction enterInteraction => EnterInteraction;
    public AudioClip areaMusic => AreaMusic;
    public Interview[] people => People;
    public Interaction[] condition => Conditions;

    public bool HasSeenEnterInteraction() => LogManager.instance.HasFinishedInteraction(EnterInteraction) || EnterInteraction == null;
    public bool IsAvailable() => LogManager.instance.HasFinishedMultipleInteractions(Conditions) || Conditions == null;
}
