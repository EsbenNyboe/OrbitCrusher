using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class VoicePlayer : MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioSource;
    public AudioObject parent;
    private float pitchThisChild;
    private float pitchMin;
    private float pitchMax;
    [HideInInspector]
    public float volume;

    public void BuildVoice()
    {
        parent = GetComponentInParent<AudioObject>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = parent.loop;
    }

    float initialVol;
    bool fadeInOrOut;
    float fadeTime;
    float fadeDelay;
    float frameBuffer;

    public void FadeOutMethodNew(float t, bool inOrOut, float delay)
    {
        //Debug.Log("fadeOut: " + parent.name + " " + gameObject.name, gameObject);
        fadeTime = t;
        //fadeTime = t;
        fadeInOrOut = inOrOut;
        fadeDelay = delay;
        StopCoroutine(FadeOutCoroutine());
        StartCoroutine(FadeOutCoroutine());
    }
    private IEnumerator FadeOutCoroutine()
    {
        yield return new WaitForSeconds(fadeDelay);

        //Debug.Log("fadeOutStart: " + parent.name + " " + gameObject.name + " " + Time.time, gameObject);

        float timeStart = Time.time;
        while (timeStart + fadeTime > Time.time)
        {
            float vol = SoundManager.fadeAnimStatic.Evaluate((Time.time - timeStart) / fadeTime);
            if (fadeInOrOut)
                vol = vol * initialVol;
            else
                vol = (-vol + 1) * initialVol;

            volume = vol;
            audioSource.volume = volume * parent.volume;
            yield return null;
        }
        if (fadeInOrOut)
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
        fadeInOrOut = SoundManager.levelMusicFadeIn;
        StartCoroutine(LvlMusicFading());
    }
    private IEnumerator LvlMusicFading()
    {
        float timeStart = Time.time;
        while (timeStart + fadeTime > Time.time)
        {
            float vol = SoundManager.fadeAnimStatic.Evaluate((Time.time - timeStart) / fadeTime);
            if (fadeInOrOut)
                vol = vol * initialVol;
            else
                vol = (-vol + 1) * initialVol;

            volume = vol;
            audioSource.volume = volume * parent.volume;
            yield return null;
        }
        if (fadeInOrOut)
            audioSource.volume = initialVol;
        else
            audioSource.volume = 0;
    }




    public void ApplyVolumeChange()
    {
        volume = initialVol;
        audioSource.volume = volume * parent.volume;
    }
    public void ApplyPitchChange() // this could be optimized by replacing it with a "fade pitch" routine
    {
        if (pitchThisChild == 0)
            pitchThisChild = (pitchMin + pitchMax) * 0.5f;
        audioSource.pitch = pitchThisChild * parent.pitch;
    }
    private void SetPitchAndVolume(int selectedFile)
    {
        switch (parent.pitchAndVolLogic)
        {
            case AudioObject.Parameters.Individual:
                pitchMin = parent.soundMultiples[selectedFile].pitch.minValue;
                pitchMax = parent.soundMultiples[selectedFile].pitch.maxValue;
                volume = parent.soundMultiples[selectedFile].volume;
                break;
            case AudioObject.Parameters.ImitateParent:
                pitchMin = 1;
                pitchMax = 1;
                volume = 1;
                break;
        }
        initialVol = volume;
        //InspectorFeedbackForParameterOverwrite(selectedFile);
        pitchThisChild = Random.Range(pitchMin, pitchMax);
        audioSource.pitch = pitchThisChild * parent.pitch;
        ApplyVolumeChange();
    }
    private void InspectorFeedbackForParameterOverwrite(int selectedFile)
    {
        parent.soundMultiples[selectedFile].pitch.minValue = pitchMin;
        parent.soundMultiples[selectedFile].pitch.maxValue = pitchMax;
        parent.soundMultiples[selectedFile].volume = volume;
    }

    public void PlayAudioScheduled(int selectedFile, double scheduledTime)
    {
        StopAllCoroutines();
        audioSource.outputAudioMixerGroup = parent.output;
        audioSource.clip = parent.soundMultiples[selectedFile].soundFile;
        SetPitchAndVolume(selectedFile);
        audioSource.PlayScheduled(scheduledTime);
    }

    public void PlayAudio(int selectedFile)
    {
        StopAllCoroutines();
        audioSource.outputAudioMixerGroup = parent.output;
        audioSource.clip = parent.soundMultiples[selectedFile].soundFile;
        SetPitchAndVolume(selectedFile);
        audioSource.Play();
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
