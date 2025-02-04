using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeChecker : MonoBehaviour
{
    public static ScreenSizeChecker instance;

    private float previousWidth;
    private float previousHeight;
    private List<UIManager> uIManagers = new List<UIManager>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            previousWidth = Screen.width;
            previousHeight = Screen.height;
            return;
        }
        Destroy(gameObject);
    }

    public void AddToList(UIManager manager)
    {
        if (!uIManagers.Contains(manager))
        {
            uIManagers.Add(manager);
        }
    }

    public void RemoveFromList(UIManager manager)
    {
        if (uIManagers.Contains(manager))
        {
            uIManagers.Remove(manager);
        }
    }

    private void Update()
    {
        if (previousWidth != Screen.width || previousHeight != Screen.height)
        {
            previousWidth = Screen.width;
            previousHeight = Screen.height;
            foreach (var item in uIManagers)
            {
                item.OnScreenSizeChange();
            }
            Debug.Log("Change Screen Size to " + Screen.width + "x" + Screen.height);
        }
    }
}