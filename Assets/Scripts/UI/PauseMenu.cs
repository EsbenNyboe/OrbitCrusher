using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool menu;
    public GameObject panel;
    public GameObject audioSettings;
    public GameObject menuIcon;
    DisplayVolSliders displayVolSliders;
    public static bool firstLoad = true;
    bool audioSettingsIsActive;

    SoundManager soundManager;
    UIManager uiManager;
    TutorialUI tutorialUI;

    // Start is called before the first frame update
    private void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        uiManager = FindObjectOfType<UIManager>();
        tutorialUI = FindObjectOfType<TutorialUI>();
    }
    void Start()
    {
        displayVolSliders = FindObjectOfType<DisplayVolSliders>();
        displayVolSliders.ToggleSliderDisplay(menu);
        panel.SetActive(menu);
    }
    public void ToggleMenu()
    {
        if (panel.activeInHierarchy)
        {
            uiManager.ToggleIngameUI(true);
            menuIcon.SetActive(true);
            if (!GameManager.inTutorial)
            {
                soundManager.UnpauseAll();
                Time.timeScale = 1;
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
        }
        else
        {
            uiManager.ToggleIngameUI(false);
            menuIcon.SetActive(false);
            if (!GameManager.inTutorial)
            {
                soundManager.PauseAll();
                Time.timeScale = 0;
            }
            else
            {
                tutorialUI.ToggleWhenMenu(false);
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
}