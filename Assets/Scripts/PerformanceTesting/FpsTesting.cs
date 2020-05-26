using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsTesting : MonoBehaviour
{
    public GameObject test1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 100; i++)
            {
                Instantiate(test1);
            }
        }
    }
}
