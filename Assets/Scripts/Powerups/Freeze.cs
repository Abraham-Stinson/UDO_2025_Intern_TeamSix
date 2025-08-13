using UnityEngine;
using System;
using UnityEngine.UI;

public class Freeze : Powerup
{
    [Header("Elements")] 
    [SerializeField] private Animator animator;
    [SerializeField] private Image iceFrameImage;
    
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float powerupDuration = 5f;
    
    [Header("Actions")] 
    public static Action started;

    private void Awake()
    {
        iceFrameImage.gameObject.SetActive(false);
    }

    public void TriggerPowerupStart()
    {
        started?.Invoke();
        
        ShowIceFrame();
        Invoke(nameof(HideIceFrame), powerupDuration);
    }

    public void Play()
    {
        animator.Play("FreezeActive");
        AudioManager.instance.PlayFreeze();

    }
    
    private void ShowIceFrame()
    {
        iceFrameImage.gameObject.SetActive(true);
        SetAlpha(0f);
        LeanTween.alpha(iceFrameImage.rectTransform, 0.6f, fadeDuration);
    }

    private void HideIceFrame()
    {
        LeanTween.alpha(iceFrameImage.rectTransform, 0f, fadeDuration);
    }
    
    private void SetAlpha(float value)
    {
        Color c = iceFrameImage.color;
        c.a = value;
        iceFrameImage.color = c;
    }
}
