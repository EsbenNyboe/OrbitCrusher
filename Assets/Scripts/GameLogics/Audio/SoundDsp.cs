using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundDsp : MonoBehaviour
{
    public bool useAudioDelay;
    public float musicDelayBuffer;
    public float musicLatencyCompensation;
    public float musicStartOffset;

    [System.Serializable]
    public class ScheduledMusic
    {
        public AudioObject audioObject;
        public double scheduledDspTiming;
        public bool queuedForReschedule;
    }

    public List<ScheduledMusic> scheduledMusic = new List<ScheduledMusic>();

    public MusicMeter musicMeter;
    public MeterLookahead meterLookahead;

    double dspTimeWhenPausing;
    double dspTimeDifference;

    public static bool dspRefInitialized;

    public static double dspTimeAtSectionStart;
    public static bool refControlIsQueued;

    #region Dsp Reference
    public double DspTimeOnNodeHit(bool nextTargetNode)
    {
        int nodeToSyncWith = LevelManager.cometDestination;
        if (nextTargetNode)
        {
            nodeToSyncWith = LevelManager.targetNodes[LevelManager.levelObjectiveCurrent];
        }
        int divsSinceSectionStart = musicMeter.CountBeatDivisionsBetweenSectionStartAndCurrentMeter();
        int divsUntilTarget = musicMeter.CountRemainingBeatDivisionsBetweenCurrentCountAndTargetMeter(LevelManager.cometTimings[nodeToSyncWith]);
        float timeBetweenSectionStartAndTargetMeter = (divsSinceSectionStart + divsUntilTarget) * musicMeter.secondsPerBeatDiv;
        double rawTime = dspTimeAtSectionStart + timeBetweenSectionStartAndTargetMeter;
        //double timeCorrelations = musicDelayBuffer - (musicStartOffset - MusicMeter.dspNudgeRest);
        //double timeCorrelations = musicDelayBuffer + (musicStartOffset - MusicMeter.dspNudgeRest);
        double timeCorrelations = musicDelayBuffer + musicStartOffset;

        double dspTimeNextNodeOrTarget = rawTime + timeCorrelations;
        return dspTimeNextNodeOrTarget;
    }
    public void InitializeDspReference(AudioObject dspRef)
    {
        float triggerDelay = 0;
        if (dspTimeAtSectionStart == 0) // runs for the first section start in current level
        {
            triggerDelay = MusicScheduledPlay_IndependentOfDspReference(dspRef);
            StartCoroutine(InitializeRefControlAtScheduledTime(true, AudioSettings.dspTime + triggerDelay));
        }
        else // runs for all other section start's in current level
        {
            LevelMusic_ScheduledPlayRelativeToDspReference(dspRef);
            double dspTime = DspTimeOnNodeHit(false);
            StartCoroutine(InitializeRefControlAtScheduledTime(false, dspTime));
        }
    }

    bool printFirst;
    double dspTimeOffsetRef;

    private IEnumerator InitializeRefControlAtScheduledTime(bool first, double dspTimeClipStart)
    {
        refControlIsQueued = true;
        float buffer = 0.04f;
        float waitTime = (float)(dspTimeClipStart - AudioSettings.dspTime);
        yield return new WaitForSeconds(waitTime + buffer);
        MusicMeter.dspNudgeRest += musicStartOffset;


        dspTimeAtSectionStart = dspTimeClipStart + musicStartOffset;
        //print("<color=red>initializeRefControl</color>");
        refControlIsQueued = false;
        if (!printFirst)
        {
            dspTimeOffsetRef = dspTimeClipStart;
            printFirst = true;
        }
        if (!first)
        {
            double dspTimeManualCalcDelayCorrection = waitTime + buffer;
            double dspTimeManualCalc = AudioSettings.dspTime + meterLookahead.lookaheadFactor * musicMeter.secondsPerBeatDiv + musicStartOffset - dspTimeManualCalcDelayCorrection;
            //print("dspTimeDifference:" + (dspTimeAtSectionStart - dspTimeManualCalc));
            // time difference is not real time: every second is 1/4th of a second (not that either. the max difference (auditory) is 1/4th sec, but the difference keeps rising...
        }
    }
    //double dspTimeDrift

    #endregion

    #region Regular Music Scheduling
    public void LevelMusic_ScheduledPlayRelativeToDspReference(AudioObject audioObject)
    {
        int refLengthInDivisions = musicMeter.divMax * musicMeter.beatMax * musicMeter.barMax;
        float refLengthInSeconds = refLengthInDivisions * musicMeter.secondsPerBeatDiv;
        if (dspTimeAtSectionStart == 0) // first section start in level
        {
            MusicScheduledPlay_IndependentOfDspReference(audioObject); 
        }
        else
        {
            MusicScheduledPlayRelativeToDspReferenceNotFirstBeat(audioObject);
        }
    }
    public void MusicScheduledPlayRelativeToDspReferenceNotFirstBeat(AudioObject audioObject)
    {
        float timeSinceSectionStart = musicMeter.CountBeatDivisionsBetweenSectionStartAndCurrentMeter() * musicMeter.secondsPerBeatDiv;
        float timeLookahead = meterLookahead.lookaheadFactor * musicMeter.secondsPerBeatDiv;
        double nextDspTiming = dspTimeAtSectionStart + timeSinceSectionStart + timeLookahead;

        //print(nextDspTiming - dspTimeOffsetRef);
        if (audioObject != null) // remove eventually (when removing referenceSampleSound)
        {
            audioObject.TriggerAudioObjectScheduled(nextDspTiming);
            if (GameManager.inTutorial)
                AddScheduledMusicToList(audioObject, nextDspTiming);
        }
    }
    private float MusicScheduledPlay_IndependentOfDspReference(AudioObject audioObject)
    {
        float delay = meterLookahead.lookaheadFactor * musicMeter.secondsPerBeatDiv;
        if (musicLatencyCompensation > delay)
            musicLatencyCompensation = delay;
        float delaySum = delay - musicLatencyCompensation;

        if (useAudioDelay)
        {
            double dspTime = AudioSettings.dspTime + delaySum;
            if (audioObject != null)
                audioObject.TriggerAudioObjectScheduled(dspTime);
            AddScheduledMusicToList(audioObject, dspTime);
        }
        else
        {
            audioObject.TriggerAudioObject();
        }
        return delaySum;
    }
    #endregion

    #region Rescheduling Music (when pausing in tutorial)
    public void AddScheduledMusicToList(AudioObject audioObject, double dspTiming)
    {
        ScheduledMusic newItem = new ScheduledMusic();
        newItem.audioObject = audioObject;
        newItem.scheduledDspTiming = dspTiming;
        scheduledMusic.Add(newItem);
    }
    public void StopAndQueueMusicForRescheduling()
    {
        StoreDspTimeReferenceWhenPausingTheTutorial();
        RemoveScheduledMusicFromListBeforeItsDspTiming(true);
    }
    public void RescheduleQueuedMusic()
    {
        SetNewSectionStartReferenceWhenUnpausingTheTutorial();
        RescheduleMusicContainedInQueue(DspTimeOnNodeHit(false));
    }
    private void StoreDspTimeReferenceWhenPausingTheTutorial()
    {
        dspTimeWhenPausing = AudioSettings.dspTime;
    }
    private void RemoveScheduledMusicFromListBeforeItsDspTiming(bool queueForReschedule) // cancel all music, that has been scheduled, but not played yet
    {
        for (int i = 0; i < scheduledMusic.Count; i++)
        {
            if (scheduledMusic[i].scheduledDspTiming > AudioSettings.dspTime)
            {
                scheduledMusic[i].audioObject.StopAudioAllVoices();
                if (queueForReschedule)
                {
                    scheduledMusic[i].queuedForReschedule = true; // puts the music in a queue during a timestop in the tutorial
                }
                else
                {
                    // this never happens
                    scheduledMusic.Remove(scheduledMusic[i]);
                    i--;
                }
            }
        }
    }
    private void SetNewSectionStartReferenceWhenUnpausingTheTutorial()
    {
        dspTimeDifference = AudioSettings.dspTime - dspTimeWhenPausing;
        StartCoroutine(SetRefControlForUnpausing());
    }
    private IEnumerator SetRefControlForUnpausing()
    {
        while(refControlIsQueued)
        {
            yield return null;
        }
        dspTimeAtSectionStart += dspTimeDifference;
        RescheduleMusicContainedInQueue(DspTimeOnNodeHit(false));
        RemoveScheduledMusicFromListAfterItsDspTiming();
    }
    private void RescheduleMusicContainedInQueue(double nextNodeDspTiming) // when music need to be rescheduled after time is back to normal in the tutorial
    {
        for (int i = 0; i < scheduledMusic.Count; i++)
        {
            if (scheduledMusic[i].queuedForReschedule)
            {
                scheduledMusic[i].queuedForReschedule = false;

                if (nextNodeDspTiming < AudioSettings.dspTime)
                    nextNodeDspTiming = AudioSettings.dspTime;

                scheduledMusic[i].scheduledDspTiming += dspTimeDifference;
                scheduledMusic[i].audioObject.TriggerAudioObjectScheduled(scheduledMusic[i].scheduledDspTiming);
            }
        }
    }
    private void RemoveScheduledMusicFromListAfterItsDspTiming() // clear up memory by removing music, that is no longer scheduled ( = has begun playing)
    {
        for (int i = 0; i < scheduledMusic.Count; i++)
        {
            if (scheduledMusic[i].scheduledDspTiming < AudioSettings.dspTime)
            {
                scheduledMusic.Remove(scheduledMusic[i]);
                i--;
            }
        }
    }

    #endregion
}
