using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurvePrint : MonoBehaviour
{
    public AnimationCurve valueCurve = new AnimationCurve();
    public static float value;

    //    public AnimationCurve accelerationCurve = new AnimationCurve();
    [HideInInspector]
    public float valueSpeed;
    [HideInInspector]
    public float valueAcceleration;

    public float lerpin;
    float lerpedSpeed;
    float oldSpeed;



    float derpTimer;
    void Update()
    {
        
        //GraphDisplay_SphereMovementSpeed();

        GraphDisplay_TimeUntilTarget();
    }

    
    private void GraphDisplay_TimeUntilTarget()
    {
        valueCurve.AddKey(Time.realtimeSinceStartup, value);
    }

    private void GraphDisplay_SphereMovementSpeed()
    {
        lerpedSpeed = Mathf.Lerp(lerpedSpeed, valueSpeed, lerpin);
        valueCurve.AddKey(Time.realtimeSinceStartup, lerpedSpeed);
        if (oldSpeed == lerpedSpeed)
        {
            derpTimer += Time.deltaTime;
            if (derpTimer > 0.1f)
                valueSpeed = 0;
        }
        else
            derpTimer = 0;
        oldSpeed = lerpedSpeed;
    }
}
