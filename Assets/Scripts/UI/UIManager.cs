using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Color uiHoverColor;

    public GameObject menuTitle;
    public GameObject ui3Dtext;
    public GameObject uiStart3D;
    TextMeshPro uiStart3Dtext;
    public GameObject uiLevelFailed3D;
    TextMeshPro uiLevelFailed3Dtext;
    public GameObject uiLevelCompleted3D;
    TextMeshPro uiLevelCompleted3Dtext;
    public GameObject uiGameWon3D;
    TextMeshPro uiGameWon3Dtext;

    public GameObject uiIngame;
    public GameObject uiStart;
    public GameObject uiLevelCompleted;
    public TextMeshProUGUI uiLevelCompletedT;
    public GameObject uiGameWon;
    public TextMeshProUGUI uiGameWonT;
    public GameObject uiLevelFailed;
    public TextMeshProUGUI uiLevelFailedT;

    public GameObject uiMenuIcon;
    public TextMeshProUGUI uiCurrentLevel;
    public TextMeshProUGUI uiCurrentLevelObjective;

    public GameManager gameManager;
    public TutorialUI tutorialUI;
    public SoundManager soundManager;

    public Player player;

    private void Start()
    {
        //uiLevelFailedT.enabled = false;
        //uiLevelCompletedT.enabled = false;
        uiGameWonT.enabled = false;
        uiStart.SetActive(true);
        uiStart3D.SetActive(true);
        uiMenuIcon.SetActive(false);

        //uiLevelCompleted.SetActive(false);
        //uiLevelCompleted3D.SetActive(false);
        //uiLevelFailed.SetActive(false);
        ActivateUItext(uiLevelFailed, uiLevelFailedT);
        ActivateUItext(uiLevelCompleted, uiLevelCompletedT);
        Activate3Dtext(uiLevelFailed3D);
        Activate3Dtext(uiGameWon3D);
        Activate3Dtext(uiLevelCompleted3D);
        //uiGameWon3D.SetActive(false);

        menuTitle.SetActive(false);

        uiStart3Dtext = uiStart3D.GetComponent<TextMeshPro>();
        uiLevelFailed3Dtext = uiLevelFailed3D.GetComponent<TextMeshPro>();
        uiLevelCompleted3Dtext = uiLevelCompleted3D.GetComponent<TextMeshPro>();
        uiGameWon3Dtext = uiGameWon3D.GetComponent<TextMeshPro>();



        if (!Player.savedGameAvailable)
        {
            uiStart.GetComponentInChildren<TextMeshProUGUI>().text = "START";
            newGameText.SetActive(false);
        }
        else
        {
            uiStart.GetComponentInChildren<TextMeshProUGUI>().text = "CONTINUE: " + (gameManager.levelToLoad + 1);
            newGameText.SetActive(true);
        }


        if (gameManager.levelToLoad == 0)
        {
        }
        else
        {
        }
        uiLevelFailed.GetComponentInChildren<TextMeshProUGUI>().text = "RETRY: " + (gameManager.levelToLoad + 1);
    }
    public GameObject newGameText;
    private void Update()
    {
        
    }

    


    private void ActivateUItext(GameObject go, TextMeshProUGUI tmp)
    {
        go.SetActive(true);
        tmp.enabled = false;
    }
    private void Activate3Dtext(GameObject go)
    {
        go.SetActive(true);
        go.GetComponent<MeshRenderer>().enabled = false;
    }

    #region UI Interactions
    public void ClickUI()
    {
        soundManager.ClickUI();
    }
    public void HoverUI()
    {
        soundManager.HoverUI();
    }

    bool firstTimeStartingGame;
    public void StartGame()
    {
        if (!AchievementButton.levelMenuOpen)
        {
            if (!firstTimeStartingGame)
            {
                firstTimeStartingGame = true;
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
            uiGameWon3D.SetActive(false);
            gameManager.LevelStartTriggered(true);
            tutorialUI.LoadTutorial();
            uiStart.SetActive(false);
            uiGameWonT.enabled = false;
            uiStart3D.SetActive(false);
            uiMenuIcon.SetActive(true);
        }
    }

    public void ContinueToNextLevel()
    {
        if (!AchievementButton.levelMenuOpen)
        {
            gameManager.LevelStartTriggered(true);
            tutorialUI.LoadTutorial();
            //uiLevelCompleted.SetActive(false);
            //uiLevelCompletedT.enabled = false;
            uiLevelCompletedT.enabled = false;
            uiLevelCompleted.GetComponent<Animator>().SetBool("BetweenLevels", false);
            uiLevelCompleted3D.GetComponent<Animator>().SetBool("BetweenLevels", false);
            uiGameWon3D.GetComponent<Animator>().SetBool("BetweenLevels", false);
            //uiLevelCompleted3D.SetActive(false);
        }
    }
    public void FromLoseScreenToAnotherLevel() // not accurate description
    {
        gameManager.LevelStartTriggered(true);
        tutorialUI.LoadTutorial();
        FailScreenExit();
    }
    public void RetryLevel()
    {
        if (!AchievementButton.levelMenuOpen)
        {
            gameManager.LevelStartTriggered(false);
            tutorialUI.LoadTutorial();
            FailScreenExit();
        }
    }

    private void FailScreenExit()
    {
        uiLevelFailedT.enabled = false;
        uiLevelFailed.GetComponent<Animator>().SetBool("BetweenLevels", false);
        uiLevelFailed3D.GetComponent<Animator>().SetBool("BetweenLevels", false);
        uiGameWon3D.GetComponent<Animator>().SetBool("BetweenLevels", false);
    }
    #endregion

    #region Script Calls
    public void ToggleIngameUI(bool notInMenu)
    {
        //uiIngame.SetActive(notInMenu);
        //ui3Dtext.SetActive(notInMenu);

        if (GameManager.gameCompleted)
        {
            uiGameWon3Dtext.enabled = notInMenu;
            //uiStart3Dtext.enabled = notInMenu;
            uiGameWonT.enabled = notInMenu;
        }
        if (GameManager.death)
        {
            uiLevelFailed3Dtext.enabled = notInMenu;
            uiLevelFailedT.enabled = notInMenu;
        }
        if (GameManager.levelCompleted)
        {
            uiLevelCompleted3Dtext.enabled = notInMenu;
            uiLevelCompletedT.enabled = notInMenu;
        }
        menuTitle.SetActive(!notInMenu);
    }
    public void ShowTextLevelFailed()
    {
        uiLevelFailed.GetComponentInChildren<TextMeshProUGUI>().text = "RETRY: " + (GameManager.levelProgression + 1);
        //uiLevelFailed.SetActive(true);

        //uiLevelFailedT.GetComponent<Animator>().Play(0);
        //uiLevelFailedT.enabled = true;

        //uiLevelFailed3D.SetActive(true);
        uiLevelFailed.GetComponent<Animator>().SetBool("BetweenLevels", true);
        uiLevelFailedT.enabled = true;
        uiLevelFailed3D.GetComponent<Animator>().SetBool("BetweenLevels", true);
        uiLevelFailed3D.GetComponent<MeshRenderer>().enabled = true;

        if (GameManager.inTutorial)
            tutorialUI.UnloadTutorial();
    }
    public void ShowTextLevelCompleted()
    {
        uiLevelCompleted.GetComponentInChildren<TextMeshProUGUI>().text = "CONTINUE: " + (GameManager.levelProgression + 1);
        //uiLevelCompleted.SetActive(true);
        //uiLevelCompletedT.GetComponent<Animator>().Play(0);
        //uiLevelCompletedT.enabled = true;

        //uiLevelCompleted3D.SetActive(true);
        uiLevelCompleted.GetComponent<Animator>().SetBool("BetweenLevels", true);
        uiLevelCompletedT.enabled = true;
        uiLevelCompleted3D.GetComponent<Animator>().SetBool("BetweenLevels", true);
        uiLevelCompleted3D.GetComponent<MeshRenderer>().enabled = true;
        
        if (GameManager.inTutorial)
            tutorialUI.UnloadTutorial();
    }
    public void ShowTextGameWon()
    {
        uiGameWon3D.SetActive(true);
        uiGameWon3D.GetComponent<MeshRenderer>().enabled = true;
        uiGameWon3D.GetComponent<Animator>().SetBool("BetweenLevels", true);

        //uiStart3D.SetActive(true);
        uiGameWonT.GetComponent<Animator>().Play(0);
        uiGameWonT.GetComponent<Animator>().Play(0);
        uiGameWonT.enabled = true;
    }
    #endregion
}
