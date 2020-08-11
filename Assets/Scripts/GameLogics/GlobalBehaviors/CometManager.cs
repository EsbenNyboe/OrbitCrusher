using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CometManager : MonoBehaviour
{
    public GameObject cometParticleEffect;
    public MusicMeter musicMeter;

    [HideInInspector]
    public CometBehavior cometMovement;
    [HideInInspector]
    public MusicMeter.MeterCondition nodeHitTiming;

    [HideInInspector] // remove hiding to show in inspector
    public float timeUntilMeterTarget; // read
    [HideInInspector] // remove hiding to show in inspector
    public float distanceToNextNode; // read
    [HideInInspector] // remove hiding to show in inspector
    public float timeProgressAdapted; // read


    [HideInInspector]
    [Range(0, 1)]
    public float expPower1;
    [HideInInspector]
    [Range(0, 1)]
    public float curveShiftPoint;
    [HideInInspector]
    [Range(1, 5)]
    public float expPower2;
    [HideInInspector]
    [Range(0, 2)]
    public float normalizationAmount;

    float initialTimeUntilTarget;
    float timeProgressExp;

    [HideInInspector]
    public float cometRestTime;
    [HideInInspector]
    public float cometStopTime;
    [HideInInspector]
    public float cometStopFactor; // not too usable these ones

    bool directionChange;


    public AnimationCurve nodeTravelCurve;
    public AnimationCurve nodeTravelAngryCurve;
    public AnimationCurve nodeTravelCrazyCurve;

    private bool crazyMovement;
    public Vector2[] crazyMovementApplications;


    private void Start()
    {
        //this used to be in awake
        cometParticleEffect.SetActive(true);
    }
    public void LoadLevelTransition()
    {
        ChangeDirectionWhenHittingNode();
        musicMeter.SubscribeToTimeChecker(CheckTimeUntilNextNodeHit, false);
        musicMeter.SubscribeToTimeChecker(CheckTimeUntilNextNodeHit, true);
    }
    public void LoadLevel()
    {
        cometMovement.light.enabled = true;
        // light on
        ChangeDirectionWhenHittingNode();
        musicMeter.SubscribeEvent(ActivateCometMovementToTheMeter, ref musicMeter.subscribeAnytime);
        musicMeter.SubscribeToTimeChecker(CheckTimeUntilNextNodeHit, false);
        musicMeter.SubscribeToTimeChecker(CheckTimeUntilNextNodeHit, true);
    }
    public void UnloadLevel()
    {
        cometMovement.light.enabled = false;
        // light off
    }

    private void ActivateCometMovementToTheMeter()
    {
        MusicMeter.MeterCondition firstBeat = new MusicMeter.MeterCondition();
        firstBeat.division = 1;
        firstBeat.beat = 1;
        if (musicMeter.MeterConditionSpecificTarget(firstBeat))
        {
            CometBehavior.isMoving = true;
            timeProgressAdapted = 0;
            musicMeter.UnsubscribeEvent(ActivateCometMovementToTheMeter, ref musicMeter.subscribeAnytime);
        }
    }

    public void ChangeDirectionWhenHittingNode()
    {
        directionChange = true;
        //initialTimeUntilTarget = musicMeter.CheckTimeRemainingUntilMeterTarget(nodeHitTiming, "directionChange", timeUntilMeterTarget, true);
        //print("initialTime:" + initialTimeUntilTarget);
    }
    private void CheckTimeUntilNextNodeHit() // when subscribed, this runs until target is reached or is unreachable
    {
        string debugID = "example";
        bool displayDebugs = false;
        MusicMeter.MeterCondition target = nodeHitTiming;
        float oldTime = timeUntilMeterTarget;
        timeUntilMeterTarget = musicMeter.CheckTimeRemainingUntilMeterTarget(target, debugID, timeUntilMeterTarget, displayDebugs);
        float timeChange = oldTime - timeUntilMeterTarget;
        if (directionChange) // this ensures that the comet doesn't try to move backwards when moving between nodes
        {
            directionChange = false;
            initialTimeUntilTarget = timeUntilMeterTarget; // this shouldn't only run when the comet changes its direction
                                                           // cases: transition start, transition end, node hit
                                                           //print("initialTime:" + initialTimeUntilTarget);
            if (timeChange < 0)
            {
            }
        }

        NewTimeProgressAlgorithm();
        //OldTimeProgressAlgorithm();

        cometMovement.FakeUpdate();
        //print("timeProgress:" + timeProgressAdapted);
    }

    private void NewTimeProgressAlgorithm()
    {
        float timeProgressClamped = timeUntilMeterTarget / initialTimeUntilTarget; // 1: set off time. 0: destination time.
        float timeProgressReversed = -timeProgressClamped + 1; // 0: set off time. 1: destination time.

        if (LevelManager.lastObjective)
            timeProgressAdapted = nodeTravelAngryCurve.Evaluate(timeProgressReversed);
        else
            timeProgressAdapted = nodeTravelCurve.Evaluate(timeProgressReversed);

        crazyMovement = false;
        if (LevelManager.lastTravelBeforeActivation)
            crazyMovement = true;

        for (int i = 0; i < crazyMovementApplications.Length; i++)
        {
            if (GameManager.levelProgression == (int)crazyMovementApplications[i].x)
            {
                if (LevelManager.levelObjectiveCurrent == (int)crazyMovementApplications[i].y)
                {
                    crazyMovement = true;
                }
            }
        }
        if (crazyMovement)
        {
            timeProgressAdapted = nodeTravelCrazyCurve.Evaluate(timeProgressReversed);
        }
    }
    private void OldTimeProgressAlgorithm()
    {
        float timeProgressClamped = timeUntilMeterTarget / initialTimeUntilTarget; // 1: set off time. 0: destination time.
        float timeProgressReversed = -timeProgressClamped + 1; // 0: set off time. 1: destination time.
        //print("timeClamped:" + timeProgressClamped);
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
    }

    private float ConvertToExponentialCurve(float timeProgress, float curveMin, float curveMax, float expPower)
    {
        float curveProgress = (timeProgress - curveMin) / (curveMax - curveMin);
        float curveProgressExp = Mathf.Pow(curveProgress, expPower);
        float timeProgressExp = curveProgressExp * (curveMax - curveMin) + curveMin;
        return timeProgressExp;
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
        return timeProgressNormalized;
    }
}