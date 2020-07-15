﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
    public float median;
    public float spacing;
    public int levelCount;

    public static bool[] lvlsUnlocked;
    public static bool[] lvlsWon;
    public static bool[] lvlsWonFullHp;
    public static bool[] lvlsWonNoDmg;

    private AchievementStar[] achievementStars;

    public enum LevelStatus
    {
        Locked,
        Unlocked,
        Silver,
        Gold,
        Diamond
    }
    public LevelStatus[] achievementStatuses;

    public static bool[] buttonDownHasHappened;

    public float textboxOffset;
    public float achievementPosY;
    public bool manualAchievements;
    public GameObject panelParent;

    public UIManager uiManager;
    public GameManager gameManager;

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

    public void EnterOrbit(int levelNumber)
    {
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
        panelParent.GetComponent<Animator>().SetBool("BetweenLevels", display);
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
        MakeAnArrayOkay();
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
                    case LevelStatus.Silver:
                        lvlsWon[i] = lvlsUnlocked[i] = true;
                        break;
                    case LevelStatus.Gold:
                        lvlsWonFullHp[i] = lvlsWon[i] = lvlsUnlocked[i] = true;
                        break;
                    case LevelStatus.Diamond:
                        lvlsWonNoDmg[i] = lvlsWonFullHp[i] = lvlsWon[i] = lvlsUnlocked[i] = true;
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
        MakeAnArrayOkay();
        CreateCompletionArrays();
        ApplyNumbersToAchievementObjects();
    }

    private void CreateCompletionArrays()
    {
        lvlsUnlocked = new bool[achievementStars.Length];
        lvlsWon = new bool[achievementStars.Length];
        lvlsWonFullHp = new bool[achievementStars.Length];
        lvlsWonNoDmg = new bool[achievementStars.Length];
    }

    private void MakeAnArrayOkay()
    {
        achievementStars = new AchievementStar[levelCount];
        int i = 0;
        foreach (Transform child in panelParent.transform)
        {
            achievementStars[i] = child.GetComponent<AchievementStar>();
            i++;
        }
    }

    public void UpdateAchievements()
    {
        lvlsUnlocked[0] = true;
        for (int i = 0; i < achievementStars.Length; i++)
        {
            if (lvlsWon[i])
            {
                lvlsUnlocked[i] = true;
                if (i + 1 < achievementStars.Length)
                    lvlsUnlocked[i + 1] = true;
            }
            achievementStars[i].SetLevelStatus(lvlsUnlocked[i], lvlsWon[i], lvlsWonFullHp[i], lvlsWonNoDmg[i]);
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

    public float achievementDelay;
    public void NewAchievement(int level)
    {
        UpdateAchievements();
        achievementStars[level].NewAchievement(achievementDelay);
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
        foreach (Transform child in panelParent.transform)
        {
            i++;
            child.GetComponent<AchievementStar>().ApplyNumberToTextbox(i);
        }
    }
    public void ApplySpacing()
    {
        int i = 0;
        foreach (Transform child in panelParent.transform)
        {
            i++;
            child.GetComponent<AchievementStar>().ApplyNumberToTextbox(i);
            float xPosition = (i - median) * spacing;
            child.localPosition = new Vector3(xPosition, 0, 0);
        }
    }
}