using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{

    public AudioMixer mixer;
    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderSounds;

    private void Awake()
    {
        sliderMaster.value = 1;
        SetLevelMaster(sliderMaster.value);
    }

    public void SetLevelMaster(float sliderValue)
    {
        mixer.SetFloat("VolMaster", Mathf.Log10(sliderValue) * 20);
    }









    // delete these:
    void OldStart()
    {
        sliderMaster.value = PlayerPrefs.GetFloat("MasterVolume");
        if (sliderMaster.value == sliderMaster.minValue)
        {
            OldSetLevelMaster(sliderMaster.value);
        }
        sliderMusic.value = PlayerPrefs.GetFloat("MusicVolume");
        if (sliderMusic.value == sliderMusic.minValue)
        {
            OldSetLevelMusic(sliderMusic.value);
        }
        sliderSounds.value = PlayerPrefs.GetFloat("SoundsVolume");
        if (sliderSounds.value == sliderSounds.minValue)
        {
            OldSetLevelSounds(sliderSounds.value);
        }
    }
    public void OldSetLevelMaster(float sliderValue)
    {
        mixer.SetFloat("VolMaster", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }
    public void OldSetLevelMusic(float sliderValue)
    {
        mixer.SetFloat("VolMusic", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }
    public void OldSetLevelSounds(float sliderValue)
    {
        mixer.SetFloat("VolSounds", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SoundsVolume", sliderValue);
    }
}