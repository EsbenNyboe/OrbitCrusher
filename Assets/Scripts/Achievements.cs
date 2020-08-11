using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
    public float median;
    public float spacing;
    public int levelCount;

    public static bool[] lvlsUnlocked;
    public static bool[] lvlsWonEasy;
    public static bool[] lvlsWon;
    public static bool[] lvlsWonFullHp;
    public static bool[] lvlsWonNoDmg;

    [HideInInspector]
    public AchievementStar[] achievementStars;

    public enum LevelStatus
    {
        Locked,
        Unlocked,
        Wood,
        Bronze,
        Silver,
        Gold
    }
    public LevelStatus[] achievementStatuses;

    public static bool[] buttonDownHasHappened;

    public float textboxOffset;
    public float achievementPosY;
    public bool manualAchievements;
    public GameObject panel;
    public Animator panelParentAnimator;

    [HideInInspector]
    public UIManager uiManager;
    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public AchievementParticleEffects achievementParticleEffects;
    public TutorialUI tutorialUI;

    private void Awake()
    {
        //MakeAnArrayOkay();
    }
    private void Start()
    {
        if (manualAchievements)
            ManualAchievementUpdate();
        buttonDownHasHappened = new bool[achievementStars.Length];
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    NewAchievement(5);
        if (GameManager.betweenLevels)
        {
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < achievementStars.Length; i++)
                {
                    if (!buttonDownHasHappened[i])
                    {
                        if (achievementStars[i].panel.activeInHierarchy)
                        {
                            achievementStars[i].HideLevelInfo();
                        }
                    }
                }
            }
        }
    }
    public void EnterOrbit(int levelNumber)
    {
        gameManager.uiManager.StopCoroutines();
        tutorialUI.ForceCloseTips();
        if (levelNumber > 0)
            GameManager.inTutorial = false;
        GameManager.levelProgression = levelNumber;
        AchievementButton.levelMenuOpen = false;

        switch (GameManager.betweenLevelsState)
        {
            case GameManager.BetweenLevelsState.GameStart:
                uiManager.StartGame();
                break;
            case GameManager.BetweenLevelsState.Retry:
                uiManager.FromLoseScreenToAnotherLevel();
                break;
            case GameManager.BetweenLevelsState.Continue:
                uiManager.ContinueToNextLevel();
                break;
            case GameManager.BetweenLevelsState.GameCompleted:
                uiManager.StartGame();
                break;
        }

    }

    public void DisplayAchievements(bool display, bool gameStart)
    {
        if (display)
        {
            for (int i = 0; i < achievementStars.Length; i++)
            {
                achievementStars[i].gameObject.SetActive(true);
            }
        }
        else if (gameStart)
        {
            for (int i = 0; i < achievementStars.Length; i++)
            {
                achievementStars[i].gameObject.SetActive(false);
            }
        }

        if (!display)
        {
            if (mostCurrentNewStar != null)
                mostCurrentNewStar.shinyStarAnimation.SetBool("NewStar", false);
        }
        //panelParent.GetComponent<Animator>().SetBool("BetweenLevels", display);
        panelParentAnimator.SetBool("BetweenLevels", display);
    }

    public void ManualAchievementUpdate()
    {
        ConvertAchievementStatusesToBools();
        UpdateAchievements();
        ApplyNumbersToAchievementObjects();
    }

    private void ApplyNumbersToAchievementObjects()
    {
        for (int i = 0; i < achievementStars.Length; i++)
        {
            achievementStars[i].gameObject.SetActive(true);
            achievementStars[i].ApplyNumberToTextbox(i + 1);
        }
    }

    public void SetPanelPlacement()
    {
        //MakeAnArrayOkay();
        ApplySpacing();
        float leftMax = (1 - median) * spacing;
        float rightMax = (levelCount - median) * spacing;
        float panelWidth = achievementStars[0].panel.GetComponent<RectTransform>().rect.width;
        float panelPosOffset = achievementStars[0].panel.GetComponent<RectTransform>().localPosition.x;
        print("panel:" + panelWidth);
        for (int i = 0; i < achievementStars.Length; i++)
        {
            float panelPosInitial = (i + 1 - median) * spacing;
            float distanceBetweenPanelOutlineAndRightMax = rightMax + textboxOffset - panelPosInitial - panelWidth / 2;
            if (distanceBetweenPanelOutlineAndRightMax < 0)
            {
                //print(i + ":" + distanceBetweenPanelOutlineAndRightMax);
                //float panelPosNew = panelPosOffset + distanceBetweenPanelOutlineAndRightMax;
                //achievementStars[i].SetPanelPositionX(panelPosNew);
            }
            else
            {
                //achievementStars[i].SetPanelPositionX(0);
            }

        }
    }


    private void ConvertAchievementStatusesToBools()
    {
        PrepareAchievementObjects();
        if (manualAchievements)
        {
            for (int i = 0; i < achievementStatuses.Length; i++)
            {

                switch (achievementStatuses[i])
                {
                    case LevelStatus.Locked:
                        break;
                    case LevelStatus.Unlocked:
                        lvlsUnlocked[i] = true;
                        break;
                    case LevelStatus.Wood:
                        lvlsWonEasy[i] = lvlsUnlocked[i] = true;
                        break;
                    case LevelStatus.Bronze:
                        lvlsWon[i] = lvlsWonEasy[i] = lvlsUnlocked[i] = true;
                        break;
                    case LevelStatus.Silver:
                        lvlsWonFullHp[i] = lvlsWon[i] = lvlsWonEasy[i] = lvlsUnlocked[i] = true;
                        break;
                    case LevelStatus.Gold:
                        lvlsWonNoDmg[i] = lvlsWonFullHp[i] = lvlsWon[i] = lvlsWonEasy[i] = lvlsUnlocked[i] = true;
                        break;
                }
                //if (lvlsWon.Length > i + 1 && lvlsWon[i + 1])
                //{
                //    lvlsUnlocked[i] = true;
                //}
            }
        }
    }


    public void PrepareAchievementObjects()
    {
        //MakeAnArrayOkay();
        CreateCompletionArrays();
        ApplyNumbersToAchievementObjects();
    }

    private void CreateCompletionArrays()
    {
        lvlsUnlocked = new bool[achievementStars.Length];
        lvlsWonEasy = new bool[achievementStars.Length];
        lvlsWon = new bool[achievementStars.Length];
        lvlsWonFullHp = new bool[achievementStars.Length];
        lvlsWonNoDmg = new bool[achievementStars.Length];
    }



    //private void MakeAnArrayOkay() //DELEEEEETE
    //{
    //    achievementStars = new AchievementStar[levelCount];
    //    int i = 0;
    //    foreach (Transform child in panelParent.transform)
    //    {
    //        achievementStars[i] = child.GetComponent<AchievementStar>();
    //        i++;
    //    }
    //}

    public void UpdateAchievements()
    {
        lvlsUnlocked[0] = true;
        for (int i = 0; i < achievementStars.Length; i++)
        {
            if (lvlsWonEasy[i])
            {
                lvlsUnlocked[i] = true;
                if (i + 1 < achievementStars.Length)
                    lvlsUnlocked[i + 1] = true;
            }
            achievementStars[i].SetLevelStatus(lvlsUnlocked[i], lvlsWonEasy[i], lvlsWon[i], lvlsWonFullHp[i], lvlsWonNoDmg[i]);
        }
    }

    int previousLevel;
    public void HighlightCompletedLevel(int level)
    {
        achievementStars[level].HighlightLevelStar();
    }
    public void ChangeLevelText(int level)  
    {
        achievementStars[previousLevel].ButtonTextPlayOtherLevel();
        achievementStars[level].ButtonTextPlaySameLevel();
        previousLevel = level;
    }

    float delay;
    public static bool goldEarned;
    public static bool silverEarned;
    public void NewAchievement(int level, bool newSilver, bool newGold)
    {
        delay = 0;
        if (newSilver)
            UIManager.delayTextCompleted = TutorialUI.tipDelay = delay = 0.3f;
        if (newGold)
            UIManager.delayTextCompleted = TutorialUI.tipDelay = delay = 1.9f;
        goldEarned = newGold;
        silverEarned = newSilver;

        StartCoroutine(NewAchievementDelay(level));
        mostCurrentNewStar = achievementStars[level];
    }
    public static AchievementStar mostCurrentNewStar;
    IEnumerator NewAchievementDelay(int level)
    {
        UpdateAchievements();
        achievementStars[level].RemoveFocus();
        ToggleStarDisplaysForNewAchievementAnimation(level);
        yield return new WaitForSeconds(delay);
        
        UpdateAchievements();
        achievementStars[level].NewAchievement();
        //achievementParticleEffects.NewAchievement(level);
    }

    private void ToggleStarDisplaysForNewAchievementAnimation(int level)
    {
        switch (GameManager.previousAchievementIndexForLastFinishedLevel)
        {
            case 0:
                achievementStars[level].starSilver.enabled = false;
                achievementStars[level].starGold.enabled = false;
                break;
            case 1:
                achievementStars[level].starWood.enabled = true;
                achievementStars[level].starBronze.enabled = false;
                achievementStars[level].starSilver.enabled = false;
                achievementStars[level].starGold.enabled = false;
                break;
            case 2:
                achievementStars[level].starBronze.enabled = true;
                achievementStars[level].starSilver.enabled = false;
                achievementStars[level].starGold.enabled = false;
                break;
            case 3:
                achievementStars[level].starSilver.enabled = true;
                achievementStars[level].starGold.enabled = false;
                break;
        }
    }

    public void ResetAchievementsOnLevelLoadTriggered()
    {
        for (int i = 0; i < achievementStars.Length; i++)
        {
            achievementStars[i].RemoveFocus();
            achievementStars[i].RemoveFocusLevelInfoDisplay();
        }
    }






    public void ApplyNumbersToTextboxes()
    {
        int i = 0;
        foreach (Transform child in panel.transform)
        {
            i++;
            child.GetComponent<AchievementStar>().ApplyNumberToTextbox(i);
        }
    }
    public void ApplySpacing()
    {
        int i = 0;
        foreach (Transform child in panel.transform)
        {
            i++;
            child.GetComponent<AchievementStar>().ApplyNumberToTextbox(i);
            float xPosition = (i - median) * spacing;
            child.localPosition = new Vector3(xPosition, 0, 0);
        }
    }
}
