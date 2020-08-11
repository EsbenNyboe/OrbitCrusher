using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    public bool useLevelMusicSystem;
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
        [HideInInspector]
        public string name;
        public int repeatAtBeatInterval;
        public int repeatLifetime;
        public int[] objectiveFilters; 
        //public ObjectiveFilter objectiveFilter; // this is not useful right now
        public AudioObject sound;
        public MusicMeter.MeterCondition soundTiming; 
        
        public bool stinger;
        [HideInInspector]
        public bool hasPlayed;
    }
    public SoundTriggers[] soundTriggers;

    [System.Serializable]
    public class ObjectiveFilter
    {
        [HideInInspector]
        public string data;
        public int objective;
        public bool allowOrBlock;
    }

    //[System.Serializable]
    //public class MeterCondition
    //{
    //    public int division;
    //    public int beat;
    //    public int bar;
    //    public int section;
    //}


    // fullhealth music

    [System.Serializable]
    public class FullHPMusic
    {
        [HideInInspector]
        public string name;
        public AudioObject sound;
        public MusicMeter.MeterCondition[] soundTimings;
    }
    public FullHPMusic[] fullHPMusic;
    public int fullHPsecSectionLength;

    public AudioObject[] lastObjQuickFadeOut;

    [Tooltip("x: objective, y: color index")]
    public Vector2[] bgColors;


    bool onOff;
    public GameObject customSpawnZones;

    //public SpawnZone[] defaultSpawnZones;

    MusicMeter musicMeter;
    CometManager cometManager;

    public int[] transposedObjectives;
    public int[] transposedObjectivesB;

    private void Start()
    {
        onOff = true;
        ToggleSpawnZoneMeshrenderers();
    }





    public void ToggleSpawnZoneMeshrenderers()
    {
        onOff = !onOff;
        foreach (Transform child in customSpawnZones.transform)
                {
                    child.GetComponent<MeshRenderer>().enabled = onOff;
                }
    }

    public void NameSoundTriggers()
    {
        for (int i = 0; i < soundTriggers.Length; i++)
        {
            if (soundTriggers[i].sound != null)
                soundTriggers[i].name = soundTriggers[i].sound.name;
            
            MusicMeter.MeterCondition mc = soundTriggers[i].soundTiming;
            soundTriggers[i].soundTiming.data = mc.division + "." + mc.beat + "." + mc.bar + "." + mc.section;

            //if (soundTriggers[i].objectiveFilter.objective == 0 && soundTriggers[i].objectiveFilter.allowOrBlock == false)
            //    soundTriggers[i].objectiveFilter.data = " ";
            //else
            //    soundTriggers[i].objectiveFilter.data = "obj.f.: " + soundTriggers[i].objectiveFilter.objective + ": " + soundTriggers[i].objectiveFilter.allowOrBlock;

            string repeatState = " R ";
            if (soundTriggers[i].repeatAtBeatInterval == 0)
                repeatState = " ";

            string oneLinerName = soundTriggers[i].soundTiming.data + repeatState + soundTriggers[i].name;
            soundTriggers[i].name = oneLinerName;

            //soundTriggers[i].soundTiming.data =
            //        e + "mc: " +
            //        soundTriggers[i].soundTiming.division + "." +
            //        soundTriggers[i].soundTiming.beat + "." +
            //        soundTriggers[i].soundTiming.bar + "." +
            //        soundTriggers[i].soundTiming.sec;

            //for (int e = 0; e < soundTriggers[i].repeats.Length; e++)
            //{
            //    soundTriggers[i].repeats[e].data =
            //        "i:" +
            //        soundTriggers[i].repeats[e].repeatInterval + " A:" +
            //        soundTriggers[i].repeats[e].repeatAmount + " M:" +
            //        soundTriggers[i].repeats[e].relatedMeterCondition;
            //}
        }
        for (int i = 0; i < fullHPMusic.Length; i++)
        {
            if (fullHPMusic[i].sound != null)
                fullHPMusic[i].name = fullHPMusic[i].sound.name;
        }
    }

    public CopyOrPaste copyPasteSoundTriggers;
    public enum CopyOrPaste
    {
        Neutral,
        Copy, 
        Paste
    }
    public static SoundTriggers[] soundTriggerClipboard;
    public void CopyPasteSoundTriggers()
    {
        switch (copyPasteSoundTriggers)
        {
            case CopyOrPaste.Copy:
                soundTriggerClipboard = soundTriggers;
                break;
            case CopyOrPaste.Paste:
                soundTriggers = soundTriggerClipboard;
                break;
        }
    }




    
    

    public void LoadLevelSettingsNew()
    {
        LevelManager.bgColors = bgColors;
        LevelManager.sampleLengthReference = sampleLengthReference;
        LevelManager.levelDesigner = this;
        LevelManager.levelMusic = GetComponent<LevelMusic>();
        LevelManager.levelObjectiveCurrent = 0;
        LevelManager.cometDestination = 0;
        LevelManager.nodes = new GameObject[nodeSettings.Length];
        LevelManager.transitionNode = transitionNode;
        LevelManager.transitionNodeAnim = transitionNode.GetComponent<Animator>();
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

        if (useLevelMusicSystem)
        {
            LevelManager.levelMusic.LoadIntoLevelManager();
        }
        else
        {
            LevelManager.levelMusic = null;
            LevelManager.soundTriggers = soundTriggers;
            LevelManager.sounds = new AudioObject[soundTriggers.Length];
            LevelManager.soundTimings = new MusicMeter.MeterCondition[soundTriggers.Length];
            for (int i = 0; i < soundTriggers.Length; i++)
            {
                LevelManager.sounds[i] = soundTriggers[i].sound;
                LevelManager.soundTimings[i] = soundTriggers[i].soundTiming;
            }
            LevelManager.fullHPMusic = fullHPMusic;
            LevelManager.fullHPsecSectionLength = fullHPsecSectionLength;

            LevelManager.lastObjQuickFadeOut = lastObjQuickFadeOut;
            LevelManager.transposedObjectives = transposedObjectives;
            LevelManager.transposedObjectivesB = transposedObjectivesB;
        }

        LevelManager.beatsPerBar = beatsPerBar;
        LevelManager.barsPerSection = barsPerSection;
        LevelManager.bpm = bpm;
        
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager.levelLoadDeveloperMode && !gameManager.levelLoadUseSaveSystem)
        {
            LevelManager.levelObjectiveCurrent = gameManager.objectiveToLoad;
            LoadSpawnSequence(LevelManager.levelObjectiveCurrent);
        }
        else
        {
            LoadSpawnSequence(0);
        }
        musicMeter = FindObjectOfType<MusicMeter>();
        LevelManager.transitionMusic = transitionSounds;
        //musicMeter.SubscribeEvent(FindObjectOfType<LevelManager>().LoadLevelTransition, ref musicMeter.subscribeAnytime);
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
            //print("duplicating comet timing");
        }
    }
    
}