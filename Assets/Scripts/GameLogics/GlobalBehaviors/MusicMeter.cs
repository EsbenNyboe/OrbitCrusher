using System.Collections;
using UnityEngine;

public class MusicMeter : MonoBehaviour
{
    public int bpm;
    [HideInInspector]
    public float secondsPerBeatDiv;
    public int divMax;
    [HideInInspector]
    public int beatMax;
    [HideInInspector]
    public int barMax;

    private int divCount;
    public static int beatCount;
    public static int barCount;
    public static int sectionCount;
    public string meterDisplay;

    public static float remainingTimeUntilNextBeatDiv;

    public delegate void EventHandler();
    public event EventHandler NextBeatDivision;
    public event EventHandler CheckTimeRemaining;

    [System.Serializable]
    public class MeterCondition
    {
        [HideInInspector]
        public string data;
        public int division;
        public int beat;
        public int bar;
        public int section;
    }
    [HideInInspector]
    public bool subscribeAnytime; // use this when you're not worried about a method having multiple simultanous subscriptions 

    bool beatMachineActive;
    float timeCurr;
    float timeTarget;
    public static float counterTime;
    float counterDrift;
    float threshHitDelay;
    [Range(0, 1)]
    public float lookaheadPercent;
    public static float dspNudgeRest;
    public static float dspNudgeTotal;


    private void LateUpdate()
    {
        if (beatMachineActive)
        {
            UpdateBeatMachine();
        }
    }
    private void FixedUpdate()
    {
        RunMethodsCheckingTimeRemainingUntilMeterCountReachesMeterTarget();
    }

    #region The Conductor: Scheduling Beat Events and Counting Beats
    private void UpdateBeatMachine()
    {
        remainingTimeUntilNextBeatDiv -= Time.deltaTime;
        timeCurr += Time.deltaTime;
        float lookaheadWindowMin = secondsPerBeatDiv * lookaheadPercent;
        float lookaheadWindowMax = secondsPerBeatDiv * (1 - lookaheadPercent);
        bool lookaheadThresholdMet = timeCurr + lookaheadWindowMin > timeTarget && timeCurr - timeTarget < lookaheadWindowMax;
        bool thresholdMet = timeCurr > timeTarget;

        if (lookaheadThresholdMet || thresholdMet) // this allows a third divTime lookahead
        {
            threshHitDelay = timeCurr - timeTarget;
            
            if (threshHitDelay > secondsPerBeatDiv) // stress-mode: when framerate is lower than bpm (16/sec)
            {
                int i = 0;
                while (timeCurr - timeTarget > secondsPerBeatDiv)
                {
                    i++;
                    // run all necessary methods for when the framerate can't keep up with the bpm frequency
                    counterTime += (float)1 / 16;
                    timeTarget += secondsPerBeatDiv;
                    BeatEvent();
                }
            }
            BeatEvent();
            
            timeTarget += secondsPerBeatDiv;

            if (dspNudgeRest > 0)
            {
                float nudgeFactor = 0.001f;
                counterTime += nudgeFactor;
                dspNudgeRest -= nudgeFactor;
                dspNudgeTotal += nudgeFactor;
            }

            counterTime += (float)1 / 16;
            counterTime += 0.0001f; // magic number: correction drift from musicStartOffset over longer periods of time
            counterDrift = timeCurr - counterTime;
            timeTarget -= counterDrift; // makes sure the next target hit interpolates with dsp time ("dsp")
            remainingTimeUntilNextBeatDiv = timeTarget - timeCurr;
            //print(dspNudgeTotal);
        }
    }    
    private void BeatEvent()
    {
        CountBeats();
        RaiseTheEvent();
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
    }
    #endregion

    #region Meter Subscription
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
    #endregion

    #region Checking If And When Meter Conditions Are Met
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

    public void SubscribeToTimeChecker(EventHandler eventHandlingMethod, bool subscribe)
    {
        if (subscribe)
            CheckTimeRemaining += eventHandlingMethod;
        else
            CheckTimeRemaining -= eventHandlingMethod;
    }
    float timeRemainingPrevFrame;
    public float CheckTimeRemainingUntilMeterTarget(MeterCondition target, string debugTag, float timeRemaining, bool displayDebugs)
    {
        int divsRemaining = CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(target);
        timeRemaining = remainingTimeUntilNextBeatDiv + (divsRemaining - 1) * secondsPerBeatDiv;
        AnimationCurvePrint.value = timeRemaining;
        //if (timeRemaining > timeRemainingPrevFrame)
        //    print("remainingX:" + remainingTimeUntilNextBeatDiv);
        //else
        //    print("remaining.:" + remainingTimeUntilNextBeatDiv);
        timeRemainingPrevFrame = timeRemaining;
        if (timeRemaining < 0)
        {
            if (displayDebugs)
                print(debugTag + ": counter has surpassed target:" + Time.time);
            timeRemaining = 0;
        }
        
        return timeRemaining;
    }
    public int CountBeatDivisionsBetweenSectionStartAndCurrentMeter()
    {
        MeterCondition sectionStart = new MeterCondition();
        sectionStart.division = sectionStart.beat = sectionStart.bar = 1;
        int divs = CountRemainingUnits(divCount, sectionStart.division, divMax);
        int beats = CountRemainingUnits(beatCount, sectionStart.beat, beatMax);
        int bars = CountRemainingUnits(barCount, sectionStart.bar, barMax);
        bars = CountRemainingUnitsContainedInHigherLevel(barCount, 0, sectionStart.bar, barMax, bars, 0);
        bars = SetRemainingUnitsToZeroWhenGettingCloseToTarget(divCount, beatCount, sectionStart.division, sectionStart.beat, barMax, bars, 0);
        beats = CountRemainingUnitsContainedInHigherLevel(beatCount, barCount, sectionStart.beat, beatMax, beats, bars);
        beats = SetRemainingUnitsToZeroWhenGettingCloseToTarget(0, divCount, 0, sectionStart.division, beatMax, beats, bars);
        divs = CountRemainingUnitsContainedInHigherLevel(divCount, beatCount, sectionStart.division, divMax, divs, beats);
        return divs;
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
    #endregion

    #region MusicMeter State Changes
    public void LoadNewMeterSettings(int newBpm, int newBeats, int newBars)
    {
        bpm = newBpm;
        beatMax = newBeats;
        barMax = newBars;
        LoadBpm();
        ResetMeterCounts();
    }
    public void ActivateMusicMeter()
    {
        ResetTimers();
        beatMachineActive = true;
    }
    public void StopMusicMeter()
    {
        ResetMeterCounts();
        beatMachineActive = false;
    }

    private void LoadBpm()
    {
        float secondsPerMinute = 60f;
        float secondsPerBeat = secondsPerMinute / bpm;
        secondsPerBeatDiv = secondsPerBeat / divMax;
    }
    private void ResetMeterCounts()
    {
        divCount = 0;
        beatCount = 0;
        barCount = 0;
        sectionCount = 0;
    }
    private void ResetTimers()
    {
        counterTime = 0;
        timeCurr = 0;
        timeTarget = secondsPerBeatDiv;
    }
    #endregion
}
