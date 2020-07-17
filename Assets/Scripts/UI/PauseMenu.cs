using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool menu;
    public GameObject panel;
    public GameObject audioSettings;
    public GameObject menuIcon;
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

    public TextMeshProUGUI pageNumberDisplay;
    public GameObject[] pagesInHowToPlay;
    public static int pageNumber = 1;

    public GameObject exitOrbitObject;
    public GameObject menuObject;

    public GameObject firstLevel;
    public GameObject secondLevel;


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
        ChoosePageInHowToPlay();

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

    public void EnterMenu()
    {
        tutorialUI.DisplayTipsOnMenuToggle(false);
        uiManager.ToggleIngameUI(false);
        menuIcon.SetActive(false);
        tutorialText.SetActive(false);
        if (!GameManager.inTutorial)
        {
            soundManager.PauseAll();
            soundManager.MenuMix(true);
            Time.timeScale = 0;
        }
        else
        {
            tutorialUI.ToggleWhenMenu(false);
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
        menuIcon.SetActive(true);
        tutorialText.SetActive(true);
        if (!GameManager.inTutorial)
        {
            Time.timeScale = 1;
            soundManager.UnpauseAll();
            soundManager.MenuMix(false);
        }
        else
        {
            tutorialUI.ToggleWhenMenu(true);
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

    public void EnterDialogue_ExitOrbit()
    {
        ToggleMenu();
        exitOrbitObject.SetActive(true);
        menuObject.SetActive(false);
    }
    public void ExitOrbit()
    {
        exitOrbitObject.SetActive(false);
        menuObject.SetActive(true);

        exitingOrbit = true;
        ToggleMenu();
        soundManager.ScheduleGameStateSound(soundManager.levelFailed, false, false);
        soundManager.ActivateGameStateSound(soundManager.levelFailed);
        GameManager.death = true;
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


    public void ResetHowToPlayPageFocus()
    {
        pageNumber = 1;
        ChoosePageInHowToPlay();
    }
    public void TurnPageInHowToPlay()
    {
        pageNumber++;
        if (pageNumber > pagesInHowToPlay.Length)
            pageNumber = 1;
        ChoosePageInHowToPlay();
    }

    private void ChoosePageInHowToPlay()
    {
        for (int i = 0; i < pagesInHowToPlay.Length; i++)
        {
            if (i + 1 == pageNumber)
                pagesInHowToPlay[i].SetActive(true);
            else
                pagesInHowToPlay[i].SetActive(false);
        }
        pageNumberDisplay.text = pageNumber + "/" + pagesInHowToPlay.Length;
    }

    public GameObject chooseEasy, chooseNormal;
    public void ChooseEasy()
    {
        chooseEasy.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        chooseEasy.GetComponentInChildren<Image>().enabled = true;
        chooseNormal.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
        chooseNormal.GetComponentInChildren<Image>().enabled = false;
        gameManager.easyMode = true;
    }
    public void ChooseNormal()
    {
        chooseNormal.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        chooseNormal.GetComponentInChildren<Image>().enabled = true;
        chooseEasy.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
        chooseEasy.GetComponentInChildren<Image>().enabled = false;
        gameManager.easyMode = false;
    }
}