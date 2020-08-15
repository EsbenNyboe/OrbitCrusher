using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObjectManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildVoices()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("AudioObject");
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i].GetComponent<AudioObject>().ReBuildVoices();
        }
    }
}
