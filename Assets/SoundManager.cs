using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioObject musicBetweenLevels;
    public AudioObject musicInMenu;
    public AudioObject debugSound;
    public AudioObject levelCompleted;
    public AudioObject levelFailed;
    public AudioObject objectiveCompleted;
    public AudioObject objectiveConcluded;
    public AudioObject objectiveFailed;
    public AudioObject objectiveActivated;
    public AudioObject sphereSpawn;
    public AudioObject spherePickup;
    public AudioObject sphereGlued;
    public AudioObject sphereDragA;
    public AudioObject sphereDragB;
    public AudioObject correctHit;
    public AudioObject incorrectHit;
    public AudioObject healthCharge;
    public AudioObject healthDrain;


    

    [HideInInspector]
    public bool newSpawnSequence;

    MusicMeter musicMeter;
    MeterLookahead meterLookahead;
    GameManager gameManager;

    private void Awake()
    {
        musicMeter = FindObjectOfType<MusicMeter>();
        meterLookahead = FindObjectOfType<MeterLookahead>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        musicBetweenLevels.VolumeChangeInParent(0f, 0f, true);
        musicBetweenLevels.TriggerAudioObject();
        musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, 1f, true);

        sphereDragA.VolumeChangeInParent(0f, 0f, true);
        sphereDragB.VolumeChangeInParent(0f, 0f, true);
        sphereDragA.TriggerAudioObject();
        sphereDragB.TriggerAudioObject();
    }
    private void Update()
    {
        sphereGluedTimer += Time.deltaTime;
        hitTimer += Time.deltaTime;
    }
    public float sphereDragVolFade;
    float sphereGluedTimer;
    public void SphereGlued()
    {
        if (sphereGluedTimer < Random.Range(0.05f, 0.1f))
        {
            sphereGlued.TriggerSoundWithDelay(Random.Range(0.02f, 0.1f));
        }
        else
        {
            sphereGlued.TriggerAudioObject();
        }
        sphereGluedTimer = 0;
        sphereDragA.VolumeChangeInParent(sphereDragA.initialVolume, sphereDragVolFade, true);
        sphereDragB.VolumeChangeInParent(0f, sphereDragVolFade, true);
    }

    public void SpherePickedUp()
    {
        spherePickup.TriggerAudioObject();
        sphereDragB.VolumeChangeInParent(sphereDragB.initialVolume, sphereDragVolFade, true);
    }
    public void SpherePickedUpNoMore()
    {
        sphereDragA.VolumeChangeInParent(0f, sphereDragVolFade, true);
        sphereDragB.VolumeChangeInParent(0f, sphereDragVolFade, true);
    }

    public void SphereSpawn(int spawnIndex)
    {
        if (newSpawnSequence)
        {
            //sphereSpawn.TriggerSpecificSoundVariant(spawnIndex % sphereSpawn.voicePlayerNew.Length);
            sphereSpawn.TriggerAudioObject();
        }
    }
    public void LevelTransition()
    {
        musicBetweenLevels.VolumeChangeInParent(0f, 2f, true);
        levelCompleted.VolumeChangeInParent(0f, 2f, true);
        levelFailed.VolumeChangeInParent(0f, 2f, true);
    }
    public void LevelCompleted()
    {
        levelCompleted.VolumeChangeInParent(levelCompleted.initialVolume, 0f, true);
        levelCompleted.TriggerAudioObject();
        musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, 5f, true);
    }
    public void LevelFailed()
    {
        levelFailed.VolumeChangeInParent(levelFailed.initialVolume, 0f, true);
        levelFailed.TriggerAudioObject();
        musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, 1f, true);
    }
    public void ObjectiveCompleted()
    {
        objectiveConcluded.TriggerAudioObject();
        objectiveCompleted.TriggerAudioObject();
        CorrectHit();
    }
    public void ObjectiveFailed()
    {
        print("objFail");
        //objectiveConcluded.TriggerAudioObject();
        objectiveFailed.TriggerAudioObject();
    }

    float hitTimer;
    public void CorrectHit()
    {
        HitTimer(correctHit);
    }
    public void IncorrectHit()
    {
        HitTimer(incorrectHit);
    }
    private void HitTimer(AudioObject hitSound)
    {
        if (hitTimer < Random.Range(0.05f, 0.1f))
        {
            hitSound.TriggerSoundWithDelay(Random.Range(0.02f, 0.1f));
        }
        else
        {
            hitSound.TriggerAudioObject();
        }
        hitTimer = 0;
    }
    public float healthFadeTime;
    public void HealthCharge()
    {
        healthCharge.TriggerAudioObject();
        healthCharge.VolumeChangeInParent(healthCharge.initialVolume, healthFadeTime, false);
    }
    public void HealthChargeStop()
    {
        healthCharge.VolumeChangeInParent(0f, healthFadeTime, false);
    }
    public void HealthDrain()
    {
        float pitchCorrelator = CalcPitchCorrelation();

        healthDrain.PitchChangeInParent(healthDrain.initialPitch * pitchCorrelator, 0f, true);
        healthDrain.TriggerAudioObject();
        healthDrain.VolumeChangeInParent(healthDrain.initialVolume, healthFadeTime, false);
    }

    private float CalcPitchCorrelation()
    {
        int max = gameManager.maxEnergy;
        int energy = GameManager.energy;
        float energyCrunched = (float)energy / max;
        float pitchMax = 1.2f;
        float pitchMin = 0.7f;
        float pitchCorrelator = pitchMin + energyCrunched * (pitchMax - pitchMin);
        return pitchCorrelator;
    }

    public void ObjectiveActivated()
    {
        objectiveActivated.TriggerAudioObject();
    }

    public void HealthDrainStop()
    {
        healthDrain.VolumeChangeInParent(0f, healthFadeTime, false);
    }
    public void PauseAll()
    {
        //musicInMenu.VolumeChangeInParent(0f, 0f, true);
        //musicInMenu.TriggerAudioObject();
        //musicInMenu.voicePlayerNew[0].audioSource.ignoreListenerPause = true;
        //musicInMenu.VolumeChangeInParent(musicInMenu.initialVolume, 2f, false);
        AudioListener.pause = true;
    }
    public void UnpauseAll()
    {
        //musicInMenu.VolumeChangeInParent(0, 0.1f, false);
        AudioListener.pause = false;
    }
}
