using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAchievementAnimation : MonoBehaviour
{
    public AudioObject newAchievementSoundA;
    public AudioObject newAchievementSoundB;

    public void PlayAchievementSoundA()
    {
        newAchievementSoundA.TriggerAudioObject();
    }
    public void PlayAchievementSoundB()
    {
        newAchievementSoundB.TriggerAudioObject();
    }
}
