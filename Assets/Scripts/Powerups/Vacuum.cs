using System;
using UnityEngine;

public class Vacuum : Powerup
{
    [Header("Elements")] 
    [SerializeField] private Animator animator;
    
    [Header("Actions")] 
    public static Action started;

    public void TriggerPowerupStart()
    {
        started?.Invoke();
    }

    public void Play()
    {
        animator.Play("VacuumActive");
    }
}
