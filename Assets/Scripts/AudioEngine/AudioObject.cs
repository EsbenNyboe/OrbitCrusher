using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

public class AudioObject : MonoBehaviour
{
    public AudioMixerGroup output;
    [Range(0, 1)]
    public float initialVolume;
    [Range(0, 2)]
    public float initialPitch;
    public Sound[] soundMultiples;

    // these can almost always be ignored:
    public Parameters pitchAndVolLogic;
    public Seq seq;
    [Range(1, 20)]
    public int voiceMax;
    public Kill kill;

    // these can almost always be ignored:
    public bool playOnStart;
    public bool loop;
    public bool stopAfterFadeOut;

    // these can always be ignored:
    public float fadeTimeTestValue;
    public bool disableFading;
    public bool useAudioSourceInterface;

    [HideInInspector]
    public float volume;
    [HideInInspector]
    public float pitch;

    [System.Serializable]
    public class Sound
    {
        public AudioClip soundFile;
        [MinMaxRange(0.5f, 2)]
        public RangedFloat pitch;
        [Range(0, 1)]
        public float volume;
    }
    public enum Parameters
    {
        ImitateParent,
        Individual
    }
    public enum Kill
    {
        First,
        Last
    }
    public enum Seq
    {
        Random,
        Sequence,
        RandomNoRepeat
    }

    private int selectedFile = -1;
    private int prevFileSelection = -1;
    private int currentVoice = -1;
    private GameObject[] voice;

    [HideInInspector]
    public VoicePlayer[] voicePlayerNew;
    private float volumePrecision = 10000f;
    private float fadeVolDestinationValue;
    private float fadeVolDestinationTime;
    private float fadePitchDestinationTime;
    private float fadePitchDestinationValue;
    private float volumeLastFrame;
    private float pitchLastFrame;

    [HideInInspector]
    public AudioSource audioSource;

    private void Awake()
    {
        if (useAudioSourceInterface)
        {
            if (GetComponent<AudioSource>() == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            else
                audioSource = GetComponent<AudioSource>();
        }
        if (voiceMax < 1)
            voiceMax = 1;
        BuildVoices(voiceMax);
        volume = volumeLastFrame = fadeVolDestinationValue = initialVolume;
        pitch = pitchLastFrame = fadePitchDestinationValue = initialPitch;
        if (playOnStart)
            TriggerAudioObject();
    }
    private void Update()
    {
        if (volume != fadeVolDestinationValue && !disableFading)
        {
            ApplyFadingToTargetValue(ref volume, fadeVolDestinationValue, fadeVolDestinationTime);
            if (stopAfterFadeOut && volume == 0)
                StopAudioAllVoices();
        }
        if (pitch != fadePitchDestinationValue && !disableFading)
            ApplyFadingToTargetValue(ref pitch, fadePitchDestinationValue, fadePitchDestinationTime);
        SetVolumeAndPitch();
    }
    private void BuildVoices(int numberOfVoices)
    {
        voice = new GameObject[numberOfVoices];
        voicePlayerNew = new VoicePlayer[numberOfVoices];
        AudioSource[] audioSources = new AudioSource[numberOfVoices];
        for (int i = 0; i < numberOfVoices; i++)
        {
            voice[i] = new GameObject();
            voice[i].transform.parent = transform;
            voice[i].name = "voice" + i;
            voicePlayerNew[i] = voice[i].AddComponent<VoicePlayer>();
        }
    }
    private void PrepareVoicing()
    {
        currentVoice += 1;
        if (currentVoice > voiceMax - 1)
        {
            currentVoice = 0;
        }
    }
    private void ChooseSequence()
    {
        switch (seq)
        {
            case Seq.Random:
                selectedFile = Random.Range(0, soundMultiples.Length);
                break;
            case Seq.Sequence:
                selectedFile += 1;
                if (selectedFile > soundMultiples.Length - 1)
                {
                    selectedFile = 0;
                }
                break;
            case Seq.RandomNoRepeat:
                while (selectedFile == prevFileSelection)
                {
                    selectedFile = Random.Range(0, soundMultiples.Length);
                }
                prevFileSelection = selectedFile;
                break;
        }
    }
    private void SetVolumeAndPitch()
    {
        if (volumeLastFrame != volume)
            for (int i = 0; i < voiceMax; i++)
                voicePlayerNew[i].ApplyVolumeChange();
        volumeLastFrame = volume;
        if (pitchLastFrame != pitch)
            for (int i = 0; i < voiceMax; i++)
                voicePlayerNew[i].ApplyPitchChange();
        pitchLastFrame = pitch;
    }
    private void ApplyFadingToTargetValue(ref float targetValue, float destination, float time)
    {
        float fadeSlope = CalculateFadeSlope(targetValue, destination, time);
        float path = destination - targetValue;
        targetValue += fadeSlope;
        targetValue = RoundToVolumePrecisionValue(targetValue);
        if (path > 0 && targetValue >= destination)
            targetValue = destination;
        else if (path < 0 && targetValue <= destination)
            targetValue = destination;
    }
    // relative fade timing makes sure, that the fade speed is the same no matter the path length
    // absolute fade timing makes sure, that the fade time is the same no matter the path length
    private void SetRelativeFadeTiming(float relativeOrigin, float relativeDestination, ref float destinationTime, float origin, float destination)
    {
        float fadeSlope = CalculateFadeSlope(relativeOrigin, relativeDestination, destinationTime);
        destinationTime = CalculateFadeDestinationTime(fadeSlope, origin, destination);
    }
    private float CalculateFadeSlope(float origin, float destination, float destinationTime)
    {
        float path = destination - origin;
        float fadeTimeRemaining = Mathf.Abs(destinationTime - Time.time);
        float fadeSlope = path * (Time.deltaTime / fadeTimeRemaining);
        fadeSlope = RoundToVolumePrecisionValue(fadeSlope);
        return fadeSlope;
    }
    private float CalculateFadeDestinationTime(float fadeSlope, float origin, float destination)
    {
        float path = destination - origin;
        float fadeTimeRemaining = path * Time.deltaTime / fadeSlope;
        float fadeVolDestinationTime = fadeTimeRemaining + Time.time;
        return fadeVolDestinationTime;
    }
    private float RoundToVolumePrecisionValue(float value)
    {
        value = Mathf.RoundToInt(value * volumePrecision) / volumePrecision;
        return value;
    }






    public void TriggerSpecificSoundVariant(int selectedVariant)
    {
        PrepareVoicing();
        if (kill == Kill.Last)
        {
            if (voicePlayerNew[currentVoice].IsPlaying())
                return;
        }
        voicePlayerNew[currentVoice].PlayAudio(selectedVariant);
    }

    public IEnumerator TriggerSoundWithDelay(float time)
    {
        yield return new WaitForSeconds(time);
        TriggerAudioObject();
    }
    public void TriggerAudioObject()
    {
        PrepareVoicing();
        if (kill == Kill.Last)
        {
            if (voicePlayerNew[currentVoice].IsPlaying())
                return;
        }
        ChooseSequence();
        voicePlayerNew[currentVoice].PlayAudio(selectedFile);
    }
    public void StopAudioAllVoices()
    {
        for (int i = 0; i < voiceMax; i++)
        {
            if (voicePlayerNew[i] != null)
                voicePlayerNew[i].StopAudio();
        }
    }
    public void PauseAllVoices()
    {
        for (int i = 0; i < voiceMax; i++)
            voicePlayerNew[i].PauseAudio();
    }
    public void UnpauseAllVoices()
    {
        for (int i = 0; i < voiceMax; i++)
            voicePlayerNew[i].UnpauseAudio();
    }
    // when absolute-time is false, fadingTime refers to the time it takes to go from 0 to 1 volume - making the resulting fade time relative, aka. shorter
    public void VolumeChangeInParent(float volDestination, float fadingTime, bool absoluteTime)
    {
        fadeVolDestinationTime = Time.time + fadingTime;
        fadeVolDestinationValue = volDestination;
        if (fadeVolDestinationValue > 1)
            fadeVolDestinationValue = 1;
        else if (fadeVolDestinationValue < 0)
            fadeVolDestinationValue = 0;
        if (!absoluteTime)
        {
            if (fadeVolDestinationValue > volume)
                SetRelativeFadeTiming(0, 1, ref fadeVolDestinationTime, volume, fadeVolDestinationValue);
            else if (fadeVolDestinationValue < volume)
                SetRelativeFadeTiming(1, 0, ref fadeVolDestinationTime, volume, fadeVolDestinationValue);
        }
        if (fadingTime == 0)
            volume = fadeVolDestinationValue;
    }
    public void PitchChangeInParent(float pitchDestination, float fadingTime, bool absoluteTime)
    {
        fadePitchDestinationTime = Time.time + fadingTime;
        fadePitchDestinationValue = pitchDestination;
        if (fadePitchDestinationValue > 2)
            fadePitchDestinationValue = 2;
        else if (fadePitchDestinationValue < 0)
            fadePitchDestinationValue = 0;
        if (!absoluteTime)
        {
            if (fadePitchDestinationValue > pitch)
                SetRelativeFadeTiming(0, 1, ref fadePitchDestinationTime, pitch, fadePitchDestinationValue);
            else if (fadePitchDestinationValue < pitch)
                SetRelativeFadeTiming(1, 0, ref fadePitchDestinationTime, pitch, fadePitchDestinationValue);
        }
        if (fadingTime == 0)
            pitch = fadePitchDestinationValue;
    }
    public bool IsPlaying()
    {
        bool oneOrMoreIsPlaying = false;
        for (int i = 0; i < voiceMax; i++)
        {
            if (voicePlayerNew[i].IsPlaying())
                oneOrMoreIsPlaying = true;
        }
        return oneOrMoreIsPlaying;
    }


    // previewing audio events: locked to random-sequence & 1 voice. Not to be used in a game. Good for testing volume and random-pitch settings.
    public void PreviewAudioEvent(AudioSource audioSource)
    {
        selectedFile = Random.Range(0, soundMultiples.Length);
        audioSource.clip = soundMultiples[selectedFile].soundFile;
        audioSource.pitch = Random.Range(soundMultiples[selectedFile].pitch.minValue, soundMultiples[selectedFile].pitch.maxValue);
        audioSource.volume = soundMultiples[selectedFile].volume;
        audioSource.Play();
    }
}