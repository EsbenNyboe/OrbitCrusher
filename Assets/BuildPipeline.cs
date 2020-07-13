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

    private void Start()
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
        GameManager gameManager = GetComponent<GameManager>();
        gameManager.godMode = false;
        gameManager.levelLoadDeveloperMode = false;
        gameManager.autoPauseEnabled = true;
        gameManager.levelLoadUseSaveSystem = true;
        gameManager.levelToLoad = 0;
        gameManager.objectiveToLoad = 0;

        Player player = gameManager.player;
        player.unlockAllDiamond = player.unlockAllGold = player.unlockAllSilver = false;
        player.UnlockAll();
        player.SavePlayer();

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        pauseMenu.godModeButton.SetActive(godModeMenuButton);

        if (debugTxt)
        {
            FindObjectOfType<Debugger>().enabled = debugTxt;
        }

        emulatePlatformInEditor = false;
    }

    public SoundDsp soundDsp;
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

    private void SetPlatformSpecifics_Mac()
    {
        Debugger.platformSpecification = "OSX";
        soundDsp.musicLatencyCompensation = latencyCompensationMac;
    }
    private void SetPlatformSpecifics_Windows()
    {
        Debugger.platformSpecification = "Windows";
        soundDsp.musicLatencyCompensation = latencyCompensationAndroid;
    }
    private void SetPlatformSpecifics_IOS()
    {
        Debugger.platformSpecification = "iPhone";
        soundDsp.musicLatencyCompensation = latencyCompensationMac;
    }

    private void SetPlatformSpecifics_Android()
    {
        Debugger.platformSpecification = "Android";
        soundDsp.musicLatencyCompensation = latencyCompensationAndroid;
    }

    private void SetPlatformSpecifics_WebGL()
    {
        Debugger.platformSpecification = "WebGL";
        soundDsp.musicLatencyCompensation = latencyCompensationMac;
    }
}
