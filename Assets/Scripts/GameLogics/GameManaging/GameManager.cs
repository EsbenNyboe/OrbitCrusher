using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool autoPauseEnabled;
    public bool godMode;
    public bool levelLoadDeveloperMode;
    public bool levelLoadUseSaveSystem;
    public int levelToLoad;
    public int objectiveToLoad;
    public bool autoUnloadActiveLevels;
    public bool frameRateDeveloperMode;
    public int targetFrameRate;

    public MusicMeter musicMeter;
    public NodeBehavior nodeBehavior;
    public CometBehavior cometMovement;
    public CometManager cometManager;
    public HealthBar healthBar;
    public SoundManager soundManager;
    public UIManager uiManager;
    private LevelDesigner levelSettings;
    public LevelManager levelManager;
    public TutorialUI tutorialUI;
    public LevelNumberDisplay levelNumberDisplay;
    public BackgroundColorManager backgroundColorManager;
    ScreenShake screenShake;

    public GameObject[] levels;
    public int tutorialEnergy;
    public int startEnergy;
    public int maxEnergy;
    public int transitionStartPoint;
    public MusicMeter.MeterCondition transitionTiming;

    public bool enableFullHpMusic;

    private GameObject currentLevel;
    public static int energy;
    public static int levelProgression;
    public static bool fullEnergy;
    public static bool inTutorial;


    public static bool betweenLevels = true;
    public static bool death;
    public static bool gameCompleted;
    public static bool levelCompleted;

    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
        Application.targetFrameRate = targetFrameRate;

        player.LoadPlayer();
        if (levelLoadUseSaveSystem)
            levelToLoad = player.lvl;

        screenShake = GetComponentInChildren<ScreenShake>();
    }
    private void Start()
    {
        
        
        if (levelLoadDeveloperMode)
        {
            LoadQuickloadLevelSelection();
        }
        else
        {
            if (levelToLoad == 0)
            {
                inTutorial = true;
                energy = tutorialEnergy;
            }
            if (autoUnloadActiveLevels)
            {
                foreach (var levelObject in levels)
                {
                    levelObject.SetActive(false);
                }
            }
            levelProgression = levelToLoad;
        }
    }
    private void Update()
    {
        if (frameRateDeveloperMode)
        {
            Application.targetFrameRate = targetFrameRate;
        }
        energyDisplay = energy;
        CheckIfEnergyFull();
    }
    public int energyDisplay;

    private void LoadQuickloadLevelSelection()
    {
        if (levelToLoad == 0)
            inTutorial = true;
        for (int i = 0; i < levels.Length; i++)
        {
            if (i == levelToLoad)
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

    public void LevelStartTriggered(bool newLevel)
    {
        loadNewLevel = newLevel;
        levelNumberDisplay.StartLevel();
        if (levelToLoad > 0)
        {
            //levelNumberDisplay.LevelNumberMakeGreen(levelToLoad);
        }
        backgroundColorManager.GradualColorOnLevelLoad(death);
        death = false;
        gameCompleted = false;
        levelCompleted = false;
        LoadTransitionToLevel();

    }



    private void LoadTransitionToLevel()
    {
        tutorialUI.HideTipsOnLevelLoaded();
        HealthBar.tutorialFadeOut = false;
        healthBar.FadeInHealthbar();
        if (inTutorial)
        {
            energy = tutorialEnergy;
            healthBar.UpdateHealthbarOnObjectiveConclusion(true);
        }
        else
        {
            energy = startEnergy;
            healthBar.UpdateHealthbarOnObjectiveConclusion(true);
        }
        soundManager.LevelTransition();
        if (levelProgression == 0)
        {
            inTutorial = true;
        }
        ChooseLevel(levelProgression);
        

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

        musicMeter.LoadNewMeterSettings(120, 8, 2);
        MusicMeter.beatCount = transitionStartPoint;
        if (inTutorial)
            MusicMeter.beatCount = 6;
        musicMeter.ActivateMusicMeter();

        musicMeter.SubscribeEvent(levelManager.LoadLevelTransition, ref musicMeter.subscribeAnytime);
        musicMeter.SubscribeEvent(RunTransitionToLevel, ref musicMeter.subscribeAnytime);
        nodeBehavior.SpawnNodes(loadNewLevel);
        uiManager.uiCurrentLevel.text = levelProgression.ToString();
        uiManager.uiCurrentLevelObjective.text = ": " + LevelManager.levelObjectiveCurrent.ToString();
        cometColor.Color_InOrbit();
    }
    private void ChooseLevel(int levelNumber)
    {
        currentLevel = levels[levelNumber];
        levelSettings = currentLevel.GetComponent<LevelDesigner>();
    }
    private void RunTransitionToLevel()
    {
        levelManager.TransitionSounds();

        if (musicMeter.MeterConditionSpecificTarget(transitionTiming))
        {
            betweenLevels = false;
            levelManager.LoadLevel();
            nodeBehavior.LoadLevel();
            cometMovement.LoadLevel();
            cometManager.LoadLevel();
            musicMeter.UnsubscribeEvent(RunTransitionToLevel, ref musicMeter.subscribeAnytime);
            musicMeter.LoadNewMeterSettings(LevelManager.bpm, LevelManager.beatsPerBar, LevelManager.barsPerSection);
        }
    }
    public CometColor cometColor;

    public static bool loadNewLevel;
    

    public static int energyPool;
    public void UpdateEnergyHealth(int amount, bool updatePool)
    {
        if (updatePool)
        //if (amount != -1)
        {
            if (amount > 0)
                amount += energyPool;
            energyPool = 0;
        }
        energy += amount;
        if (energy < 1)
        {
            if (!godMode)
                death = true;
        }
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    private void CheckIfEnergyFull()
    {
        if (!fullEnergy && energy >= maxEnergy)
        {
            fullEnergy = true; 
        }
        else if (fullEnergy && energy < maxEnergy)
        {
            fullEnergy = false;
        }
    }

    public Player player;
    public void LevelCompleted()
    {
        screenShake.ScreenShakeLevelCompleted();
        levelCompleted = true;
        backgroundColorManager.LevelCompleted();
        levelNumberDisplay.LevelCompleted();
        healthBar.FadeOutHealthbar();
        HoverGraphicText.allButtonsActive = false;
        objectiveToLoad = 0;
        if (inTutorial)
        {
            soundManager.TutorialCompleted();
            inTutorial = false;

        }
        HealthBar.tutorialFadeOut = true;
        energy = 0;
        UnloadLevel();
        levelProgression++;
        player.lvl = levelProgression;
        player.SavePlayer();
        if (levelProgression >= levels.Length)
        {
            gameCompleted = true;
            uiManager.ShowTextGameWon();
            levelProgression = 0;
            levelLoadDeveloperMode = false;
            godMode = false;
            soundManager.StartTutMusic();
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
            screenShake.ScreenShakeLevelFailed();
            cometMovement.LevelFailed();
            backgroundColorManager.LevelFailed();
            levelNumberDisplay.LevelFailed();
            tutorialUI.LoadTipOnLevelFailed();
            HoverGraphicText.allButtonsActive = false;
            healthBar.FadeOutHealthbar();
            LevelManager.firstTimeHittingTarget = true;
            uiManager.ShowTextLevelFailed();
            UnloadLevel();
        }
    }
    private void UnloadLevel()
    {
        nodeBehavior.AllAppear(false);
        nodeBehavior.AllExplode();
        betweenLevels = true;
        levelManager.UnloadLevel();
        CometBehavior.isMoving = false;
        musicMeter.StopMusicMeter();
        SoundDsp.dspTimeAtSectionStart = 0;
        cometColor.Color_OutOfOrbit();
    }

    public void ToggleGodMode()
    {
        godMode = !godMode;
    }
}