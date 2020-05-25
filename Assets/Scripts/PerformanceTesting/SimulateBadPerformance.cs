using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateBadPerformance : MonoBehaviour
{
    public bool enableSimulation;
    public GameObject performanceHeavyObject;
    [Range(1,500)]
    public int performanceBadness;
}