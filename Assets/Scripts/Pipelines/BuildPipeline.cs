using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPipeline : MonoBehaviour
{
    public bool debugTxt;
    public bool godModeMenuButton;
    public enum TargetPlatform
    {
        Windows,
        Mac,
        WebGL,
        Android,
        IOS
    }
    public TargetPlatform platformEmulation;
    public bool emulatePlatformInEditor;

    [Tooltip("-:delay, 0:normal, +:predelay")]
    public float latencyCompensationMac;
    [Tooltip("-:delay, 0:normal, +:predelay")]
    public float latencyCompensationAndroid;

    [HideInInspector]
    public SoundDsp soundDsp;
    [HideInInspector]
    public AudioObjectManager audioObjectManager;
    [HideInInspector]
    public GameManager gameManager;

    public BackgroundScript backgroundScript;

    private void Awake()
    {
        if (emulatePlatformInEditor)
        {
            EmulatePlatform();
        }
        else
        {
            DetectPlatform();
        }
    }

    private void EmulatePlatform()
    {
        switch (platformEmulation)
        {
            case TargetPlatform.Windows:
                SetPlatformSpecifics_Windows();
                break;
            case TargetPlatform.Mac:
                SetPlatformSpecifics_Mac();
                break;
            case TargetPlatform.WebGL:
                SetPlatformSpecifics_WebGL();
                break;
            case TargetPlatform.Android:
                SetPlatformSpecifics_Android();
                break;
            case TargetPlatform.IOS:
                SetPlatformSpecifics_IOS();
                break;
        }
    }

    public void PrepareForBuild()
    {
        audioObjectManager.BuildVoices();

        //GameManager gameManager = GetComponent<GameManager>();
        if (gameManager != null)
        {
            gameManager.godMode = false;
            gameManager.levelLoadDeveloperMode = false;
            gameManager.autoPauseEnabled = true;
            gameManager.levelLoadUseSaveSystem = true;
            gameManager.levelToLoad = 0;
            gameManager.objectiveToLoad = 0;

            Player player = gameManager.player;
            player.unlockAllGold = player.unlockAllSilver = player.unlockAllBronze = player.unlockAllWood = false;
            player.UnlockAll();
            player.lvl = 0;
            player.SavePlayer();

            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            pauseMenu.godModeButton.SetActive(godModeMenuButton);

            Debugger debugger = FindObjectOfType<Debugger>();
            debugger.enabled = debugTxt;

            emulatePlatformInEditor = false;

            backgroundScript.LoadStarContainers();
        }
    }

    private static void ResetPlayerData(bool[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = false;
        }
    }

    public void DetectPlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
                SetPlatformSpecifics_Mac();
                break;
            case RuntimePlatform.WindowsPlayer:
                SetPlatformSpecifics_Windows();
                break;
            case RuntimePlatform.IPhonePlayer:
                SetPlatformSpecifics_IOS();
                break;
            case RuntimePlatform.Android:
                SetPlatformSpecifics_Android();
                break;
            case RuntimePlatform.WebGLPlayer:
                SetPlatformSpecifics_WebGL();
                break;
        }
    }

    public SceneManagerScript sceneManagerScript;
    private void SetPlatformSpecifics_Mac()
    {
        Debugger.platformSpecification = "OSX";
        if (soundDsp != null)
            soundDsp.musicLatencyCompensation = latencyCompensationMac;
    }
    private void SetPlatformSpecifics_Windows()
    {
        Debugger.platformSpecification = "Windows";
        if (soundDsp != null)
            soundDsp.musicLatencyCompensation = latencyCompensationAndroid;

        if (sceneManagerScript != null)
            sceneManagerScript.delayNextSceneLoad += windowsLoadingScreenDelay;
        else
            backgroundScript.useBgStars = true;
    }
    public float windowsLoadingScreenDelay;
    private void SetPlatformSpecifics_IOS()
    {
        Debugger.platformSpecification = "iPhone";
        if (soundDsp != null)
            soundDsp.musicLatencyCompensation = latencyCompensationMac;
    }

    private void SetPlatformSpecifics_Android()
    {
        Debugger.platformSpecification = "Android";
        if (soundDsp != null)
            soundDsp.musicLatencyCompensation = latencyCompensationAndroid;
    }

    private void SetPlatformSpecifics_WebGL()
    {
        SoundManager.platformIsWebGL = true;
        WebGLMessage.platformIsWebGL = true;
        Debugger.platformSpecification = "WebGL";
        if (soundDsp != null)
            soundDsp.musicLatencyCompensation = latencyCompensationMac;
    }
}
