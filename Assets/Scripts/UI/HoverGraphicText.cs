using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // 1

public class HoverGraphicText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIManager uiManager;
    TextMeshProUGUI text;
    public static Color colorNormal;
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
        if (buttonDelay && !allButtonsActive)
        {
            allButtonsActive = true;
            thisButtonActive = false;
            StartCoroutine(PostponeTrigger());
        }
        else
            thisButtonActive = true;
    }
    IEnumerator PostponeTrigger()
    {
        yield return new WaitForSeconds(delayTime);
        thisButtonActive = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (thisButtonActive)
        {
            text.color = colorHover;
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
    }
}