using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

    [HideInInspector]
    public MusicMeter musicMeter;
    [HideInInspector]
    public NodeBehavior nodeBehavior;
    [HideInInspector]
    public CometBehavior cometMovement;
    [HideInInspector]
    public CometManager cometManager;
    [HideInInspector]
    public HealthBar healthBar;
    [HideInInspector]
    public SoundManager soundManager;
    [HideInInspector]
    public UIManager uiManager;
    [HideInInspector]
    public LevelManager levelManager;
    [HideInInspector]
    public TutorialUI tutorialUI;
    [HideInInspector]
    public BackgroundColorManager backgroundColorManager;
    [HideInInspector]
    public SoundDsp soundDsp;
    [HideInInspector]
    public ScreenShake screenShake;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public Achievements achievements;
    [HideInInspector]
    public CometColor cometColor;


    public GameObject[] levels;
    [HideInInspector]
    public LevelDesigner[] levelDesigners;
    private LevelDesigner levelDesigner;

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

    public enum BetweenLevelsState
    {
        GameStart,
        Retry,
        Continue,
        GameCompleted
    }
    public static BetweenLevelsState betweenLevelsState;

    public int energyDisplay;

    public static bool loadNewLevel;


    public static int energyPool;
    public static int hpAtLevelCompletion;
    bool fullHpBonusChecked;

    public static int damageTakenThisLevel;
    [HideInInspector]
    public bool easyMode;



    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
        Application.targetFrameRate = targetFrameRate;

        achievements.PrepareAchievementObjects();
        player.LoadPlayer();
        if (levelLoadUseSaveSystem)
        {
            levelToLoad = player.lvl;
            easyMode = player.easyMode;
            Achievements.lvlsWonEasy = player.lvlsWonEasy;
            Achievements.lvlsWon = player.lvlsWon;
            Achievements.lvlsWonFullHp = player.lvlsWonFullHp;
            Achievements.lvlsWonNoDmg = player.lvlsWonZeroDmg;
        }
        achievements.UpdateAchievements();
        //screenShake = GetComponentInChildren<ScreenShake>();
    }
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //print(Achievements.lvlsWon[levels.Length - 1]);
        if (Achievements.lvlsWon[levels.Length - 1])
        {
            //achievements.GetComponent<Animator>().SetBool("AllCompleted", true);
            achievements.DisplayAchievements(true, true);
        }
        else
        {
            achievements.DisplayAchievements(false, true);
        }

        if (levelLoadUseSaveSystem)
        {
            betweenLevelsState = BetweenLevelsState.GameStart;

            if (levelToLoad == 0)
            {
                //inTutorial = true;
                //energy = tutorialEnergy;
            }
            else if (levelToLoad == levels.Length - 1)
            {
                //betweenLevelsState = BetweenLevelsState.GameCompleted;
            }
            else
            {
                //betweenLevelsState = BetweenLevelsState.Continue;
            }
            //print("lvlToLoad:" + levelToLoad);
            if (autoUnloadActiveLevels)
            {
                foreach (var levelObject in levels)
                {
                    levelObject.SetActive(false);
                }
            }
            levelProgression = levelToLoad;
        }
        else
        {
            LoadQuickloadLevelSelection();
        }
    }
    private void Update()
    {
        //if (levelLoadDeveloperMode)
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        LoadQuickloadLevelSelection();
        //        levelLoadDeveloperMode = false;
        //    }
        //}
        if (frameRateDeveloperMode)
        {
            Application.targetFrameRate = targetFrameRate;
        }
        energyDisplay = energy;
        CheckIfEnergyFull();
    }

    public void NewGame()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            easyMode = false;
            Achievements.lvlsUnlocked[i] = Achievements.lvlsWonEasy[i] = Achievements.lvlsWon[i] = Achievements.lvlsWonFullHp[i] = Achievements.lvlsWonNoDmg[i] = false;
            player.lvlsWon[i] = player.lvlsWonFullHp[i] = player.lvlsWonZeroDmg[i] = false;
        }
        Achievements.lvlsUnlocked[0] = true;
        achievements.UpdateAchievements();
        player.lvl = 0;
        
        player.NewGame();

        levelProgression = 0;
        uiManager.StartGame();
    }


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
        if (levelProgression == 0)
        {
            inTutorial = true;
            energy = tutorialEnergy;
            soundManager.StartTutMusic();
        }
        backgroundColorManager.GradualColorOnLevelLoad(death);
        death = false;
        gameCompleted = false;
        levelCompleted = false;
        LoadTransitionToLevel();
        achievements.DisplayAchievements(false, false);
        achievements.ResetAchievementsOnLevelLoadTriggered();
        damageTakenThisLevel = 0;
        fullHpBonusChecked = false;
        hpAtLevelCompletion = 0;
        if (levelLoadUseSaveSystem)
            godMode = easyMode;

        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
    }



    public ProgressBar progressBar;
    private void LoadTransitionToLevel()
    {
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
                levelDesigner.LoadLevelSettingsNew();
            }
            else
                levelObject.SetActive(false);
        }

        progressBar.DisplayProgress(true);
        progressBar.SetProgressBarLevel(levelProgression, LevelManager.targetNodes.Length);
        progressBar.SetProgressBarStage(LevelManager.levelObjectiveCurrent);


        SoundDsp.dspTimeAtSectionStart = 0; // this could be overwritten, correct? 
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

        MenuIcon.inTransition = true;
    }
    private void ChooseLevel(int levelNumber)
    {
        currentLevel = levels[levelNumber];
        levelDesigner = levelDesigners[levelNumber];
        //levelDesigner = currentLevel.GetComponent<LevelDesigner>();
    }
    private void RunTransitionToLevel()
    {
        levelManager.TransitionSounds();

        if (musicMeter.MeterConditionSpecificTarget(transitionTiming))
        {
            screenShake.ScreenShakeLevelStart();
            betweenLevels = false;
            levelManager.LoadLevel();
            nodeBehavior.LoadLevel();
            cometMovement.LoadLevel();
            cometManager.LoadLevel();
            musicMeter.UnsubscribeEvent(RunTransitionToLevel, ref musicMeter.subscribeAnytime);
            musicMeter.LoadNewMeterSettings(LevelManager.bpm, LevelManager.beatsPerBar, LevelManager.barsPerSection);
            StartCoroutine(CustomDelay(0.1f));
        }
    }

    private IEnumerator CustomDelay(float t)
    {
        yield return new WaitForSeconds(t);
        MenuIcon.inTransition = false;
    }
    public void UpdateEnergyHealth(int amount, bool updatePool)
    {

        if (amount != 1)
        {
            damageTakenThisLevel -= amount;
        }

        if (updatePool)
        //if (amount != -1)
        {
            if (amount > 0)
                amount += energyPool;
            else if (energyPool > 0)
                amount += 1;
            energyPool = 0;
        }
        energy += amount;
        if (energy < 1)
        {
            energy = 0;
            if (!godMode)
                death = true;
        }
        else if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }


        if (!fullHpBonusChecked && LevelManager.levelObjectiveCurrent + 1 == LevelManager.targetNodes.Length)
        {
            fullHpBonusChecked = true;
            hpAtLevelCompletion = energy;
        }
        else if (fullHpBonusChecked && amount < 0)
        {
            hpAtLevelCompletion = 0;
        }
        //print(hpAtLevelCompletion);
        
        soundManager.LevelWonChooseSound();
        if (LevelManager.lastObjective && hpAtLevelCompletion != maxEnergy)
            soundManager.LevelWonDmgOnLastObjective();
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

    public static int previousAchievementIndexForLastFinishedLevel;
    public static bool silver;
    public static bool gold;
    public void LevelCompleted()
    {
        achievements.DisplayAchievements(true, false);
        bool newAchievement = false;
        bool newSilver = false;
        bool newGold = false;

        if (Achievements.lvlsWonEasy[levelProgression])
            previousAchievementIndexForLastFinishedLevel = 1;
        if (Achievements.lvlsWon[levelProgression])
            previousAchievementIndexForLastFinishedLevel = 2;
        if (Achievements.lvlsWonFullHp[levelProgression])
            previousAchievementIndexForLastFinishedLevel = 3;
        if (Achievements.lvlsWonNoDmg[levelProgression])
            previousAchievementIndexForLastFinishedLevel = 4;

        if (easyMode)
        {
            if (!Achievements.lvlsWonEasy[levelProgression])
            {
                Achievements.lvlsUnlocked[levelProgression] = true;
                if (levelProgression + 1 < levels.Length)
                    Achievements.lvlsUnlocked[levelProgression + 1] = true;
                Achievements.lvlsWonEasy[levelProgression] = true;
                newAchievement = true;
                player.lvlsWonEasy[levelProgression] = true;
            }
        }
        else
        {
            if (hpAtLevelCompletion == maxEnergy)
            {
                if (!Achievements.lvlsWonFullHp[levelProgression])
                {
                    //print("full hp");
                    Achievements.lvlsWonFullHp[levelProgression] = true;
                    newAchievement = true;
                    player.lvlsWonFullHp[levelProgression] = true;
                    newSilver = true;
                }
                silver = true;
            }
            else
                silver = false;
            if (damageTakenThisLevel == 0)
            {
                if (!Achievements.lvlsWonNoDmg[levelProgression])
                {
                    //print("no dmg");
                    Achievements.lvlsWonNoDmg[levelProgression] = true;
                    newAchievement = true;
                    player.lvlsWonZeroDmg[levelProgression] = true;
                    newGold = true;
                }
                gold = true;
            }
            else
                gold = false;
            if (!Achievements.lvlsWon[levelProgression])
            {
                //print("completed");
                Achievements.lvlsUnlocked[levelProgression] = true;
                if (levelProgression + 1 < levels.Length)
                    Achievements.lvlsUnlocked[levelProgression + 1] = true;
                Achievements.lvlsWonEasy[levelProgression] = true;
                Achievements.lvlsWon[levelProgression] = true;
                newAchievement = true;
                player.lvlsWon[levelProgression] = true;
                tutorialUI.DisplayTip(levelProgression);
            }
        }
        if (newAchievement)
        {
            achievements.NewAchievement(levelProgression, newSilver, newGold);
            achievements.ChangeLevelText(levelProgression);
        }
        else
        {
            achievements.HighlightCompletedLevel(levelProgression);
            achievements.ChangeLevelText(levelProgression);
        }
        player.easyMode = easyMode;


        screenShake.ScreenShakeLevelCompleted();
        levelCompleted = true;
        backgroundColorManager.LevelCompleted();
        healthBar.FadeOutHealthbar();
        HoverGraphicText.allButtonsActive = false;
        objectiveToLoad = 0;
        HealthBar.tutorialFadeOut = true;
        energy = 0;
        levelProgression++;
        if (levelProgression >= levels.Length)
        {
            betweenLevelsState = BetweenLevelsState.GameCompleted;
            gameCompleted = true;
            uiManager.ShowTextGameWon();
            levelProgression = 0;
            levelLoadDeveloperMode = false; // why though??
            godMode = false;
        }
        else
        {
            betweenLevelsState = BetweenLevelsState.Continue;
            uiManager.ShowTextLevelCompleted(newSilver, newGold);
        }
        if (inTutorial)
        {
            soundManager.TutorialCompleted();
            inTutorial = false;
        }
        UnloadLevel();
    }

    public void LevelFailed()
    {
        if (!godMode)
        {
            betweenLevelsState = BetweenLevelsState.Retry;
            screenShake.ScreenShakeLevelFailed();
            cometMovement.LevelFailed();
            backgroundColorManager.LevelFailed();
            HoverGraphicText.allButtonsActive = false;
            healthBar.FadeOutHealthbar();
            LevelManager.firstTimeHittingTarget = true;
            uiManager.ShowTextLevelFailed();
            UnloadLevel();
            achievements.DisplayAchievements(true, false);
            achievements.HighlightCompletedLevel(levelProgression);
            achievements.ChangeLevelText(levelProgression);
        }
    }

    private void UnloadLevel()
    {
        soundManager.ToggleTransposedMusic(false, false);
        soundManager.FadeInMusicBetweenLevels();

        PauseMenu.exitingOrbit = false;
        nodeBehavior.AllAppear(false);
        nodeBehavior.AllExplode();
        betweenLevels = true;
        levelManager.UnloadLevel();
        CometBehavior.isMoving = false;
        musicMeter.StopMusicMeter();
        cometColor.Color_OutOfOrbit();
        cometManager.UnloadLevel();
        //achievements.NewAchievement(levelProgression);
        player.lvl = levelProgression;
        player.SavePlayer();

        StartCoroutine(soundManager.UnloadAudioDataLevelMusic());

        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        //Screen.orientation = ScreenOrientation.AutoRotation;
        StartCoroutine(soundManager.StopTranToObjActivationSounds());
    }

    public void ToggleGodMode()
    {
        godMode = !godMode;
    }
}