using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject menuTitle;
    public GameObject ui3Dtext;
    public GameObject uiStart3D;
    public GameObject uiLevelFailed3D;
    public GameObject uiLevelCompleted3D;
    public GameObject uiIngame;
    public GameObject uiStart;
    public GameObject uiLevelCompleted;
    public GameObject uiGameWon;
    public GameObject uiLevelFailed;
    public GameObject uiMenuIcon;
    public Color uiHoverColor;
    GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        uiStart.SetActive(true);
        uiStart3D.SetActive(true);
        uiMenuIcon.SetActive(false);

        uiLevelCompleted.SetActive(false);
        uiLevelCompleted3D.SetActive(false);
        uiLevelFailed.SetActive(false);
        uiLevelFailed3D.SetActive(false);
        uiGameWon.SetActive(false);

        menuTitle.SetActive(false);
    }

    public void ShowTextGameWon()
    {
        uiStart3D.SetActive(true);
        uiGameWon.SetActive(true);
    }
    public void ShowTextLevelCompleted()
    {
        uiLevelCompleted.SetActive(true);
        uiLevelCompleted3D.SetActive(true);
    }
    public void ShowTextLevelFailed()
    {
        uiLevelFailed3D.SetActive(true);
        uiLevelFailed.SetActive(true);
    }
    public void StartGame()
    {
        gameManager.LevelStartTriggered();
        uiStart.SetActive(false);
        uiStart3D.SetActive(false);
        uiGameWon.SetActive(false);
        uiMenuIcon.SetActive(true);
    }
    public void ContinueToNextLevel()
    {
        gameManager.LevelStartTriggered();
        uiLevelCompleted.SetActive(false);
        uiLevelCompleted3D.SetActive(false);
    }
    public void RetryLevel()
    {
        gameManager.LevelStartTriggered();
        uiLevelFailed.SetActive(false);
        uiLevelFailed3D.SetActive(false);
    }
    public void ToggleIngameUI(bool notInMenu)
    {
        uiIngame.SetActive(notInMenu);
        ui3Dtext.SetActive(notInMenu);
        if (notInMenu)
        {
            menuTitle.SetActive(false);
        }
        else
        {
            menuTitle.SetActive(true);
        }
    }
}
