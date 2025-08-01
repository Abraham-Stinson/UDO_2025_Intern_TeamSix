using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
  [Header(" Go Up Settings")] 
  [SerializeField] private float goUpDistance;
  [SerializeField] private float goUpDuration;
  [SerializeField] private LeanTweenType goUpEasing;
  
  [Header(" Smash Settings")] 
  [SerializeField] private float smmashDuration;
  [SerializeField] private LeanTweenType smashEasing;

  [Header(" Effects")] 
  [SerializeField] private ParticleSystem mergeParticles;
  
  public static event Action merged;
  
  private void Awake()
  {
    ItemSpotsManager.mergeStarted += OnMergeStarted;
  }

  private void OnMergeStarted(List<Item> items)
  {
    for (int i = 0; i < items.Count; i++)
    {
      Vector3 targetPos = items[i].transform.position + items[i].transform.up * goUpDistance;
      Action callback = null;

      if (i == 0)
        callback = () => SmashItems(items);

      LeanTween.move(items[i].gameObject, targetPos, goUpDuration)
        .setEase(goUpEasing)
        .setOnComplete(callback);

    }
  }

  private void SmashItems(List<Item> items)
  {
    items.Sort((a,b) => a.transform.position.x.CompareTo(b.transform.position.x));
    float targetX = items[1].transform.position.x;

    LeanTween.moveX(items[0].gameObject, targetX, smmashDuration)
      .setEase(smashEasing)
      .setOnComplete(() => FinalizeMerge(items));
  }

  private void FinalizeMerge(List<Item> items)
  {
    for (int i = 0; i < items.Count; i++)
    {
      Destroy(items[i].gameObject);

      /*
      ParticleSystem particles = Instantiate(mergeParticles, items[1].transform.position, quaternion.identity, transform);
      particles.Play(); // Bu birleştiği kısıma particle effekt yapıcaz bu sistem hiç optimize değil yalnız pool sistem kullanarak yaparız şimdilik bırakıyorum
      */
      
    }
    merged?.Invoke();

  }
}
