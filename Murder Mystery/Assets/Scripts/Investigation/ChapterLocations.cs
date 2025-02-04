using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChapterLocations
{
    [SerializeField] private List<PartLocations> PartLocations;
    public List<PartLocations> partLocations => PartLocations;
}

[Serializable]
public class PartLocations
{
    [SerializeField] private List<Location> Locations = new List<Location>();
    [SerializeField] private int CurrentLocation;
    public List<Location> locations => Locations;
    public int currentLocation => CurrentLocation;
}