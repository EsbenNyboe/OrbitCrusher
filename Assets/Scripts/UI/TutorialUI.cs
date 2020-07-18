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

    public GameObject tips;
    public GameObject tipGameModes, tipOrbActivation, tipDamageControl, tipReenteringOrbits;

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
    public UIManager uiManager;

    public static int numberOfActiveTextboxes;

    private void Awake()
    {
        textSkipTutorial.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        panel.SetActive(false);
        tips.SetActive(true);
        tipGameModes.SetActive(false);
        tipOrbActivation.SetActive(false);
        tipDamageControl.SetActive(false);
        tipReenteringOrbits.SetActive(false);
    }
    private void Update()
    {
        if (GameManager.inTutorial && clickAnywhereAllowed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClickAnywhere();
            }
        }
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
        NumberDisplayTargetNode.targetNodeIndexMemory = LevelManager.levelObjectiveCurrent;
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

    GameObject[] textStackHidden;
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
        if (numberOfActiveTextboxes > 1)
        {
            AddToStack(text4);
            AddToStack(text5B);
            AddToStack(text5);
            AddToStack(text6);
            AddToStack(text7);
        }
        timeStopQueued = true;
        yield return new WaitForSeconds(time);
        //soundManager.StopAndQueueTutorialMusicForReschedule();
        TimeStopInstant();
        timeStopQueued = false;
    }
    public void TimeNormal()     // called from the canvas
    {
        StopAllCoroutines();
        numberOfActiveTextboxes--;
        if (numberOfActiveTextboxes > 0)
        {
            RemoveFromStack(text4);
            RemoveFromStack(text5B);
            RemoveFromStack(text5);
            RemoveFromStack(text6);
            RemoveFromStack(text7);
        }
        if (numberOfActiveTextboxes == 0)
        {
            tutorialPause = false;
            Time.timeScale = 1;
            //soundManager.RescheduleQueuedMusic();
            soundDsp.RescheduleQueuedMusic();
        }
        clickAnywhereAllowed = false;
    }
    private void AddToStack(GameObject text)
    {
        if (text.activeInHierarchy)
        {
            GameObject[] oldArray = textStackHidden;
            int length = 1;
            if (oldArray != null)
                length = oldArray.Length + 1;
            textStackHidden = new GameObject[length];
            for (int i = 0; i < length; i++)
            {
                if (length - 1 > i)
                    textStackHidden[i] = oldArray[i];
            }
            textStackHidden[length - 1] = text;
            text.SetActive(false);
        }
    }
    private void RemoveFromStack(GameObject text)
    {
        if (textStackHidden != null && textStackHidden.Length > 0)
        {
            if (textStackHidden[textStackHidden.Length - 1] == text)
            {
                GameObject[] oldArray = textStackHidden;
                textStackHidden = new GameObject[oldArray.Length - 1];
                for (int i = 0; i < textStackHidden.Length; i++)
                {
                    textStackHidden[i] = oldArray[i];
                }
                text.SetActive(true);
            }
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
        textSkipTutorial.SetActive(true);
        tutorialSkip = false;
    }


    private GameObject activeTextbox;
    private bool clickAnywhereAllowed;
    private IEnumerator EnableClickAnywhere(GameObject go)
    {
        yield return new WaitForSecondsRealtime(2f);
        activeTextbox = go;
        clickAnywhereAllowed = true;
    }
    private void ClickAnywhere()
    {
        activeTextbox.SetActive(false);
        activeTextbox.GetComponentInChildren<HoverGraphicText>().TextClicked();
        TimeNormal();
    }

    public void ShowTextFirstSpawn()
    {
        if (!textShown1 && !tutorialSkip)
        {
            panel.SetActive(true);
            text1.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstSpawnDelay));
            StartCoroutine(EnableClickAnywhere(text1));
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
            StartCoroutine(EnableClickAnywhere(text2));
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
                StartCoroutine(EnableClickAnywhere(text5B));
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
                StartCoroutine(EnableClickAnywhere(text3));
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
                StartCoroutine(EnableClickAnywhere(text5));
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
            StartCoroutine(EnableClickAnywhere(text6));
        }
        textShown6 = true;
    }
    public void ShowTextFirstRedNodeHit()
    {
        if (!textShown7 && !tutorialSkip)
        {
            text7.SetActive(true);
            StartCoroutine(TimeStopDelayed(firstRedNodeHitDelay));
            StartCoroutine(EnableClickAnywhere(text7));
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
                StartCoroutine(EnableClickAnywhere(text8));
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
                StartCoroutine(EnableClickAnywhere(text4));
            }
            textShown4 = true;
        }
    }


    public void DisplayTipsOnMenuToggle(bool notInMenu)
    {
        tips.SetActive(notInMenu);
    }

    public void DisplayTip(int levelNumber)
    {
        print("display:" + levelNumber);
        switch (levelNumber)
        {
            case 1:
                StartCoroutine(ShowTip(tipGameModes));
                break;
            case 3:
                StartCoroutine(ShowTip(tipOrbActivation));
                break;
            case 5:
                StartCoroutine(ShowTip(tipDamageControl));
                break;
            case 7:
                StartCoroutine(ShowTip(tipReenteringOrbits));
                break;
        }
    }

    private IEnumerator ShowTip(GameObject tip)
    {
        tip.SetActive(true);
        ToggleComponentsActivation(tip, false);
        yield return new WaitForSeconds(0.1f);
        ToggleComponentsActivation(tip, true);
        TipUIDisabling();
    }

    private static void ToggleComponentsActivation(GameObject tip, bool status)
    {
        tip.GetComponent<Image>().enabled = status;
        tip.GetComponentInChildren<TextMeshProUGUI>().enabled = status;
        tip.GetComponentInChildren<Button>().GetComponentInParent<TextMeshProUGUI>().enabled = status;
        tip.GetComponentInChildren<HoverGraphicText>().GetComponent<TextMeshProUGUI>().enabled = status;
    }

    private void TipUIDisabling()
    {
        uiManager.uiLevelCompletedT.enabled = false;
        uiManager.uiLevelCompletedT.GetComponentInParent<Button>().enabled = false;
    }
    public void TipUIEnable()
    {
        uiManager.uiLevelCompletedT.enabled = true;
        uiManager.uiLevelCompletedT.GetComponentInParent<Button>().enabled = true;
    }
}