using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelNumberDisplay : MonoBehaviour
{
    public GameObject[] levelNumbers;
    Animator fadeInAndOut;

    private void Start()
    {
        //fadeInAndOut = GetComponent<Animator>();
    }
    public void LevelCompleted()
    {
        //DisplayLevelNumbers();
        //LevelNumberMakeGreen(true);
    }
    public void LevelFailed()
    {
        //DisplayLevelNumbers();
        //LevelNumberMakeGreen(false);
    }
    public void StartLevel()
    {
        //fadeInAndOut.SetBool("isBetweenLevels", false);
    }
    private void DisplayLevelNumbers()
    {
        //fadeInAndOut.enabled = true;
        //fadeInAndOut.SetBool("isBetweenLevels", true);
    }

    public void LevelNumberMakeGreen(bool win)
    {
        
        for (int i = 0; i < GameManager.levelProgression + 1; i++)
        {
            
            if (i == GameManager.levelProgression)
            {
                if (win)
                {
                    if (levelNumbers.Length > i)
                    {
                        levelNumbers[i].GetComponent<Animator>().enabled = true;
                        levelNumbers[i].GetComponent<Animator>().SetBool("CompletedNow", true);
                    }
                }
            }
            else
            {
                if (levelNumbers.Length > i)
                {
                    levelNumbers[i].GetComponent<Animator>().enabled = true;
                    levelNumbers[i].GetComponent<Animator>().SetBool("CompletedNow", false);
                }
            }
        }
    }
}
