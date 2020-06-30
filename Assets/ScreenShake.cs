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


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ScreenShakeLevelCompleted();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ScreenShakeLevelFailed();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ScreenShakeObjectiveCompleted();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ScreenShakeObjectiveFailed();
        }

    }

    public void ScreenShakeLevelCompleted()
    {
        CameraShaker.Instance.ShakeOnce(magnitudeLvlWin, roughnessLvlWin, fadeInTimeLvlWin, fadeOutTimeLvlWin);
    }
    public void ScreenShakeLevelFailed()
    {
        CameraShaker.Instance.ShakeOnce(magnitudeLvlFail, roughnessLvlFail, fadeInTimeLvlFail, fadeOutTimeLvlFail);
    }
    public void ScreenShakeObjectiveCompleted()
    {
        CameraShaker.Instance.ShakeOnce(magnitudeObjWin, roughnessObjWin, fadeInTimeObjWin, fadeOutTimeObjWin);
    }
    public void ScreenShakeObjectiveFailed()
    {
        CameraShaker.Instance.ShakeOnce(magnitudeObjFail, roughnessObjFail, fadeInTimeObjFail, fadeOutTimeObjFail);
    }
}
