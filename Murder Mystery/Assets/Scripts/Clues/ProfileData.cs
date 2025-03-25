using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Profile", menuName = "Clue/Profile")]
public class ProfileData : ClueData
{
    [SerializeField] private Profile[] Versions;

    public Profile[] versions => Versions;
}

[Serializable]
public class GainProfile : GainClue
{
    [SerializeField] private ProfileData GainedProfile;

    public ProfileData gainedProfile => GainedProfile;

    public GainProfile(ProfileData data, int version)
    {
        GainedProfile = data;
        Version = version;
    }
}
