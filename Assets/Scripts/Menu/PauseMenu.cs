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

    // Start is called before the first frame update
    private void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        uiManager = FindObjectOfType<UIManager>();
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
            soundManager.UnpauseAll();
            Time.timeScale = 1;
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
            soundManager.PauseAll();
            Time.timeScale = 0;
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