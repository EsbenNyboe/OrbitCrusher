using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    #region variables
    public MeterLookahead meterLookahead;
    public SoundDsp soundDsp;
    public SoundManager soundManager;
    public MusicMeter musicMeter;

    [System.Serializable]
    public class Part
    {
        [HideInInspector]
        public string name;
        public int objective; // is automatically set on last objective 
        public int transpositionID;
        public Sound[] sound;

        [HideInInspector]
        public int[] repeatIndex;
        [HideInInspector]
        public bool[] repeatActive;
    }
    [System.Serializable]
    public class Sound
    {
        [HideInInspector]
        public string name;
        public AudioObject audioObject;

        [Tooltip("x:section, y:bar")]
        public Vector2 mcSectionBar; // don't reference this
        [Tooltip("x:beat, y:division")]
        public Vector2 mcBeatDivision; // don't reference this
        [HideInInspector]
        public MusicMeter.MeterCondition meterCondition;

        [HideInInspector]
        public bool stinger; // used to be editor-parameter, but removed for simplicity

        [Tooltip("x:interval, y:lifetime")]
        public Vector2 repeat; // don't reference this
        [HideInInspector]
        public int repeatAtBeatInterval;
        [HideInInspector]
        public int repeatLifetime;

        [HideInInspector] 
        public int objectiveConditionSpecial; // overwrites the part objective.... not used..

        public ClutchTrigger clutchTrigger; // don't reference this
        [HideInInspector]
        public bool clutchFadein;
        [HideInInspector]
        public bool clutchRiser;

        public FadeOutType fadeOutType; // don't reference this
        [HideInInspector]
        public bool clutchFadeout;
        [HideInInspector]
        public bool noFadeout;


        [HideInInspector]
        public bool hasPlayed;
        [HideInInspector]
        public bool hasFadedIn;
        [HideInInspector]
        public bool wasClutchFadein;
        [HideInInspector]
        public MusicMeter.MeterCondition localMeter;
    }
    public enum ClutchTrigger
    {
        Disabled,
        Riser,
        Hit
    }
    public enum FadeOutType
    {
        Default,
        NoFadeout,
        ClutchFadeout
    }
    public Part[] part;

    public AudioObject[] lastObjQuickFadeout;

    public Sound levelCompletedRiser;

    MusicMeter.MeterCondition mcOne;
    bool newPart;

    int activePart;
    int musicMeterOffsetSection;
    int musicMeterOffsetBar;

    bool prePlayInterval;
    bool clutchFadeoutAllowed;

    //delete:
    public int sectionDisplay;
    public int barDisplay;
    public string meterDisplay;


    #endregion

    private void Start() 
    {
        //PrepareLevelMusic();    //TEMPORARY: REMOVE WHEN FINISHED WITH LEVELMUSIC DESIGNING!!!!!
        mcOne = new MusicMeter.MeterCondition();
        mcOne.section = mcOne.bar = 0;
        mcOne.beat = mcOne.division = 1;
    }
    #region EditorMethods
    public void PrepareLevelMusic()
    {
        ConvertEditorParametersToCodableVariables();
        GenerateAutomaticLastObjectiveContext();
        NameSounds();
        NameParts();
    }
    private void NameParts()
    {
        for (int i = 0; i < part.Length; i++)
        {
            string name = "";
            if (i == 0) { name = "A"; }
            else if (i == 1) { name = "B"; }
            else if (i == 2) { name = "C"; }
            else if (i == 3) { name = "D"; }
            else if (i == 4) { name = "E"; }
            else if (i == 5) { name = "F"; }
            else if (i == 6) { name = "G"; }
            part[i].name = name;
        }
        part[part.Length - 1].name = "LAST"; 
    }

    private void NameSounds()
    {
        for (int i = 0; i < part.Length; i++)
        {
            for (int e = 0; e < part[i].sound.Length; e++)
            {
                part[i].sound[e].name = part[i].sound[e].audioObject.name;
            }
        }
    }
    private void ConvertEditorParametersToCodableVariables()
    {
        SoundConversions(levelCompletedRiser);
        for (int i = 0; i < part.Length; i++)
        {
            Part p = part[i];
            for (int e = 0; e < p.sound.Length; e++)
            {
                SoundConversions(p.sound[e]);
            }
        }
    }

    private static void SoundConversions(Sound s)
    {
        s.stinger = true;

        s.meterCondition = new MusicMeter.MeterCondition();
        s.meterCondition.section = (int)s.mcSectionBar.x;
        s.meterCondition.bar = (int)s.mcSectionBar.y;
        s.meterCondition.beat = (int)s.mcBeatDivision.x;
        if (s.mcBeatDivision.y < 1)
            s.mcBeatDivision.y = 1;
        s.meterCondition.division = (int)s.mcBeatDivision.y;

        s.repeatAtBeatInterval = (int)s.repeat.x;
        s.repeatLifetime = (int)s.repeat.y;

        switch (s.clutchTrigger)
        {
            case ClutchTrigger.Disabled:
                s.clutchFadein = false;
                s.clutchRiser = false;
                break;
            case ClutchTrigger.Riser:
                s.clutchFadein = true;
                s.clutchRiser = true;
                break;
            case ClutchTrigger.Hit:
                s.clutchFadein = true;
                s.clutchRiser = false;
                break;
        }

        s.hasPlayed = s.hasFadedIn = s.wasClutchFadein = false;

        if (s.clutchFadein)
            s.wasClutchFadein = true;

        switch (s.fadeOutType)
        {
            case FadeOutType.Default:
                s.clutchFadeout = false;
                s.noFadeout = false;
                break;
            case FadeOutType.NoFadeout:
                s.clutchFadeout = false;
                s.noFadeout = true;
                break;
            case FadeOutType.ClutchFadeout:
                s.clutchFadeout = true;
                s.noFadeout = false;
                break;
        }
    }

    private void GenerateAutomaticLastObjectiveContext()
    {
        part[part.Length - 1].objective = GetComponent<LevelDesigner>().levelObjectives.Length - 1;
    }
    #endregion

    public void LoadIntoLevelManager()
    {
        LevelManager.part = part;
        LevelManager.lastObjQuickFadeOut = lastObjQuickFadeout;
    }
    public void CheckIfTransposed()
    {
        bool objectiveIsTransposedOnce = false;
        bool objectiveIsTransposedTwice = false;
        switch (part[activePart].transpositionID)
        {
            case 1:
                objectiveIsTransposedOnce = true;
                break;
            case 2:
                objectiveIsTransposedTwice = true;
                break;
        }
        soundManager.ToggleTransposedMusic(objectiveIsTransposedOnce, objectiveIsTransposedTwice);
    }
    public void CheckMusicTimings(bool inTransition) // subscribed
    {
        if (!inTransition)
        {
            CheckForPrePlaySounds(activePart, false);
            if (activePart + 1 < part.Length)
                CheckForPrePlaySounds(activePart + 1, true);
        }

        if (prePlayInterval)
        {
            if (part.Length > activePart + 1 && part[activePart + 1].objective == LevelManager.levelObjectiveCurrent + 1)
            {
                Sound[] s = part[activePart + 1].sound;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].clutchFadein && !s[i].clutchRiser)
                    {
                        FadeInPrePlayedHit(activePart + 1, i);
                    }
                }
            }
            prePlayInterval = false;
        }

        if (clutchFadeoutAllowed)
        {
            CheckForFadeouts();
        }
        if (ObjectiveCondition())
        {
            if (newPart)
            {
                CheckForFadeouts();
            }

            Sound[] s = part[activePart].sound;
            for (int i = 0; i < s.Length; i++)
            {
                bool objectiveFilter = false;
                if (s[i].objectiveConditionSpecial != 0)
                {
                    //if (s[i].objectiveConditionSpecial != LevelManager.levelObjectiveCurrent)
                    //    objectiveFilter = true;
                }

                if (!objectiveFilter)
                {
                    if (!inTransition)
                        MusicRepeater(s, i);

                    CheckTimingsAndPlaySounds(activePart, s, i);
                }

                if (LevelManager.lastObjective)
                {
                    if (meterLookahead.SoundLookaheadRelativeToCondition(levelCompletedRiser.meterCondition))
                    {
                        if (!levelCompletedRiser.hasPlayed)
                        {
                            levelCompletedRiser.hasPlayed = true;
                            levelCompletedRiser.audioObject.VolumeChangeInParent(0, 0, false);
                        }
                        soundDsp.LevelMusic_ScheduledPlayRelativeToDspReference(levelCompletedRiser.audioObject);
                    }
                }
            }
        }
    }
    public void AllOrbsHaveHitTarget()
    {
        int p = activePart;
        if (p < part.Length - 1 && part[p + 1].objective == LevelManager.levelObjectiveCurrent + 1)
        {
            Sound[] s = part[p].sound;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].clutchFadeout)
                {
                    s[i].hasPlayed = true;
                    for (int e = 0; e < s[i].audioObject.voicePlayerNew.Length; e++)
                    {
                        if (s[i].clutchFadeout && s[i].audioObject.voicePlayerNew[e].IsPlaying())
                        {
                            soundManager.LevelMusicFadeOutMethodNew(s[i].audioObject.voicePlayerNew[e]);
                        }
                        part[p].repeatIndex[i] = 0;
                        part[p].repeatActive[i] = false;
                    }
                }
            }
        }


        prePlayInterval = true;
        CheckForClutchFadeins(p);
        if (p + 1 < part.Length)
        {
            CheckForClutchFadeins(p + 1);

            if (part[p + 1].objective == LevelManager.levelObjectiveCurrent + 1)
                clutchFadeoutAllowed = true;
        }
        else if (LevelManager.lastObjective)
        {
            levelCompletedRiser.audioObject.VolumeChangeInParent(levelCompletedRiser.audioObject.initialVolume, 0.1f, false);
        }
    }
    public void ObjectiveCompleted()
    {
        clutchFadeoutAllowed = false;
    }
    public void LevelUnloaded()
    {
        levelCompletedRiser.hasPlayed = false;
        musicMeterOffsetSection = 0;
        musicMeterOffsetBar = 0;
        activePart = 0;
        for (int i = 0; i < part.Length; i++)
        {
            for (int e = 0; e < part[i].sound.Length; e++)
            {
                if (part[i].sound[e].wasClutchFadein)
                    part[i].sound[e].clutchFadein = true;
                part[i].sound[e].hasPlayed = false;
                part[i].sound[e].hasFadedIn = false;

                //part[i].repeatIndex[e] = 0;
                part[i].repeatActive[e] = false;
            }
        }

        newPart = false; // ???
    }







    private void CheckForPrePlaySounds(int p, bool nextPart)
    {
        Sound[] s = part[p].sound;
        if (part[p].objective - 1 == LevelManager.levelObjectiveCurrent)
        {
            for (int i = 0; i < s.Length; i++)
            {
                // check special objective condition (code needs rearranging to do so)
                if (s[i].clutchFadein)
                {
                    if (nextPart && s[i].meterCondition.section != 0)
                    {
                    }
                    else
                        CheckTimingsAndPlaySounds(p, s, i);
                }
            }
        }
    }


    private void CheckForFadeouts()
    {
        if (meterLookahead.SoundLookaheadRelativeToCondition(mcOne)) // early fade out
        {
            //print("early");
            FadeOut(0.5f);
        }
        else if (musicMeter.MeterConditionSpecificTarget(mcOne)) // late fade out
        {
            //print("late");
            FadeOut(0.01f);
        }
    }
    private void FadeOut(float fadeOutTime)
    {
        if (!clutchFadeoutAllowed)
        {
            newPart = false;
            if (activePart > 0)
            {
                CheckIfItShouldFadeOut(fadeOutTime, activePart - 1);
            }
        }
        else
        {
            CheckIfItShouldFadeOut(fadeOutTime, activePart);
        }
    }

    private void CheckIfItShouldFadeOut(float fadeOutTime, int p)
    {
        bool clutchFadeOutTriggered = false;
        Sound[] s = part[p].sound;
        for (int i = 0; i < s.Length; i++)
        {
            bool checkForFadeout = true;
            if (s[i].noFadeout)
                checkForFadeout = false;
            //else if (s[i].clutchFadeout && p != activePart)
            //    checkForFadeout = false;
            else if (!s[i].clutchFadeout && p == activePart)
                checkForFadeout = false;

            if (s[i].clutchFadeout)
                checkForFadeout = false; ///////

            for (int e = 0; e < s[i].audioObject.voicePlayerNew.Length; e++)
            {
                if (checkForFadeout && s[i].audioObject.voicePlayerNew[e].IsPlaying())
                {
                    //print("fadeout " + s[i].audioObject.name + " time:" + fadeOutTime);

                    soundManager.LevelMusicFadeOutMethod(s[i].audioObject.voicePlayerNew[e], fadeOutTime, false);
                    if (s[i].clutchFadeout)
                        clutchFadeOutTriggered = true;
                    part[p].repeatIndex[i] = 0;
                    part[p].repeatActive[i] = false;
                }
            }
        }
        if (clutchFadeOutTriggered)
            clutchFadeoutAllowed = false;
    }

    private bool ObjectiveCondition()
    {
        bool objective = false;
        for (int i = 0; i < part.Length; i++)
        {
            if (part[i].objective <= LevelManager.levelObjectiveCurrent)
            {
                if (i == part.Length - 1 || part[i + 1].objective > LevelManager.levelObjectiveCurrent)
                {
                    objective = true;
                    if (activePart != i)
                    {
                        SetMusicMeterOffset();
                        newPart = true;
                        //print("activePartChange");
                    }
                    activePart = i;
                }
            }
        }
        return objective;
    }

    private void SetMusicMeterOffset()
    {
        musicMeterOffsetSection = MusicMeter.sectionCount - 1;
        musicMeterOffsetBar = MusicMeter.barCount - 1;
        if (musicMeterOffsetBar + 1 == musicMeter.barMax)
        {
            //musicMeterOffsetSection += 1;
            //musicMeterOffsetBar = 0;
        }
        meterDisplay = musicMeter.meterDisplay;
    }

    private void MusicRepeater(Sound[] s, int i)
    {
        if (part[activePart].repeatActive[i])
        {
            MusicMeter.MeterCondition nxtBeat = new MusicMeter.MeterCondition();
            nxtBeat.division = 1;

            if (meterLookahead.SoundLookaheadRelativeToCondition(nxtBeat))
            {
                part[activePart].repeatIndex[i]++;
                //repeatIndex[i]++;
                if ((float)part[activePart].repeatIndex[i] / s[i].repeatAtBeatInterval > s[i].repeatLifetime)
                {
                    part[activePart].repeatActive[i] = false;
                }
                else if (part[activePart].repeatIndex[i] % s[i].repeatAtBeatInterval == 0)
                {
                    if (s[i].clutchFadein && !s[i].hasFadedIn)
                    {
                    }
                    else
                    {
                        s[i].clutchFadein = false;
                        if (s[i].hasPlayed)
                            PlaySound(activePart, i);
                    }
                }
            }
        }
    }
    MusicMeter.MeterCondition localMeter;
    private void CheckTimingsAndPlaySounds(int p, Sound[] s, int i)
    {
        //print(s[i].name + ": " + s[i].meterCondition.section + " " + s[i].meterCondition.bar + " " + s[i].meterCondition.beat + " " + s[i].meterCondition.division);

        localMeter = new MusicMeter.MeterCondition();
        if (s[i].meterCondition.section != 0)
        {
            localMeter.section = musicMeterOffsetSection + s[i].meterCondition.section;
        }
        if (s[i].meterCondition.bar != 0)
        {
            localMeter.bar = musicMeterOffsetBar + s[i].meterCondition.bar;
            if (!s[i].clutchFadein && p != 0)
            {
                localMeter.bar += 1;
                if (localMeter.bar > musicMeter.barMax)
                {
                    localMeter.bar = localMeter.bar - musicMeter.barMax;
                    //localMeter.bar = localMeter.bar % musicMeter.barMax;
                    //localMeter.bar = 1;
                    localMeter.section += 1;
                }
            }
            //if (localMeter.bar + 1 > musicMeter.barMax)
            //{
            //    localMeter.bar = 1;
            //    localMeter.section += 1;
            //}
        }
        localMeter.beat = s[i].meterCondition.beat;
        localMeter.division = s[i].meterCondition.division;

        sectionDisplay = musicMeterOffsetSection;
        barDisplay = musicMeterOffsetBar;

        if (meterLookahead.SoundLookaheadRelativeToCondition(localMeter))
        {
            if (s[i].repeatAtBeatInterval > 0 && !part[p].repeatActive[i])
            {
                if (s[i].stinger && s[i].hasPlayed)
                {
                }
                else 
                {
                    part[p].repeatIndex[i] = 0;
                    part[p].repeatActive[i] = true;
                }
            }
            if (s[i].stinger)
            {
                if (!s[i].hasPlayed)
                {
                    s[i].hasPlayed = true;
                    PlaySound(p, i);
                }
            }
            else
            {
                PlaySound(p, i);
            }
        }
        s[i].localMeter = localMeter;
        StartCoroutine(WaitForEndFrameToAllowForSoundPlay(s[i]));
    }

    IEnumerator WaitForEndFrameToAllowForSoundPlay(Sound s)
    {
        yield return new WaitForEndOfFrame();
        if (s.clutchFadein && !s.clutchRiser && !s.hasFadedIn)
        {
            if (musicMeter.MeterConditionSpecificTarget(s.localMeter))
            {
                s.audioObject.StopAudioAllVoices();
            }
        }
    }


    private void CheckForClutchFadeins(int p)
    {
        Sound[] s = part[p].sound;
        if (part[p].objective == LevelManager.levelObjectiveCurrent + 1)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].clutchFadein && s[i].clutchRiser)
                {
                    if (!s[i].hasFadedIn)
                        FadeinPrePlayedRiser(p, i);
                }
            }
        }
    }

    
    private void PlaySound(int p, int i)
    {
        Sound s = part[p].sound[i];
        //Debug.Log(s.name + " " + Time.time, gameObject);
        if (s.clutchFadein)
        {
            PrePlaySound(p, i);
            //print("pre:" + s.name + " " + Time.time);
        }
        else
        {
            if (!LevelManager.lastObjective)
                s.audioObject.VolumeChangeInParent(s.audioObject.initialVolume, 0, false);
            soundDsp.LevelMusic_ScheduledPlayRelativeToDspReference(s.audioObject);
            //print("play:" + s.meterCondition.section + "." + s.meterCondition.bar + " " + s.name + " " + Time.time);
        }
    }
    private void PrePlaySound(int p, int i)
    {
        Sound s = part[p].sound[i];
        //print("preplay " + s.audioObject.name);
        if (!s.hasFadedIn) 
        {
            s.audioObject.VolumeChangeInParent(0, 0, false);
            s.audioObject.StopAudioAllVoices();
            s.hasPlayed = false;
            part[p].repeatIndex[i] = 0;
        }
        if (part[p].objective > LevelManager.levelObjectiveCurrent || s.meterCondition.section != 0)
            soundDsp.LevelMusic_ScheduledPlayRelativeToDspReference(s.audioObject);
    }


    private void FadeInPrePlayedHit(int p, int i)
    {
        Sound s = part[p].sound[i];
        s.hasFadedIn = true;
        s.audioObject.VolumeChangeInParent(s.audioObject.initialVolume, 0f, false);
        //print("fadein Hit " + s.audioObject.name);
    }

    private void FadeinPrePlayedRiser(int p, int i)
    {
        Sound s = part[p].sound[i];
        s.hasFadedIn = true;
        s.audioObject.VolumeChangeInParent(s.audioObject.initialVolume, 0.1f, false);
        //print("fadein Riser " + s.audioObject.name);
    }
}
