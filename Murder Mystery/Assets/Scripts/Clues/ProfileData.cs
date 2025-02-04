using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Profile", menuName = "Clue/Profile2")]
public class ProfileData : ClueData
{
    [SerializeField] private Profile2[] Versions;

    public Profile2[] versions => Versions;
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
