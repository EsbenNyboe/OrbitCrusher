using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioObject orbSpawn, orbPickup, orbGlued, orbPickupDenied, orbDrag;
    public AudioObject [] orbDragGlued;
    public AudioObject levelCompleted, levelFailed, objectiveConcluded, objectiveCompleted, objectiveFailed;
    public AudioObject correctHit, incorrectHit, incorrectHitComet;
    public AudioObject healthChargeLower, healthChargeUpper, healthDrainLower, healthDrainUpper;
    public AudioObject objectiveActivated, cometWallHit, uiClick, uiHover;
    public AudioObject musicBetweenLevels;
    public AudioObject musTutLoopA, musTutLoopB, musTutLoopC;
    public float musTutLoopFadeIn;
    public float musTutLoopFadeOut;

    [HideInInspector]
    public bool newSpawnSequence;

    public GameManager gameManager;
    public TutorialUI tutorialUI;

    private void Awake()
    {
        //gameManager = FindObjectOfType<GameManager>();
        //tutorialUI = FindObjectOfType<TutorialUI>();
    }

    private void Start()
    {
        if (GameManager.inTutorial)
        {
            musTutLoopA.VolumeChangeInParent(0f, 0f, true);
            musTutLoopA.TriggerAudioObject();
            musTutLoopB.VolumeChangeInParent(0f, 0f, true);
            musTutLoopB.TriggerAudioObject();
            musTutLoopC.VolumeChangeInParent(0f, 0f, true);
            musTutLoopC.TriggerAudioObject();
        }

        musicBetweenLevels.VolumeChangeInParent(0f, 0f, true);
        musicBetweenLevels.TriggerAudioObject();
        musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, 5f, true);

        orbDrag.VolumeChangeInParent(0f, 0f, true);
        orbDrag.TriggerAudioObject();
        for (int i = 0; i < orbDragGlued.Length; i++)
        {
            orbDragGlued[i].VolumeChangeInParent(0f, 0f, true);
            orbDragGlued[i].TriggerAudioObject();
        }
    }
    private void Update()
    {
        orbGluedTimer += Time.deltaTime;
        hitTimer += Time.deltaTime;
        cometWallHitTimer += Time.deltaTime;
    }
    public float sphereDragVolFade;
    float orbGluedTimer;
    int orbsGluedIndex;
    public void SphereGlued()
    {
        if (orbsGluedIndex >= orbDragGlued.Length)
        {
            orbsGluedIndex = orbDragGlued.Length - 1;
        }
        if (orbGluedTimer < Random.Range(0.01f, 0.05f))
        {
            orbGlued.TriggerSoundWithDelay(Random.Range(0.01f, 0.02f));
        }
        else
        {
            orbGlued.TriggerAudioObject();
        }
        orbGluedTimer = 0;

        for (int i = 0; i < orbDragGlued.Length; i++)
        {
            if (i == orbsGluedIndex)
                orbDragGlued[i].VolumeChangeInParent(orbDragGlued[i].initialVolume, sphereDragVolFade, false);
            else
                orbDragGlued[i].VolumeChangeInParent(0f, sphereDragVolFade, false);
        }
        orbDrag.VolumeChangeInParent(0, sphereDragVolFade, false);
        orbsGluedIndex++;
    }

    public void OrbPickup()
    {
        orbPickup.TriggerAudioObject();
        orbDrag.VolumeChangeInParent(orbDrag.initialVolume, sphereDragVolFade, false);
    }
    public void OrbPickupDenied()
    {
        tutorialUI.ShowTextPickupDenied();
        orbPickupDenied.TriggerAudioObject();
    }
    public void KillSphereSoundInstant()
    {
        orbDrag.VolumeChangeInParent(0, 0, false);
        for (int i = 0; i < orbDragGlued.Length; i++)
        {
            orbDragGlued[i].VolumeChangeInParent(0, 0, false);
        }
    }
    public void SpherePickedUpNoMore()
    {
        orbDrag.VolumeChangeInParent(0, sphereDragVolFade, false);
        for (int i = 0; i < orbDragGlued.Length; i++)
        {
            orbDragGlued[i].VolumeChangeInParent(0, sphereDragVolFade, false);
        }
        orbsGluedIndex = 0;
    }

    public void SphereSpawn(int spawnIndex)
    {
        if (newSpawnSequence)
        {
            //sphereSpawn.TriggerSpecificSoundVariant(spawnIndex % sphereSpawn.voicePlayerNew.Length);
            orbSpawn.VolumeChangeInParent(orbSpawn.initialVolume, 0.01f, true);
            spawnSequenceRepeatIndex = 0;
        }
        orbSpawn.TriggerAudioObject();
    }
    int spawnSequenceRepeatIndex;
    public void RepeatingSpawnSequence()
    {
        spawnSequenceRepeatIndex++;
        float decreaseFactorPerRepeat = 1 - spawnSequenceRepeatIndex * 0.13f;
        if (decreaseFactorPerRepeat < 0.4f)
            decreaseFactorPerRepeat = 0.4f;
        float decreasedVolume = orbSpawn.initialVolume * decreaseFactorPerRepeat;
        orbSpawn.VolumeChangeInParent(decreasedVolume, 0.01f, true);
    }

    public void LevelTransition()
    {
        musicBetweenLevels.VolumeChangeInParent(0f, 3f, false);
        levelCompleted.VolumeChangeInParent(0f, 2f, false);
        levelFailed.VolumeChangeInParent(0f, 2f, false);
    }
    public void LevelCompleted()
    {
        levelCompleted.VolumeChangeInParent(levelCompleted.initialVolume, 0f, false);
        levelCompleted.TriggerAudioObject();
        musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, 5f, false);
    }
    public void LevelFailed()
    {
        levelFailed.VolumeChangeInParent(levelFailed.initialVolume, 0f, false);
        levelFailed.TriggerAudioObject();
        musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, 5f, false);
    }
    public void ObjectiveCompleted()
    {
        //objectiveConcluded.TriggerAudioObject();
        objectiveCompleted.TriggerAudioObject();
        //CorrectHit();
    }
    public void ObjectiveFailed()
    {
        //objectiveConcluded.TriggerAudioObject();
        objectiveFailed.TriggerAudioObject();
    }

    float hitTimer;
    public void CorrectNodeHit()
    {
        HitTimer(correctHit);
    }
    public void IncorrectNodeHit()
    {
        HitTimer(incorrectHit);
    }
    public void CometHitsOrb()
    {
        HitTimer(incorrectHitComet);
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
    public void HealthChargeInstant()
    {
        healthChargeLower.TriggerAudioObject();
        healthChargeLower.VolumeChangeInParent(healthChargeLower.initialVolume, 0, false);
        healthChargeUpper.TriggerAudioObject();
        healthChargeUpper.VolumeChangeInParent(healthChargeUpper.initialVolume, 0, false);
    }
    public void HealthCharge()
    {
        healthChargeLower.TriggerAudioObject();
        healthChargeLower.VolumeChangeInParent(healthChargeLower.initialVolume, healthFadeTime, false);
        healthChargeUpper.TriggerAudioObject();
        healthChargeUpper.VolumeChangeInParent(healthChargeUpper.initialVolume, healthFadeTime, false);
    }
    public void HealthChargeStop()
    {
        healthChargeLower.VolumeChangeInParent(0f, healthFadeTime, false);
        healthChargeUpper.VolumeChangeInParent(0f, healthFadeTime, false);
    }
    public void HealthDrainInstant()
    {
        healthDrainLower.PitchChangeInParent(healthDrainLower.initialPitch, 0, false);
        healthDrainLower.TriggerAudioObject();
        healthDrainLower.VolumeChangeInParent(healthDrainLower.initialVolume, 0, false);

        healthDrainUpper.PitchChangeInParent(healthDrainUpper.initialPitch, 0, false);
        healthDrainUpper.TriggerAudioObject();
        healthDrainUpper.VolumeChangeInParent(healthDrainUpper.initialVolume, 0, false);
    }
    public void HealthDrain()
    {
        float pitchCorrelatorLower = CalcPitchCorrelation(0.85f, 1.0f);

        healthDrainLower.PitchChangeInParent(healthDrainLower.initialPitch * pitchCorrelatorLower, 0f, false);
        healthDrainLower.TriggerAudioObject();
        healthDrainLower.VolumeChangeInParent(healthDrainLower.initialVolume, healthFadeTime, false);

        float pitchCorrelatorUpper = CalcPitchCorrelation(0.7f, 1.0f);

        healthDrainUpper.PitchChangeInParent(healthDrainUpper.initialPitch * pitchCorrelatorUpper, 0f, false);
        healthDrainUpper.TriggerAudioObject();
        healthDrainUpper.VolumeChangeInParent(healthDrainUpper.initialVolume, healthFadeTime, false);
    }

    private float CalcPitchCorrelation(float pitchMin, float pitchMax)
    {
        int max = gameManager.maxEnergy;
        int energy = GameManager.energy;
        float energyCrunched = (float)energy / max;
        float pitchCorrelator = pitchMin + energyCrunched * (pitchMax - pitchMin);
        return pitchCorrelator;
    }

    public void ObjectiveActivated()
    {
        objectiveActivated.TriggerAudioObject();
    }

    public void HealthDrainStop()
    {
        healthDrainLower.VolumeChangeInParent(0f, healthFadeTime, false);
        healthDrainUpper.VolumeChangeInParent(0f, healthFadeTime, false);
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



    float cometWallHitTimer;
    public void CometWallHit()
    {
        if (cometWallHitTimer > 0.2f)
        {
            cometWallHitTimer = 0;
            cometWallHit.TriggerAudioObject();
        }
    }

    public void ClickUI()
    {
        uiClick.TriggerAudioObject();
    }
    public void HoverUI()
    {
        uiHover.TriggerAudioObject();
    }


    public void TutorialCompleted()
    {
        musTutLoopA.VolumeChangeInParent(0, musTutLoopFadeOut, true);
        musTutLoopB.VolumeChangeInParent(0, musTutLoopFadeOut, true);
        musTutLoopC.VolumeChangeInParent(0, musTutLoopFadeOut, true);
        StartCoroutine(StopTutMusic());
    }
    IEnumerator StopTutMusic()
    {
        yield return new WaitForSeconds(musTutLoopFadeOut * 2);
        musTutLoopA.StopAudioAllVoices();
        musTutLoopB.StopAudioAllVoices();
        musTutLoopC.StopAudioAllVoices();
    }
    public void TutorialMusicLoops(int nodeIndex)
    {
        switch (nodeIndex)
        {
            case 0:
                musTutLoopA.VolumeChangeInParent(musTutLoopA.initialVolume, musTutLoopFadeIn, true);
                musTutLoopC.VolumeChangeInParent(0, musTutLoopFadeOut, true);
                break;
            case 1:
                musTutLoopB.VolumeChangeInParent(musTutLoopB.initialVolume, musTutLoopFadeIn, true);
                musTutLoopA.VolumeChangeInParent(0, musTutLoopFadeOut, true);
                break;
            case 2:
                musTutLoopC.VolumeChangeInParent(musTutLoopC.initialVolume, musTutLoopFadeIn, true);
                musTutLoopB.VolumeChangeInParent(0, musTutLoopFadeOut, true);
                break;
        }
    }
}

