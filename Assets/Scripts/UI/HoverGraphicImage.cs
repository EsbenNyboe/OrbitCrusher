using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // 1
using UnityEngine.UI;

public class HoverGraphicImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static UIManager uiManager;
    public Image image;
    public static Color colorNormal;

    public static Color colorHover;
    public static bool colorLoaded;

    void Awake()
    {
        if (image == null)
            image = GetComponent<Image>();

        if (!colorLoaded)
        {
            colorLoaded = true;
            uiManager = FindObjectOfType<UIManager>();
            colorNormal = image.color;
        }
        //colorHover = uiManager.uiHoverColor;
        //Debug.Log("col", gameObject);
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
        image.color = uiManager.uiHoverColor;
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