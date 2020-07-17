using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public static int beatsPerBar;
    public static int barsPerSection;
    public static int bpm;

    public static LevelDesigner levelDesigner;
    public static int levelObjectiveCurrent;

    public static GameObject[] nodes;
    public static GameObject transitionNode;
    public static MusicMeter.MeterCondition[] cometTimings;
    public static int cometDestination; // is this the right place for this?
    public static AudioObject[] sounds;
    public static AudioObject sampleLengthReference; // not used (but don't delete)
    public static MusicMeter.MeterCondition[] soundTimings;

    public static bool nodeHitSubscription;

    public static int[] targetNodes;
    public static SpawnZone[] spawnZones;
    public static int [] spawnTimings;

    public static LevelDesigner.SoundTriggers[] soundTriggers;
    public static LevelDesigner.FullHPMusic[] fullHPMusic;
    public static int fullHPsecSectionLength;
    public static AudioObject[] lastObjQuickFadeOut;

    public static bool firstTimeHittingTarget = true;
    
    public static AudioObject[] transitionMusic; // 4/4 // not used (but don't delete)

    public static int[] transposedObjectives;

    public float failFadeOutTime;
    public float completedFadeOutTime;
    public static bool levelCompleted;

    int[] repeatIndex;
    bool[] repeatActive;

    bool scheduledLvlFail;
    bool scheduledObjFail;
    bool scheduledWin;

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
    ScreenShake screenShake;

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

    private void Awake()
    {
        screenShake = GetComponentInChildren<ScreenShake>();
    }
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
                    soundManager.ActivateGameStateSound(soundManager.levelCompleted);
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
            soundManager.StopGameStateSound(soundManager.levelCompleted);
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
                        soundManager.ScheduleGameStateSound(soundManager.levelCompleted, true, certainWinLvl);
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
        musicMeter.UnsubscribeEvent(LoadLevelTransition, ref musicMeter.subscribeAnytime);
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].StopAudioAllVoices();
            sounds[i].VolumeChangeInParent(sounds[i].initialVolume, 0, true);
        }
    }
    bool repeatDataReset;
    public void TransitionSounds()
    {
        if (!repeatDataReset)
        {
            repeatIndex = new int[sounds.Length];
            repeatActive = new bool[sounds.Length];
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
        if (GameManager.death)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].VolumeChangeInParent(0, failFadeOutTime, false);
            }
        }
        else
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].VolumeChangeInParent(0, completedFadeOutTime, false);
            }
        }
        musicMeter.UnsubscribeEvent(CheckIfNodeIsHitByComet, ref nodeHitSubscription);
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
            cometColor.Color_Enraged();
            backgroundColorManager.LastObjective();
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
            for (int i = 0; i < soundTriggers.Length; i++)
            {
                soundTriggers[i].hasPlayed = false;
            }
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

    bool lastObjective;
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

        gameManager.UpdateEnergyHealth(1, true);


        levelObjectiveCurrent++;
        //SetOrbSoundsToTransposed();

        if (levelObjectiveCurrent == targetNodes.Length - 1)
        {
            lastObjective = true;
        }
        //if (levelObjectiveCurrent + 1 >= targetNodes.Length)
        //{
        //    if (MusicMeter.barCount % musicMeter.barMax == 1)
        //    {
        //        MusicMeter.barCount++;
        //    }
        //}

        if (levelObjectiveCurrent >= targetNodes.Length)
        {
            soundManager.ActivateGameStateSound(soundManager.levelCompleted);
            levelObjectiveCurrent = 0;
            lastObjective = false;
            for (int i = 0; i < soundTriggers.Length; i++)
            {
                soundTriggers[i].hasPlayed = false;
            }
            gameManager.LevelCompleted();
        }
        else
        {
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
    }

    private void SetOrbSoundsToTransposed(bool timingSectionStart)
    {
        bool objectiveIsTransposed = false;
        for (int i = 0; i < transposedObjectives.Length; i++)
        {
            if (transposedObjectives[i] == levelObjectiveCurrent)
                objectiveIsTransposed = true;
        }
        if (!timingSectionStart && objectiveIsTransposed)
        {
            // wait until section start to apply transposition
        }
        else
        {
            soundManager.ToggleTransposedMusic(objectiveIsTransposed);
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
            
            if (cometDestination == targetNodes[levelObjectiveCurrent] && firstNodeHitInLevel)
            {
                if (firstTimeHittingTarget)
                {
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
    }

    private void CheckCometTimingForLookaheadMusic()
    {
        if (GameManager.death)
        {
            soundManager.FadeInMusicBetweenLevels();
        }

        if (meterLookahead.SoundLookaheadRelativeToCondition(cometTimings[cometDestination]))
        {
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
                    if (lastObjective)
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
                            soundManager.FadeInMusicBetweenLevels();
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
                    soundManager.FadeInMusicBetweenLevels();
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
                    print("section:" + fullHPstartSection + "context:" + startSectionContext);
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
