using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TrailerPipeline : MonoBehaviour
{
    public bool useTrailerSettings;
    public bool useGameplayVideoSettings;
    public static bool useTrailerSettingsImSerious;
    public bool unlockAllLevels;
    public float bgFadeinTime;
    public float autoLevelStartTime;

    public GameObject[] unwantedUI;

    public void ApplySettings()
    {
        print("remember to choose snapshot: game or trailer");
        for (int i = 0; i < unwantedUI.Length; i++)
        {
            unwantedUI[i].SetActive(!useTrailerSettings);
        }

        if (unlockAllLevels)
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
        useTrailerSettingsImSerious = useTrailerSettings;
        if (useTrailerSettings)
        {
            GameManager.devStartDelay = autoLevelStartTime;
            BackgroundColorManager.startFadeinTime = bgFadeinTime;
        }
    }
    private void Update()
    {
        if (useTrailerSettings || useGameplayVideoSettings)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                // win level!
                //LevelManager.levelObjectiveCurrent = LevelManager.targetNodes.Length - 1;
                //FindObjectOfType<LevelManager>().ObjectiveCompleted();

                NumberDisplayTargetNode.targetNodeIndexMemory = LevelManager.levelObjectiveCurrent;
                LevelManager.levelObjectiveCurrent = LevelManager.targetNodes.Length - 1;

                SoundManager soundManager = FindObjectOfType<SoundManager>();
                LevelManager levelManager = FindObjectOfType<LevelManager>();

                soundManager.LevelWonChooseSound();
                soundManager.FadeInMusicBetweenLevels(5f);
                soundManager.levelWon.VolumeChangeInParent(soundManager.levelWon.initialVolume, 0f, false);
                soundManager.levelWon.TriggerAudioObject();

                levelManager.ObjectiveCompleted();
            }
        }
    }
}