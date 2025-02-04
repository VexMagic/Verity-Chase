using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    public void ChangeLocation()
    {
        LocationManager.instance.ChangeLocation(false);
    }
}
