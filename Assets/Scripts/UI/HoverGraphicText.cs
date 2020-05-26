using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // 1
using UnityEngine.UI;

public class HoverGraphicText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIManager uiManager;
    TextMeshProUGUI text;
    Color colorNormal;
    Color colorHover;

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        text = GetComponent<TextMeshProUGUI>();
        colorNormal = text.color;
        colorHover = uiManager.uiHoverColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = colorHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = colorNormal;
    }
    public void TextClicked()
    {
        text.color = colorNormal;
    }
}