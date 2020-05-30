using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometBehavior : MonoBehaviour
{
    //public GameObject cometLight;
    public GameObject cometParticleEffect;
    public MusicMeter musicMeter;
    public CometMovement cometMovement;
    [HideInInspector]
    public MusicMeter.MeterCondition nodeHitTiming;

    [HideInInspector] // remove hiding to show in inspector
    public float timeUntilMeterTarget; // read
    [HideInInspector] // remove hiding to show in inspector
    public float distanceToNextNode; // read
    [HideInInspector] // remove hiding to show in inspector
    public float timeProgressAdapted; // read

    [Range(0, 1)]
    public float expPower1;
    [Range(0, 1)]
    public float curveShiftPoint;
    [Range(1, 5)]
    public float expPower2;
    [Range(0, 2)]
    public float normalizationAmount;

    float initialTimeUntilTarget;
    float timeProgressExp;

    public float cometRestTime;
    public float cometStopTime;
    public float cometStopFactor; // not too usable these ones

    void Awake()
    {
        cometParticleEffect.SetActive(true);
        //musicMeter = FindObjectOfType<MusicMeter>();
        //cometMovement = FindObjectOfType<CometMovement>();
    }
    public void LoadLevelTransition()
    {
        musicMeter.SubscribeToTimeChecker(CheckTimeUntilNextNodeHit, false);
        musicMeter.SubscribeToTimeChecker(CheckTimeUntilNextNodeHit, true);
    }
    public void LoadLevel()
    {
        
        musicMeter.SubscribeEvent(ActivateComet, ref musicMeter.subscribeAnytime);
        musicMeter.SubscribeToTimeChecker(CheckTimeUntilNextNodeHit, false);
        musicMeter.SubscribeToTimeChecker(CheckTimeUntilNextNodeHit, true);
    }
    private void ActivateComet()
    {
        MusicMeter.MeterCondition firstBeat = new MusicMeter.MeterCondition();
        firstBeat.division = 1;
        firstBeat.beat = 1;
        if (musicMeter.MeterConditionSpecificTarget(firstBeat))
        {
            CometMovement.isMoving = true;
            timeProgressAdapted = 0;
            musicMeter.UnsubscribeEvent(ActivateComet, ref musicMeter.subscribeAnytime);
        }
    }

    public void PlayLight()
    {
        //cometLight.GetComponent<ParticleSystem>().Play();
    }

    
    public void CheckTimeUntilNextNodeHit() // when subscribed, this runs until target is reached or is unreachable
    {
        string debugID = "example";
        bool displayDebugs = true;
        MusicMeter.MeterCondition target = nodeHitTiming;
        float oldTime = timeUntilMeterTarget;
        timeUntilMeterTarget = musicMeter.CheckTimeRemainingUntilMeterTarget(nodeHitTiming, debugID, timeUntilMeterTarget, displayDebugs);
        float timeChange = oldTime - timeUntilMeterTarget;
        if (timeChange < 0)
            initialTimeUntilTarget = timeUntilMeterTarget;
        float timeProgressClamped = timeUntilMeterTarget / initialTimeUntilTarget; // 1: set off time. 0: destination time.
        float timeProgressReversed = -timeProgressClamped + 1; // 0: set off time. 1: destination time.
        if (timeProgressReversed < 0)
            timeProgressReversed = 0;
        if (timeProgressReversed < cometRestTime)
        {
            timeProgressReversed = 0;
        }
        else
        {
            timeProgressReversed -= cometRestTime;
            timeProgressReversed *= 1 + cometRestTime;
        }
        if (timeProgressReversed > 1 + cometRestTime - cometStopTime)
        {
            timeProgressReversed *= cometStopFactor;
        }


        if (timeProgressReversed >= curveShiftPoint)
        {
            timeProgressExp = ConvertToExponentialCurve(timeProgressReversed, curveShiftPoint, 1, expPower2);
            timeProgressAdapted = NormalizeTheExponentialCurve(timeProgressReversed, timeProgressExp, timeProgressReversed - curveShiftPoint, 1 - curveShiftPoint);
        }
        else
        {
            timeProgressExp = ConvertToExponentialCurve(timeProgressReversed, 0, curveShiftPoint, expPower1);
            timeProgressAdapted = NormalizeTheExponentialCurve(timeProgressReversed, timeProgressExp, curveShiftPoint - timeProgressReversed, curveShiftPoint);
        }
        //print("t:" + timeProgressAdapted);
        cometMovement.FakeUpdate();
    }
    private float NormalizeTheExponentialCurve(float timeProgress, float timeProgressExp, float distanceToCurveShiftPoint, float maxDistance)
    {
        float expAmount;
        float distanceAccumulator = distanceToCurveShiftPoint / maxDistance;
        distanceAccumulator = 1;
        if (normalizationAmount == 0)
        {
            expAmount = 2;
        }
        else
             expAmount = 2 * distanceAccumulator - normalizationAmount;
        float linearAmount = 2 - expAmount;


        float timeProgressNormalized = (timeProgressExp * expAmount + timeProgress * linearAmount) / 2;
//        float timeProgressNormalized = (timeProgressExp + timeProgress) / 2f;
        return timeProgressNormalized;
    }
    private float ConvertToExponentialCurve(float timeProgress, float curveMin, float curveMax, float expPower)
    {
        float curveProgress = (timeProgress - curveMin) / (curveMax - curveMin);
        float curveProgressExp = Mathf.Pow(curveProgress, expPower);
        float timeProgressExp = curveProgressExp * (curveMax - curveMin) + curveMin;
        return timeProgressExp;
    }
}