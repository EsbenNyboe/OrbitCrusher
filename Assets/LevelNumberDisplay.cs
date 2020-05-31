using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNumberDisplay : MonoBehaviour
{
    public GameObject[] levelNumbers;
    int mostRecentLevelCompleted;
    Animator fadeInAndOut;

    private void Start()
    {
        fadeInAndOut = GetComponent<Animator>();
        //fadeInAndOut.runtimeAnimatorController.animationClips[0].legacy = true;
        //fadeInAndOut.runtimeAnimatorController.animationClips[1].legacy = true;
    }
    public void LevelCompleted(int level)
    {
        mostRecentLevelCompleted = level;
        DisplayLevelNumbers();
    }
    public void LevelFailed()
    {
        DisplayLevelNumbers();
    }
    public void StartLevel()
    {
        fadeInAndOut.SetBool("isBetweenLevels", false);
    }
    private void DisplayLevelNumbers()
    {
        fadeInAndOut.enabled = true;
        fadeInAndOut.SetBool("isBetweenLevels", true);
    }

    public void LevelNumberMakeGreen()
    {
        levelNumbers[mostRecentLevelCompleted].GetComponent<Animator>().enabled = true;
    }
}
