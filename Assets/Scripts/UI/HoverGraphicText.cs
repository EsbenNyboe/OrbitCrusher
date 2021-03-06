﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // 1

public class HoverGraphicText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIManager uiManager;
    TextMeshProUGUI text;
    Color colorNormal;
    public static Color colorHover;
    public bool buttonDelay;
    public float delayTime;
    public static bool allButtonsActive;
    bool thisButtonActive;

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        text = GetComponent<TextMeshProUGUI>();
        colorNormal = text.color;
        colorHover = uiManager.uiHoverColor;
    }
    void Start()
    {
        
    }
    void OnEnable()
    {
        if (buttonDelay)
        {
            thisButtonActive = false;
            StartCoroutine(PostponeTrigger());
        }
        else
            thisButtonActive = true;
    }
    IEnumerator PostponeTrigger()
    {
        yield return new WaitForSecondsRealtime(delayTime);
        thisButtonActive = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = colorHover;
        if (thisButtonActive)
        {
            uiManager.HoverUI();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = colorNormal;
    }
    public void TextClicked()
    {
        text.color = colorNormal;
        uiManager.ClickUI();
        PauseMenu.ClickedOnUI();
    }
}