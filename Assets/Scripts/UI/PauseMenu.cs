using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;

public class PauseMenu : MonoBehaviour
{
    public static bool menu;
    public GameObject panel;
    public GameObject audioSettings;
    public MenuIcon menuIcon;
    public GameObject tutorialText;
    
    public static bool firstLoad = true;
    bool audioSettingsIsActive;

    public DisplayVolSliders displayVolSliders;
    public SoundManager soundManager;
    public UIManager uiManager;
    public TutorialUI tutorialUI;
    public GameManager gameManager;

    public GameObject exitOrbitText;
    public GameObject exitOrbitImage;
    public static bool exitingOrbit;

    public GameObject godModeButton;

    public TextMeshProUGUI pageNumberDisplayHowToPlay;
    public TextMeshProUGUI pageNumberDisplayAbout;
    private TextMeshProUGUI pageNumberDisplayCurrentSubMenu;
    public GameObject[] pagesInHowToPlay;
    public GameObject[] pagesInAbout;
    private GameObject[] pagesInCurrentSubMenu;
    public static int pageNumber = 1;

    public GameObject exitOrbitObject;
    public GameObject menuObject;

    public GameObject firstLevel;
    public GameObject secondLevel;

    public TextMeshProUGUI easyModeOnTmPro;
    public Animator easyModeOnAnim;


    // Start is called before the first frame update
    private void Awake()
    {
        //soundManager = FindObjectOfType<SoundManager>();
        //uiManager = FindObjectOfType<UIManager>();
        //tutorialUI = FindObjectOfType<TutorialUI>();
    }
    void Start()
    {
        //displayVolSliders = FindObjectOfType<DisplayVolSliders>();
        displayVolSliders.ToggleSliderDisplay(menu);
        panel.SetActive(menu);
        pageNumber = 1;
        ResetPagesInAbout();
        ResetPagesInHowToPlay();

        if (gameManager.easyMode)
            ChooseEasy();
        else
            ChooseNormal();
    }

    public static bool menuFrameClicked;
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!menuFrameClicked && menu)
            {
                //print("mouseAny");
                firstLevel.SetActive(true);
                foreach (Transform child in secondLevel.transform)
                {
                    child.gameObject.SetActive(false);
                }
                ExitMenu();
            }
            menuFrameClicked = false;
        }
    }
    public static void ClickedOnUI()
    {
        //print("clickedOnUI");
        menuFrameClicked = true;
    }
    public void ClickedWithinMenuFrame()
    {
        menuFrameClicked = true;
    }
    
    public void ToggleMenu()
    {
        if (GameManager.betweenLevels || GameManager.inTutorial)
        {
            exitOrbitText.SetActive(false);
            exitOrbitImage.SetActive(false);
        }
        else
        {
            exitOrbitText.SetActive(true);
            exitOrbitImage.SetActive(true);
        }
        if (panel.activeInHierarchy)
        {
            ExitMenu();
        }
        else
        {
            EnterMenu();
        }
    }

    public CometBehavior cometBehavior;
    public void EnterMenu()
    {
        uiManager.ToggleIngameUI(false);
        tutorialUI.DisplayTipsOnMenuToggle(false);
        menuIcon.DeactivateAnimator();
        menuIcon.gameObject.SetActive(false);
        tutorialText.SetActive(false);

        if (GameManager.inTutorial)
        {
            tutorialUI.ToggleWhenMenu(false);
        }
        else if (GameManager.betweenLevels)
        {
            Time.timeScale = 0;
            //cometBehavior.speedFreeBird = cometBehavior.speedFreeBird * 0.25f;
            soundManager.MenuMix(true);
        }
        else
        {
            soundManager.PauseAll();
            soundManager.MenuMix(true);
            Time.timeScale = 0;
        }

        menu = true;
        if (audioSettingsIsActive)
            displayVolSliders.ToggleSliderDisplay(true);

        panel.SetActive(menu);
    }

    private void ExitMenu()
    {
        tutorialUI.DisplayTipsOnMenuToggle(true);
        uiManager.ToggleIngameUI(true);
        menuIcon.gameObject.SetActive(true);
        tutorialText.SetActive(true);

        if (GameManager.inTutorial)
        {
            tutorialUI.ToggleWhenMenu(true);
        }
        else if (GameManager.betweenLevels)
        {
            Time.timeScale = 1;
            //cometBehavior.speedFreeBird = cometBehavior.speedFreeBird * 4f;
            soundManager.MenuMix(false);
        }
        else
        {
            Time.timeScale = 1;
            soundManager.UnpauseAll();
            soundManager.MenuMix(false);
        }

        if (!GameManager.inTutorial)
        {
        }
        else
        {
        }
        menu = false;
        displayVolSliders.ToggleSliderDisplay(false);
        if (audioSettings.activeInHierarchy)
            audioSettingsIsActive = true;
        else
            audioSettingsIsActive = false;

        ResetHowToPlayPageFocus();

        panel.SetActive(menu);
    }
    public GameObject skipTutorial;
    public void EnterDialogue_SkipTutorial()
    {
        ToggleMenu();
        skipTutorial.SetActive(true);
        menuObject.SetActive(false);
    }


    public void EnterDialogue_ExitOrbit()
    {
        ToggleMenu();
        exitOrbitObject.SetActive(true);
        menuObject.SetActive(false);
    }

    public LevelManager levelManager;
    public void ExitOrbit()
    {
        exitOrbitObject.SetActive(false);
        menuObject.SetActive(true);

        exitingOrbit = true;
        ToggleMenu();
        soundManager.ScheduleGameStateSound(soundManager.levelFailed, false, false);
        soundManager.ActivateGameStateSound(soundManager.levelFailed);
        GameManager.death = true;
        gameManager.godMode = false;
        gameManager.godMode = false;

        //gameManager.levelManager.FadeOutLevelMusicNew(true);
    }
    public void CloseApplication()
    {
        Application.Quit();
    }

    public void HoverUI()
    {
        soundManager.HoverUI();
    }
    public void ClickUI()
    {
        soundManager.ClickUI();
    }


    public void ResetPagesInHowToPlay()
    {
        pageNumberDisplayCurrentSubMenu = pageNumberDisplayHowToPlay;
        pagesInCurrentSubMenu = pagesInHowToPlay;
        ChoosePageInHowToPlay();
    }
    public void ResetPagesInAbout()
    {
        pageNumberDisplayCurrentSubMenu = pageNumberDisplayAbout;
        pagesInCurrentSubMenu = pagesInAbout;
        ChoosePageInHowToPlay();
    }
    public void ResetHowToPlayPageFocus()
    {
        pageNumber = 1;
        ChoosePageInHowToPlay();
    }
    public void TurnPageInHowToPlay_Right()
    {
        pageNumber++;
        if (pageNumber > pagesInCurrentSubMenu.Length)
            pageNumber = 1;
        ChoosePageInHowToPlay();
    }
    public void TurnPageInHowToPlay_Left()
    {
        pageNumber--;
        if (pageNumber < 1)
            pageNumber = pagesInCurrentSubMenu.Length;
        ChoosePageInHowToPlay();
    }

    private void ChoosePageInHowToPlay()
    {
        for (int i = 0; i < pagesInCurrentSubMenu.Length; i++)
        {
            if (i + 1 == pageNumber)
                pagesInCurrentSubMenu[i].SetActive(true);
            else
                pagesInCurrentSubMenu[i].SetActive(false);
        }
        pageNumberDisplayCurrentSubMenu.text = pageNumber + "/" + pagesInCurrentSubMenu.Length;
    }

    public GameObject chooseEasy, chooseNormal;
    public TextMeshProUGUI chooseEasyTmPro, chooseNormalTmPro;
    public Image chooseEasyImage, chooseNormalImage;
    public void ChooseEasy()
    {
        easyModeOnTmPro.enabled = true;
        chooseEasyTmPro.fontStyle = FontStyles.Bold;
        chooseEasyImage.enabled = true;
        chooseNormalTmPro.fontStyle = FontStyles.Normal;
        chooseNormalImage.enabled = false;
        gameManager.easyMode = true;
    }
    public void ChooseNormal()
    {
        easyModeOnTmPro.enabled = false;
        chooseNormalTmPro.fontStyle = FontStyles.Bold;
        chooseNormalImage.enabled = true;
        chooseEasyTmPro.fontStyle = FontStyles.Normal;
        chooseEasyImage.enabled = false;
        gameManager.easyMode = false;
    }

    public void LoadLevel()
    {
        easyModeOnAnim.SetBool("BetweenLevels", false);
    }
    public void UnloadLevel()
    {
        easyModeOnAnim.SetBool("BetweenLevels", true);
        menuObject.SetActive(true);
        exitOrbitObject.SetActive(false);
        skipTutorial.SetActive(false);
    }
}