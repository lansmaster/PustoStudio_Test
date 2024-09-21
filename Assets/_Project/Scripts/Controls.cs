using System;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public event Action<bool> ChangeModeEnabled;
    public event Action ManuallyModeDisabled;
 
    public void ChangeTime()
    {
        ChangeModeEnabled?.Invoke(true);
    }

    public void ApplyNewTime()
    {
        ChangeModeEnabled?.Invoke(false);
    }

    public void LoadTimeFromServer()
    {
        ManuallyModeDisabled?.Invoke();
    }
}
