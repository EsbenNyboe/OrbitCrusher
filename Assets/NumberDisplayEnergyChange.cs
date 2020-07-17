using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberDisplayEnergyChange : MonoBehaviour
{
    TextMeshPro textMesh;
    Animator animator;

    public Color positive, neutral, negative;
    [Range(0,1)]
    public float aColorComponent;

    public static int amountOnTargetMemory;
    int num;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        Color c = textMesh.color;
        textMesh.color = new Color(c.r, c.g, c.b, aColorComponent);
    }
    public void NumberDrain()
    {
        print("drain");
        num = amountOnTargetMemory - ObjectiveManager.amountSpawned;
        if (num == -1)
        {
            num = 0;
            textMesh.color = neutral;
            textMesh.text = "";
        }
        else
        {
            textMesh.color = negative;
            textMesh.text = "- ";
        }

        Display();
    }
    public void NumberCharge()
    {
        if (GameManager.fullEnergy)
            textMesh.color = neutral;
        else
            textMesh.color = positive;
        print("charge");
        num = ObjectiveManager.amountSpawned;
        textMesh.text = "+ ";

        Display();
    }

    private void Display()
    {
        textMesh.text += Mathf.Abs(num).ToString();
        animator.enabled = true;
        animator.Play(0);
    }
}
