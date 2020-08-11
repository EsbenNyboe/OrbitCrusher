using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Color uiHoverColor;

    public GameObject menuTitle;



    public GameObject uiStart3D;
    public TextMeshPro uiStart3Dtext;

    public GameObject uiLevelFailed3D;
    public MeshRenderer uiLevelFailed3DMesh;
    public TextMeshPro uiLevelFailed3Dtext;
    public Animator uiLevelFailed3DAnim;

    public GameObject uiLevelCompleted3D;
    public MeshRenderer uiLevelCompleted3DMesh;
    public TextMeshPro uiLevelCompleted3Dtext;
    public Animator uiLevelCompleted3DAnim;
    public Animator lightLevelWin;
    public Light lightLevelWinLight;

    public GameObject uiGameWon3D;
    public MeshRenderer uiGameWon3DMesh;
    public TextMeshPro uiGameWon3Dtext;
    public Animator uiGameWon3DAnim;
    public Light uiGameWon3DLight;

    public GameObject uiStart;
    public TextMeshProUGUI uiStartTmPro;

    public GameObject uiLevelCompleted;
    public TextMeshProUGUI uiLevelCompletedTmPro;
    public Animator uiLevelCompletedAnim;

    public TextMeshProUGUI uiGameWonTxtTmPro;
    public Animator uiGameWonTxtAnim;

    public GameObject uiLevelFailed;
    public TextMeshProUGUI uiLevelFailedTmPro;
    public Animator uiLevelFailedAnim;

    public TextMeshProUGUI creditsTmPro;
    public Animator creditsAnim;

    public TextMeshProUGUI creditsChildTmPro;


    public GameObject uiMenuIcon;
    public TextMeshProUGUI uiCurrentLevel;
    public TextMeshProUGUI uiCurrentLevelObjective;

    public GameManager gameManager;
    public TutorialUI tutorialUI;
    public SoundManager soundManager;

    public Player player;

    private void Start()
    {
        lightLevelWinLight.enabled = false;
        uiGameWon3DLight.enabled = false;

        creditsAnim.SetBool("BetweenLevels", true);
        creditsAnim.SetBool("Long", false);
        //uiLevelFailedT.enabled = false;
        //uiLevelCompletedT.enabled = false;
        uiGameWonTxtTmPro.enabled = false;
        uiStart.SetActive(true);
        uiStart3D.SetActive(true);
        uiMenuIcon.SetActive(false);

        //uiLevelCompleted.SetActive(false);
        //uiLevelCompleted3D.SetActive(false);
        //uiLevelFailed.SetActive(false);
        ActivateUItext(uiLevelFailed, uiLevelFailedTmPro);
        ActivateUItext(uiLevelCompleted, uiLevelCompletedTmPro);

        uiLevelFailed3D.SetActive(true);
        uiLevelFailed3DMesh.enabled = false;

        uiGameWon3D.SetActive(true);
        uiGameWon3DMesh.enabled = false;

        uiLevelCompleted3D.SetActive(true);
        uiLevelCompleted3DMesh.enabled = false;

        //Activate3Dtext(uiLevelFailed3D);
        //Activate3Dtext(uiGameWon3D);
        //Activate3Dtext(uiLevelCompleted3D);

        menuTitle.SetActive(false);

        if (!Player.savedGameAvailable)
        {
            uiStartTmPro.text = "START";
            newGameText.SetActive(false);
        }
        else
        {
            uiStartTmPro.text = "CONTINUE: " + (gameManager.levelToLoad + 1);
            newGameText.SetActive(true);
        }


        if (gameManager.levelToLoad == 0)
        {
        }
        else
        {
        }
        //uiLevelFailedT.text = "RETRY: " + (gameManager.levelToLoad + 1);
        uiLevelFailedTmPro.text = "RE-ENTER: " + (gameManager.levelToLoad + 1);
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
    //private void Activate3Dtext(GameObject go)
    //{
    //    go.SetActive(true);
    //    go.GetComponent<MeshRenderer>().enabled = false;
    //}

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
        lightLevelWinLight.enabled = true;
        uiGameWon3DLight.enabled = true;

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
            uiGameWonTxtTmPro.enabled = false;
            uiStart3D.SetActive(false);
            uiMenuIcon.SetActive(true);
            creditsAnim.SetBool("BetweenLevels", false);
        }
    }

    public void ContinueToNextLevel()
    {
        if (!AchievementButton.levelMenuOpen)
        {
            gameManager.LevelStartTriggered(true);
            tutorialUI.LoadTutorial();
            uiLevelCompletedTmPro.enabled = false;
            uiLevelCompletedAnim.SetBool("BetweenLevels", false);
            //uiLevelCompleted.GetComponent<Animator>().SetBool("BetweenLevels", false);
            uiLevelCompleted3DAnim.SetBool("BetweenLevels", false);
            //uiLevelCompleted3D.GetComponent<Animator>().SetBool("BetweenLevels", false);
            uiGameWon3DAnim.SetBool("BetweenLevels", false);
            //uiGameWon3D.GetComponent<Animator>().SetBool("BetweenLevels", false);
            uiGameWonTxtAnim.SetBool("BetweenLevels", false);
            //uiGameWonTxtTmPro.GetComponent<Animator>().SetBool("BetweenLevels", false);
            creditsAnim.SetBool("BetweenLevels", false);
        }
    }
    public void FromLoseScreenToAnotherLevel() // not accurate description
    {
        gameManager.LevelStartTriggered(true);
        FailScreenExit();
    }
    public void RetryLevel()
    {
        if (!AchievementButton.levelMenuOpen)
        {
            gameManager.LevelStartTriggered(false);
            FailScreenExit();
        }
    }

    private void FailScreenExit()
    {
        tutorialUI.LoadTutorial();
        uiLevelFailedTmPro.enabled = false;
        uiLevelFailedAnim.SetBool("BetweenLevels", false);
        //uiLevelFailed.GetComponent<Animator>().SetBool("BetweenLevels", false);
        uiLevelFailed3DAnim.SetBool("BetweenLevels", false);
        //uiLevelFailed3D.GetComponent<Animator>().SetBool("BetweenLevels", false);
        uiGameWon3DAnim.SetBool("BetweenLevels", false);
        //uiGameWon3D.GetComponent<Animator>().SetBool("BetweenLevels", false);
        creditsAnim.SetBool("BetweenLevels", false);
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
            uiGameWonTxtTmPro.enabled = notInMenu;
        }
        if (GameManager.death)
        {
            uiLevelFailed3Dtext.enabled = notInMenu;
            uiLevelFailedTmPro.enabled = notInMenu;
        }
        if (GameManager.levelCompleted)
        {
            //tutorialUI.tipGameModes, tipOrbActivation, tipDamageControl, tipReenteringOrbits;

            if (tutorialUI.tipGameModes.activeInHierarchy)
            {

            }

            bool tipIsActive = false;
            foreach (Transform child in tutorialUI.tips.transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    tipIsActive = true;
                }
            }
            if (!tipIsActive)
            {
                uiLevelCompletedTmPro.enabled = notInMenu;
            }
            uiLevelCompleted3Dtext.enabled = notInMenu;
        }
        if (GameManager.betweenLevels)// || GameManager.inTutorial)
            menuTitle.SetActive(!notInMenu);
        
        creditsTmPro.enabled = notInMenu;
        //creditsAnim.GetComponent<TextMeshProUGUI>().enabled = notInMenu;
        creditsChildTmPro.enabled = notInMenu;
        //creditsAnim.GetComponentInChildren<TextMeshProUGUI>().enabled = notInMenu;
    }
    public void ShowTextLevelFailed()
    {
        uiLevelFailedTmPro.text = "RE-ENTER: " + (GameManager.levelProgression + 1);
        //uiLevelFailed.GetComponentInChildren<TextMeshProUGUI>().text = "RE-ENTER: " + (GameManager.levelProgression + 1);
        uiLevelFailedAnim.SetBool("BetweenLevels", true);
        //uiLevelFailed.GetComponent<Animator>().SetBool("BetweenLevels", true);
        uiLevelFailedTmPro.enabled = true;

        if (!PauseMenu.exitingOrbit)
        {
            uiLevelFailed3DAnim.SetBool("BetweenLevels", true);
            //uiLevelFailed3D.GetComponent<Animator>().SetBool("BetweenLevels", true);
            uiLevelFailed3DMesh.enabled = true;
            //uiLevelFailed3D.GetComponent<MeshRenderer>().enabled = true;
        }

        if (GameManager.inTutorial)
            tutorialUI.UnloadTutorial();

        creditsAnim.SetBool("BetweenLevels", true);
        creditsAnim.SetBool("Long", true);
    }
    public static float delayTextCompleted;

    public string txtWin, txtWinSilver, txtWinGold;
    public void ShowTextLevelCompleted(bool newSilver, bool newGold)
    {
        if (!GameManager.silver && !GameManager.gold)
            uiLevelCompleted3Dtext.text = txtWin;
        else 
        {
            if (GameManager.gold)
                uiLevelCompleted3Dtext.text = txtWinGold;
            else if (GameManager.silver)
                uiLevelCompleted3Dtext.text = txtWinSilver;
        }
        if (gameManager.easyMode)
            uiLevelCompleted3Dtext.text = txtWin;

        StartCoroutine(DelayedWinText(delayTextCompleted));
        uiLevelCompleted3DAnim.SetBool("BetweenLevels", true);
        uiLevelCompleted3DMesh.enabled = true;
        lightLevelWinLight.enabled = false;

        if (GameManager.inTutorial)
            tutorialUI.UnloadTutorial();

        creditsAnim.SetBool("BetweenLevels", true);
        creditsAnim.SetBool("Long", true);
    }
    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

    IEnumerator DelayedWinText(float t)
    {
        yield return new WaitForSeconds(t);
        uiLevelCompletedTmPro.text = "ENTER: " + (GameManager.levelProgression + 1);
        uiLevelCompletedAnim.SetBool("BetweenLevels", true);
        uiLevelCompletedTmPro.enabled = true;
        lightLevelWinLight.enabled = true;
        lightLevelWin.Play(0);
    }
    public void ShowTextGameWon()
    {
        uiGameWon3D.SetActive(true);
        uiGameWon3DMesh.enabled = true;
        //uiGameWon3D.GetComponent<MeshRenderer>().enabled = true;
        uiGameWon3DAnim.SetBool("BetweenLevels", true);
        //uiGameWon3D.GetComponent<Animator>().SetBool("BetweenLevels", true);

        uiGameWonTxtAnim.SetBool("BetweenLevels", true); 
        //uiGameWonTxtTmPro.GetComponent<Animator>().SetBool("BetweenLevels", true);
        uiGameWonTxtTmPro.enabled = true;

        creditsAnim.SetBool("BetweenLevels", true);
        creditsAnim.SetBool("Long", false);
    }
    #endregion
}
