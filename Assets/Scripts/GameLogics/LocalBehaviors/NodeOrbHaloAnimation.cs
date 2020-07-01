using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeOrbHaloAnimation : MonoBehaviour
{
    Animator anim;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void CollOrb(bool isTarget, bool isCompleted)
    {
        //anim.SetBool("TargetCompleted", isCompleted);

        if (isTarget)
            anim.SetBool("CollGood", true);
        else
            anim.SetBool("CollBad", true);
    }
}