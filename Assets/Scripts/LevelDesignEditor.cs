using UnityEngine;
using UnityEditor;

public class LevelDesignEditor : EditorWindow
{
    [MenuItem("Window/LevelDesignEditor")]
    public static void ShowWindow()
    {
        GetWindow<LevelDesignEditor>("LevelDesignEditor");
    }

    string godModeDisplay = "Godmode";
    bool godMode;
    string devModeDisplay = "Gamemode";
    bool devMode;
    int levelSelection;
    int objectiveSelection;

    GameManager gameManager;
    private void Awake()
    {
        
    }
    private void OnGUI()
    {
        ButtonToggle(ref godModeDisplay, ref godMode, "Godmode: ON", "Godmode: OFF", "godmode");
        ButtonToggle(ref devModeDisplay, ref devMode, "Gamemode: DEVELOPER", "Gamemode: REGULAR", "devmode");
        levelSelection = EditorGUILayout.IntField("Quickload Level:", levelSelection);
        objectiveSelection = EditorGUILayout.IntField("Quickload Objective:", objectiveSelection);
        if (GUILayout.Button("apply quickload selection"))
        {
            gameManager = FindObjectOfType<GameManager>();
            gameManager.levelQuickLoadReal = levelSelection;
            gameManager.objectiveQuickLoad = objectiveSelection;
        }
    }

    private void ButtonToggle(ref string display, ref bool windowParameter, string on, string off, string buttonType)
    {
        if (GUILayout.Button(display))
        {
            gameManager = FindObjectOfType<GameManager>();
            if (!windowParameter)
            {
                windowParameter = true;
                display = on;
            }
            else
            {
                windowParameter = false;
                display = off;
            }
            if (buttonType == "godmode")
                gameManager.godMode = windowParameter;
            else if (buttonType == "devmode")
                gameManager.levelLoadDeveloperMode = windowParameter;
        }
    }

    
}
