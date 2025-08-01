using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header(" SFX ")]
    [SerializeField] private AudioSource itemSelected;
    [SerializeField] private AudioSource itemPlaced;
    [SerializeField] private AudioSource itemsMerged;
    [SerializeField] private AudioSource cardComplete;

    private void Awake()
    {
        ItemSpotsManager.itemPickedUp += PlayItemSelected;
        ItemSpotsManager.itemPlaced     += PlayItemPlaced;

        MergeManager.merged += PlayItemsMerged;

        GoalCard.complete += PlayCardComplete;
    }

    private void OnDestroy()
    {
        ItemSpotsManager.itemPickedUp -= PlayItemSelected;
        ItemSpotsManager.itemPlaced -= PlayItemPlaced;

        MergeManager.merged -= PlayItemsMerged;

        GoalCard.complete -= PlayCardComplete;
    }

    private void PlayItemSelected(Item unused)
    {
        PlaySource(itemSelected, UnityEngine.Random.Range(.9f, 1f));
    }

    private void PlayItemPlaced()
    {
        //PlaySource(itemPlaced);
    }

    private void PlayItemsMerged()
    {
        PlaySource(itemsMerged);
    }

    private void PlayCardComplete()
    {
        PlaySource(cardComplete);
    }

    private void PlaySource(AudioSource source, float pitch)
    {
        source.pitch = pitch;
        PlaySource(source);
    }

    private void PlaySource(AudioSource source)
    {
        source.Play();
    }
}
