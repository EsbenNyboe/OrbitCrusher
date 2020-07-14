using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioScript : MonoBehaviour
{
    void Start()
    {
        Camera.main.aspect = 16.0f / 10.0f;
    }
}
