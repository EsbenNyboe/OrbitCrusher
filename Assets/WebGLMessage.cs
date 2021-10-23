using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WebGLMessage : MonoBehaviour
{
    public Image messageImage;
    public TextMeshProUGUI messageTmpro;
    public Animator toggleMessageAnim;
    public static bool platformIsWebGL;
    public float timeThreshFirst;
    public float timeThreshSecond;
    bool shownOnce;
    bool shownTwice;

    private void Awake()
    {
        messageImage.gameObject.SetActive(true);
        messageTmpro.gameObject.SetActive(true);
        messageImage.enabled = false;
        messageTmpro.enabled = false;
    }
    private void Start()
    {
        if (!platformIsWebGL)
            gameObject.SetActive(false);
    }

    bool displayingMessage;
    float timeLastMessageShown;
    private void Update()
    {
        //DisplayMessage();
    }
    public void DisplayMessage()
    {
        if (platformIsWebGL && !displayingMessage)
        {
            if (!shownOnce && Time.realtimeSinceStartup > timeThreshFirst)
            {
                shownOnce = true;
                Display();
            }
            else if (!shownTwice && Time.realtimeSinceStartup > timeThreshSecond)
            {
                shownTwice = true;
                Display();
            }
        }
    }

    private void Display()
    {
        messageImage.enabled = true;
        messageTmpro.enabled = true;
        toggleMessageAnim.SetBool("Display", true);
        displayingMessage = true;
    }

    public void HideMessage()
    {
        if (platformIsWebGL)
        {
            displayingMessage = false;
            toggleMessageAnim.SetBool("Display", false);
        }
    }
}
