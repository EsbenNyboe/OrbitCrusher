using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceDrainer : MonoBehaviour
{
    SimulateBadPerformance simulateBadPerformance;

    // Start is called before the first frame update
    void Start()
    {
        simulateBadPerformance = FindObjectOfType<SimulateBadPerformance>();
    }

    float intensitySpeed;
    // Update is called once per frame
    void Update()
    {
        if (simulateBadPerformance.enableSimulation)
        {
            for (int i = 0; i < simulateBadPerformance.performanceBadness * 1000; i++)
            {
                Light light = GetComponent<Light>();
                if (light.intensity > 1)
                    intensitySpeed = -0.00001f;
                else if (light.intensity < 0.1f)
                    intensitySpeed = 0.00001f;
                light.intensity += intensitySpeed;
            }
        }
    }
}
