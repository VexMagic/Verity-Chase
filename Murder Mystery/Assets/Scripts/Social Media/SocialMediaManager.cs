using UnityEngine;
using System.Runtime.InteropServices;

public class SocialMediaManager : MonoBehaviour
{
    public static void OpenURL(string url)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        OpenTab(url);
        return;
#endif
        Application.OpenURL(url);
    }

    [DllImport("__Internal")]
    private static extern void OpenTab(string url);
}
