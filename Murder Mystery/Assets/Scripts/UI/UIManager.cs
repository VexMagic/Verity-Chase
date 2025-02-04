using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIManager : MonoBehaviour
{
    private void Start()
    {
        ScreenSizeChecker.instance.AddToList(this);
    }

    private void OnDestroy()
    {
        ScreenSizeChecker.instance.RemoveFromList(this);
    }

    public virtual void OnScreenSizeChange() { }
}
