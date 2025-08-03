using UnityEngine;
using System;
public class Freeze : Powerup
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
        animator.Play("FreezeActive");
        AudioManager.instance.PlayFreeze();

    }
}
