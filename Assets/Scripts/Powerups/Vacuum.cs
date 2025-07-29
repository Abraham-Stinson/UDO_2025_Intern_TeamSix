using System;
using UnityEngine;

public class Vacuum : Powerup
{
    [Header("Actions")] 
    public static Action started;

    public void TriggerPowerupStart()
    {
        started?.Invoke();
    }
}
