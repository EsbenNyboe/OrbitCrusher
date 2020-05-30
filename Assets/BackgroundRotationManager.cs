using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRotationManager : MonoBehaviour
{
    public GameObject bg1;
    public GameObject bg2;
    bool bg1AnimStarted;
    bool bg2AnimStarted;
    Animator bg1Animator;
    Animator bg2Animator;

    private void Start()
    {
        bg1Animator = bg1.GetComponent<Animator>();
        bg2Animator = bg2.GetComponent<Animator>();
    }
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
