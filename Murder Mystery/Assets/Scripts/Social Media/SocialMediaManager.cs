using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialMediaManager : MonoBehaviour
{
    public void Discord()
    {
        LoadURL("https://discord.gg/TucG8cWX4s");
    }

    public void Bluesky()
    {
        LoadURL("https://bsky.app/profile/veritychase.bsky.social");
    }

    public void Twitter()
    {
        LoadURL("https://x.com/VerityChaseGame");
    }

    private void LoadURL(string url)
    {
        Application.OpenURL(url);
    }
}
