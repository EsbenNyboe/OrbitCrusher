using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool godMode;
    public bool levelLoadDeveloperMode;
    [Range(0, 5)]
    public int levelQuickLoad;
    public static bool inTutorial;
    public GameObject [] levels;
    GameObject currentLevel;

    private int levelProgression;
    public int levelQuickLoadReal;
    public int objectiveQuickLoad;
    public float firstLoadDelay;
    public static bool betweenLevels = true;
    public static bool preTransition = true;
    public int tutorialEnergy;
    public int startEnergy;
    public int maxEnergy;
    public static int energy;
    public MusicMeter.MeterCondition transitionTiming;
    MusicMeter musicMeter;
    SpawnManager spawnManager;
    NodeBehavior nodeBehavior;
    CometMovement cometMovement;
    CometBehavior cometBehavior;
    HealthBar healthBar;
    SoundManager soundManager;
    UIManager uiManager;
    LevelDesigner levelSettings;
    LevelManager levelManager;

    public bool autoUnloadActiveLevels;
    private void Awake()
    {
        Application.targetFrameRate = 600;
        levelManager = FindObjectOfType<LevelManager>();
        musicMeter = FindObjectOfType<MusicMeter>();
        spawnManager = FindObjectOfType<SpawnManager>();
        nodeBehavior = FindObjectOfType<NodeBehavior>();
        cometMovement = FindObjectOfType<CometMovement>();
        cometBehavior = FindObjectOfType<CometBehavior>();
        healthBar = FindObjectOfType<HealthBar>();
        soundManager = FindObjectOfType<SoundManager>();
        uiManager = FindObjectOfType<UIManager>();
        


        if (levelLoadDeveloperMode)
        {
            LoadQuickloadLevelSelection();
        }
        else
        {
            inTutorial = true;
            if (autoUnloadActiveLevels)
            {
                foreach (var levelObject in levels)
                {
                    levelObject.SetActive(false);
                }
            }
            //firstLoad = FirstLoad();
            //StartCoroutine(firstLoad);
        }
    }

    private void LoadQuickloadLevelSelection()
    {
        if (levelQuickLoadReal == 0)
            inTutorial = true;
        for (int i = 0; i < levels.Length; i++)
        {
            if (i == levelQuickLoadReal)
            {
                levelProgression = i;
                levels[i].SetActive(true);
            }
            else
                levels[i].SetActive(false);
        }
        StartCoroutine(DeveloperStart());
    }
    IEnumerator DeveloperStart()
    {
        yield return new WaitForSeconds(1f);
        uiManager.StartGame();
    }

    //private void LoadTheLevelThatsAlreadyActiveInTheScene()
    //{
    //    for (int i = 0; i < levels.Length; i++)
    //    {
    //        if (levels[i].activeSelf)
    //        {
    //            levelProgression = i;
    //            levels[i].SetActive(false);
    //        }
    //    }
    //}

    bool gameActive = true;
    bool firstLevelLoad = true;
    private void Update()
    {
        
    }

    public void LevelStartTriggered()
    {
        death = false;
        preTransition = false;
        LevelManager.levelCompleted = false;
        LoadTransitionToLevel();
    }

    IEnumerator FirstLoad()
    {
        LoadGame(false);
        yield return new WaitForSeconds(firstLoadDelay);
        LoadGame(true);
        if (levelQuickLoad >= levels.Length)
            levelQuickLoad = 0;
        levelProgression = levelQuickLoad;

        //LoadTransitionToNewLevel();
    }
    IEnumerator firstLoad;

    private void LoadGame(bool load)
    {
        musicMeter.enabled = load;
        spawnManager.enabled = load;
        if (!load)
            foreach (var levelObject in levels)
            {
                levelObject.SetActive(load);
            }
    }

    public void LevelCompleted()
    {
        objectiveQuickLoad = 0;
        if (inTutorial)
        {
            soundManager.TutorialCompleted();
            inTutorial = false;
        }
        soundManager.LevelCompleted();
        UnloadLevel();
        levelProgression++;
        if (levelProgression >= levels.Length)
        {
            uiManager.ShowTextGameWon();
            levelProgression = 0;
            levelLoadDeveloperMode = false;
            godMode = false;
            inTutorial = true;
        }
        else
        {
            uiManager.ShowTextLevelCompleted();
        }
    }
    public void LevelFailed()
    {
        if (!godMode)
        {
            LevelManager.firstTimeHittingTarget = true;
            uiManager.ShowTextLevelFailed();
            soundManager.LevelFailed();
            UnloadLevel();
        }
    }
    private void LoadTransitionToLevel()
    {
        if (energy < startEnergy)
        {
            if (inTutorial)
                energy = tutorialEnergy;
            else
                energy = startEnergy;
            healthBar.UpdateHealthbarOnObjectiveConclusion(true);
        }
        soundManager.LevelTransition();
        ChooseLevel(levelProgression);
        musicMeter.ResetMeterCounts();
        //musicMeter.SubscribeEvent(musicMeter.ResetMeterCounts, ref musicMeter.subscribeAnytime);
        musicMeter.SubscribeEvent(RunTransitionToLevel, ref musicMeter.subscribeAnytime);
        foreach (var levelObject in levels)
        {
            if (levelObject == currentLevel)
            {
                levelObject.SetActive(true);
                levelSettings.LoadLevelSettingsNew();
            }
            else
                levelObject.SetActive(false);
        }
        nodeBehavior.SpawnNodes();
        uiManager.uiCurrentLevel.text = levelProgression.ToString();
        uiManager.uiCurrentLevelObjective.text = ": " + LevelManager.levelObjectiveCurrent.ToString();
    }
    public AudioObject debugSound;
    public MusicMeter.MeterCondition testInterval;
    private void RunTransitionToLevel()
    {
        MeterLookahead meterLookahead = FindObjectOfType<MeterLookahead>();
        if (meterLookahead.SoundLookaheadConditionSpecific(testInterval))
            debugSound.TriggerAudioObject();

        //if (musicMeter.MeterConditionIntervals(testInterval))
        if (musicMeter.MeterConditionSpecificTarget(transitionTiming))
        {
            betweenLevels = false;
            levelManager.LoadLevel();
            nodeBehavior.LoadLevel();
            cometMovement.LoadLevel();
            cometBehavior.LoadLevel();
            musicMeter.UnsubscribeEvent(RunTransitionToLevel, ref musicMeter.subscribeAnytime);
            musicMeter.ResetMeterCounts();
            //musicMeter.SubscribeEvent(musicMeter.ResetMeterCounts, ref musicMeter.subscribeAnytime);
        }
    }

    private void UnloadLevel()
    {
        nodeBehavior.AllAppear(false);
        nodeBehavior.AllExplode();
        MusicMeter.sampleControlledMeter = false;
        betweenLevels = true;
        preTransition = true;
        //levels[levelProgression].SetActive(false);
        levelManager.UnloadLevel();
        CometMovement.isMoving = false;
    }

    private void ChooseLevel(int levelNumber)
    {
        currentLevel = levels[levelNumber];
        levelSettings = currentLevel.GetComponent<LevelDesigner>();
    }

    public static bool death;
    public static bool fullEnergy;
    public void UpdateEnergyHealth(int amount)
    {
        if (energy >= maxEnergy)
        {
            fullEnergy = true;
        }

        energy += amount;
        if (energy < 0)
        {
            if (!godMode)
                death = true;
        }
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }

        if (energy < maxEnergy)
        {
            fullEnergy = false;
        }
        //healthBar.UpdateHealthBarControlValue(energy);
        //print("<color=red> energy: </color>" + energy);
    }
}
