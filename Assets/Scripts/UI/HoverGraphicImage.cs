using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // 1
using UnityEngine.UI;

public class HoverGraphicImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIManager uiManager;
    public Image image;
    public static Color colorNormal;
    public static Color colorHover;

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (image == null)
            image = GetComponent<Image>();
        colorNormal = image.color;
        colorHover = uiManager.uiHoverColor;
    }
    private bool thisButtonActive;
    void OnEnable()
    {
        thisButtonActive = false;
        StartCoroutine(PostponeTrigger());
    }
    private IEnumerator PostponeTrigger()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        thisButtonActive = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = colorHover;
        if (thisButtonActive)
        {
            uiManager.HoverUI();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = colorNormal;
    }
    public void ImageClicked()
    {
        image.color = colorNormal;
        uiManager.ClickUI();
        PauseMenu.ClickedOnUI();
    }
}