using UnityEngine;
using System.Collections;

// The code example shows how to implement a metronome that procedurally generates the click sounds via the OnAudioFilterRead callback.
// While the game is paused or the suspended, this time will not be updated and sounds playing will be paused. Therefore developers of music scheduling routines do not have to do any rescheduling after the app is unpaused

[RequireComponent(typeof(AudioSource))]
public class DspTimer : MonoBehaviour
{
    // this is not used for anything

    public int bpm;
    public static int signatureHi = 4;
    public int signatureLo = 4;

    public float gain = 0.5F;
    private double nextTick = 0.0F;
    private double nextBeat = 0.0F;
    private float amp = 0.0F;
    private float phase = 0.0F;
    private double sampleRate = 0.0F;
    private int accent;
    private bool running = false;

    double dspTimeOffset;
    public static double dspTimeNormalized;
    public static double dspTimeLastTick;
    public static double dspTimeUntilNextTick;
    double dspTimeSum;

    MusicMeter musicMeter;
    SourceScript sourceScript;

    public bool enableMetronomeSound;

    float ttttt;
    void Awake()
    {
        AudioConfiguration config = AudioSettings.GetConfiguration();
        //print(config.sampleRate);
        dspTimeOffset = AudioSettings.dspTime;
        musicMeter = FindObjectOfType<MusicMeter>();
        musicMeter.bpm = 120;
        musicMeter.beatMax = 8;
        musicMeter.barMax = 2;
        sampleRate = AudioSettings.outputSampleRate;
        SetTickTimer();
        running = true;
        sourceScript = FindObjectOfType<SourceScript>();
    }

    

    private void SetTickTimer()
    {
        accent = signatureHi;
        double startTick = AudioSettings.dspTime;
        nextTick = nextBeat = startTick * sampleRate;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        dspTimeNormalized = AudioSettings.dspTime - dspTimeOffset;
        dspTimeUntilNextTick = musicMeter.secondsPerBeatDiv - (dspTimeNormalized - dspTimeLastTick);

        bpm = musicMeter.bpm;
        //print("bpm"+bpm);
        double samplesPerBeat = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double samplesPerTick = samplesPerBeat / musicMeter.divMax;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;
        int n = 0;
        while (n < dataLen)
        {
            //print("tick:" + nextTick);
            float x = gain * amp * Mathf.Sin(phase);
            int i = 0;
            while (i < channels)
            {
                data[n * channels + i] += x;
                i++;
            }
            while (sample + n >= nextTick)
            {
                nextTick += samplesPerTick;

                dspTrigger = true;
                //print("dsp:" + dspTimeNormalized);
                RaiseTheDspEvent();
            }
            while (sample + n >= nextBeat)
            {
                nextBeat += samplesPerBeat;
                amp = 1.0F;
                if (++accent > signatureHi)
                {
                    accent = 1;
                    amp *= 2.0F;
                }
                //Debug.Log("Tick: " + accent + "/" + signatureHi);
            }
            phase += amp * 0.3F;
            if (enableMetronomeSound)
                amp *= 0.993F;
            else
                amp = 0;
            n++;
        }
    }

    private void RaiseTheDspEvent()
    {
        dspTimeLastTick = dspTimeNormalized;
        if (NextTick != null)
            NextTick();
    }

    private void Start()
    {
        
        StartCoroutine(WaitForAudioSystem());
    }

    IEnumerator WaitForAudioSystem()
    {
        yield return new WaitForSeconds(1f);
        //musicMeter.OldBeatSystem();
        //musicMeter.SubscribeEvent(TestSoundCondition, ref musicMeter.subscribeAnytime);
        //musicMeter.SubscribeEvent(MetronomeConsistencyTester, ref musicMeter.subscribeAnytime);

        //sourceScript.PlaySoound();
    }

    void MetronomeConsistencyTester()
    {
        MetronomeConsistencyConditionCheck(metronomeVeryLong, metVeryLongCond);
        MetronomeConsistencyConditionCheck(metronomeShortLoop, metShortLoopCond);
        MetronomeConsistencyConditionCheck(metronomeSingle, metSingleCond);
    }

    private void MetronomeConsistencyConditionCheck(AudioObject sound, MusicMeter.MeterCondition[] condition)
    {
        for (int i = 0; i < condition.Length; i++)
        {
            if (musicMeter.MeterConditionSpecificTarget(condition[i]))
            {
                sound.TriggerAudioObject();
            }
        }
    }

    public AudioObject metronomeVeryLong;
    public AudioObject metronomeShortLoop;
    public AudioObject metronomeSingle;
    public MusicMeter.MeterCondition[] metVeryLongCond;
    public MusicMeter.MeterCondition[] metShortLoopCond;
    public MusicMeter.MeterCondition[] metSingleCond;




    public bool activateMetronome1;
    public bool activateMetronome2;
    public bool activateMetronome4;
    public bool activateMetronome8;
    public bool activateMetronome16;

    public AudioObject metronome1;
    public AudioObject metronome2;
    public AudioObject metronome4;
    public AudioObject metronome8;
    public AudioObject metronome16;
    public AudioObject testSoundLongTimingOtherAudioObject;
    public MusicMeter.MeterCondition[] testCondition1;
    public MusicMeter.MeterCondition[] testCondition2;
    public MusicMeter.MeterCondition[] testCondition4;
    public MusicMeter.MeterCondition[] testCondition8;
    public MusicMeter.MeterCondition[] testCondition16;
    bool notFirst;
    double timeLastSound;
    double timeSinceSound;
    double dspTimeLastSound;
    double dspTimeSinceSound;
    double timeDifference;

    private void Update()
    {
        //RaiseTheMainThreadEvent();

        
        int testBpm = 120;
        int timeSamples = metronomeVeryLong.voicePlayerNew[0].audioSource.timeSamples;
        int sampleRate = AudioSettings.outputSampleRate;
        int tickPeriodInTimeSamples = Mathf.RoundToInt(sampleRate * 60f / testBpm / timeSamplesDivision);
        if ((float)(timeSamples - timeSamplesOffset) / tickPeriodInTimeSamples > timeSamplesPeriod)
        {
            timeSamplesPeriod++;
            metronomeSingle.TriggerAudioObject(); // do this from invokeRepeating (adjust invokeRepeating accourding to timeSamples-drifting)
            int driftInSamples = timeSamples % tickPeriodInTimeSamples;
            if (driftInSamples > Mathf.Abs(timeSamplesOffset*2))
            {
                driftInSamples = driftInSamples - tickPeriodInTimeSamples;
            }
            float driftInSeconds = (float)driftInSamples / sampleRate;
            //print(driftInSeconds);
        }
    }
    int timeSamplesPeriod;
    public int timeSamplesOffset;
    public int timeSamplesDivision;

    public void TestSoundCondition()
    {
        sourceScript.SubscribedMethod();
        //AudioSourceTest(ref clip1Play, clip1, clip1Conditions);
        //AudioSourceTest(ref clip2Play, clip2, clip2Conditions);

        if (!notFirst)
        {
            notFirst = true;
            //musicMeter.ResetMeterCounts();
            ResetTimer();
        }
        for (int i = 0; i < testCondition1.Length; i++)
        {
            if (musicMeter.MeterConditionSpecificTarget(testCondition1[i]))
            {
                //timeSinceSound = Time.time - timeLastSound;
                //dspTimeSinceSound = dspTimeNormalized - dspTimeLastSound;
                //timeDifference = timeSinceSound - dspTimeSinceSound;
                //print("time since:" + timeSinceSound);
                //print("dspt since:" + dspTimeSinceSound);
                //print("diff: " + timeDifference);

                //print(musicMeter.meterDisplay + "tim " + Time.time);
                //print(musicMeter.meterDisplay + "dsp " + dspTimeNormalized);

                //timeLastSound = Time.time;
                //dspTimeLastSound = dspTimeNormalized;
            }
        }

        MetronomeTesting(activateMetronome1, testCondition1, metronome1);
        MetronomeTesting(activateMetronome2, testCondition2, metronome2);
        MetronomeTesting(activateMetronome4, testCondition4, metronome4);
        MetronomeTesting(activateMetronome8, testCondition8, metronome8);
        MetronomeTesting(activateMetronome16, testCondition16, metronome16);
    }

    private void MetronomeTesting(bool active, MusicMeter.MeterCondition[] condition, AudioObject sound)
    {
        if (active)
        {
            if (sound.IsPlaying())
            {
                Debug.Log("isPlaying:" + sound.gameObject);
            }
            for (int i = 0; i < condition.Length; i++)
            {
                if (musicMeter.MeterConditionSpecificTarget(condition[i]))
                {
                    sound.TriggerAudioObject();
                }
            }
        }
    }

    
    public void ResetTimer()
    {
        signatureHi = musicMeter.beatMax;
        SetTickTimer();
        SubscribeToTick(ResetTimer, false);
    }
    public delegate void EventHandler();
    public event EventHandler NextTick;
    private bool dspTrigger;
    private void RaiseTheMainThreadEvent()
    {
        if (dspTrigger)
        {
            //musicMeter.RaiseEventsOnTick();
            dspTrigger = false;
        }
    }
    public void SubscribeToTick(EventHandler eventHandlingMethod, bool subscribe)
    {
        if (subscribe)
            NextTick += eventHandlingMethod;
        else
            NextTick -= eventHandlingMethod;
    }
    public void SubscribeToTickSmart(EventHandler eventHandlingMethod, bool subscribe, ref bool isSubscribed)
    {
        if (subscribe)
        {
            if (!isSubscribed)
            {
                NextTick += eventHandlingMethod;
                isSubscribed = true;
            }
        }
        else
        {
            NextTick -= eventHandlingMethod;
            isSubscribed = false;
        }
    }
}