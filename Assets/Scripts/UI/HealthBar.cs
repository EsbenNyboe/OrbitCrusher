using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthBar : MonoBehaviour
{

    [Range(0, 1)]
    public float aColorHealthbar, aColorShadow, aColorFrame;
    public GameObject healthbarAnimGameObject;
    private Animator healthbarAnimator;

    public GameObject shadow;
    public GameObject frame;
    private Animator frameAnimator;

    public float minSize;
    public float maxSize;
    float scaleY;
    float scaleZ;
    public AnimationCurve sizeCurve;
    public Animation tutorialEndFadeOut;

    Material material;
    Material shadowMaterial;
    SpriteRenderer frameSprite;
    [HideInInspector]
    public Color cLow, cMedium, cHigh, cDrain, cCharge;
    public AnimationCurve energyCorrelation;
    public AnimationCurve colorCurve;
    [Range(0, 1)]
    public float curveController;

    public float speed;

    public GameManager gameManager;
    public SoundManager soundManager;

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

    public void ApplyColors(Color low, Color medium, Color high, Color drain, Color charge)
    {
        cLow = new Color(low.r, low.g, low.b, 0);
        cMedium = new Color(medium.r, medium.g, medium.b, 0);
        cHigh = new Color(high.r, high.g, high.b, 0);
        cDrain = new Color(drain.r, drain.g, drain.b, 0);
        cCharge = new Color(charge.r, charge.g, charge.b, 0);
    }

    private void Awake()
    {
        //gameManager = FindObjectOfType<GameManager>();
        //soundManager = FindObjectOfType<SoundManager>();
        scaleY = transform.localScale.y;
        scaleZ = transform.localScale.z;
        transform.localScale = new Vector3(0, scaleY, scaleZ);
        shadow.transform.localScale = new Vector3(0, scaleY, scaleZ);
        material = GetComponent<MeshRenderer>().material;
        shadowMaterial = shadow.GetComponent<MeshRenderer>().material;
    }
    private void Start()
    {
        healthbarAnimator = healthbarAnimGameObject.GetComponent<Animator>();
        frameAnimator = frame.GetComponent<Animator>();
        frameSprite = frame.GetComponent<SpriteRenderer>();
    }
    public bool setColorManually;
    private void FixedUpdate()
    {
        if (setColorManually)
        {
            if (BackgroundColorManager.colorTransTesterHealthbarSync != 0)
                curveController = BackgroundColorManager.colorTransTesterHealthbarSync;
            material.color = SetNewColorNow(0.05f, material.color, cLow, cMedium, cHigh);
        }

        if (fadeOutAnimationDone)
        {
            ScaleOffset(shadowGradualStatus, ref shadowScaleOld, shadow.transform.localScale);
            SetGradualStatus(ref shadowGradualStatus);
            shadow.transform.localScale = SetScale(shadowGradualStatus, shadowScaleOld);
            if (pointUp)
            {
                shadowMaterial.color = cCharge;
            }
            else
            {
                shadowMaterial.color = cDrain;
            }
            if (!dontUpdateMainHealthbar)
            {
                ScaleOffset(mainGradualStatus, ref mainScaleOld, transform.localScale);
                ColorOffset(mainGradualStatus, ref mainColorOld, material.color);
                SetGradualStatus(ref mainGradualStatus);
                transform.localScale = SetScale(mainGradualStatus, mainScaleOld);
                material.color = SetNewColorNow(mainGradualStatus, mainColorOld, cLow, cMedium, cHigh);
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
        Color col = shadowMaterial.color;
        shadowMaterial.color = new Color(col.r, col.g, col.b, aColorShadow);
        col = material.color;
        material.color = new Color(col.r, col.g, col.b, aColorHealthbar);
        if (!frameColorIndependent)
        {
            col = frameSprite.color;
            frameSprite.color = new Color(col.r, col.g, col.b, aColorFrame);
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
    private Color SetNewColorNow(float gradualStatus, Color oldColor, Color red, Color yellow, Color green)
    {
        Color color;
        Color targetColor = Color.Lerp(red, yellow, curveController * 2f);
        if (curveController > 0.5f)
        {
            targetColor = Color.Lerp(yellow, green, (curveController - 0.5f) * 2f);
        }
        color = Color.Lerp(oldColor, targetColor, gradualSizingCurve.Evaluate(gradualStatus));
        return color;
    }

    private Vector3 SetScale(float gradualStatus, Vector3 oldScale)
    {
        Vector3 scale;
        Vector3 newScale = Vector3.Lerp(new Vector3(minSize, scaleY, scaleZ), new Vector3(maxSize, scaleY, scaleZ), curveController);
        //float time = energyCorrelation.Evaluate(curveController);
        scale = Vector3.Lerp(oldScale, newScale, gradualSizingCurve.Evaluate(gradualStatus));
        return scale;
    }
    private void UpdateHealthBarControlValue()
    {
        curveController = (float) (GameManager.energy + GameManager.energyPool) / gameManager.maxEnergy;
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
            if (fadeOutAnimationDone && transform.localScale.x < maxSize)
                soundManager.HealthCharge();
            dontUpdateMainHealthbar = false;
            shadowGradualStatus = 1; // make shadow increase instantly
            mainGradualStatus = 0; // make main increase gradually
        }
        else
        {
            drainDone = false;
            if (fadeOutAnimationDone)
                soundManager.HealthDrain();
            //if (!tutorialFadeOut)
            //    soundManager.HealthDrain();
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
    public static bool tutorialFadeOut;

    public bool fadeOutAnimationDone = true;
    public void FadeOutHealthbar()
    {
        healthbarAnimator.enabled = true;
        healthbarAnimator.Play(0);
        StartCoroutine(DelayedHealthbarDrain());
    }
    IEnumerator DelayedHealthbarDrain()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateHealthbarOnObjectiveConclusion(false);
    }
    public static bool frameColorIndependent = true;
    public void FadeInHealthbar()
    {
        healthbarAnimator.enabled = false;
        fadeOutAnimationDone = true;
        aColorFrame = aColorHealthbar = aColorShadow = 1;
        frameColorIndependent = true;
        frameAnimator.enabled = true;
        frameAnimator.Play(0);
        StartCoroutine(DelayedFrameDependence());
    }
    IEnumerator DelayedFrameDependence()
    {
        yield return new WaitForSeconds(1.5f);
        frameColorIndependent = false;
        frameAnimator.enabled = false;
    }
}
