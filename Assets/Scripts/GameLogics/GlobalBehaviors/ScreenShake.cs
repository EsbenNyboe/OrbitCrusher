using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ScreenShake : MonoBehaviour
{
    [Header("Level Win")]
    public float magnitudeLvlWin;
    public float roughnessLvlWin;
    public float fadeInTimeLvlWin;
    public float fadeOutTimeLvlWin;
    
    [Header("Level Failed")]
    public float magnitudeLvlFail;
    public float roughnessLvlFail;
    public float fadeInTimeLvlFail;
    public float fadeOutTimeLvlFail;
    
    [Header("Objective Win")]
    public float magnitudeObjWin;
    public float roughnessObjWin;
    public float fadeInTimeObjWin;
    public float fadeOutTimeObjWin;
    
    [Header("Objective Fail")]
    public float magnitudeObjFail;
    public float roughnessObjFail;
    public float fadeInTimeObjFail;
    public float fadeOutTimeObjFail;

    [Header("Collision Good Node")]
    public float magnitudeCollGood;
    public float roughnessCollGood;
    public float fadeInTimeCollGood;
    public float fadeOutTimeCollGood;

    [Header("Collision Bad Node")]
    public float magnitudeCollNode;
    public float roughnessCollNode;
    public float fadeInTimeCollNode;
    public float fadeOutTimeCollNode;

    [Header("Collision Bad Comet")]
    public float magnitudeCollComet;
    public float roughnessCollComet;
    public float fadeInTimeCollComet;
    public float fadeOutTimeCollComet;

    [Header("Level Start First Node Hit")]
    public float magnitudeLvlStart;
    public float roughnessLvlStart;
    public float fadeInTimeLvlStart;
    public float fadeOutTimeLvlStart;

    private void Update()
    {
       

    }

    public void ScreenShakeLevelStart()
    {
        if (!GameManager.inTutorial)
            CameraShaker.Instance.ShakeOnce(magnitudeLvlStart, roughnessLvlStart, fadeInTimeLvlStart, fadeOutTimeLvlStart);
    }
    public void ScreenShakeLevelCompleted()
    {
        if (!GameManager.inTutorial)
            CameraShaker.Instance.ShakeOnce(magnitudeLvlWin, roughnessLvlWin, fadeInTimeLvlWin, fadeOutTimeLvlWin);
    }
    public void ScreenShakeLevelFailed()
    {
        if (!GameManager.inTutorial)
            CameraShaker.Instance.ShakeOnce(magnitudeLvlFail, roughnessLvlFail, fadeInTimeLvlFail, fadeOutTimeLvlFail);
    }
    public void ScreenShakeObjectiveCompleted()
    {
        if (!GameManager.inTutorial)
            CameraShaker.Instance.ShakeOnce(magnitudeObjWin, roughnessObjWin, fadeInTimeObjWin, fadeOutTimeObjWin);
    }
    public void ScreenShakeObjectiveFailed()
    {
        if (!GameManager.inTutorial)
            CameraShaker.Instance.ShakeOnce(magnitudeObjFail, roughnessObjFail, fadeInTimeObjFail, fadeOutTimeObjFail);
    }
    public void ScreenShakeCollGoodNode()
    {
        if (!GameManager.inTutorial)
            CameraShaker.Instance.ShakeOnce(magnitudeCollGood, roughnessCollGood, fadeInTimeCollGood, fadeOutTimeCollGood);
    }
    public void ScreenShakeCollBadNode()
    {
        if (!GameManager.inTutorial)
            CameraShaker.Instance.ShakeOnce(magnitudeCollNode, roughnessCollNode, fadeInTimeCollNode, fadeOutTimeCollNode);
    }
    public void ScreenShakeCollBadComet()
    {
        if (!GameManager.inTutorial)
            CameraShaker.Instance.ShakeOnce(magnitudeCollComet, roughnessCollComet, fadeInTimeCollComet, fadeOutTimeCollComet);
    }
}