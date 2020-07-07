using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public GameObject panel;
    public GameObject textSkipTutorial;
    public GameObject text1;
    public GameObject text2;
    public GameObject text3;
    public GameObject text4;
    public GameObject text5B;
    public GameObject text5;
    public GameObject text6;
    public GameObject text7;
    public GameObject text8;

    public GameObject showTip;
    public TextMeshProUGUI showTipT;
    public GameObject tips;
    public GameObject tipUnclickables;
    public GameObject tipCometDanger;
    public GameObject tipPointExplanation;
    bool pointExplanationShown;

    public static bool timeStopQueued;
    public static bool textShown1;
    bool textShown2;
    bool textShown3;
    bool textShown4;
    bool textShown5B;
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
    public SoundDsp soundDsp;

    public static int numberOfActiveTextboxes;

    bool tipLoaded;
    bool tipShown;

    private void Awake()
    {
        textSkipTutorial.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        panel.SetActive(false);
        showTip.SetActive(true);
        showTipT.enabled = false;
        //showTipT.enabled = true;
    }

    public void LoadTutorial()
    {
        if (GameManager.inTutorial)
        {
            ResetTutorial();
            panel.SetActive(true);
            textSkipTutorial.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            textSkipTutorial.GetComponent<Animator>().Play(0);
        }
    }
    public void UnloadTutorial()
    {
        ResetTutorial();
        panel.SetActive(false);
        textSkipTutorial.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }
    public static bool tutorialSkip;
    public void SkipTutorial()
    {
        Time.timeScale = 1;
        textSkipTutorial.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        tutorialSkip = true;
        StopAllCoroutines();
        numberOfActiveTextboxes = 0;
        tutorialPause = false;
        panel.SetActive(false);
        UnloadTutorial();
        LevelManager.levelObjectiveCurrent = 10;
        levelManager.ObjectiveCompleted();
        //soundManager.LevelCompleted(false);
        //soundManager.FadeInMusicBetweenLevels();
        soundManager.levelCompleted.TriggerAudioObject();
    }
    bool tutorialPause;
    public void ToggleWhenMenu(bool notInMenu)
    {
        if (GameManager.inTutorial)
        {
            panel.SetActive(notInMenu);
            textSkipTutorial.GetComponentInChildren<TextMeshProUGUI>().enabled = notInMenu;
            if (!tutorialPause)
            {
                if (notInMenu)
                {
                    soundDsp.RescheduleQueuedMusic();
                    Time.timeScale = 1;
                }
                else
                {
                    soundDsp.StopAndQueueMusicForRescheduling();
                    Time.timeScale = 0;
                }
            }
        }
    }

    public void TimeStopInstant()
    {
        if (!tutorialSkip)
        {
            soundDsp.StopAndQueueMusicForRescheduling();
            //soundManager.KillOrbSoundInstant();
            tutorialPause = true;
            Time.timeScale = 0;
        }
    }
    IEnumerator TimeStopDelayed(float time)
    {
        soundManager.StopHealthSoundsWhenPausing();
        numberOfActiveTextboxes++;
        timeStopQueued = true;
        yield return new WaitForSeconds(time);
        //soundManager.StopAndQueueTutorialMusicForReschedule();
        TimeStopInstant();
        timeStopQueued = false;
    }
    public void TimeNormal()     // called from the canvas
    {
        numberOfActiveTextboxes--;
        if (numberOfActiveTextboxes == 0)
        {
            tutorialPause = false;
            Time.timeScale = 1;
            //soundManager.RescheduleQueuedMusic();
            soundDsp.RescheduleQueuedMusic();
        }
    }
    public void ResetTutorial()
    {
        numberOfActiveTextboxes = 0;
        textShown1 = textShown2 = textShown3 = textShown4 = textShown5B = textShown5 = textShown6 = textShown7 = textShown8 = false;
        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        text4.SetActive(false);
        text5B.SetActive(false);
        text5.SetActive(false);
        text6.SetActive(false);
        text7.SetActive(false);
        text8.SetActive(false);
        tipUnclickables.SetActive(false);
        tipCometDanger.SetActive(false);
        tipPointExplanation.SetActive(false);
        tips.SetActive(false);
        textSkipTutorial.SetActive(true);
        tutorialSkip = false;
    }



    public void ShowTextFirstSpawn()
    {
        if (!textShown1 && !tutorialSkip)
        {
            panel.SetActive(true);
            text1.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstSpawnDelay));
        }
        textShown1 = true;
    }
    bool text1Done;
    public Button text1Button;
    public void ClickOrbAfterFirstSpawn()
    {
        if (!text1Done && textShown1 && !textShown2) 
        {
            text1Done = true;
            text1Button.onClick.Invoke();
        }
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
    public void ShowTextCorrectHitButStillMoreOrbsToGo()
    {
        if (GameManager.inTutorial)
        {
            if (!textShown5B && !tutorialSkip && EnergySphereBehavior.gluedObjects == null)
            {
                text5B.SetActive(true);
                StartCoroutine(TimeStopDelayed(firstCorrectHitDelay));
            }
            textShown5B = true;
        }
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
        {
            showTipT.enabled = true;
            showTip.GetComponent<Animator>().SetBool("betweenLevels", true);
        }
    }
    
    public void ShowTipsWhenDesired()
    {
        tipShown = true;
        tips.SetActive(true);
        //showTip.SetActive(false);
        showTip.GetComponent<Animator>().SetBool("betweenLevels", false);
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
        //showTip.SetActive(false);
        if (showTip.activeInHierarchy)
            showTip.GetComponent<Animator>().SetBool("betweenLevels", false);
    }
    public void DisplayTipsOnMenuToggle(bool notInMenu)
    {
        if (tipShown)
            tips.SetActive(notInMenu);
        showTipT.enabled = notInMenu;
    }
}