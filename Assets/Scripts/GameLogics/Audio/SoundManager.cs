using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // main thread sounds
    public AudioObject orbPickup, orbGlued, orbGluedAlt, orbPickupDenied, orbDrag, orbDisconnect;
    public AudioObject[] orbDragGlued;
    public AudioObject correctHitStandard, correctHitTransA, correctHitTransB;
    public AudioObject incorrectHit, incorrectHitComet;
    public AudioObject healthChargeLower, healthChargeUpper, healthDrainLower, healthDrainUpper;
    public AudioObject cometWallHit, uiClick, uiHover;
    public static AudioObject uiClickStatic, uiHoverStatic;
    public AudioObject musicBetweenLevels, musicPerfection;

    // dsp sounds
    public AudioObject orbSpawnStandard, orbSpawnTransposed, orbSpawnTransposedTwice;
    [HideInInspector]
    public AudioObject levelWon;
    public AudioObject levelWonBronze, levelWonSilver, levelWonGold;
    public AudioObject levelFailed, objectiveFailed;
    public AudioObject objectiveCompletedStandard, objectiveCompletedTransA, objectiveCompletedTransB;
    public AudioObject objectiveActivated;
    public AudioObject musTutLoopA, musTutLoopB, musTutLoopC;

    public AudioObject tranToObjActivate, tranToObjActivateShort;
    public float tranToObjActivateLookaheadTime, tranToObjActivateShortLookaheadTime;

    public AnimationCurve fadeOutAnim;
    public static AnimationCurve fadeAnimStatic;

    public static float levelMusicFadeTime;
    public static float levelMusicInitialVol;
    public static bool levelMusicFadeIn;
    public void LevelMusicFadeOutMethod(VoicePlayer voicePlayer, float fadeTime, bool fadeIn)
    {
        levelMusicFadeTime = fadeTime;
        levelMusicInitialVol = 1;
        levelMusicFadeIn = fadeIn;
        voicePlayer.LetMeDoTheCoroutiningOkPlz();
    }

    public void LevelMusicFadeOutMethodNew(VoicePlayer voicePlayer)
    {
        float timeUntilTarget = musicMeter.CheckTimeRemainingUntilMeterTarget(LevelManager.cometTimings[LevelManager.targetNodes[LevelManager.levelObjectiveCurrent]], "", 0, false);
        float threshold = 0.2f;
        float fadeTime;
        float delay = 0;
        float frameBuffer = Time.deltaTime;
        if (timeUntilTarget > threshold)
        {
            fadeTime = threshold;
            delay = timeUntilTarget - threshold - frameBuffer - soundDsp.musicLatencyCompensation;
        }
        else
        {
            fadeTime = timeUntilTarget - frameBuffer - soundDsp.musicLatencyCompensation;
            if (fadeTime < 0)
                fadeTime = 0.02f;
        }
        float extensionAmount = 0f;
        fadeTime *= 1 + extensionAmount;


        voicePlayer.FadeOutMethodNew(fadeTime, false, delay);
    }


    public void LevelWonChooseSound()
    {
        if (GameManager.damageTakenThisLevel == 0)
        {
            levelWon = levelWonGold;
        }
        else if (GameManager.energy == gameManager.maxEnergy)
        {
            if (GameManager.inTutorial || LevelManager.levelObjectiveCurrent + 2 >= LevelManager.targetNodes.Length)
                levelWon = levelWonSilver;
            else
                levelWon = levelWonBronze;
        }
        else
        {
            levelWon = levelWonBronze;
        }
    }
    public void LevelWonDmgOnLastObjective()
    {
        levelWon = levelWonBronze;
    }


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

    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public TutorialUI tutorialUI;
    [HideInInspector]
    public MusicMeter musicMeter;
    [HideInInspector]
    public MeterLookahead meterLookahead;
    [HideInInspector]
    public SoundDsp soundDsp;

    private void Awake()
    {
        uiClickStatic = uiClick;
        uiHoverStatic = uiHover;
    }
    private void Start()
    {
        fadeAnimStatic = fadeOutAnim;
        ToggleTransposedMusic(false, false);
        musicBetweenLevels.VolumeChangeInParent(0f, 0f, true);
        FadeInMusicBetweenLevels(5f);
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
        if (musicBetweenLevelsAllowed && !musicPerfectionIsPlaying)
        {
            if (menu)
            {
                cometWallHit.VolumeChangeInParent(cometWallHit.initialVolume * 0.25f, 0, false);
                musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume * 0.25f, 0, false);
                //musicPerfection.VolumeChangeInParent(musicPerfection.initialVolume * 0.25f, 0, false);
            }
            else
            {
                cometWallHit.VolumeChangeInParent(cometWallHit.initialVolume, 0, false);
                musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, 0, false);
                //musicPerfection.VolumeChangeInParent(musicPerfection.initialVolume, 0, false);
            }
        }
    }
    public void PauseAll()
    {
        AudioListener.pause = true;
    }
    public void UnpauseAll()
    {
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
                //musTutLoopA.VolumeChangeInParent(musTutLoopA.initialVolume, musTutLoopFadeIn, false);
                //musTutLoopC.VolumeChangeInParent(0, musTutLoopFadeOut, false);
                StartCoroutine(TutorialLoopFadeIn(musTutLoopA));
                StartCoroutine(TutorialLoopFadeOut(musTutLoopC));
                break;
            case 1:
                //musTutLoopB.VolumeChangeInParent(musTutLoopB.initialVolume, musTutLoopFadeIn, false);
                //musTutLoopA.VolumeChangeInParent(0, musTutLoopFadeOut, false);
                StartCoroutine(TutorialLoopFadeIn(musTutLoopB));
                StartCoroutine(TutorialLoopFadeOut(musTutLoopA));
                break;
            case 2:
                //musTutLoopC.VolumeChangeInParent(musTutLoopC.initialVolume, musTutLoopFadeIn, false);
                //musTutLoopB.VolumeChangeInParent(0, musTutLoopFadeOut, false);
                StartCoroutine(TutorialLoopFadeIn(musTutLoopC));
                StartCoroutine(TutorialLoopFadeOut(musTutLoopB));
                break;
        }
    }
    IEnumerator TutorialLoopFadeOut(AudioObject ao)
    {
        float t = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < t + musTutLoopFadeOut)
        {
            float timePassed = Time.realtimeSinceStartup - t;
            float timeProgress = timePassed / musTutLoopFadeOut;
            float timeProgressReversed = -timeProgress + 1;
            float volProgress = timeProgressReversed * ao.initialVolume;
            ao.VolumeChangeInParent(volProgress, 0, false);
            //print(timeProgress);
            yield return null;
        }
        ao.VolumeChangeInParent(0, 0, false);
    }
    IEnumerator TutorialLoopFadeIn(AudioObject ao)
    {
        float t = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < t + musTutLoopFadeIn)
        {
            float timePassed = Time.realtimeSinceStartup - t;
            float timeProgress = timePassed / musTutLoopFadeIn;
            float volProgress = timeProgress * ao.initialVolume;
            ao.VolumeChangeInParent(volProgress, 0, false);
            //print(timeProgress);
            yield return null;
        }
        ao.VolumeChangeInParent(ao.initialVolume, 0, false);
    }

    #endregion

    #region Between Levels
    public void FadeInMusicBetweenLevels(float fadeTime)
    {
        StartCoroutine(FadeInMusicBetweenLevelsLameDelay(fadeTime));
    }
    IEnumerator FadeInMusicBetweenLevelsLameDelay(float fadeTime)
    {
        yield return new WaitForSeconds(0.1f);
        //musicBetweenLevels.TriggerAudioObject();
        if (!musicBetweenLevels.IsPlaying())
        {
            musicBetweenLevels.TriggerAudioObject();
        }
        musicBetweenLevels.VolumeChangeInParent(musicBetweenLevels.initialVolume, fadeTime, false);
        musicBetweenLevelsAllowed = true;
    }
    bool musicPerfectionIsPlaying;
    public void LevelTransition()
    {
        if (levelWon == null)
            levelWon = levelWonBronze;
        levelWon.VolumeChangeInParent(0f, 2f, false);
        //levelWonSilver.VolumeChangeInParent(0f, 2f, false);
        //levelWonGold.VolumeChangeInParent(0f, 2f, false);
        levelFailed.VolumeChangeInParent(0f, 2f, false);
        StopAllCoroutines();
        musicPerfectionIsPlaying = false;
        musicBetweenLevels.VolumeChangeInParent(0f, 3f, false);
        musicPerfection.VolumeChangeInParent(0f, 3f, false);
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
        if (!PauseMenu.menu)
        {
            tutorialUI.ShowTextPickupDenied();
            orbPickupDenied.TriggerAudioObject();
        }
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
        //orbDisconnect.TriggerAudioObject();
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
    [HideInInspector]
    public AudioObject objectiveCompleted;
    AudioObject correctHit;
    public void ToggleTransposedMusic(bool isTransposedOnce, bool isTransposedTwice)
    {
        if (isTransposedTwice)
        {
            orbSpawn = orbSpawnTransposedTwice;
            objectiveCompleted = objectiveCompletedTransB;
            correctHit = correctHitTransB;
        }
        else if (isTransposedOnce)
        {
            orbSpawn = orbSpawnTransposed;
            objectiveCompleted = objectiveCompletedTransA;
            correctHit = correctHitTransA;
        }
        else
        {
            orbSpawn = orbSpawnStandard;
            objectiveCompleted = objectiveCompletedStandard;
            correctHit = correctHitStandard;
        }

        objectiveCompleted = objectiveCompletedStandard;
    }

    public IEnumerator StopOrbSpawnSounds()
    {
        yield return new WaitForSeconds(0.001f);
        orbSpawn.StopAudioAllVoices();
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

        //float decreaseFactorPerRepeat = 1 - spawnSequenceRepeatIndex * 0.11f;
        //if (decreaseFactorPerRepeat < 0.6f)
        //    decreaseFactorPerRepeat = 0.6f;
        //float decreasedVolume = orbSpawn.initialVolume * decreaseFactorPerRepeat;
        //orbSpawn.VolumeChangeInParent(decreasedVolume, 0.1f, false);
    }
    #endregion

    public IEnumerator ScheduleTranToObjActivation(bool nextTarget)
    {
        yield return new WaitForEndOfFrame();
        double dspTime = soundDsp.DspTimeOnNodeHit(nextTarget) - soundDsp.musicStartOffset - tranToObjActivateLookaheadTime;

        if (dspTime > AudioSettings.dspTime + 0.1f)
        {
            tranToObjActivate.TriggerAudioObjectScheduled(dspTime);
            if (GameManager.inTutorial)
            {
                soundDsp.AddScheduledMusicToList(tranToObjActivate, dspTime);
            }
        }
        else
        {
            dspTime = dspTime + tranToObjActivateLookaheadTime - tranToObjActivateShortLookaheadTime;
            tranToObjActivateShort.TriggerAudioObjectScheduled(dspTime);
            if (GameManager.inTutorial)
            {
                soundDsp.AddScheduledMusicToList(tranToObjActivateShort, dspTime);
            }
        }
    }
    public IEnumerator StopTranToObjActivationSounds()
    {
        yield return new WaitForSeconds(0.1f);
        tranToObjActivate.StopAudioAllVoices();
        tranToObjActivateShort.StopAudioAllVoices();
    }

    #region Music Stingers For GameState Changes
    public void ObjectiveActivatedPlaySound()
    {
        double dspTime = soundDsp.DspTimeOnNodeHit(false) - soundDsp.musicStartOffset;
        objectiveActivated.TriggerAudioObjectScheduled(dspTime);
        if (GameManager.inTutorial)
        {
            soundDsp.AddScheduledMusicToList(objectiveActivated, dspTime);
        }
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
            soundDsp.AddScheduledMusicToList(audioObject, dspTime);
        }
    }
    public void ActivateGameStateSound(AudioObject audioObject)
    {
        audioObject.VolumeChangeInParent(audioObject.initialVolume, 0, false);
        if (gameManager.godMode)
        {
            if (audioObject == levelFailed)
            {
                audioObject.VolumeChangeInParent(0, 0, false);
            }
        }
    }
    public void StopGameStateSound(AudioObject audioObject)
    {
        audioObject.StopAudioAllVoices();
    }

    public void LevelCompletedPreFadeOut(AudioObject[] lastObjPreFadeOut)
    {
        for (int i = 0; i < lastObjPreFadeOut.Length; i++)
        {
            lastObjPreFadeOut[i].VolumeChangeInParent(0, 0.1f, false);
        }
    }
    #endregion

    public IEnumerator UnloadAudioDataLevelMusic()
    {
        yield return new WaitForSeconds(0.5f);
        if (LevelManager.levelDesigner.useLevelMusicSystem)
        {
            LevelMusic.Part[] part = LevelManager.levelMusic.part;
            for (int i = 0; i < part.Length; i++)
            {
                LevelMusic.Sound[] s = part[i].sound;
                for (int e = 0; e < s.Length; e++)
                {
                    for (int a = 0; a < s[e].audioObject.soundMultiples.Length; a++)
                    {
                        s[e].audioObject.soundMultiples[a].soundFile.UnloadAudioData();
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < LevelManager.sounds.Length; i++)
            {
                for (int e = 0; e < LevelManager.sounds[i].soundMultiples.Length; e++)
                {
                    LevelManager.sounds[i].soundMultiples[e].soundFile.UnloadAudioData();
                }
            }
        }
    }




    public void PlayLvlWinMusicPerfection()
    {
        StartCoroutine(LvlWinMusicPerfection());
    }

    IEnumerator LvlWinMusicPerfection()
    {
        musicPerfectionIsPlaying = true;
        yield return new WaitForSecondsRealtime(0.5f);
        musicBetweenLevels.VolumeChangeInParent(0, 1.5f, false);
        yield return new WaitForSecondsRealtime(2.5f);
        musicPerfection.VolumeChangeInParent(musicPerfection.initialVolume, 1f, false);
        musicPerfection.TriggerAudioObject();

        yield return new WaitForSecondsRealtime(21f);
        FadeInMusicBetweenLevels(5f);
        musicPerfectionIsPlaying = false;
    }
}