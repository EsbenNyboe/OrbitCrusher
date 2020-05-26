using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthBar : MonoBehaviour
{
    public GameObject shadow;

    public float minSize;
    public float maxSize;
    float scaleY;
    float scaleZ;
    public AnimationCurve sizeCurve;

    Material material;
    Material shadowMaterial;
    public Color redColor, greenColor;
    public Color shadowRedColor, shadowGreenColor;
    public AnimationCurve colorCurve;
    [Range(0, 1)]
    public float curveController;

    public float speed;

    GameManager gameManager;
    SoundManager soundManager;

    bool pointUp;
    bool dontUpdateMainHealthbar;
    float shadowGradualStatus;
    public float mainGradualStatus;
    Vector3 shadowScaleOld;
    Vector3 mainScaleOld;
    Color shadowColorOld;
    Color mainColorOld;

    bool chargeDone;
    bool drainDone;

    public AnimationCurve gradualSizingCurve;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scaleY = transform.localScale.y;
        scaleZ = transform.localScale.z;
        transform.localScale = new Vector3(0, scaleY, scaleZ);
        material = GetComponent<MeshRenderer>().material;
        shadowMaterial = shadow.GetComponent<MeshRenderer>().material;
    }
    private void FixedUpdate()
    {
        ScaleOffset(shadowGradualStatus, ref shadowScaleOld, shadow.transform.localScale);
        SetGradualStatus(ref shadowGradualStatus);
        shadow.transform.localScale = SetScale(shadowGradualStatus, shadowScaleOld);
        if (pointUp)
        {
            shadowMaterial.color = shadowGreenColor;
        }
        else
        {
            shadowMaterial.color = shadowRedColor;
        }
        if (!dontUpdateMainHealthbar)
        {
            ScaleOffset(mainGradualStatus, ref mainScaleOld, transform.localScale);
            ColorOffset(mainGradualStatus, ref mainColorOld, material.color);
            SetGradualStatus(ref mainGradualStatus);
            transform.localScale = SetScale(mainGradualStatus, mainScaleOld);
            material.color = SetNewColorNow(mainGradualStatus, mainColorOld, redColor, greenColor);
        }
        if (!chargeDone && mainGradualStatus >= 1)
        {
            chargeDone = true;
            soundManager.HealthChargeStop();
        }
        if (!drainDone && shadowGradualStatus >= 1)
        {
            drainDone = true;
            soundManager.HealthDrainStop();
        }
    }

    private void SetGradualStatus(ref float gradualStatus)
    {
        if (gradualStatus < 1)
            gradualStatus += speed;
    }
    private void ScaleOffset(float gradualStatus, ref Vector3 oldScale, Vector3 currentScale)
    {
        if (gradualStatus == 0)
        {
            oldScale = currentScale;
        }
    }
    private void ColorOffset(float gradualStatus, ref Color oldColor, Color currentColor)
    {
        if (gradualStatus == 0)
        {
            oldColor = currentColor;
        }
    }
    private void SetOffsetValues(float gradualStatus, ref Vector3 oldScale, Vector3 currentScale, ref Color oldColor, Color currentColor)
    {
        if (gradualStatus == 0)
        {
            oldScale = currentScale;
            oldColor = currentColor;
        }
    }
    private Color SetNewColorNow(float gradualStatus, Color oldColor, Color red, Color green)
    {
        Color color;
        Color targetColor = Color.Lerp(red, green, curveController);
        color = Color.Lerp(oldColor, targetColor, gradualSizingCurve.Evaluate(gradualStatus));
        return color;
    }

    private Vector3 SetScale(float gradualStatus, Vector3 oldScale)
    {
        Vector3 scale;
        Vector3 newScale = Vector3.Lerp(new Vector3(minSize, scaleY, scaleZ), new Vector3(maxSize, scaleY, scaleZ), curveController);
        scale = Vector3.Lerp(oldScale, newScale, gradualSizingCurve.Evaluate(gradualStatus));
        return scale;
    }
    private void UpdateHealthBarControlValue()
    {
        curveController = 1f * GameManager.energy / gameManager.maxEnergy;
        //fakeEnergy += eChange;
        //curveController = 1f * fakeEnergy / gameManager.maxEnergy;
    }
    int fakeEnergy;
    int eChange;
    public void UpdateHealthbarOnObjectiveConclusion(bool getPoints)
    {
        pointUp = getPoints;
        UpdateHealthBarControlValue();
        if (getPoints)
        {
            chargeDone = false;
            if (transform.localScale.x < maxSize)
                soundManager.HealthCharge();
            dontUpdateMainHealthbar = false;
            shadowGradualStatus = 1; // make shadow increase instantly
            mainGradualStatus = 0; // make main increase gradually
        }
        else
        {
            drainDone = false;
            soundManager.HealthDrain();
            dontUpdateMainHealthbar = false;
            shadowGradualStatus = 0; // make shadow decrease gradually
            mainGradualStatus = 1; // make main decrease instantly
        }
    }
    public void UpdateHealthbarOnCollision(bool getPoints)
    {
        pointUp = getPoints;
        UpdateHealthBarControlValue();
        if (getPoints)
        {
            dontUpdateMainHealthbar = true; // make shadow increase instantly
            shadowGradualStatus = 1;
        }
        else
        {
            drainDone = false;
            soundManager.HealthDrain();
            dontUpdateMainHealthbar = false;
            shadowGradualStatus = 0; // make shadow decrease gradually
            mainGradualStatus = 1; // make main decrease instantly
        }
    }
}
