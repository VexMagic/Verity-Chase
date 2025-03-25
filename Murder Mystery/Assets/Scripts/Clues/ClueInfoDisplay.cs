using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI evidenceDescription;
    [SerializeField] private TextMeshProUGUI profileDescription;
    [SerializeField] private TextMeshProUGUI age;
    [SerializeField] private TextMeshProUGUI gender;
    [SerializeField] private GameObject evidence;
    [SerializeField] private GameObject profile;
    [SerializeField] private Image image;

    public void SetValues(GainClue clue)
    {
        if (clue is GainAnyClueType)
        {
            int version = clue.version;
            var tempData = (clue as GainAnyClueType).gainedClue;
            if (tempData is EvidenceData)
            {
                Evidence tempEvidence = (tempData as EvidenceData).versions[version];

                title.text = (tempData as EvidenceData).title;
                image.sprite = tempEvidence.sprite;
                evidenceDescription.text = tempEvidence.description;
                evidence.SetActive(true);
                profile.SetActive(false);
            }
            else if (tempData is ProfileData)
            {
                Profile tempProfile = (tempData as ProfileData).versions[version];

                title.text = (tempData as ProfileData).title;
                image.sprite = tempProfile.sprite;
                profileDescription.text = tempProfile.description;
                age.text = tempProfile.age;
                gender.text = tempProfile.gender;
                evidence.SetActive(false);
                profile.SetActive(true);
            }
        }
        else if (clue is GainEvidence)
        {
            Evidence tempEvidence = (clue as GainEvidence).gainedEvidence.versions[(clue as GainEvidence).version];
            
            title.text = (clue as GainEvidence).gainedEvidence.title;
            image.sprite = tempEvidence.sprite;
            evidenceDescription.text = tempEvidence.description;
            evidence.SetActive(true);
            profile.SetActive(false);
        }
        else if (clue is GainProfile)
        {
            Profile tempProfile = (clue as GainProfile).gainedProfile.versions[(clue as GainProfile).version];

            title.text = (clue as GainProfile).gainedProfile.title;
            image.sprite = tempProfile.sprite;
            profileDescription.text = tempProfile.description;
            age.text = tempProfile.age;
            gender.text = tempProfile.gender;
            evidence.SetActive(false);
            profile.SetActive(true);
        }
    }
}
