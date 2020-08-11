using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TrailerPipeline : MonoBehaviour
{
    public bool useTrailerSettings;

    public GameObject[] unwantedUI;

    public void ApplySettings()
    {
        print("remember to choose snapshot: game or trailer");
        for (int i = 0; i < unwantedUI.Length; i++)
        {
            unwantedUI[i].SetActive(!useTrailerSettings);
        }

        UnlockAllLevels();
    }

    private void UnlockAllLevels()
    {
        Player player = FindObjectOfType<Player>();

        for (int i = 0; i < player.lvlsWonEasy.Length; i++)
        {
            player.lvlsWon[i] = useTrailerSettings;
        }
        player.UnlockAll();
        player.SavePlayer();
    }

    private void Awake()
    {
        if (useTrailerSettings)
        {
            //UnlockAllLevels();
        }
    }
    private void Update()
    {
        if (useTrailerSettings)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                // win level!
                LevelManager.levelObjectiveCurrent = LevelManager.targetNodes.Length - 1;
                FindObjectOfType<LevelManager>().ObjectiveCompleted();
            }
        }
    }
}