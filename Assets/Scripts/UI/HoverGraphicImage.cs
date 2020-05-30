using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // 1
using UnityEngine.UI;

public class HoverGraphicImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIManager uiManager;
    Image image;
    Color colorNormal;
    Color colorHover;

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        image = GetComponent<Image>();
        colorNormal = image.color;
        colorHover = uiManager.uiHoverColor;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = colorHover;
        uiManager.HoverUI();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = colorNormal;
    }
    public void ImageClicked()
    {
        image.color = colorNormal;
        uiManager.ClickUI();
    }
}