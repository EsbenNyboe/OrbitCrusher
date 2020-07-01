using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorManager : MonoBehaviour
{
    public Color colorRed;
    public Color colorYellow; // purple
    public Color colorGreen; // blue
    public BackgroundScript bg1;
    public BackgroundScript bg2;

    public AnimationCurve colorTransition;

    public bool enableColorTesting;
    [Range(0,1)]
    public float colorTransTester;
    public static float colorTransTesterHealthbarSync;

    public float colorLerpFactor;
    public float colorLerpFactorOnLevelLoad;
    public float colorLerpFactorChangeSpeed;
    float colorLerpFactorInitial;
    public float colorStateLerp; // read

    public GameManager gameManager;

    public void ApplyColors(Color bgA, Color bgB, Color bgC)
    {
        colorRed = bgA;
        colorYellow = bgB;
        colorGreen = bgC;
    }

    private void Start()
    {
        colorLerpFactorInitial = colorLerpFactor;
        //gameManager = FindObjectOfType<GameManager>();
        bg1.LoadMaterial();
        bg2.LoadMaterial();
    }
    private void FixedUpdate()
    {
        if (colorLerpFactor >= colorLerpFactorInitial)
            colorLerpFactor = colorLerpFactorInitial;
        else
            colorLerpFactor += colorLerpFactorChangeSpeed;

        if (enableColorTesting)
        {
            colorTransTesterHealthbarSync = colorTransTester;
            colorStateLerp = Mathf.Lerp(colorStateLerp, colorTransTester, colorLerpFactor);
        }
        else if (GameManager.betweenLevels)
        {
            colorStateLerp = Mathf.Lerp(colorStateLerp, colorBetweenLvls, colorLerpFactor);
        }
        else
        {
            float energyFactor = (float)GameManager.energy / gameManager.maxEnergy;
            float colorState = colorTransition.Evaluate(energyFactor);
            colorStateLerp = Mathf.Lerp(colorStateLerp, colorState, colorLerpFactor);
        }
        bg1.ColorChange(colorRed, colorYellow, colorGreen, colorStateLerp);
        bg2.ColorChange(colorRed, colorYellow, colorGreen, colorStateLerp);
    }
    float colorBetweenLvls = 1;
    float colorInLvl;
    public void GradualColorOnLevelLoad(bool death)
    {
        if (!death)
            colorLerpFactor = colorLerpFactorOnLevelLoad;
    }


    public void LevelCompleted()
    {
        colorBetweenLvls = 1;
        colorLerpFactor = colorLerpFactorInitial;
    }
    public void LevelFailed()
    {
        colorBetweenLvls = 0;
        colorLerpFactor = colorLerpFactorInitial;
    }
}
