using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // main thread sounds
    public AudioObject orbPickup, orbGlued, orbGluedAlt, orbPickupDenied, orbDrag;
    public AudioObject[] orbDragGlued;
    public AudioObject correctHit, incorrectHit, incorrectHitComet;
    public AudioObject healthChargeLower, healthChargeUpper, healthDrainLower, healthDrainUpper;
    public AudioObject cometWallHit, uiClick, uiHover;
    public AudioObject musicBetweenLevels;

    // dsp sounds
    public AudioObject orbSpawnStandard, orbSpawnTransposed;
    public AudioObject levelCompleted, levelFailed, objectiveCompleted, objectiveFailed;
    public AudioObject objectiveActivated;
    public AudioObject musTutLoopA, musTutLoopB, musTutLoopC;

    public float musTutLoopFadeIn;
    public float musTutLoopFadeOut;
    public float sphereDragVolFade;
    public float healthFadeTime;

    [HideInInspector]
    public bool newSpawnSequence;

    int spawnSequenceRepeatIndex;

    float orbGluedTimer;
    int orbsGluedIndex;

    bool musicBetweenLevelsAllowed;
    float hitTimer;

    float cometWallHitTimer;

    public GameManager gameManager;
    public TutorialUI tutorialUI;
    public MusicMeter musicMeter;
    public MeterLookahead meterLookahead;
    SoundDsp soundDsp;

    private void Awake()
    {
        soundDsp = GetComponent<SoundDsp>();
    }
    private void Start()
    {
        ToggleTransposedMusic(false);
        musicBetweenLevels.VolumeChangeInParent(0f, 0f, true);
        FadeInMusicBetweenLevels();
        musicBetweenLevelsAllowed = false; // a little confused about this one... 

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
        //if (Input.GetKeyDown(KeyCode.Space))
        //    FadeInMusicBetweenLevels();

        orbGluedTimer += Time.deltaTime;
        hitTimer += Time.deltaTime;
        cometWallHitTimer += Time.deltaTime;
    }

    #region Menu
    public void MenuMix(bool menu)
    {
        print("menumix");
        if (musicBetweenLevelsAllowed)
        {
            if (menu)
            {
                musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume * 0.25f, 0, false);
            }
            else
            {
                musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, 0, false);
            }
        }
    }
    public void PauseAll()
    {
        print("pause");
        AudioListener.pause = true;
    }
    public void UnpauseAll()
    {
        print("unpause");
        AudioListener.pause = false;
    }
    #endregion

    #region Tutorial
    public void StartTutMusic()
    {
        musTutLoopA.VolumeChangeInParent(0f, 0f, true);
        musTutLoopA.TriggerAudioObject();
        musTutLoopB.VolumeChangeInParent(0f, 0f, true);
        musTutLoopB.TriggerAudioObject();
        musTutLoopC.VolumeChangeInParent(0f, 0f, true);
        musTutLoopC.TriggerAudioObject();
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
                musTutLoopA.VolumeChangeInParent(musTutLoopA.initialVolume, musTutLoopFadeIn, false);
                musTutLoopC.VolumeChangeInParent(0, musTutLoopFadeOut, false);
                break;
            case 1:
                musTutLoopB.VolumeChangeInParent(musTutLoopB.initialVolume, musTutLoopFadeIn, false);
                musTutLoopA.VolumeChangeInParent(0, musTutLoopFadeOut, false);
                break;
            case 2:
                musTutLoopC.VolumeChangeInParent(musTutLoopC.initialVolume, musTutLoopFadeIn, false);
                musTutLoopB.VolumeChangeInParent(0, musTutLoopFadeOut, false);
                break;
        }
    }
    #endregion

    #region Between Levels
    public void FadeInMusicBetweenLevels()
    {
        StartCoroutine(FadeInMusicBetweenLevelsLameDelay());
    }
    IEnumerator FadeInMusicBetweenLevelsLameDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if (!musicBetweenLevels.IsPlaying())
        {
            musicBetweenLevels.TriggerAudioObject();
        }
        musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, 5f, false);
        musicBetweenLevelsAllowed = true;
    }
    public void LevelTransition()
    {
        levelCompleted.VolumeChangeInParent(0f, 2f, false);
        levelFailed.VolumeChangeInParent(0f, 2f, false);
        musicBetweenLevels.VolumeChangeInParent(0f, 3f, false);
        musicBetweenLevelsAllowed = false;
    }
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
    #endregion

    #region Health
    public void StopHealthSoundsWhenPausing()
    {
        healthChargeLower.StopAudioAllVoices();
        healthChargeUpper.StopAudioAllVoices();
        healthDrainLower.StopAudioAllVoices();
        healthDrainUpper.StopAudioAllVoices();
    }
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
    public void HealthDrainStop()
    {
        healthDrainLower.VolumeChangeInParent(0f, healthFadeTime, false);
        healthDrainUpper.VolumeChangeInParent(0f, healthFadeTime, false);
    }
    #endregion

    #region Orb Interaction
    public void OrbGlued()
    {
        if (orbsGluedIndex >= orbDragGlued.Length)
        {
            orbsGluedIndex = orbDragGlued.Length - 1;
        }
        if (orbGluedTimer < Random.Range(0.01f, 0.05f))
        {
            float r = Random.Range(0.01f, 0.02f);
            orbGlued.TriggerSoundWithDelay(r);
            orbGluedAlt.TriggerSoundWithDelay(r);
        }
        else
        {
            orbGlued.TriggerAudioObject();
            orbGluedAlt.TriggerAudioObject();
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
    public void KillOrbSoundInstant()
    {
        orbDrag.VolumeChangeInParent(0, 0, false);
        for (int i = 0; i < orbDragGlued.Length; i++)
        {
            orbDragGlued[i].VolumeChangeInParent(0, 0, false);
        }
    }
    public void OrbPickedUpNoMore()
    {
        orbDrag.VolumeChangeInParent(0, sphereDragVolFade, false);
        for (int i = 0; i < orbDragGlued.Length; i++)
        {
            orbDragGlued[i].VolumeChangeInParent(0, sphereDragVolFade, false);
        }
        orbsGluedIndex = 0;
    }
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
    #endregion

    #region Orb Spawn

    AudioObject orbSpawn; // tbc
    public void ToggleTransposedMusic(bool isTransposed)
    {
        if (isTransposed)
            orbSpawn = orbSpawnTransposed;
        else
            orbSpawn = orbSpawnStandard;
    }
    public void OrbSpawn(int spawnIndex)
    {
        if (newSpawnSequence)
        {
            orbSpawn.VolumeChangeInParent(orbSpawn.initialVolume, 0.01f, true);
            spawnSequenceRepeatIndex = 0;
        }
        soundDsp.MusicScheduledPlayRelativeToDspReferenceNotFirstBeat(orbSpawn);
    }
    public void RepeatingSpawnSequence()
    {
        spawnSequenceRepeatIndex++;

        float decreaseFactorPerRepeat = 1 - spawnSequenceRepeatIndex * 0.11f;
        if (decreaseFactorPerRepeat < 0.6f)
            decreaseFactorPerRepeat = 0.6f;
        float decreasedVolume = orbSpawn.initialVolume * decreaseFactorPerRepeat;
        orbSpawn.VolumeChangeInParent(decreasedVolume, 0.1f, false);
    }
    #endregion

    #region Music Stingers For GameState Changes
    public void ObjectiveActivatedPlaySound()
    {
        double dspTime = soundDsp.DspTimeOnNodeHit(false) - soundDsp.musicStartOffset;
        objectiveActivated.TriggerAudioObjectScheduled(dspTime);
        if (GameManager.inTutorial)
        {
            //AddToScheduledMusicPool(objectiveActivated, dspTime);
            soundDsp.AddScheduledMusicToList(objectiveActivated, dspTime);
        }
        //MusicScheduledPlayRelativeToDspReferenceNotFirstBeat(objectiveActivated);
    }
    public void ScheduleGameStateSound(AudioObject audioObject, bool nextTarget, bool isActivated)
    {
        double dspTime = soundDsp.DspTimeOnNodeHit(nextTarget) - soundDsp.musicStartOffset;
        audioObject.TriggerAudioObjectScheduled(dspTime);
        if (!isActivated)
        {
            audioObject.VolumeChangeInParent(0, 0, false);
        }
        if (GameManager.inTutorial)
        {
            //AddToScheduledMusicPool(audioObject, dspTime);
            soundDsp.AddScheduledMusicToList(audioObject, dspTime);
        }
    }
    public void ActivateGameStateSound(AudioObject audioObject)
    {
        audioObject.VolumeChangeInParent(audioObject.initialVolume, 0, false);
    }
    public void StopGameStateSound(AudioObject audioObject)
    {
        audioObject.StopAudioAllVoices();
    }

    public void LevelCompletedPreFadeOut(AudioObject[] lastObjPreFadeOut)
    {
        for (int i = 0; i < lastObjPreFadeOut.Length; i++)
        {
            print("do it:" + Time.time);
            lastObjPreFadeOut[i].VolumeChangeInParent(0, 0.1f, false);
        }
    }
    #endregion

}