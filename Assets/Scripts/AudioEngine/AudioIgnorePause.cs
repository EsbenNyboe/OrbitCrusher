using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioIgnorePause : MonoBehaviour
{
    public AudioSource[] audioSources;
    public AudioObject audioObject;
    private void Start()
    {
        audioObject = GetComponent<AudioObject>();
        audioSources = new AudioSource[audioObject.voiceMax];
        for (int i = 0; i < audioObject.voiceMax; i++)
        {
            audioSources[i] = audioObject.voicePlayerNew[i].audioSource;
            audioSources[i].ignoreListenerPause = true;
        }
    }
    public void LoadVariables()
    {
    }
}