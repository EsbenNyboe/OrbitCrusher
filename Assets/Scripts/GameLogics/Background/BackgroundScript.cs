using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    [HideInInspector]
    public MeshRenderer meshRenderer;
    Material material;

    [Tooltip("the animator sets this to 0-1")]
    [Range(0,1)]
    public float aColorComponent;

    public void LoadMaterial()
    {
        material = meshRenderer.material;
        //material = GetComponent<MeshRenderer>().material;
    }

    public void ColorChange(Color red, Color yellow, Color green, float colorTransitionOutput)
    {
        Color col;
        col = Color.Lerp(red, yellow, colorTransitionOutput * 2);
        if (colorTransitionOutput > 0.5f)
        {
            col = Color.Lerp(yellow, green, (colorTransitionOutput - 0.5f) * 2);
        }
       
        col = new Color(col.r, col.g, col.b, aColorComponent);
        material.color = col;
    }



    public void ColorChangeNew(Color start, Color end, float transitionOutput)
    {
        Color col;
        col = Color.Lerp(start, end, transitionOutput);
        col = new Color(col.r, col.g, col.b, aColorComponent);
        material.color = col;
    }

}
