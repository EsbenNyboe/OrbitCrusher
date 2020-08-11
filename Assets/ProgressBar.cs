using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [HideInInspector]
    public int currentLevel;
    [HideInInspector]
    public int currentStage;
    int maxStage;
    [HideInInspector]
    public bool displayProgress;

    public Image[] barImages;
    public RectTransform[] barTransforms;
    public Animator[] barAnimators;
    public ProgressBarChunk[] progressBarChunks;

    public float medianValue, offset, spacingValue;

    public Color cDefault, cCompleted, cCurrent;

    public Animator parentAnim;

    public TextMeshProUGUI levelNumberTmpro;

    private void Start()
    {
        for (int i = 0; i < barImages.Length; i++)
        {
            barImages[i].enabled = false;
            progressBarChunks[i].stageText.enabled = false;
        }
        levelNumberTmpro.enabled = false;
        ApplyNumbers();
    }
    public void DisplayProgress(bool displayStatus)
    {
        displayProgress = displayStatus;
        //if (GameManager.inTutorial)
        //    displayProgress = false;

        if (displayProgress)
        {
            for (int i = 0; i < barImages.Length; i++)
            {
                barImages[i].color = cDefault;
                barAnimators[i].SetBool("CurrentStage", false);
            }
            parentAnim.SetBool("BetweenLevels", false);
        }
        else
        {
            parentAnim.SetBool("BetweenLevels", true);
            if (GameManager.death)
                parentAnim.SetBool("WinOrLose", false);
            else
                parentAnim.SetBool("WinOrLose", true);
        }
    }
    public void SetProgressBarLevel(int level, int amountOfStages)
    {
        currentLevel = level;
        maxStage = amountOfStages;
        if (maxStage > progressBarChunks.Length)
        {
            print("error: progress bar not big enough for amount of stages");
            maxStage = progressBarChunks.Length;
        }

        string numberAppend = "th";
        if (currentLevel < 3)
        {
            if (currentLevel == 0)
                numberAppend = "st";
            else if (currentLevel == 1)
                numberAppend = "nd";
            else if (currentLevel == 2)
                numberAppend = "rd";
        }
        levelNumberTmpro.enabled = true;
        levelNumberTmpro.text = (currentLevel + 1) + numberAppend + " ORBIT";
        for (int i = 0; i < maxStage; i++)
        {
            barImages[i].enabled = true;
            progressBarChunks[i].stageText.enabled = true;
        }
        for (int i = maxStage; i < barImages.Length; i++)
        {
            barImages[i].enabled = false;
            progressBarChunks[i].stageText.enabled = false;
        }
    }

    public void SetProgressBarStage(int stage)
    {
        currentStage = stage;
        if (currentStage < maxStage)
        {
            barImages[currentStage].color = cCurrent;
            barAnimators[currentStage].SetBool("CurrentStage", true);
        }
        if (currentStage - 1 < maxStage)
        {
            for (int i = 0; i < currentStage; i++)
            {
                barImages[i].color = cCompleted;
                barAnimators[i].SetBool("CurrentStage", false);
            }
        }
    }






    public void PlacementAndSize()
    {
        for (int i = 0; i < barTransforms.Length; i++)
        {
            barTransforms[i].SetTop(medianValue - spacingValue * i + offset);
            barTransforms[i].SetBottom(medianValue + spacingValue * i - offset);
        }
        ApplyNumbers();
    }
    public void ApplyNumbers()
    {
        for (int i = 0; i < progressBarChunks.Length; i++)
        {
            progressBarChunks[i].stageText.text = (i + 1).ToString();
        }
    }
}
