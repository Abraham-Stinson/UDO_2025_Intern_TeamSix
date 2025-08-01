using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SettingsManager : MonoBehaviour
{
    #region Music and SFX settings
    [SerializeField] private Slider musicVol, sfxVol;
    [SerializeField] private AudioMixer audioMixer;

    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", musicVol.value);
    }
    public void ChangeSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", sfxVol.value);
    }
    #endregion
    #region Vibration

    //!
    public static bool isVibrate = true;
    [SerializeField] private Toggle vibrationToggle;

    public void ChangVibration()
    {
        isVibrate = vibrationToggle.isOn;
    }
    #endregion
}
