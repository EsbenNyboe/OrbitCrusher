﻿using System.Collections;
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

    private void Awake()
    {
    }
    private void Start()
    {
        GameStarted();
        colorLerpFactorInitial = colorLerpFactor;
        //gameManager = FindObjectOfType<GameManager>();
        bg1.LoadMaterial();
        bg2.LoadMaterial();
    }
    private void FixedUpdate()
    {
        //if (colorLerpFactor >= colorLerpFactorInitial)
        //    colorLerpFactor = colorLerpFactorInitial;
        //else
        //    colorLerpFactor += colorLerpFactorChangeSpeed;

        //if (enableColorTesting)
        //{
        //    colorTransTesterHealthbarSync = colorTransTester;
        //    colorStateLerp = Mathf.Lerp(colorStateLerp, colorTransTester, colorLerpFactor);
        //}
        //else if (GameManager.betweenLevels)
        //{
        //    colorStateLerp = Mathf.Lerp(colorStateLerp, colorBetweenLvls, colorLerpFactor);
        //}
        //else
        //{
        //    float energyFactor = (float)GameManager.energy / gameManager.maxEnergy;
        //    float colorState = colorTransition.Evaluate(energyFactor);
        //    colorStateLerp = Mathf.Lerp(colorStateLerp, colorState, colorLerpFactor);
        //}
        //bg1.ColorChange(colorRed, colorYellow, colorGreen, colorStateLerp);
        //bg2.ColorChange(colorRed, colorYellow, colorGreen, colorStateLerp);
    }
    public void GradualColorOnLevelLoad(bool death)
    {
        if (!death)
            colorLerpFactor = colorLerpFactorOnLevelLoad;
    }


    public Color betweenLevels, lvlLoad, lastObj, lvlFail; //blue, blue, dark blue, orange

    bool colorFade;
    Color colorFadeStart, colorFadeEnd;
    public Animator bgColorFadeAnim;



    private void LoadColors(Color fadeStart, Color fadeEnd)
    {
        colorFade = true;
        colorFadeStart = fadeStart;
        colorFadeEnd = fadeEnd;
        colorLerpT = 0;
        bgColorFadeAnim.Play(0);
    }
    private void LoadColorEnd(Color c)
    {
        colorFade = true;
        colorFadeStart = colorFadeEnd;
        colorFadeEnd = c;
        colorLerpT = 0;
        bgColorFadeAnim.Play(0);
    }
    public void GameStarted()
    {
        LoadColors(betweenLevels, betweenLevels);
    }
    public void LvlLoaded()
    {
        LoadColorEnd(lvlLoad);
    }
    public void LastObjective()
    {
        LoadColorEnd(lastObj);
    }
    public void LevelCompleted()
    {
        LoadColorEnd(betweenLevels);

        colorLerpFactor = colorLerpFactorInitial;
    }
    public void LevelFailed()
    {
        LoadColorEnd(lvlFail);

        colorLerpFactor = colorLerpFactorInitial;
    }


    [Range(0, 1)]
    public float colorLerpT;
    private void Update()
    {
        //if (!colorFade && colorFadeStart != colorFadeEnd)
        //{
        //    colorFadeStart = colorFadeEnd;//don't understand
        //}

        bg1.ColorChangeNew(colorFadeStart, colorFadeEnd, colorLerpT);
        bg2.ColorChangeNew(colorFadeStart, colorFadeEnd, colorLerpT);

        if (colorFade)
        {
            if (colorLerpT > 0.999f)
            {
                colorFade = false;
            }
        }
    }
}
