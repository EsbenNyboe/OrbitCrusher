using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public static int beatsPerBar;
    public static int barsPerSection;
    public static int bpm;

    public static LevelDesigner levelDesigner;
    public static LevelMusic levelMusic;
    public static int levelObjectiveCurrent;

    public static GameObject[] nodes;
    public static GameObject transitionNode;
    public static Animator transitionNodeAnim;
    public static MusicMeter.MeterCondition[] cometTimings;
    public static int cometDestination; // is this the right place for this?
    public static AudioObject[] sounds;
    public static AudioObject sampleLengthReference; // not used (but don't delete)
    public static MusicMeter.MeterCondition[] soundTimings;

    public static bool nodeHitSubscription;

    public static int[] targetNodes;
    public static SpawnZone[] spawnZones;
    public static int [] spawnTimings;

    public static LevelMusic.Part[] part;

    public static LevelDesigner.SoundTriggers[] soundTriggers;
    public static LevelDesigner.FullHPMusic[] fullHPMusic;
    public static int fullHPsecSectionLength;
    public static AudioObject[] lastObjQuickFadeOut;

    public static bool firstTimeHittingTarget = true;
    
    public static AudioObject[] transitionMusic; // 4/4 // not used (but don't delete)

    public static int[] transposedObjectives;
    public static int[] transposedObjectivesB;

    public float failFadeOutTime;
    public float completedFadeOutTime;
    public static bool levelCompleted;

    int[] repeatIndex;
    bool[] repeatActive;

    bool scheduledLvlFail;
    bool scheduledObjFail;
    bool scheduledWin;

    public static Vector2[] bgColors;

    public MusicMeter musicMeter;
    public CometBehavior cometMovement;
    public CometManager cometManager;
    public ObjectiveManager objectiveManager;
    public NodeBehavior nodeBehavior;
    public MeterLookahead meterLookahead;
    public SpawnManager spawnManager;
    public GameManager gameManager;
    public HealthBar healthBar;
    public SoundManager soundManager;
    public SoundDsp soundDsp;
    public TutorialUI tutorialUI;
    public UIManager uiManager;
    public BackgroundColorManager backgroundColorManager;
    public NumberDisplayEnergyChange numberDisplayEnergyChange;
    public ScreenShake screenShake;
    public ProgressBar progressBar;

    bool objectiveActive;


    bool plausibleWin;
    bool plausibleFail;
    bool certainWin;
    bool certainFail;
    
    bool certainWinLvl;
    bool certainWinObj;

    bool certainFailLvlNextNode;
    
    bool soundWinObj;
    bool soundWinLvl;
    bool soundFailObj;
    bool soundFailLvl;

    private void Update()
    {
        if (objectiveActive)
        {
            CheckTheGameStateDichotomies_PlausibleOrCertain_FailOrWin_ObjOrLvl();
        }
    }

    #region Game State Sound Handling
    private void CheckTheGameStateDichotomies_PlausibleOrCertain_FailOrWin_ObjOrLvl()
    {
        if (!certainWin && !certainFail)
        {
            plausibleFail = true;
            plausibleWin = ObjectiveManager.amountOnTraps == 0;
        }
        if (plausibleWin && !certainWin && objectiveManager.HasAllEnergySpheresHitTheTargetNode())
        {
            certainWin = true;
            plausibleWin = plausibleFail = false;
        }
        if (plausibleFail && !certainFail && ObjectiveManager.amountOnTraps > 0)
        {
            certainFail = true;
            plausibleWin = plausibleFail = false;
        }

        if (certainWin)
        {
            if (!certainWinLvl && !certainWinObj)
            {
                if (levelObjectiveCurrent + 1 >= targetNodes.Length)
                {
                    certainWinLvl = true;
                    StopFailSoundLvl("lvl.win");
                    StopFailSoundObj("lvl.win");
                    soundManager.ActivateGameStateSound(soundManager.levelWon);
                    soundManager.LevelCompletedPreFadeOut(lastObjQuickFadeOut);
                }
                else
                {
                    certainWinObj = true;
                    StopFailSoundLvl("obj.win");
                    StopFailSoundObj("obj.win");
                    soundManager.ActivateGameStateSound(soundManager.objectiveCompleted);
                }
            }
        }
        if (certainFail && !certainFailLvlNextNode)
        {
            StopWinSounds("obj.fail");

            if (GameManager.energy < 1)
            {
                soundManager.ActivateGameStateSound(soundManager.levelFailed);
                certainFailLvlNextNode = true;
                StopFailSoundObj("lvl.fail");
            }
            else
            {
                soundManager.ActivateGameStateSound(soundManager.objectiveFailed); // problematic
            }
        }
    }
    private void ResetGameStateDichotomies()
    {
        plausibleWin = certainWin = plausibleFail = certainFail = certainFailLvlNextNode = certainWinLvl = certainWinObj = false;
        soundFailLvl = soundFailObj = soundWinLvl = soundWinObj = false;
    }

    private void StopWinSounds(string eventDescription)
    {
        if (soundWinObj)
        {
            soundWinObj = false;
            //print(eventDescription + ": stop obj.win" + Time.time);
            soundManager.StopGameStateSound(soundManager.objectiveCompleted);
        }
        else if (soundWinLvl)
        {
            soundWinLvl = false;
            //print(eventDescription + ": stop lvl.win" + Time.time);
            soundManager.StopGameStateSound(soundManager.levelWon);
        }
    }
    private void StopFailSoundObj(string eventDescription)
    {
        if (soundFailObj)
        {
            soundFailObj = false;
            //print(eventDescription + ": stop obj.fail" + Time.time);
            soundManager.StopGameStateSound(soundManager.objectiveFailed);
        }
    }
    private void StopFailSoundLvl(string eventDescription)
    {
        if (soundFailLvl)
        {
            soundFailLvl = false;
            //print(eventDescription + ": stop lvl.fail" + Time.time);
            soundManager.StopGameStateSound(soundManager.levelFailed);
        }
    }    
    
    private void ScheduleSoundsForNextNode()
    {
        if (!soundFailLvl)
        {
            if (certainFail || plausibleFail)
            {
                if (GameManager.energy + CountRemainingSpheresNotYetIncludedInScoreAccounting() < 1)
                {
                    soundFailLvl = true;
                    //print("schedule lvl fail for next node" + Time.time);
                    if (!PauseMenu.exitingOrbit)
                        soundManager.ScheduleGameStateSound(soundManager.levelFailed, false, certainFailLvlNextNode);
                }
            }
        }
    }

    private void ScheduleSoundsForNextTarget()
    {
        if (objectiveActive)
        {
            if (plausibleWin || certainWin)
            {
                if (levelObjectiveCurrent + 1 >= targetNodes.Length)
                {
                    if (!soundWinLvl)
                    {
                        soundWinLvl = true;
                        //print("schedule lvl win for next target" + Time.time);
                        soundManager.ScheduleGameStateSound(soundManager.levelWon, true, certainWinLvl);
                    }
                }
                else if (!soundWinObj)
                {
                    soundWinObj = true;
                    //print("schedule obj win for next target" + Time.time);
                    soundManager.ScheduleGameStateSound(soundManager.objectiveCompleted, true, certainWinObj);
                }
            }
            if (plausibleFail || certainFail)
            {
                if (!soundFailObj && !GameManager.death)
                {
                    soundFailObj = true;
                    //print("schedule obj fail for next target" + Time.time);
                    soundManager.ScheduleGameStateSound(soundManager.objectiveFailed, true, certainFail);
                }
            }
        }
    }
    #endregion

    #region Game State Events
    public void LoadLevelTransition()
    {
        backgroundColorManager.LvlLoaded();
        cometManager.nodeHitTiming = gameManager.transitionTiming;
        cometManager.LoadLevelTransition();
        cometMovement.LoadLevelTransition();
        cometMovement.light.intensity = cometMovement.intensityLow;
        musicMeter.UnsubscribeEvent(LoadLevelTransition, ref musicMeter.subscribeAnytime);

        if (levelDesigner.useLevelMusicSystem)
        {
            for (int i = 0; i < levelMusic.part.Length; i++)
            {
                LevelMusic.Sound[] s = levelMusic.part[i].sound;
                for (int e = 0; e < s.Length; e++)
                {
                    s[e].audioObject.StopAudioAllVoices();
                    s[e].audioObject.VolumeChangeInParent(s[e].audioObject.initialVolume, 0, true);
                }
            }
        }
        else
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].StopAudioAllVoices();
                sounds[i].VolumeChangeInParent(sounds[i].initialVolume, 0, true);
            }
        }
    }
    bool repeatDataReset;
    public void TransitionSounds()
    {
        if (!repeatDataReset)
        {
            if (levelDesigner.useLevelMusicSystem)
            {
                //int soundLength = 0;
                for (int i = 0; i < levelMusic.part.Length; i++)
                {
                    LevelMusic.Sound[] s = levelMusic.part[i].sound;
                    for (int e = 0; e < s.Length; e++)
                    {
                        //soundLength++;
                    }
                    levelMusic.part[i].repeatIndex = new int[s.Length];
                    levelMusic.part[i].repeatActive = new bool[s.Length];
                }
                //LevelMusic.repeatIndex = new int[soundLength];
                //LevelMusic.repeatActive = new bool[soundLength];
            }
            else
            {
                repeatIndex = new int[sounds.Length];
                repeatActive = new bool[sounds.Length];
            }

            repeatDataReset = true;
        }
        CheckForMusicTriggers(true);
    }
    public void LoadLevel()
    {
        repeatDataReset = false;
        cometManager.nodeHitTiming = cometTimings[cometDestination];
        musicMeter.SubscribeEvent(CheckIfNodeIsHitByComet, ref nodeHitSubscription);
    }
    public void UnloadLevel()
    {
        progressBar.DisplayProgress(false);
        if (levelDesigner.useLevelMusicSystem)
        {
            FadeOutLevelMusicNew(false);
            levelMusic.LevelUnloaded();
        }
        else
        {
            if (GameManager.death)
            {
                FadeOutLevelMusic(sounds, failFadeOutTime);
            }
            else
            {
                FadeOutLevelMusic(sounds, completedFadeOutTime);
            }
        }
        cometWallHits.ToggleEdgehitParticles(true);
        cometWallHits.SetEdgehitColor(!GameManager.death);
        musicMeter.UnsubscribeEvent(CheckIfNodeIsHitByComet, ref nodeHitSubscription);
    }

    public void FadeOutLevelMusicNew(bool exitOrbit)
    {
        float fadeTime;
        if (exitOrbit)
        {
            fadeTime = 0.5f;
        }
        else if (GameManager.death)
        {
            fadeTime = failFadeOutTime;
        }
        else
        {
            fadeTime = completedFadeOutTime;
        }

        for (int i = 0; i < levelMusic.part.Length; i++)
        {
            LevelMusic.Sound[] s = levelMusic.part[i].sound;
            AudioObject[] ao = new AudioObject[s.Length];

            for (int e = 0; e < s.Length; e++)
            {
                ao[e] = s[e].audioObject;
            }

            FadeOutLevelMusic(ao, fadeTime);
            //if (GameManager.death)
            //{
            //    FadeOutLevelMusic(ao, failFadeOutTime);
            //}
            //else
            //{
            //    FadeOutLevelMusic(ao, completedFadeOutTime);
            //}
        }
    }

    private void FadeOutLevelMusic(AudioObject[] audioObject, float fadeOutTime)
    {
        for (int i = 0; i < audioObject.Length; i++)
        {
            audioObject[i].VolumeChangeInParent(0, fadeOutTime, false);
        }
    }

    private void ObjectiveActivated()
    {
        objectiveActive = true;
        nodeBehavior.HighlightNewTarget(cometDestination);
        objectiveManager.ResetSphereArrays();
        spawnManager.StartNewSpawnSequence(false);
        nodeBehavior.CollisionNodeCometColor(cometDestination, true, true, false);
        if (lastObjective)
        {
            cometColor.Color_InOrbit();
            cometColor.Color_Enraged();
            cometMovement.light.intensity = cometMovement.intensityHigh;
        }
        else
        {
            cometColor.Color_InOrbit();
        }
    }
    private void ObjectiveFailed()
    {
        screenShake.ScreenShakeObjectiveFailed();
        int sphereYetToBeAccountedFor = CountRemainingSpheresNotYetIncludedInScoreAccounting();
        objectiveManager.RemoveEnergySpheres();
        objectiveManager.ResetSphereArrays();
        gameManager.UpdateEnergyHealth(sphereYetToBeAccountedFor, true);

        StopWinSounds("targethit");
        if (GameManager.death)
        {
            lastObjective = false;
            objectiveActive = false;
            //soundManager.ActivateGameStateSound(soundManager.levelFailed);
            StopFailSoundObj("targethit");
            spawnManager.CancelSpawnSequence();
            gameManager.LevelFailed();
            ResetLevelMusicValues();
            nodes[targetNodes[levelObjectiveCurrent]].GetComponentInChildren<NumberDisplayTargetNode>().DeactivateNumberDisplay();
        }
        else
        {
            numberDisplayEnergyChange.NumberDrain();
            soundManager.ActivateGameStateSound(soundManager.objectiveFailed);
            StopFailSoundLvl("targethit");
            spawnManager.ReloadSpawnSequence(false);
            if (sphereYetToBeAccountedFor < 0 || HealthBar.energyPoolMemory > 0)
                healthBar.UpdateHealthbarOnObjectiveConclusion(false);
        }

        ResetGameStateDichotomies();
        nodes[cometDestination].GetComponentInChildren<NumberDisplayTargetNode>().SetText();
    }

    private static void ResetLevelMusicValues()
    {
        if (levelDesigner.useLevelMusicSystem)
        {
            for (int i = 0; i < levelMusic.part.Length; i++)
            {
                for (int e = 0; e < levelMusic.part[i].sound.Length; e++)
                {
                    levelMusic.part[i].sound[e].hasPlayed = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < soundTriggers.Length; i++)
            {
                soundTriggers[i].hasPlayed = false;
            }
        }
    }

    public static bool lastObjective;
    public void ObjectiveCompleted()
    {
        int t = targetNodes[NumberDisplayTargetNode.targetNodeIndexMemory];
        if (!GameManager.inTutorial)
        {
            t = targetNodes[levelObjectiveCurrent];
        }
        nodes[t].GetComponentInChildren<NumberDisplayTargetNode>().DeactivateNumberDisplay();
        spawnManager.CancelSpawnSequence();
        screenShake.ScreenShakeObjectiveCompleted();
        objectiveActive = false;
        objectiveManager.RemoveEnergySpheres();
        objectiveManager.ResetSphereArrays();
        firstTimeHittingTarget = true;
        betweenObjectives = true;
        isWithinCriticalActivationThreshold = false;

        gameManager.UpdateEnergyHealth(1, true);

        soundManager.OrbPickedUpNoMore();

        levelObjectiveCurrent++;

        if (levelObjectiveCurrent == targetNodes.Length - 1)
        {
            lastObjective = true;
        }

        progressBar.SetProgressBarStage(levelObjectiveCurrent);

        if (levelObjectiveCurrent >= targetNodes.Length)
        {
            soundManager.ActivateGameStateSound(soundManager.levelWon);
            levelObjectiveCurrent = targetNodes.Length - 1;
            lastObjective = false;
            ResetLevelMusicValues();
            gameManager.LevelCompleted();
        }
        else
        {
            cometWallHits.ToggleEdgehitParticles(false);
            cometColor.Color_BetweenPuzzles();
            numberDisplayEnergyChange.NumberCharge();
            soundManager.ActivateGameStateSound(soundManager.objectiveCompleted);
            healthBar.UpdateHealthbarOnObjectiveConclusion(true);
            uiManager.uiCurrentLevelObjective.text = ": " + levelObjectiveCurrent.ToString();
            levelDesigner.LoadSpawnSequence(levelObjectiveCurrent);
            nodeBehavior.RemoveTargetHighlighting(targetNodes[levelObjectiveCurrent]);
        }
        nodeBehavior.TargetExplode(cometDestination);
        if (GameManager.inTutorial)
        {
            tutorialUI.ShowTextFirstHealthbarCharge();
        }

        ResetGameStateDichotomies();

        CheckIfBackgroundShouldChange(true);
        if (levelDesigner.useLevelMusicSystem)
            levelMusic.ObjectiveCompleted();

    }
    public CometWallHits cometWallHits;

    public bool bgPreFaded;
    public void CheckIfBackgroundShouldChange(bool targetIsReached)
    {
        if (bgColors.Length > 0)
        {
            for (int i = 0; i < bgColors.Length; i++)
            {
                if (targetIsReached && levelObjectiveCurrent == (int)bgColors[i].x)
                {
                    if (bgPreFaded)
                        bgPreFaded = false;
                    else
                        backgroundColorManager.LoadSpecialColor((int)bgColors[i].y);
                }
                else if (!targetIsReached && levelObjectiveCurrent + 1 == (int)bgColors[i].x)
                {
                    MusicMeter.MeterCondition nxtTarget = cometTimings[targetNodes[levelObjectiveCurrent]];
                    float timeUntilTarget = musicMeter.CheckTimeRemainingUntilMeterTarget(nxtTarget, "", 0, false);
                    float preFadeTiming = 0.7f;
                    if (timeUntilTarget > preFadeTiming)
                    {
                        BackgroundColorManager.currentBgIndex = (int)bgColors[i].y;
                        StartCoroutine(backgroundColorManager.LoadSpecialColorDelayed(timeUntilTarget - preFadeTiming));
                        bgPreFaded = true;
                    }
                }
            }
        }
    }

    private void SetOrbSoundsToTransposed(bool timingSectionStart)
    {

        if (levelDesigner.useLevelMusicSystem)
        {
            levelMusic.CheckIfTransposed();
        }
        else
        {
            bool objectiveIsTransposedOnce = false;
            bool objectiveIsTransposedTwice = false;
            for (int i = 0; i < transposedObjectives.Length; i++)
            {
                if (transposedObjectives[i] == levelObjectiveCurrent)
                    objectiveIsTransposedOnce = true;
            }
            for (int i = 0; i < transposedObjectivesB.Length; i++)
            {
                if (transposedObjectivesB[i] == levelObjectiveCurrent)
                    objectiveIsTransposedTwice = true;
            }
            if (!timingSectionStart && objectiveIsTransposedOnce)
            {
                // wait until section start to apply transposition // can be deleted, right?
            }
            else
            {
                soundManager.ToggleTransposedMusic(objectiveIsTransposedOnce, objectiveIsTransposedTwice);
            }
        }
    }
    #endregion

    private void CheckIfNodeIsHitByComet()
    {
        if (!GameManager.levelCompleted) // this is never set to true
        {
            CheckForMusicTriggers(false);
            CheckForLastMinuteStateChangesForLookaheadMusic();
        }
        CheckCometTimingForGameStateChanges();
        CheckCometTimingForLookaheadMusic(); // used to be on top
    }
    private int CountRemainingSpheresNotYetIncludedInScoreAccounting()
    {
        int failsAccountedFor = ObjectiveManager.amountOnTraps;
        int noncollidedSpawns = objectiveManager.energySpheresSpawned.Length - objectiveManager.collidedSpheresOnTarget.Length;
        int sphereYetToBeAccountedFor = failsAccountedFor - noncollidedSpawns;
        return sphereYetToBeAccountedFor;
    }
    public CometColor cometColor;
    bool firstNodeHitInLevel;

    bool includeFirstObjectiveNodeAnim = true;
    public static bool betweenObjectives;
    public static bool lastTravelBeforeActivation;
    
    private void CheckCometTimingForGameStateChanges()
    {
        if (musicMeter.MeterConditionSpecificTarget(cometTimings[cometDestination]))
        {
            scheduledLvlFail = scheduledObjFail = scheduledWin = false;
            if (GameManager.inTutorial)
            {
                soundManager.TutorialMusicLoops(cometDestination);
            }

            if (cometDestination == 0)
            {
            }

            if (betweenObjectives && cometDestination == targetNodes[levelObjectiveCurrent] - 1)
            {
                //lastTravelBeforeActivation = true;
            }

            if (cometDestination == targetNodes[levelObjectiveCurrent] && firstNodeHitInLevel)
            {
                if (firstTimeHittingTarget)
                {
                    transitionTriggered = false;
                    betweenObjectives = lastTravelBeforeActivation = false;
                    firstTimeHittingTarget = false;
                    if (GameManager.inTutorial)
                    {
                        tutorialUI.ShowTextThreeAligned(levelObjectiveCurrent);
                    }
                    ObjectiveActivated();
                    nodes[cometDestination].GetComponentInChildren<NumberDisplayTargetNode>().ShowNumberDisplay();
                }
                else
                {
                    if (objectiveManager.HasAllEnergySpheresHitTheTargetNode())
                    {
                        StopFailSoundLvl("targethit");
                        StopFailSoundObj("targethit");
                        ObjectiveCompleted();
                        nodeBehavior.CollisionNodeCometColor(cometDestination, true, false, true);
                    }
                    else
                    {
                        if (GameManager.energy + CountRemainingSpheresNotYetIncludedInScoreAccounting() < 1)
                        {
                            //soundManager.ActivateGameStateSound(soundManager.levelFailed);
                        }
                        ObjectiveFailed();
                        nodeBehavior.CollisionNodeCometColor(cometDestination, true, false, false);
                        if (GameManager.inTutorial && CountRemainingSpheresNotYetIncludedInScoreAccounting() < 0)
                        {
                            tutorialUI.ShowTextFirstObjectiveFailed();
                        }
                    }
                }

            }
            else if (GameManager.death)
            {
                ObjectiveFailed();
            }
            else
            {
                if (!certainFailLvlNextNode && !firstTimeHittingTarget)
                {
                    StopFailSoundLvl("nodehit");
                }
                nodeBehavior.CollisionNodeCometColor(cometDestination, false, false, false);
            }
            cometDestination++;
            if (cometDestination >= nodes.Length)
                cometDestination = 0;
            cometManager.ChangeDirectionWhenHittingNode();
            cometMovement.ChangeDirectionToNextDestination(cometDestination);
            cometManager.nodeHitTiming = cometTimings[cometDestination];

            firstNodeHitInLevel = true;
        }
        TransitionToObjectiveActivation();
    }

    bool transitionTriggered;
    bool upcomingActivationTimingChecked;
    bool isWithinCriticalActivationThreshold;
    private void TransitionToObjectiveActivation()
    {
        if (includeFirstObjectiveNodeAnim && !firstNodeHitInLevel)
        {
            betweenObjectives = true;
        }


        int targetIndex = targetNodes[levelObjectiveCurrent];
        MusicMeter.MeterCondition targetTiming = cometTimings[targetIndex];
        if (betweenObjectives) // slå sammen med firstTimeHittingTarget
        {
            if (musicMeter.CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(targetTiming) == nodeBehavior.activationAnimLookahead)
            {
                nodeBehavior.ObjectiveActivationAnimation(targetIndex);
                if (lastObjective)
                {
                    backgroundColorManager.LastObjective();
                }
            }
            if (!transitionTriggered)
            {
                if (firstNodeHitInLevel)
                {
                    soundManager.tranToObjActivate.VolumeChangeInParent(soundManager.tranToObjActivate.initialVolume, 0.1f, false);
                    StartCoroutine(soundManager.ScheduleTranToObjActivation(true));
                    transitionTriggered = true;
                }
            }
        }
        else
        {
            //int nextTargetIndex = 0;
            //if (levelObjectiveCurrent + 1 < targetNodes.Length)
            //{
            //    nextTargetIndex = targetNodes[levelObjectiveCurrent + 1];
            //}
            //MusicMeter.MeterCondition nextTargetTiming = cometTimings[nextTargetIndex];
            //int lookahead = (int)(soundManager.tranToObjActivateLookaheadTime * musicMeter.divMax) + soundDsp.meterLookahead.lookaheadFactor;

            //if (!isWithinCriticalActivationThreshold)
            //{
            //    nextTargetTiming = targetTiming;
            //}
            //if (musicMeter.CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(nextTargetTiming) == lookahead)
            //{
            //    if (isWithinCriticalActivationThreshold)
            //        soundManager.tranToObjActivate.VolumeChangeInParent(soundManager.tranToObjActivate.initialVolume, 0.1f, false);
            //    isWithinCriticalActivationThreshold = true;
            //}


        }


        if (!upcomingActivationTimingChecked && !transitionTriggered)
        {

        }
        //if (musicMeter.CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(targetTiming) == soundManager.tranToObjActivateLookaheadTime)
        //{
        //    soundManager.tranToObjActivate.VolumeChangeInParent(0f, 0.1f, false);
        //    soundManager.tranToObjActivate.TriggerAudioObject(); // scheduled!!
        //}
    }

    private void CheckCometTimingForLookaheadMusic()
    {
        if (GameManager.death) // FIX: don't do every frame, duh
        {
            if (PauseMenu.exitingOrbit)
                soundManager.FadeInMusicBetweenLevels(1f);
            else
                soundManager.FadeInMusicBetweenLevels(5f);
        }

        if (meterLookahead.SoundLookaheadRelativeToCondition(cometTimings[cometDestination]))
        {
            if (PauseMenu.exitingOrbit)
                FadeOutLevelMusicNew(true);
            ScheduleSoundsForNextNode();

            if (cometDestination == 0)
            {
                //soundManager.SetDspReferenceSectionStart();
            }
            if (cometDestination == targetNodes[levelObjectiveCurrent])
            {
                ScheduleSoundsForNextTarget();
                if (firstTimeHittingTarget)
                {
                    SetOrbSoundsToTransposed(false);
                    soundManager.ObjectiveActivatedPlaySound();
                    spawnManager.StartNewSpawnSequence(true);
                    //                        if (        for (int i = 0; i < s.Length; i++)
                    //        {
                    //            if (s[i].clutchFadein && s[i].clutchRiser)
                    //            {
                    //                if (riserFilter && !s[i].hasFadedIn)
                    //                    FadeinPrePlayedRiser(p, i);
                    //            }
                    //        }
                    //)
                    //{
                    if (!levelDesigner.useLevelMusicSystem) // replace with better fading system
                    {
                        for (int i = 0; i < soundTriggers.Length; i++)
                        {
                            bool playOnLastObjective = false;
                            for (int e = 0; e < soundTriggers[i].objectiveFilters.Length; e++)
                            {
                                if (soundTriggers[i].objectiveFilters[e] == targetNodes.Length - 1)
                                    playOnLastObjective = true;
                            }
                            if (!playOnLastObjective && !GameManager.inTutorial)
                            {
                                sounds[i].VolumeChangeInParent(0, 1, false);
                            }
                        }
                    }
                //}
                //if (levelObjectiveCurrent + 1 >= targetNodes.Length)
                //{
                //    for (int i = 0; i < soundTriggers.Length; i++)
                //    {
                //        if (!soundTriggers[i].stinger) // not ideal: replace with "last objective"-bool or something
                //            soundTriggers[i].sound.VolumeChangeInParent(0, 1, true);
                //    }
                //}
            }
            else
            {
                if (objectiveManager.HasAllEnergySpheresHitTheTargetNode())
                {
                    if (levelObjectiveCurrent + 1 >= targetNodes.Length)
                    {
                        soundManager.FadeInMusicBetweenLevels(5f);
                    }
                    else
                    {
                    }
                }
                else
                {
                    // alternative: reload spawn sequence every time - then stop the sequence & sounds, if lvlFail or clutchWin
                    spawnManager.ReloadSpawnSequence(true);

                    if (GameManager.energy + CountRemainingSpheresNotYetIncludedInScoreAccounting() > 0)
                    {
                        soundManager.ActivateGameStateSound(soundManager.objectiveFailed);
                        scheduledObjFail = true;
                    }
                    else if (!gameManager.godMode)
                    {
                        //soundManager.FadeInMusicBetweenLevels();
                        scheduledLvlFail = true;
                        soundManager.ActivateGameStateSound(soundManager.levelFailed);
                    }
                    else if (gameManager.godMode)
                    {
                        soundManager.ActivateGameStateSound(soundManager.objectiveFailed);
                    }
                    soundManager.RepeatingSpawnSequence();
                }
            }
        }
    }
}



private void CheckForLastMinuteStateChangesForLookaheadMusic()
    {
        if (scheduledLvlFail && !scheduledObjFail)
        {
            if (GameManager.energy + CountRemainingSpheresNotYetIncludedInScoreAccounting() > 0)
            {
                scheduledObjFail = true;
            }
        }
        if (scheduledObjFail)
        {
            if (ObjectiveManager.amountOnTraps == 0)
            {
                if (ObjectiveManager.amountOnTarget == ObjectiveManager.amountSpawned)
                {
                    scheduledObjFail = false;
                    scheduledWin = true;
                }
            }
            if (scheduledWin)
            {
                scheduledWin = false;
                if (levelObjectiveCurrent + 1 >= targetNodes.Length)
                    soundManager.FadeInMusicBetweenLevels(5f);
            }
            else if (scheduledLvlFail)
            {
                scheduledLvlFail = false;
            }
        }
    }

    public static int fullHPstartSection;
    bool fullHPtriggered;
    bool scheduleFullHPStartTrigger;
    void CheckForMusicTriggers(bool inTransition) // find a way to load this before game events at any time
    {
        MusicMeter.MeterCondition sampleRef = new MusicMeter.MeterCondition();
        sampleRef.division = sampleRef.beat = sampleRef.bar = 1;
        sampleRef.section = 0;

        if (meterLookahead.SoundLookaheadRelativeToCondition(sampleRef))
        {
            SetOrbSoundsToTransposed(true);
            soundDsp.InitializeDspReference(sampleLengthReference);
            //soundManager.InitializeDspReference(sampleLengthReference);
        }

        if (gameManager.enableFullHpMusic)
            FullHpMusic();




        if (levelDesigner.useLevelMusicSystem)
        {
            levelMusic.CheckMusicTimings(inTransition);
        }
        else
        {
            OldLevelMusicTriggering(inTransition);
        }
    }

    private void OldLevelMusicTriggering(bool inTransition)
    {
        for (int i = 0; i < soundTriggers.Length; i++)
        {
            bool objectiveFilter = false;


            if (!GameManager.inTutorial)
            {
                if (soundTriggers[i].objectiveFilters.Length > 0)
                {
                    objectiveFilter = true;
                    for (int of = 0; of < soundTriggers[i].objectiveFilters.Length; of++)
                    {
                        int objectiveCurrentAfterTargetHit = levelObjectiveCurrent;
                        //if (certainWin)
                        //    objectiveCurrentAfterTargetHit ++;
                        if (soundTriggers[i].objectiveFilters[of] == objectiveCurrentAfterTargetHit)
                        {
                            objectiveFilter = false;
                        }
                    }
                }
                else if (lastObjective)
                {
                    objectiveFilter = true;
                }
            }

            if (!objectiveFilter)
            {
                if (!inTransition)
                    MusicRepeater(i);

                if (meterLookahead.SoundLookaheadRelativeToCondition(soundTimings[i]))
                {
                    if (soundTriggers[i].repeatAtBeatInterval > 0 && !repeatActive[i])
                    {
                        if (soundTriggers[i].stinger && soundTriggers[i].hasPlayed)
                        {
                        }
                        else
                        {
                            repeatIndex[i] = 0;
                            repeatActive[i] = true;
                        }
                    }
                    if (soundTriggers[i].stinger)
                    {
                        if (!soundTriggers[i].hasPlayed)
                        {
                            soundDsp.LevelMusic_ScheduledPlayRelativeToDspReference(sounds[i]);
                            //soundManager.LevelMusic_ScheduledPlayRelativeToDspReference(sounds[i]);

                            soundTriggers[i].hasPlayed = true;
                        }
                    }
                    else
                    {
                        soundDsp.LevelMusic_ScheduledPlayRelativeToDspReference(sounds[i]);
                        //soundManager.LevelMusic_ScheduledPlayRelativeToDspReference(sounds[i]);
                    }
                }
            }
        }
    }

    private void MusicRepeater(int i)
    {
        MusicMeter.MeterCondition nxtBeat = new MusicMeter.MeterCondition();
        nxtBeat.division = 1;
        if (repeatActive[i])
        {
            if (meterLookahead.SoundLookaheadRelativeToCondition(nxtBeat))
            {
                repeatIndex[i]++;
                if ((float)repeatIndex[i] / soundTriggers[i].repeatAtBeatInterval > soundTriggers[i].repeatLifetime)
                {
                    repeatActive[i] = false;
                }
                else if (repeatIndex[i] % soundTriggers[i].repeatAtBeatInterval == 0)
                {
                    soundDsp.LevelMusic_ScheduledPlayRelativeToDspReference(sounds[i]);
                    //soundManager.LevelMusic_ScheduledPlayRelativeToDspReference(sounds[i]);
                }
            }
        }
    }

    private void FullHpMusic()
    {
        if (!lastObjective && !GameManager.betweenLevels && !GameManager.inTutorial)
        {
            bool fullEnergyPredicted = GameManager.energy + GameManager.energyPool >= gameManager.maxEnergy;
            bool allSpheresHaveHit = objectiveManager.collidedSpheresOnTarget.Length == objectiveManager.energySpheresSpawned.Length;
            bool aboutToBeFullEnergy = fullEnergyPredicted && allSpheresHaveHit;

            if (GameManager.fullEnergy || aboutToBeFullEnergy)
            {
                if (!fullHPtriggered)
                {
                    int startSectionContext = MusicMeter.sectionCount % fullHPsecSectionLength;
                    int startBarContext = MusicMeter.barCount % musicMeter.barMax;
                    if (startBarContext == 0)
                        startBarContext = musicMeter.barMax;

                    fullHPstartSection = MusicMeter.sectionCount;
                    fullHPstartSection += startSectionContext - fullHPsecSectionLength;

                    if (startSectionContext == 0 && startBarContext < musicMeter.barMax / 2)
                    {
                        fullHPstartSection += fullHPsecSectionLength;
                    }
                    //if (startSectionContext == 0)
                    //    startSectionContext = 2;

                    //startSectionContext += 1; // choose the next section to start playing


                    //if (MusicMeter.sectionCount % 4 == 0)
                    //{
                    //    fullHPstartSection -= 4;
                    //}
                    //print("section:" + fullHPstartSection + "context:" + startSectionContext);
                    fullHPtriggered = true;
                }
                for (int i = 0; i < fullHPMusic.Length; i++)
                {
                    for (int s = 0; s < fullHPMusic[i].soundTimings.Length; s++)
                    {

                        MusicMeter.MeterCondition relativeCondition = new MusicMeter.MeterCondition();
                        relativeCondition.division = relativeCondition.beat = 1;
                        relativeCondition.bar = fullHPMusic[i].soundTimings[s].bar;
                        relativeCondition.section = fullHPMusic[i].soundTimings[s].section;

                        bool anySection = relativeCondition.section == 0;
                        if (!anySection)
                            relativeCondition.section += fullHPstartSection;

                        if (anySection || relativeCondition.section >= MusicMeter.sectionCount)
                        {
                            if (meterLookahead.SoundLookaheadRelativeToCondition(relativeCondition))
                            {
                                //print(fullHPMusic[i].sound.name + " : " + musicMeter.CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(relativeCondition));
                                soundDsp.MusicScheduledPlayRelativeToDspReferenceNotFirstBeat(fullHPMusic[i].sound);
                                //print(fullHPMusic[i].sound.name + " : " + relativeCondition.bar + " secStart:"+fullHPstartSection + "mm:"+MusicMeter.sectionCount);
                            }
                        }
                    }
                }
            }
            else
            {
                fullHPtriggered = false;
            }
        }
    }
}
