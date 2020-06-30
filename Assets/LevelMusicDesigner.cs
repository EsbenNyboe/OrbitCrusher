using UnityEngine;

public class LevelMusicDesigner : MonoBehaviour
{
    public int beatsPerBar;
    public int barsPerSection;


    public SoundTriggerRepeat[] transitionToLevel;
    public SoundTriggerObjectiveFilter[] levelObjectiveFilter;
    public SoundTriggerSimple[] levelSimple;
    public SoundTriggerRepeat[] levelRepeat;
    //public SoundTriggerObjectiveFilter[] fullHpObjectiveFilter;
    public SoundTriggerSimple[] fullHpSimple;
    public SoundTriggerRepeat[] fullHpRepeat;
    public SoundTriggerRepeat[] lastObjective;


    //high level classes
    [System.Serializable]
    public class SoundTriggerSimple
    {
        [HideInInspector]
        public string name;
        public AudioObject audioObject;
        public MeterConditionSimple[] simpleMeterConditions;
    }
    [System.Serializable]
    public class SoundTriggerRepeat
    {
        [HideInInspector]
        public string name;
        public AudioObject audioObject;
        public MeterCondition[] meterConditions;
        public Repeat[] repeats;
    }
    [System.Serializable]
    public class SoundTriggerObjectiveFilter
    {
        [HideInInspector]
        public string name;
        public AudioObject audioObject;
        public MeterCondition[] meterConditions;
        public Repeat[] repeats;
        public ObjectiveFilter[] objectiveFilters;
    }
    // low level classes
    [System.Serializable]
    public class MeterCondition
    {
        [HideInInspector]
        public string data;
        public int division;
        public int beat;
        public int bar;
        public int sec;
    }
    [System.Serializable]
    public class MeterConditionSimple
    {
        [HideInInspector]
        public string data;
        public int bar;
        public int sec;
    }
    [System.Serializable]
    public class Repeat
    {
        [HideInInspector]
        public string data;
        public int repeatInterval;
        public int repeatAmount;
        public int relatedMeterCondition;
    }
    [System.Serializable]
    public class ObjectiveFilter
    {
        [HideInInspector]
        public string data;
        public int objectiveNumber;
        public bool allowOrBlock;
        public int relatedMeterCondition;
    }

    private void Start()
    {
        
    }
    private void Update()
    {
        
    }


    #region Inspector Naming
    public void UpdateInspectorNames()
    {
        Naming_Repeat(transitionToLevel);
        Naming_Repeat(levelRepeat);
        Naming_Repeat(fullHpRepeat);
        Naming_Repeat(lastObjective);
        Naming_Simple(levelSimple);
        Naming_Simple(fullHpSimple);
        Naming_Objective(levelObjectiveFilter);
    }
    private void Naming_Simple(SoundTriggerSimple[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i].audioObject != null)
                s[i].name = s[i].audioObject.name;
            for (int e = 0; e < s[i].simpleMeterConditions.Length; e++)
            {
                s[i].simpleMeterConditions[e].data =
                    "b:" +
                    s[i].simpleMeterConditions[e].bar + " s:" +
                    s[i].simpleMeterConditions[e].sec;
            }
        }
    }
    private void Naming_Repeat(SoundTriggerRepeat[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i].audioObject != null)
                s[i].name = s[i].audioObject.name;
            for (int e = 0; e < s[i].meterConditions.Length; e++)
            {
                s[i].meterConditions[e].data =
                    e + "mc: " +
                    s[i].meterConditions[e].division + "." +
                    s[i].meterConditions[e].beat + "." +
                    s[i].meterConditions[e].bar + "." +
                    s[i].meterConditions[e].sec;
            }
            for (int e = 0; e < s[i].repeats.Length; e++)
            {
                s[i].repeats[e].data =
                    "i:" + 
                    s[i].repeats[e].repeatInterval + " A:" +
                    s[i].repeats[e].repeatAmount + " M:" +
                    s[i].repeats[e].relatedMeterCondition;
            }
        }
    }
    private void Naming_Objective(SoundTriggerObjectiveFilter[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i].audioObject != null)
                s[i].name = s[i].audioObject.name;
            for (int e = 0; e < s[i].meterConditions.Length; e++)
            {
                s[i].meterConditions[e].data =
                    e + "mc: " +
                    s[i].meterConditions[e].division + "." +
                    s[i].meterConditions[e].beat + "." +
                    s[i].meterConditions[e].bar + "." +
                    s[i].meterConditions[e].sec;
            }
            for (int e = 0; e < s[i].objectiveFilters.Length; e++)
            {
                s[i].objectiveFilters[e].data =
                    "objFil: " +
                    s[i].objectiveFilters[e].objectiveNumber + " " +
                    s[i].objectiveFilters[e].allowOrBlock + " " +
                    s[i].objectiveFilters[e].relatedMeterCondition;
            }
            for (int e = 0; e < s[i].repeats.Length; e++)
            {
                s[i].repeats[e].data =
                    "i:" +
                    s[i].repeats[e].repeatInterval + " a:" +
                    s[i].repeats[e].repeatAmount + " m:" +
                    s[i].repeats[e].relatedMeterCondition;
            }
        }
    }
    #endregion



    public class SoundTrigger
    {
        public AudioObject audioObject;
        public MusicMeter.MeterCondition meterCondition;
        public int repeatInterval;
        public int repeatAmount;
        
        public bool objectiveFilter;
        public int objectiveNumber;
        public bool allowOrBlock;

        public bool lvl;
        public bool fullHp;
        public bool lastObj;
    }
    public SoundTrigger[] soundTriggers;

    int index;
    public void CreateCentralArrayForAllSoundTriggers()
    {
        soundTriggers = new SoundTrigger[100];
        index = 0;

        RepeatArrayCopy(transitionToLevel);
        RepeatArrayCopy(levelRepeat);
        RepeatArrayCopy(fullHpRepeat);
        RepeatArrayCopy(lastObjective);

        SimpleArrayCopy(levelSimple);
        SimpleArrayCopy(fullHpSimple);
        
        ObjectiveArrayCopy(levelObjectiveFilter);

        for (int i = 0; i < levelRepeat.Length; i++)
        {
            // objective conditions
        }

        SoundTrigger[] temp = soundTriggers;
        soundTriggers = new SoundTrigger[index];
        for (int i = 0; i < soundTriggers.Length; i++)
        {
            soundTriggers[i] = temp[i];
        }
    }

    private void ObjectiveArrayCopy(SoundTriggerObjectiveFilter[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            for (int mc = 0; mc < s[i].meterConditions.Length; mc++)
            {
                soundTriggers[index].audioObject = s[i].audioObject;
                soundTriggers[index].meterCondition.division = 1;
                soundTriggers[index].meterCondition.beat = 1;
                soundTriggers[index].meterCondition.bar = s[i].meterConditions[mc].bar;
                soundTriggers[index].meterCondition.section = s[i].meterConditions[mc].sec;

                for (int e = 0; e < s[i].repeats.Length; e++)//silly
                {
                    if (mc == s[i].repeats[e].relatedMeterCondition)
                    {
                        soundTriggers[index].repeatInterval = s[i].repeats[e].repeatInterval;
                        soundTriggers[index].repeatAmount = s[i].repeats[e].repeatAmount;
                    }
                }

                for (int e = 0; e < s[i].objectiveFilters.Length; e++)//silly
                {
                    if (mc == s[i].objectiveFilters[e].relatedMeterCondition)
                    {
                        soundTriggers[index].objectiveNumber = s[i].objectiveFilters[e].objectiveNumber;
                        soundTriggers[index].allowOrBlock = s[i].objectiveFilters[e].allowOrBlock;
                    }
                }

                index++;
            }

            
            
            for (int e = 0; e < s[i].meterConditions.Length; e++)
            {
                s[i].meterConditions[e].data =
                    e + "mc: " +
                    s[i].meterConditions[e].division + "." +
                    s[i].meterConditions[e].beat + "." +
                    s[i].meterConditions[e].bar + "." +
                    s[i].meterConditions[e].sec;
            }
            for (int e = 0; e < s[i].objectiveFilters.Length; e++)
            {
                s[i].objectiveFilters[e].data =
                    "objFil: " +
                    s[i].objectiveFilters[e].objectiveNumber + " " +
                    s[i].objectiveFilters[e].allowOrBlock + " " +
                    s[i].objectiveFilters[e].relatedMeterCondition;
            }
            for (int e = 0; e < s[i].repeats.Length; e++)
            {
                s[i].repeats[e].data =
                    "i:" +
                    s[i].repeats[e].repeatInterval + " a:" +
                    s[i].repeats[e].repeatAmount + " m:" +
                    s[i].repeats[e].relatedMeterCondition;
            }
        }
    }
    private void SimpleArrayCopy(SoundTriggerSimple[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            for (int mc = 0; mc < s[i].simpleMeterConditions.Length; mc++)
            {
                soundTriggers[index].audioObject = s[i].audioObject;
                soundTriggers[index].meterCondition.division = 1;
                soundTriggers[index].meterCondition.beat = 1;
                soundTriggers[index].meterCondition.bar = s[i].simpleMeterConditions[mc].bar;
                soundTriggers[index].meterCondition.section = s[i].simpleMeterConditions[mc].sec;
                index++;
            }
        }
    }

    private void RepeatArrayCopy(SoundTriggerRepeat[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            for (int mc = 0; mc < s[i].meterConditions.Length; mc++)
            {
                soundTriggers[index].audioObject = s[i].audioObject;
                soundTriggers[index].meterCondition.division = s[i].meterConditions[mc].division;
                soundTriggers[index].meterCondition.beat = s[i].meterConditions[mc].beat;
                soundTriggers[index].meterCondition.bar = s[i].meterConditions[mc].bar;
                soundTriggers[index].meterCondition.section = s[i].meterConditions[mc].sec;

                for (int e = 0; e < s[i].repeats.Length; e++)//silly
                {
                    if (mc == s[i].repeats[e].relatedMeterCondition)
                    {
                        soundTriggers[index].repeatInterval = s[i].repeats[e].repeatInterval;
                        soundTriggers[index].repeatAmount = s[i].repeats[e].repeatAmount;
                    }
                }
                index++;
            }
        }
    }

    public void CheckSoundTriggers()
    {

    }
}
