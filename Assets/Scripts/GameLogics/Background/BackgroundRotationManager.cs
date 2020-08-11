using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRotationManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject bg1;
    [HideInInspector]
    public GameObject bg2;
    bool bg1AnimStarted;
    bool bg2AnimStarted;
    [HideInInspector]
    public Animator bg1Animator;
    [HideInInspector]
    public Animator bg2Animator;

    public void RotateBackground1()
    {
        if (!bg1AnimStarted)
        {
            bg1Animator.Play(0);
            bg1AnimStarted = true;
        }
    }
    public void RotateBackground2()
    {
        if (!bg2AnimStarted)
        {
            bg2Animator.Play(0);
            bg2AnimStarted = true;
        }
    }
}
