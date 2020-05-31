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
    public static bool tutorialPause;
    public void ToggleMenu()
    {
        if (panel.activeInHierarchy)
        {
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
                if (!tutorialPause)
                    Time.timeScale = 1;
            }
            menu = false;
            displayVolSliders.ToggleSliderDisplay(false);
            if (audioSettings.activeInHierarchy)
                audioSettingsIsActive = true;
            else
                audioSettingsIsActive = false;
        }
        else
        {
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
                if (!tutorialPause)
                    Time.timeScale = 0;
            }
            menu = true;
            if (audioSettingsIsActive)
                displayVolSliders.ToggleSliderDisplay(true);
        }
        panel.SetActive(menu);
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