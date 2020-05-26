using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public GameObject panel;
    public GameObject textSkipTutorial;
    public GameObject text1;
    public GameObject text2;
    public GameObject text3;
    public GameObject text4;
    public GameObject text5;
    public GameObject text6;
    public GameObject text7;

    bool timeStopQueued;
    public static bool textShown1;
    bool textShown2;
    bool textShown3;
    bool textShown4;
    bool textShown5;
    bool textShown6;
    bool textShown7;
    public float firstSpawnDelay;
    public float firstCorrectHitDelay;
    public float healthBarDelay;
    public float firstObjFailDelay;
    public float firstCometHitDelay;
    public float firstRedNodeHitDelay;
    public float threeAlignedDelay;

    GameManager gameManager;
    LevelManager levelManager;
    SoundManager soundManager;

    int numberOfActiveTextboxes;

    private void Awake()
    {
        panel.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }
    public void LoadTutorial()
    {
        if (GameManager.inTutorial)
        {
            ResetTutorial();
            panel.SetActive(true);
        }
    }
    public void UnloadTutorial()
    {
        panel.SetActive(false);
    }
    public void SkipTutorial()
    {
        TimeNormal();
        panel.SetActive(false);
        GameManager.inTutorial = false;
        LevelManager.levelObjectiveCurrent = 10;
        levelManager.ObjectiveCompleted();
    }
    public void ToggleWhenMenu(bool notInMenu)
    {
        if (GameManager.inTutorial)
        {
            panel.SetActive(notInMenu);
        }
    }

    public void TimeStopInstant()
    {
        soundManager.KillSphereSoundInstant();
        Time.timeScale = 0;
    }
    IEnumerator TimeStopDelayed(float time)
    {
        numberOfActiveTextboxes++;
        timeStopQueued = true;
        yield return new WaitForSeconds(time);
        TimeStopInstant();
        timeStopQueued = false;
    }
    public void TimeNormal()
    {
        numberOfActiveTextboxes--;
        if (numberOfActiveTextboxes == 0)
            Time.timeScale = 1;
    }
    public void ResetTutorial()
    {
        numberOfActiveTextboxes = 0;
        textShown1 = textShown2 = textShown3 = textShown4 = textShown5 = textShown6 = false;
        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        text4.SetActive(false);
        text5.SetActive(false);
        text6.SetActive(false);
        text7.SetActive(false);
        textSkipTutorial.SetActive(true);
    }



    public void ShowText1FirstSpawn()
    {
        if (!textShown1)
        {
            text1.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstSpawnDelay));
        }
        textShown1 = true;
    }
    public void ShowText2FirstCorrectHit()
    {
        if (!textShown2)
        {
            text2.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstCorrectHitDelay));
        }
        textShown2 = true;
    }
    public void ShowText7FirstHealthbarCharge()
    {
        if (textShown1 && textShown2)
        {
            if (!textShown7)
            {
                if (timeStopQueued)
                {
                    StopAllCoroutines();
                }
                text7.SetActive(true);
                StartCoroutine(TimeStopDelayed(healthBarDelay));
            }
            textShown7 = true;
        }
    }

    public void ShowText3FirstObjectiveFailed()
    {
        if (textShown1 && textShown2)
        {
            if (!textShown3)
            {
                text3.SetActive(true);
                StartCoroutine(TimeStopDelayed(firstObjFailDelay));
            }
            textShown3 = true;
        }
    }
    public void ShowText4FirstCometHit()
    {
        if (!textShown4)
        {
            text4.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstCometHitDelay));
        }
        textShown4 = true;
    }
    public void ShowText5FirstRedNodeHit()
    {
        if (!textShown5)
        {
            text5.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstRedNodeHitDelay));
        }
        textShown5 = true;
    }
    public void ShowText6ThreeAligned(int currentObjective)
    {
        if (currentObjective == 5)
        {
            if (!textShown6)
            {
                text6.SetActive(true);
                StartCoroutine(TimeStopDelayed(threeAlignedDelay));
            }
            textShown6 = true;
        }
    }

    
}