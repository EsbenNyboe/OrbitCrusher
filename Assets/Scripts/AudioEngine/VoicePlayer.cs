using UnityEngine;
using UnityEngine.Audio;

public class VoicePlayer : MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioSource;
    AudioObject parent;
    private float pitchThisChild;
    private float pitchMin;
    private float pitchMax;
    private float volume;

    void Awake()
    {
        parent = GetComponentInParent<AudioObject>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = parent.loop;
        if (parent.useAudioSourceInterface) // this is not used, but could come in handy, if 3D spatialization becomes relevant
        {
            audioSource.dopplerLevel = parent.audioSource.dopplerLevel;
            audioSource.maxDistance = parent.audioSource.maxDistance;
            audioSource.minDistance = parent.audioSource.minDistance;
            audioSource.priority = parent.audioSource.priority;
            audioSource.reverbZoneMix = parent.audioSource.reverbZoneMix;
            audioSource.rolloffMode = parent.audioSource.rolloffMode;
            audioSource.spatialBlend = parent.audioSource.spatialBlend;
            audioSource.spatialize = parent.audioSource.spatialize;
            audioSource.spread = parent.audioSource.spread;
            audioSource.velocityUpdateMode = parent.audioSource.velocityUpdateMode;
        }
    }
    private void Start()
    {
        
    }

    private void Update()
    {
    }

    public void ApplyVolumeChange()
    {
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
        audioSource.outputAudioMixerGroup = parent.output;
        audioSource.clip = parent.soundMultiples[selectedFile].soundFile;
        SetPitchAndVolume(selectedFile);
        //audioSource.PlayDelayed(delayTime);
        audioSource.PlayScheduled(scheduledTime);
    }
    public void PlayAudio(int selectedFile)
    {
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
