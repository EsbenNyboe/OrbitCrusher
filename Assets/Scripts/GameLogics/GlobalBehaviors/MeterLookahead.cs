using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterLookahead : MonoBehaviour
{
    public MusicMeter musicMeter;
    public int lookaheadFactor;

    public bool SoundLookaheadRelativeToCondition(MusicMeter.MeterCondition target)
    {
        if (musicMeter.CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(target) == lookaheadFactor)
            return true;
        else
            return false;
    }    
}