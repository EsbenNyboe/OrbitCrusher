using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Debugger : MonoBehaviour
{
    float deltaTime = 0.0f;
    bool pauseFPS;
    public bool enableFpsPausing;

    void Update()
    {
        if (enableFpsPausing)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (pauseFPS)
                    pauseFPS = false;
                else if (!pauseFPS)
                    pauseFPS = true;
            }
        }
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    void OnGUI()
    {

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(1, 1, 1, 1);
        float msec = deltaTime * 1000.0f;
        int fps = Mathf.RoundToInt(1.0f / deltaTime);
        if (!pauseFPS)
            text = platformSpecification + " " + fps;
        GUI.Label(rect, text, style);
    }
    string text;
    public static string platformSpecification;
}
