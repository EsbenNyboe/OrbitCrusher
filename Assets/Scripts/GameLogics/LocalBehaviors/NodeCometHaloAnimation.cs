using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCometHaloAnimation : MonoBehaviour
{
    public Animator anim;
    public Color colorBad; // not  used
    public Color colorGood; // not  used
    public Color colorPulse; // not  used
    public NodeCollisionScript nodeCollisionScript;

    private void Awake()
    {
        //anim = GetComponent<Animator>();
    }
    public void ApplyColors(Color cBad, Color cGood, Color cPulse)
    {
        colorBad = cBad;
        colorGood = cGood;
        colorPulse = cPulse;
    }


    bool orbCollRunning;
    public void OrbCollDone()
    {
        orbCollRunning = false;
    }
    public void Spawn()
    {
        anim.SetBool("LevelLoaded", true);
    }
    public void HighlightNewTarget(bool isTarget)
    {
        nodeCollisionScript.ChooseCollider(isTarget);
    }
    public void CollComet(bool isTarget, bool first, bool isCompleted)
    {
        anim.SetBool("IsTarget", isTarget);
        anim.SetBool("TargetCompleted", isCompleted);
        anim.SetTrigger("CollComet");
    }
    public void CollOrb(bool isTarget, bool isCompleted)
    {
        anim.SetBool("IsTarget", isTarget);
        anim.SetBool("TargetCompleted", isCompleted);
        if (!orbCollRunning)
        {
            anim.SetTrigger("CollOrb");
            orbCollRunning = true;
        }
        else
        {
            anim.Play(0);
        }
    }
    public void Despawn()
    {
        anim.SetBool("LevelLoaded", false);
    }
}
