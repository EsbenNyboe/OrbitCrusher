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
    public TargetPlatform targetPlatform;

    public float latencyCompensationMac;
    public float latencyCompensationAndroid;

    private void Start()
    {
        DetectPlatform();
    }

    public void PrepareForBuild()
    {
        // add "godMode menu" toggle

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


        SoundDsp soundDsp = FindObjectOfType<SoundDsp>();
        switch (targetPlatform)
        {
            case TargetPlatform.Windows:
                break;
            case TargetPlatform.Mac:
                soundDsp.musicLatencyCompensation = latencyCompensationMac;
                break;
            case TargetPlatform.WebGL:
                break;
            case TargetPlatform.Android:
                soundDsp.musicLatencyCompensation = latencyCompensationAndroid;
                break;
            case TargetPlatform.IOS:
                break;
        }
    }

    public void DetectPlatform()
    {
        string platformTxt = "";
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
                platformTxt = "OSX";
                break;
            case RuntimePlatform.WindowsPlayer:
                platformTxt = "Windows";
                break;
            case RuntimePlatform.IPhonePlayer:
                platformTxt = "iPhone";
                break;
            case RuntimePlatform.Android:
                platformTxt = "Android";
                break;
            case RuntimePlatform.WebGLPlayer:
                platformTxt = "WebGL";
                break;
        }
        Debugger.platformSpecification = platformTxt;
    }
}
