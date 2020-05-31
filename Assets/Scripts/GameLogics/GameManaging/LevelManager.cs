using UnityEngine;

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
    public static AudioObject sampleLengthReference;
    public static MusicMeter.MeterCondition[] soundTimings;
    public static SpawnZone[] spawnSequence;

    public static bool nodeHitSubscription;
    public static bool soundPlayerSubscription;

    public static int[] targetNodes;
    public static SpawnZone[] spawnZones;
    public static int [] spawnTimings;

    public static LevelDesigner.SoundTriggers[] soundTriggers;
    //public static SpawnZone[] defaultSpawnZones;

    public static bool[] soundPrePlayed;

    public static bool firstTimeHittingTarget = true;

    public MusicMeter musicMeter;
    public CometMovement cometMovement;
    public CometBehavior cometBehavior;
    public ObjectiveManager objectiveManager;
    public NodeBehavior nodeBehavior;
    public MeterLookahead meterLookahead;
    public SpawnManager spawnManager;
    public GameManager gameManager;
    public HealthBar healthBar;
    public SoundManager soundManager;
    public TutorialUI tutorialUI;
    public UIManager uiManager;

    public bool enableTransitionMusic;
    public static AudioObject[] transitionMusic; // 4/4

    public float failFadeOutTime;
    public float completedFadeOutTime;
    public static bool levelCompleted;

    private void Awake()
    {
        //tutorialUI = FindObjectOfType<TutorialUI>();
        //gameManager = FindObjectOfType<GameManager>();
        //cometMovement = FindObjectOfType<CometMovement>();
        //objectiveManager = FindObjectOfType<ObjectiveManager>();
        //musicMeter = FindObjectOfType<MusicMeter>();
        //cometBehavior = FindObjectOfType<CometBehavior>();
        //nodeBehavior = FindObjectOfType<NodeBehavior>();
        //meterLookahead = FindObjectOfType<MeterLookahead>();
        //spawnManager = FindObjectOfType<SpawnManager>();
        //healthBar = FindObjectOfType<HealthBar>();
        //soundManager = FindObjectOfType<SoundManager>();
        //uiManager = FindObjectOfType<UIManager>();
    }
    public void LoadLevelTransition()
    {
        //LoadNewMusicMeter();
        cometBehavior.nodeHitTiming = gameManager.transitionTiming;
        cometBehavior.LoadLevelTransition();
        cometMovement.LoadLevelTransition();
        musicMeter.UnsubscribeEvent(LoadLevelTransition, ref musicMeter.subscribeAnytime);
        musicMeter.SubscribeEvent(FirstSoundOnLevelStart, ref musicMeter.subscribeAnytime);
    }
    public float transitionDelay;
    bool transitionBegun;
    public void TransitionSounds()
    {
        if (enableTransitionMusic)
        {
            if (!transitionBegun)
            {
                for (int i = 0; i < transitionMusic.Length; i++)
                {
                    StartCoroutine(transitionMusic[i].TriggerSoundWithDelay(transitionDelay));
                    //transitionMusic[i].TriggerAudioObject();
                }
                transitionBegun = true;
            }
        }
    }

    private void LoadNewMusicMeter()
    {
        OverwriteOldMeterUnit(ref musicMeter.beatMax, beatsPerBar);
        OverwriteOldMeterUnit(ref musicMeter.barMax, barsPerSection);
        OverwriteOldMeterUnit(ref musicMeter.bpm, bpm);
    }

    private void OverwriteOldMeterUnit(ref int musicMeterUnit, int newUnit)
    {
        if (newUnit != 0)
        {
            musicMeterUnit = newUnit;
        }
    }
    public void LoadLevel()
    {
        repeatIndex = new int[sounds.Length];
        repeatActive = new bool[sounds.Length];
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].StopAudioAllVoices();
            sounds[i].VolumeChangeInParent(sounds[i].initialVolume, 0, true);
        }
        cometBehavior.nodeHitTiming = cometTimings[cometDestination];
        musicMeter.SubscribeEvent(CheckIfNodeIsHitByComet, ref nodeHitSubscription);
    }
    public void UnloadLevel()
    {
        transitionBegun = false;
        if (GameManager.death)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].VolumeChangeInParent(0, failFadeOutTime, true);
            }
        }
        else
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].VolumeChangeInParent(0, completedFadeOutTime, true);
            }
        }
        musicMeter.UnsubscribeEvent(CheckIfNodeIsHitByComet, ref nodeHitSubscription);
    }


    void CheckIfNodeIsHitByComet()
    {
        //if (meterLookahead.SoundLookaheadConditionSpecific(cometTimings[cometDestination]))
        //{
        if (musicMeter.MeterConditionSpecificTarget(cometTimings[cometDestination]))
        {
            //soundManager.SoundCometHitsNode();
            if (cometDestination == targetNodes[levelObjectiveCurrent])
            {
                if (objectiveManager.HasAllEnergySpheresHitTheTargetNode())
                {
                    //if (levelObjectiveCurrent + 1 >= targetNodes.Length)
                    //{
                    //    //levelCompleted = true;
                    //    soundManager.LevelCompleted();
                    //}
                    //else
                    //    soundManager.SoundObjectiveCompleted();
                }
                else
                {
                    
                }
            }
        }
        if (!levelCompleted)
            CheckIfSoundShouldPlay();


        // need to smash these together at some point 
        if (musicMeter.MeterConditionSpecificTarget(cometTimings[cometDestination]))
        {
            if (GameManager.inTutorial)
            {
                soundManager.TutorialMusicLoops(cometDestination);
            }
            if (cometDestination == targetNodes[levelObjectiveCurrent])
            {
                if (firstTimeHittingTarget)
                {
                    firstTimeHittingTarget = false;
                    if (GameManager.inTutorial)
                    {
                        tutorialUI.ShowTextThreeAligned(levelObjectiveCurrent);
                    }
                    soundManager.ObjectiveActivated();
                    nodeBehavior.HighlightNewTarget(cometDestination);
                    objectiveManager.ResetSphereArrays();
                    spawnManager.StartNewSpawnSequence();
                    nodeBehavior.CollisionNodeCometColor(cometDestination, true, true, false);
                }
                else
                {
                    if (objectiveManager.HasAllEnergySpheresHitTheTargetNode())
                    {
                        
                        ObjectiveCompleted();
                        nodeBehavior.CollisionNodeCometColor(cometDestination, true, false, true);
                    }
                    else
                    {
                        if (GameManager.inTutorial && CountRemainingSpheresNotYetIncludedInScoreAccounting() < 0)
                        {
                            tutorialUI.ShowTextFirstObjectiveFailed();
                        }
                        if (GameManager.energy + CountRemainingSpheresNotYetIncludedInScoreAccounting() > 0)
                        {
                            soundManager.ObjectiveFailed();
                        }
                        ObjectiveFailed();
                        nodeBehavior.CollisionNodeCometColor(cometDestination, true, false, false);
                        soundManager.RepeatingSpawnSequence();
                    }
                }
            }
            else if (GameManager.death)
            {
                ObjectiveFailed();
            }
            else
            {
                nodeBehavior.CollisionNodeCometColor(cometDestination, false, false, false);
            }
            //cometBehavior.PlayLight();
            //nodeBehavior.CollisionNodeComet(cometDestination);
            cometDestination++;
            if (cometDestination >= nodes.Length)
                cometDestination = 0;
            cometMovement.ChangeDirection(cometDestination);
            cometBehavior.nodeHitTiming = cometTimings[cometDestination];
        }
    }


    private void ObjectiveFailed() // need sound: if death: player death
    {
        //int successes = objectiveManager.collidedSpheresOnTarget.Length;
        int sphereYetToBeAccountedFor = CountRemainingSpheresNotYetIncludedInScoreAccounting();
        objectiveManager.RemoveEnergySpheres();
        objectiveManager.ResetSphereArrays();
        gameManager.UpdateEnergyHealth(sphereYetToBeAccountedFor);
        if (GameManager.death)
        {
            gameManager.LevelFailed();
        }
        else
        {
            spawnManager.ReloadSpawnSequence();
            if (sphereYetToBeAccountedFor < 0)
                healthBar.UpdateHealthbarOnObjectiveConclusion(false);
        }
    }

    private int CountRemainingSpheresNotYetIncludedInScoreAccounting()
    {
        int failsAccountedFor = objectiveManager.collidedSpheresOnTraps.Length;
        int noncollidedSpawns = objectiveManager.energySpheresSpawned.Length;
        int sphereYetToBeAccountedFor = failsAccountedFor - noncollidedSpawns;
        return sphereYetToBeAccountedFor;
    }

    public void ObjectiveCompleted()
    {
        if (GameManager.inTutorial)
        {
            tutorialUI.ShowTextFirstHealthbarCharge();
        }
        objectiveManager.RemoveEnergySpheres();
        objectiveManager.ResetSphereArrays();
        firstTimeHittingTarget = true;

        gameManager.UpdateEnergyHealth(1);
        levelObjectiveCurrent++;
        if (levelObjectiveCurrent >= targetNodes.Length)
        {
            levelObjectiveCurrent = 0;
            gameManager.LevelCompleted();
        }
        else
        {
            healthBar.UpdateHealthbarOnObjectiveConclusion(true);
            uiManager.uiCurrentLevelObjective.text = ": " + levelObjectiveCurrent.ToString();
            soundManager.ObjectiveCompleted();
            levelDesigner.LoadSpawnSequence(levelObjectiveCurrent);
            nodeBehavior.RemoveTargetHighlighting(targetNodes[levelObjectiveCurrent]);
            //spawnManager.StartNewSpawnSequence();
        }
        nodeBehavior.TargetExplode(cometDestination);
    }

    void FirstSoundOnLevelStart()
    {
        MusicMeter.MeterCondition levelStart = new MusicMeter.MeterCondition();
        levelStart.division = 1;
        if (meterLookahead.SoundLookaheadConditionSpecific(levelStart))
        {
            //soundFirst.TriggerAudioObject();
            musicMeter.UnsubscribeEvent(FirstSoundOnLevelStart, ref musicMeter.subscribeAnytime);
        }
    }

    int[] repeatIndex;
    bool[] repeatActive;
    void CheckIfSoundShouldPlay() // find a way to load this before game events at any time
    {
        if (!GameManager.inTutorial)
        {
            MusicMeter.MeterCondition sampleRef = new MusicMeter.MeterCondition();
            sampleRef.division = sampleRef.beat = sampleRef.bar = 1;
            sampleRef.section = 0;
            if (musicMeter.MeterConditionSpecificTarget(sampleRef))
            {
                sampleLengthReference.TriggerAudioObject();
                MusicMeter.sampleController = sampleLengthReference.voicePlayerNew[0].audioSource;
                MusicMeter.sampleControlledMeter = true;
                musicMeter.InitializeSampleController();
            }
        }


        for (int i = 0; i < soundTriggers.Length; i++)
        {
            bool objectiveFilter = false;
            if (soundTriggers[i].objectiveFilters.Length > 0)
            {
                objectiveFilter = true;
                for (int of = 0; of < soundTriggers[i].objectiveFilters.Length; of++)
                {
                    if (soundTriggers[i].objectiveFilters[of] == levelObjectiveCurrent)
                    {
                        objectiveFilter = false;
                    }
                }
            }
            if (!objectiveFilter)
            {
                MusicRepeater(i);

                //if (meterLookahead.SoundLookaheadConditionSpecific(soundTimings[i]))
                if (musicMeter.MeterConditionSpecificTarget(soundTimings[i]))
                {
                    if (soundTriggers[i].repeatAtBeatInterval > 0 && !repeatActive[i])
                    {
                        repeatIndex[i] = 0;
                        repeatActive[i] = true;
                    }
                    if (soundTriggers[i].stinger)
                    {
                        if (!soundTriggers[i].hasPlayed)
                        {
                            sounds[i].TriggerAudioObject();
                            soundTriggers[i].hasPlayed = true;
                        }
                    }
                    else
                        sounds[i].TriggerAudioObject();                    
                }
            }
        }
    }

    private void MusicRepeater(int i)
    {
        if (repeatActive[i])
        {
            if (MusicMeter.divCount == 1)
            {
                repeatIndex[i]++;
                if ((float)repeatIndex[i] / soundTriggers[i].repeatAtBeatInterval > soundTriggers[i].repeatLifetime)
                {
                    repeatActive[i] = false;
                }
                else if (repeatIndex[i] % soundTriggers[i].repeatAtBeatInterval == 0)
                {
                    sounds[i].TriggerAudioObject();
                }
            }
        }
    }
}
