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
    public float timeThreshAfterFirst;

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
        if (platformIsWebGL && Time.realtimeSinceStartup > timeThreshFirst && Time.realtimeSinceStartup - timeLastMessageShown > timeThreshAfterFirst && !displayingMessage)
        {
            messageImage.enabled = true;
            messageTmpro.enabled = true;
            toggleMessageAnim.SetBool("Display", true);
            displayingMessage = true;
        }
    }
    public void HideMessage()
    {
        if (platformIsWebGL)
        {
            timeLastMessageShown = Time.realtimeSinceStartup;
            displayingMessage = false;
            toggleMessageAnim.SetBool("Display", false);
        }
    }
}
