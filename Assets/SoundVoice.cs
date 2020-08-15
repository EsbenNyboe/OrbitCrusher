using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundVoice: MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public SoundBehavior parentSoundBehavior;
    [HideInInspector]
    public float volume;
    float initialVol;
    public static bool developerMode;

    private float pitch;
    private float pitchMin;
    private float pitchMax;

    bool fadeType;
    float fadeTime;
    float fadeDelay;
    float frameBuffer;

    void Awake()
    {
        developerMode = true;
        if (developerMode)
            LoadVariables(); 
    }
    public void LoadVariables()
    {
        parentSoundBehavior = GetComponentInParent<SoundBehavior>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = parentSoundBehavior.loop;

        initialVol = volume;
        audioSource.volume = volume * parentSoundBehavior.volume;
    }





    public void PlayAudioScheduled(int selectedFile, double scheduledTime)
    {
        StopAllCoroutines();
        audioSource.outputAudioMixerGroup = parentSoundBehavior.output;
        audioSource.clip = parentSoundBehavior.soundMultiples[selectedFile].soundFile;
        SetPitchAndVolume(selectedFile);
        audioSource.PlayScheduled(scheduledTime);
    }
    public void PlayAudio(int selectedFile)
    {
        StopAllCoroutines();
        audioSource.outputAudioMixerGroup = parentSoundBehavior.output;
        audioSource.clip = parentSoundBehavior.soundMultiples[selectedFile].soundFile;
        SetPitchAndVolume(selectedFile);
        audioSource.Play();
    }


    public void FadingThisVoiceOnly(bool inOrOut, float t, float delay)
    {
        fadeType = inOrOut;
        fadeTime = t;
        fadeDelay = delay;
        StopCoroutine(FadeProcedure());
        StartCoroutine(FadeProcedure());
    }
    private IEnumerator FadeProcedure()
    {
        yield return new WaitForSeconds(fadeDelay);

        float timeStart = Time.time;
        while (timeStart + fadeTime > Time.time)
        {
            float vol = SoundManager.fadeAnimStatic.Evaluate((Time.time - timeStart) / fadeTime);
            if (fadeType)
                vol = vol * initialVol;
            else
                vol = (-vol + 1) * initialVol;

            volume = vol;
            audioSource.volume = volume * parentSoundBehavior.volume;
            yield return null;
        }
        if (fadeType)
            audioSource.volume = initialVol;
        else
            audioSource.volume = 0;

        float fadeTimeOccurred = Time.time - timeStart;
        //Debug.Log("fadeOutDone: " + parent.name + " " + gameObject.name + " " + fadeTimeOccurred, gameObject);
    }








    public void LetMeDoTheCoroutiningOkPlz()
    {
        //print("old fading");
        fadeTime = SoundManager.levelMusicFadeTime;
        fadeType = SoundManager.levelMusicFadeIn;
        StartCoroutine(LvlMusicFading());
    }
    private IEnumerator LvlMusicFading()
    {
        float timeStart = Time.time;
        while (timeStart + fadeTime > Time.time)
        {
            float vol = SoundManager.fadeAnimStatic.Evaluate((Time.time - timeStart) / fadeTime);
            if (fadeType)
                vol = vol * initialVol;
            else
                vol = (-vol + 1) * initialVol;

            volume = vol;
            audioSource.volume = volume * parentSoundBehavior.volume;
            yield return null;
        }
        if (fadeType)
            audioSource.volume = initialVol;
        else
            audioSource.volume = 0;
    }
    public void ApplyVolumeChange()
    {
        volume = initialVol;
        audioSource.volume = volume * parentSoundBehavior.volume;
    }






    public void ApplyPitchChange() // this could be optimized by replacing it with a "fade pitch" routine
    {
        if (pitch == 0)
            pitch = (pitchMin + pitchMax) * 0.5f;
        audioSource.pitch = pitch * parentSoundBehavior.pitch;
    }
    private void SetPitchAndVolume(int selectedFile)
    {
        switch (parentSoundBehavior.pitchAndVolLogic)
        {
            case SoundBehavior.Parameters.Individual:
                pitchMin = parentSoundBehavior.soundMultiples[selectedFile].pitch.minValue;
                pitchMax = parentSoundBehavior.soundMultiples[selectedFile].pitch.maxValue;
                volume = parentSoundBehavior.soundMultiples[selectedFile].volume;
                break;
            case SoundBehavior.Parameters.ImitateParent:
                pitchMin = 1;
                pitchMax = 1;
                volume = 1;
                break;
        }
        initialVol = volume;
        //InspectorFeedbackForParameterOverwrite(selectedFile);
        pitch = Random.Range(pitchMin, pitchMax);
        audioSource.pitch = pitch * parentSoundBehavior.pitch;
        ApplyVolumeChange();
    }





    
    public void StopAudio()
    {
        audioSource.Stop();
    }
    public void PauseAudio()
    {
        audioSource.Pause();
    }
    public void UnpauseAudio()
    {
        audioSource.UnPause();
    }
    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
}
