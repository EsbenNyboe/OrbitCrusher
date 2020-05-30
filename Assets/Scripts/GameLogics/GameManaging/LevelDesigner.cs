using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    public int beatsPerBar;
    public int barsPerSection;
    public int bpm;
    [System.Serializable]
    public class NodeSettings
    {
        public GameObject node;
        public MusicMeter.MeterCondition cometTiming;
    }
    public NodeSettings[] nodeSettings;

    public GameObject transitionNode;
    public AudioObject[] transitionSounds; // 4/4 

    [System.Serializable]
    public class LevelObjectives
    {
        public int targetNode;
        public SpawnZone[] spawnZones;
        public int[] spawnTimings; // relative to the target meter condition
    }
    public LevelObjectives[] levelObjectives;

    public AudioObject sampleLengthReference;
    [System.Serializable]
    public class SoundTriggers
    {
        public int repeatAtBeatInterval;
        public int repeatLifetime;
        public int[] objectiveFilters;
        public AudioObject sound;
        public MusicMeter.MeterCondition soundTiming;
        public bool stinger;
        [HideInInspector]
        public bool hasPlayed;
    }
    public SoundTriggers[] soundTriggers;
    //public SpawnZone[] defaultSpawnZones;

    MusicMeter musicMeter;
    CometBehavior cometBehavior;

    private void Start()
    {
        
    }

    public void LoadLevelSettingsNew()
    {
        LevelManager.sampleLengthReference = sampleLengthReference;
        LevelManager.levelDesigner = this;
        LevelManager.levelObjectiveCurrent = 0;
        LevelManager.cometDestination = 0;
        LevelManager.nodes = new GameObject[nodeSettings.Length];
        LevelManager.transitionNode = transitionNode;
        LevelManager.cometTimings = new MusicMeter.MeterCondition[nodeSettings.Length];
        LevelManager.targetNodes = new int[levelObjectives.Length];
        //LevelManager.defaultSpawnZones = defaultSpawnZones;
        for (int i = 0; i < levelObjectives.Length; i++)
        {
            LevelManager.targetNodes[i] = levelObjectives[i].targetNode;
        }

        for (int i = 0; i < nodeSettings.Length; i++)
        {
            LevelManager.nodes[i] = nodeSettings[i].node;
            LevelManager.cometTimings[i] = nodeSettings[i].cometTiming;
            if (i > 0)
            {
                LazyMansDuplicationOfFormerTimingValues(ref LevelManager.cometTimings[i].division, LevelManager.cometTimings[i - 1].division);
                LazyMansDuplicationOfFormerTimingValues(ref LevelManager.cometTimings[i].beat, LevelManager.cometTimings[i - 1].beat);
            }
        }
        LevelManager.soundTriggers = soundTriggers;
        LevelManager.sounds = new AudioObject[soundTriggers.Length];
        LevelManager.soundTimings = new MusicMeter.MeterCondition[soundTriggers.Length];
        for (int i = 0; i < soundTriggers.Length; i++)
        {
            LevelManager.sounds[i] = soundTriggers[i].sound;
            LevelManager.soundTimings[i] = soundTriggers[i].soundTiming;
        }

        LevelManager.beatsPerBar = beatsPerBar;
        LevelManager.barsPerSection = barsPerSection;
        LevelManager.bpm = bpm;

        LevelManager.soundPrePlayed = new bool[soundTriggers.Length];
        
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager.levelLoadDeveloperMode)
        {
            LevelManager.levelObjectiveCurrent = gameManager.objectiveQuickLoad;
            LoadSpawnSequence(LevelManager.levelObjectiveCurrent);
        }
        else
        {
            LoadSpawnSequence(0);
        }
        musicMeter = FindObjectOfType<MusicMeter>();
        LevelManager.transitionMusic = transitionSounds;
        musicMeter.SubscribeEvent(FindObjectOfType<LevelManager>().LoadLevelTransition, ref musicMeter.subscribeAnytime);
        //FindObjectOfType<LevelManager>().LoadLevelTransition();
    }

    public void LoadSpawnSequence(int currentObjective)
    {
        LevelManager.spawnZones = new SpawnZone[levelObjectives[currentObjective].spawnZones.Length];
        LevelManager.spawnTimings = new int[levelObjectives[currentObjective].spawnTimings.Length];
        for (int i = 0; i < levelObjectives[currentObjective].spawnZones.Length; i++)
        {
            LevelManager.spawnZones[i] = levelObjectives[currentObjective].spawnZones[i];
        }
        for (int i = 0; i < levelObjectives[currentObjective].spawnTimings.Length; i++)
        {
            LevelManager.spawnTimings[i] = levelObjectives[currentObjective].spawnTimings[i];
        }
        if (LevelManager.spawnZones.Length > LevelManager.spawnTimings.Length)
        {
            int[] temp = new int[LevelManager.spawnZones.Length];
            for (int i = 0; i < LevelManager.spawnTimings.Length; i++)
            {
                temp[i] = LevelManager.spawnTimings[i];
            }
            for (int i = LevelManager.spawnTimings.Length - 1; i < LevelManager.spawnZones.Length; i++)
            {
                temp[i] = temp[i - 1] + 4;
            }
            LevelManager.spawnTimings = temp;
        }
        //if (levelObjectives[currentObjective].spawnSequence.Length == 0)
        //{
        //    levelObjectives[currentObjective].spawnSequence = new SpawnZone[defaultSpawnZones.Length]; // fix
        //    LevelManager.spawnSequence = new SpawnZone[defaultSpawnZones.Length];
        //}
    }

    private void LazyMansDuplicationOfFormerTimingValues(ref int unit, int formerUnit)
    {
        if (unit == 0)
        {
            unit = formerUnit;
            print("duplicating comet timing");
        }
    }
    
}