using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationPause : MonoBehaviour
{
    public GameManager gameManager;
    public PauseMenu pauseMenu;

    bool isPaused = false;

    void OnGUI()
    {
        if (isPaused)
            GUI.Label(new Rect(100, 100, 50, 30), "Game paused");
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (!GameManager.betweenLevels && gameManager.autoPauseEnabled)
        {
            isPaused = !hasFocus;
            if (isPaused)
            {
                pauseMenu.EnterMenu();
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!GameManager.betweenLevels && gameManager.autoPauseEnabled)
        {
            isPaused = pauseStatus;
            if (isPaused)
            {
                pauseMenu.EnterMenu();
            }
        }
    }
}
