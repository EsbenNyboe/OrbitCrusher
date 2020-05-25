using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterLookahead : MonoBehaviour
{
    MusicMeter musicMeter;
    public static int lookaheadCurrent;
    public AnimationCurve lookaheadCurve;
    public int lookaheadMin;
    public int lookaheadMax;
    private float secondsPerBeatTest;
    public bool lookaheadEnable;

    private void Awake()
    {
        musicMeter = FindObjectOfType<MusicMeter>();
    }
    public bool SoundLookaheadConditionSpecific(MusicMeter.MeterCondition target)
    {
        if (lookaheadEnable)
        {
            if (musicMeter.CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(target) == lookaheadCurrent)
                return true;
            else
                return false;
        }
        else
        {
            if (musicMeter.MeterConditionSpecificTarget(target))
                return true;
            else
                return false;
        }
    }



    private void Update()
    {
        if (lookaheadEnable)
            lookaheadCurrent = SoundLookaheadAdjustedToBpm();
        else
            lookaheadCurrent = 1;
        secondsPerBeatTest = musicMeter.secondsPerBeatDiv * lookaheadCurrent;
    }

    private int SoundLookaheadAdjustedToBpm()
    {
        float bpm = musicMeter.bpm;
        float min = MusicMeter.bpmMin;
        float max = MusicMeter.bpmMax;
        float bpmCurveInput = (bpm - min) / (max - min);
        float lookaheadCurveOutput = lookaheadCurve.Evaluate(bpmCurveInput);
        float lookaheadInDivs = lookaheadMin + lookaheadCurveOutput * (lookaheadMax - lookaheadMin);
        int lookaheadRounded = Mathf.RoundToInt(lookaheadInDivs);

        return lookaheadRounded;
    }
}