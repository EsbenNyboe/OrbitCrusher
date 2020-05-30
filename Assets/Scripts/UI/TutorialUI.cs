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
    public GameObject text8;

    public GameObject showTip;
    public GameObject tips;
    public GameObject tipUnclickables;
    public GameObject tipCometDanger;
    public GameObject tipPointExplanation;
    bool pointExplanationShown;

    bool timeStopQueued;
    public static bool textShown1;
    bool textShown2;
    bool textShown3;
    bool textShown4;    
    bool textShown5;
    bool textShown6;
    bool textShown7;
    bool textShown8;

    bool tutorialActive;
    public float firstSpawnDelay;
    public float firstCorrectHitDelay;
    public float healthBarDelay;
    public float firstObjFailDelay;
    public float firstCometHitDelay;
    public float firstRedNodeHitDelay;
    public float threeAlignedDelay;
    public float clickDeniedDelay;

    public LevelManager levelManager;
    public SoundManager soundManager;

    public static int numberOfActiveTextboxes;

    private void Awake()
    {
        panel.SetActive(false);
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
    public static bool tutorialSkip;
    public void SkipTutorial()
    {
        tutorialSkip = true;
        StopAllCoroutines();
        numberOfActiveTextboxes = 0;
        Time.timeScale = 1;
        panel.SetActive(false);
        UnloadTutorial();
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
        if (!tutorialSkip)
        {
            soundManager.KillSphereSoundInstant();
            Time.timeScale = 0;
        }
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
        textShown1 = textShown2 = textShown3 = textShown4 = textShown5 = textShown6 = textShown7 = textShown8 = false;
        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        text4.SetActive(false);
        text5.SetActive(false);
        text6.SetActive(false);
        text7.SetActive(false);
        text8.SetActive(false);
        tipUnclickables.SetActive(false);
        tipCometDanger.SetActive(false);
        tipPointExplanation.SetActive(false);
        tips.SetActive(false);
        showTip.SetActive(false);
        textSkipTutorial.SetActive(true);
    }



    public void ShowTextFirstSpawn()
    {
        if (!textShown1 && !tutorialSkip)
        {
            text1.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstSpawnDelay));
        }
        textShown1 = true;
    }
    public void ShowTextFirstCorrectHit()
    {
        if (!textShown2 && !tutorialSkip)
        {
            text2.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstCorrectHitDelay));
        }
        textShown2 = true;
    }
    public void ShowTextFirstHealthbarCharge()
    {
        if (textShown1 && textShown2 && !tutorialSkip)
        {
            if (!textShown3)
            {
                if (timeStopQueued)
                {
                    StopAllCoroutines();
                }
                text3.SetActive(true);
                StartCoroutine(TimeStopDelayed(healthBarDelay));
            }
            textShown3 = true;
        }
    }

    public void ShowTextFirstObjectiveFailed()
    {
        if (textShown1 && textShown2 && !tutorialSkip)
        {
            if (!textShown5)
            {
                text5.SetActive(true);
                StartCoroutine(TimeStopDelayed(firstObjFailDelay));
            }
            textShown5 = true;
        }
    }
    public void ShowTextFirstCometHit()
    {
        if (!textShown6 && !tutorialSkip)
        {
            text6.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstCometHitDelay));
        }
        textShown6 = true;
    }
    public void ShowTextFirstRedNodeHit()
    {
        if (!textShown7 && !tutorialSkip)
        {
            text7.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstRedNodeHitDelay));
        }
        textShown7 = true;
    }
    public void ShowTextThreeAligned(int currentObjective)
    {
        if (currentObjective == 5)
        {
            if (!textShown8 && !tutorialSkip)
            {
                text8.SetActive(true);
                StartCoroutine(TimeStopDelayed(threeAlignedDelay));
            }
            textShown8 = true;
        }
    }

    public void ShowTextPickupDenied()
    {
        if (GameManager.inTutorial)
        {
            if (!textShown4 && !tutorialSkip)
            {
                text4.SetActive(true);
                StartCoroutine(TimeStopDelayed(clickDeniedDelay));
            }
            textShown4 = true;
        }
    }


    public void LoadTipOnLevelFailed()
    {
        if (!tipLoaded)
        {
            tips.SetActive(false);
            if (!textShown4)
            {
                tipUnclickables.SetActive(true);
                textShown4 = true;
                tipLoaded = true;
            }
            else if (!textShown6)
            {
                tipCometDanger.SetActive(true);
                textShown6 = true;
                tipLoaded = true;
            }
            else if (!pointExplanationShown)
            {
                tipPointExplanation.SetActive(true);
                pointExplanationShown = true;
                tipLoaded = true;
            }
        }
        if (tipLoaded)
            showTip.SetActive(true);
    }
    bool tipLoaded;
    bool tipShown;
    public void ShowTipsWhenDesired()
    {
        tipShown = true;
        tips.SetActive(true);
        showTip.SetActive(false);
    }
    public void HideTipsOnLevelLoaded()
    {
        if (tipShown)
        {
            tipUnclickables.SetActive(false);
            tipCometDanger.SetActive(false);
            tipPointExplanation.SetActive(false);
            tipShown = false;
            tipLoaded = false;
        }
        showTip.SetActive(false);
    }
}