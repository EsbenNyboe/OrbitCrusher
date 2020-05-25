using System.Collections;
using UnityEngine;

public class MusicMeter : MonoBehaviour
{
    public int bpm; // read
    public static int bpmMin = 50;
    public static int bpmMax = 200;

    public int divMax; // this is the only unchangeable meter setting
    [HideInInspector]
    public int beatMax;
    [HideInInspector]
    public int barMax;

    private int bpmPreviousFrame;
    [HideInInspector]
    public float secondsPerBeatDiv; 
    
    public static int divCount;
    public static int beatCount;
    private int barCount;
    private int sectionCount;
    public string meterDisplay;

    public delegate void EventHandler();
    public event EventHandler NextBeatDivision;
    public event EventHandler CheckTimeRemaining;

    float timeOfLastBeatDiv;
    public static float remainingTimeUntilNextBeatDiv;
    float timeUntilNextBar;

    [System.Serializable]
    public class MeterCondition
    {
        public int division;
        public int beat;
        public int bar;
        public int section;
    }
    [HideInInspector]
    public bool subscribeAnytime; // use this when you're not worried about a method having multiple simultanous subscriptions 

    private void Awake()
    {
    }
    void LateUpdate() 
    {
        remainingTimeUntilNextBeatDiv = timeBetweenBeatDivs - (Time.time - timeOfLastBeatDiv);

        bpm = Mathf.Clamp(bpm, bpmMin, bpmMax);

        
        if (bpmPreviousFrame != bpm)
        {
            SetNewSecondsPerBeatDivWhenChangingTheBpm();
            StopAllCoroutines();
            StartCoroutine(WaitUntilNextBeatDiv(remainingTimeUntilNextBeatDiv));
            //OldBeatSystem();
        }
        RunMethodsCheckingTimeRemainingUntilMeterCountReachesMeterTarget();
    }
    int nextDivisionIndex;

    IEnumerator WaitUntilNextBeatDiv(float time)
    {
        yield return new WaitForSeconds(time);
        CallBeatEventsAndCoroutine();
    }
    private void CallBeatEventsAndCoroutine()
    {
        BeatDivision();
        if (sampleControlledMeter)
        {
            timeBetweenBeatDivs = Mathf.Lerp(timeBetweenBeatDivs, NewWaitTimeFromSampleControllerProgress(), sampleControllerLerpTime);
            //timeBetweenDivsWebGL = timeBetweenBeatDivs + webGLtimeOffsetPerDiv;
            //timeBetweenDivsWebGL = Mathf.Lerp(timeBetweenDivsWebGL, NewWaitTimeFromSampleControllerProgress(), sampleControllerLerpTime);
        }
        StartCoroutine(WaitUntilNextBeatDiv(timeBetweenBeatDivs));
    }
    public float webGLtimeOffsetPerSec;
    float webGLtimeOffsetPerDiv;
    float timeBetweenDivsWebGL;
    public float sampleControllerLerpTime;
    float timeBetweenBeatDivs;
    
    public void InitializeSampleController()
    {
        timeOffset = Time.time;
        int sampleControllerLength = sampleController.clip.samples;
        int sampleRate = AudioSettings.outputSampleRate;
        samplesPerDivision = Mathf.RoundToInt(sampleRate * 60f / bpm / divMax);
        divisionIndexLength = sampleControllerLength / samplesPerDivision;
        sampleControllerDivisionIndex = 0;
        webGLtimeOffsetPerDiv = webGLtimeOffsetPerSec * 60f / bpm / divMax;
        //print("offset:" + webGLtimeOffsetPerDiv);
    }
    


    float timeDriftAverage;
    float[] timeDriftAvgSum;
    int timeDriftAverageIndex;
    private float NewWaitTimeFromSampleControllerProgress()
    {
        if (sampleControllerDivisionIndex > divisionIndexLength)
            sampleControllerDivisionIndex = 0;

        int controllerProgressSamples = sampleController.timeSamples;
        int sampleTargetForThisDivision = sampleControllerDivisionIndex * samplesPerDivision;
        sampleControllerDivisionIndex++;
        int samplesDrifted = controllerProgressSamples - sampleTargetForThisDivision;
        // drift: this is the number of samples, the sound is off relative to the coroutine's beat division timing
        int sampleRate = AudioSettings.outputSampleRate;
        float timeDrifted = (float)samplesDrifted / sampleRate;


        if (timeDriftAvgSum == null)
        {
            timeDriftAvgSum = new float[5];
        }
        timeDriftAvgSum[timeDriftAverageIndex] = timeDrifted;
        timeDriftAverageIndex++;
        if (timeDriftAverageIndex >= 5)
        {
            timeDriftAverageIndex = 0;
        }
        if (timeDriftAvgSum[4] != 0)
        {
            timeDriftAverage = 0;
            for (int i = 0; i < timeDriftAvgSum.Length; i++)
            {
                timeDriftAverage += timeDriftAvgSum[i];
            }
            timeDriftAverage /= 5;
        }
        AnimationCurvePrint.value = timeDriftAverage;

        float newWaitTime;
        //newWaitTime = secondsPerBeatDiv - timeDriftAverage;
        //print("drift:" + timeDriftAverage);
        //print(newWaitTime);
        newWaitTime = secondsPerBeatDiv - timeDrifted;
        return newWaitTime;
    }
    private int samplesPerDivision;
    private int divisionIndexLength;
    private int timeSamplesOffset = -500;
    private int sampleControllerDivisionIndex;
    private float timeOffset;
    public static bool sampleControlledMeter;
    public static AudioSource sampleController;
    


    public void OldBeatSystem()
    {
        CancelInvoke();
        InvokeRepeating("BeatDivision", remainingTimeUntilNextBeatDiv, secondsPerBeatDiv);
    }

    private void BeatDivision()
    {
        timeOfLastBeatDiv = Time.time;
        CountBeats();
        RaiseTheEvent();
    }
    public void RaiseEventsOnTick()
    {
        //timeOfLastBeatDiv = (float)DspTimer.dspTimeNormalized;
        //CountBeats();
        //RaiseTheEvent();
        //print("mm:" + DspTimer.dspTimeNormalized);
    }

    private void SetNewSecondsPerBeatDivWhenChangingTheBpm()
    {
        bpmPreviousFrame = bpm;
        float secondsPerMinute = 60f;
        float secondsPerBeat = secondsPerMinute / bpm;
        secondsPerBeatDiv = secondsPerBeat / divMax;
        timeBetweenBeatDivs = secondsPerBeatDiv;
    }
    
    private void RaiseTheEvent()
    {
        if (NextBeatDivision != null)
            NextBeatDivision();
    }
    private void CountBeats()
    {
        divCount++;
        if (divCount % divMax == 1)
        {
            divCount = 1;
            beatCount++;
            if (beatCount % beatMax == 1)
            {
                beatCount = 1;
                barCount++;
                if (barCount % barMax == 1)
                {
                    barCount = 1;
                    sectionCount++;
                }
            }
        }
        meterDisplay = divCount + "." + beatCount + "." + barCount + "." + sectionCount;
        //print(meterDisplay + " " + Mathf.RoundToInt(1000*Time.time)/1000f);
    }    
    public void SubscribeEvent(EventHandler eventHandlingMethod, ref bool isSubscribed)
    {
        if (!isSubscribed)
            NextBeatDivision += eventHandlingMethod;
        isSubscribed = true;
        subscribeAnytime = false;
    }
    public void UnsubscribeEvent(EventHandler eventHandlingMethod, ref bool isSubscribed)
    {
        NextBeatDivision -= eventHandlingMethod;
        isSubscribed = false;
    }
    public bool MeterConditionIntervals(MeterCondition interval) // check if meter matches the general interval condition
    {
        int div = interval.division;
        int beat = interval.beat;
        int bar = interval.bar;
        int sect = interval.section;

        if (div > divMax)
            print("meter error: division interval bigger than division counter max");
        if (beat > beatMax)
            print("meter error: beat interval bigger than beat counter max");
        if (bar > barMax)
            print("meter error: bar interval bigger than bar counter max");

        //if (sect > 1 && sectionCount % sect != 1)
        //    return false;

        if (bar > 1 && barCount % bar != 1)
            return false;
        if (beat > 1 && beatCount % beat != 1)
            return false;
        if (div > 1 && divCount % div != 1)
            return false;
        return true;
    }

    //public int testCount;
    //public int testInterval;
    //public int testMax;
    //public int testOutput;
    //private void Update()
    //{
    //    testOutput = testCount % testInterval;

    //}

    public bool MeterConditionSpecificTarget(MeterCondition target) // checks if meter counts matches the specific meter condition
    {
        int div = target.division;
        int beat = target.beat;
        int bar = target.bar;
        int sect = target.section;

        // if a meter condition is 0, the following code bypasses that condition and accepts any counter-value for that meter category:
        if (div == 0)
            div = divCount;
        if (beat == 0)
            beat = beatCount;
        if (bar == 0)
            bar = barCount;
        if (sect == 0)
            sect = sectionCount;
        if (div > divMax)
            print("meter error: division condition bigger than division counter max");
        if (beat > beatMax)
            print("meter error: beat condition bigger than beat counter max");
        if (bar > barMax)
            print("meter error: bar condition bigger than bar counter max");

        // if the meter count matches the meter condition, this bool sends back the green light to trigger the function of the subscribed event
        if (div == divCount && beat == beatCount &&
            bar == barCount && sect == sectionCount)
            return true;
        else
            return false;
    }






    // the rest of the code below handles requests for checking if and when a meter condition is hit (in seconds)
    void RunMethodsCheckingTimeRemainingUntilMeterCountReachesMeterTarget()
    {
        if (CheckTimeRemaining != null)
            CheckTimeRemaining();
    }

    // when float is included in a subscription, it runs the calculation every frame until target is reached or is deemed unreachable
    public float CheckTimeRemaining_WhenSubscribed(EventHandler eventMethod, MeterCondition target, string debugID, float timeRemaining, bool displayDebug)
    { 
        if (!MeterConditionSpecificTarget(target))
        {
            timeRemaining = CheckTimeRemainingUntilMeterTarget(target, debugID, timeRemaining, displayDebug);
            if (timeRemaining < 0)
                SubscribeToTimeChecker(eventMethod, false); // unsubscribe / stop checking if target has been surpassed by counter
        }
        else
        {
            if (displayDebug)
                print(debugID + ": target reached:" + Time.time);
            SubscribeToTimeChecker(eventMethod, false); // unsubscribe / stop checking if target is hit by counter
        }
        return timeRemaining;
    }
    public void SubscribeToTimeChecker(EventHandler eventHandlingMethod, bool subscribe)
    {
        if (subscribe)
            CheckTimeRemaining += eventHandlingMethod;
        else
            CheckTimeRemaining -= eventHandlingMethod;
    }
    // "manual time check" (the method below) can also be called externally as an alternative to "subscription time check" (above) 
    // pro: the manual time check can be called anytime. con: the manual time check doesn't have as reliable precision as a subscription has
    // therefore: use manual time check when in need of a single time check, where frame-perfect precision isn't required
    public float CheckTimeRemainingUntilMeterTarget(MeterCondition target, string debugTag, float timeRemaining, bool displayDebugs)
    {
        int divsRemaining = CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(target);
        //timeRemaining = remainingTimeUntilNextBeatDiv + (divsRemaining - 1) * secondsPerBeatDiv;
        timeRemaining = remainingTimeUntilNextBeatDiv + (divsRemaining - 1) * secondsPerBeatDiv;
        if (timeRemaining < 0 && displayDebugs)
        {
            print(debugTag + ": counter has surpassed target:" + Time.time);
        }
        
        return timeRemaining;
    }

    

    public int CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(MeterCondition t)
    {
        int divRem = CountRemainingUnits(t.division, divCount, divMax);
        int beatRem = CountRemainingUnits(t.beat, beatCount, beatMax);
        int barRem = CountRemainingUnits(t.bar, barCount, barMax);
        int sectRem = CountRemainingUnits(t.section, sectionCount, 0);
        barRem = CountRemainingUnitsContainedInHigherLevel(t.bar, t.section, barCount, barMax, barRem, sectRem);
        barRem = SetRemainingUnitsToZeroWhenGettingCloseToTarget(t.division, t.beat, divCount, beatCount, barMax, barRem, sectRem);
        beatRem = CountRemainingUnitsContainedInHigherLevel(t.beat, t.bar, beatCount, beatMax, beatRem, barRem);
        beatRem = SetRemainingUnitsToZeroWhenGettingCloseToTarget(0, t.division, 0, divCount, beatMax, beatRem, barRem);
        divRem = CountRemainingUnitsContainedInHigherLevel(t.division, t.beat, divCount, divMax, divRem, beatRem);
        return divRem;
    }
    int CountRemainingUnits(int t, int count, int max)
    {
        int rem = 0;
        if (t != 0)
        {
            if (count < t)
            {
                while (rem < t - count)
                    rem++;
            }
            else if (count >= t)
            {
                while (rem < t + max - count)
                    rem++;
            }
        }
        else
            rem = 0;
        return rem;
    }
    int SetRemainingUnitsToZeroWhenGettingCloseToTarget(int tGrCh, int tCh, int countGrCh, int countCh, int max, int rem, int remPar)
    {
        if (remPar == 0 && rem == max)
        {
            if (countCh < tCh)
                rem = 0;
            else if (countCh == tCh && countGrCh < tGrCh)
                rem = 0;
        }
        return rem;
    }
    int CountRemainingUnitsContainedInHigherLevel(int target, int targetParent, int count, int max, int rem, int remParent)
    {
        if (targetParent != 0)
        {
            if (count < target)
                rem += remParent * max;
            else
                rem += remParent * max - max;
        }
        return rem;
    }



    public void ResetMeterCounts()
    {
        divCount = 0;
        beatCount = 0;
        barCount = 0;
        sectionCount = 0;
        //dspTimer.SubscribeToTick(dspTimer.ResetTimer, true);
        //StartBeatCounter();
        //print("meterReset");
    }


    //public MeterCondition NextMeterConditionForTargetInterval(MeterCondition interval, int lookahead)
    //{
    //    int intervalDiv = interval.division;
    //    int intervalBeat = interval.beat;
    //    int intervalBar = interval.bar;
    //    int intervalSect = interval.section;
    //    MeterCondition nextTarget = new MeterCondition();
    //    NextSpecificTargetForThisIntervalUnit(intervalDiv, ref intervalBeat, ref nextTarget.division, divCount, divMax, lookahead);
    //    nextTarget.beat = beatCount + intervalBeat;
    //    if (nextTarget.beat > beatMax)
    //    {
    //        nextTarget.beat = 1;
    //        nextTarget.bar += 1;
    //    }
    //    nextTarget.bar += barCount;
    //    if (nextTarget.bar > barMax)
    //    {
    //        nextTarget.bar = 1;
    //        nextTarget.section += 1;
    //    }
    //    nextTarget.section += sectionCount;
    //    // screw the others, this is fine. for beat, bar and section intervals - use the meterconditionspecific!

    //    //NextSpecificTargetForThisIntervalUnit(intervalBeat, ref intervalBar, ref nextTarget.beat, beatCount, beatMax);
    //    //NextSpecificTargetForThisIntervalUnit(intervalBar, ref intervalSect, ref nextTarget.bar, barCount, barMax);
    //    //if (intervalSect > 1)
    //    //    nextTarget.section = sectionCount + sectionCount % intervalSect;
    //    return nextTarget;
    //}

    //private void NextSpecificTargetForThisIntervalUnit(int interval, ref int intervalNext, ref int nextTarget, int count, int max, int lookahead)
    //{
    //    if (interval > 1)
    //    {
    //        nextTarget = 1 + count - count % interval + interval;
    //        if (count % interval == 0)
    //        {
    //            nextTarget -= interval;
    //        }
    //        if (nextTarget > max)
    //        {
    //            nextTarget = nextTarget % interval;
    //            intervalNext++;
    //        }
    //        nextTarget += lookahead;
    //    }
    //}

    //public int CountRemainingBeatDivisionsBetweenCurrentCountAndTargetInterval(MeterCondition t)
    //{
    //    int divRem = 0;
    //    return divRem;
    //}
}
