using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceScript : MonoBehaviour
{
    AudioSource source;
    public AudioClip clip1;
    public AudioClip clip2;
    bool clip1Play;
    bool clip2Play;
    public MusicMeter.MeterCondition[] clip1Conditions;
    public MusicMeter.MeterCondition[] clip2Conditions;
    MusicMeter musicMeter;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        musicMeter = FindObjectOfType<MusicMeter>();
        
    }
    public void PlaySoound()
    {
        source = GetComponent<AudioSource>();
        source.clip = clip1;
        source.Play();
    }
    public void SubscribedMethod()
    {
        if (musicMeter.MeterConditionSpecificTarget(clip1Conditions[0]))
        {
            //source.clip = clip1;
            //source.Play();
        }
        
        AudioSourceTest(ref clip1Play, clip1, clip1Conditions);
        //AudioSourceTest(ref clip2Play, clip2, clip2Conditions);
    }
    private void Update()
    {
        if (clip1Play)
        {
            if (source.isPlaying)
            {
                if (source.timeSamples >= AudioSettings.outputSampleRate * 2 - 1500)
                {
                    source.clip = clip1;
                    source.Play();
                    clip1Play = false;
                    //print("smartass play" + Time.time);
                }
            }
            else
            {
                source.clip = clip1;
                source.Play();
                clip1Play = false;
                //print("play" + Time.time);
            }
        }
        //print(source.timeSamples);
    }

    private void AudioSourceTest(ref bool play, AudioClip clip, MusicMeter.MeterCondition[] condition)
    {
        for (int i = 0; i < condition.Length; i++)
        {
            if (musicMeter.MeterConditionSpecificTarget(condition[i]))
            {
                play = true;
            }
        }
    }
}
