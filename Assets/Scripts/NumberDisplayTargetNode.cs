using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberDisplayTargetNode : MonoBehaviour
{
    public static int totalOrbsInObjective;

    [HideInInspector]
    public TextMeshPro textMesh;
    [HideInInspector]
    public Animator animator;

    bool activated;

    public static int targetNodeIndexMemory;

    private void Start()
    {
        textMesh.enabled = false;
        textMesh.text = "0";
    }
    public void ShowNumberDisplay()
    {
        if (!activated)
        {
            textMesh.enabled = true;
            animator.SetBool("Display", true);
            SetText();
            activated = true;
        }
    }
    public void OrbHitsTarget() // objectiveManager=?
    {
        SetText();
    }

    public void AllOrbsHaveHitTarget() // objectiveManager?
    {
        SetText();
        DeactivateNumberDisplay();
    }

    public void DeactivateNumberDisplay()
    {
        StartCoroutine(DeactivateNextFrame());
        animator.SetBool("Display", false);
        activated = false;
    }

    private IEnumerator DeactivateNextFrame()
    {
        yield return new WaitForEndOfFrame();
    }

    public void SetText()
    {
        textMesh.text = (ObjectiveManager.amountSpawned - ObjectiveManager.amountOnTarget).ToString();
    }

}