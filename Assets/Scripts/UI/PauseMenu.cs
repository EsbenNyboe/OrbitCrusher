using System.Collections;
using System.Collections.Generic;
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

        panel.SetActive(menu);
    }

    public void ExitOrbit()
    {
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
}