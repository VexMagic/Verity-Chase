using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetDeviceBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    public void ResetAllBindings()
    {
        foreach (InputActionMap item in inputActions.actionMaps)
        {
            item.RemoveAllBindingOverrides();
        }
    }
}
