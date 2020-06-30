using UnityEngine;
using TMPro;
using UnityEditor;

public class FontManager : MonoBehaviour
{
    ColorManager colorManager;

    public int fontIndex; // can be directly changed in inspector
    public string fontName;
    public TMP_FontAsset[] fonts;
    public float fontSizingAmount;

    Color textboxColor, textboxTextColor, textboxOkColor;
    float textboxSize, textboxTextSize, textboxOkSize;
    public GameObject[] textbox, textboxText, textboxOk, textOtherInteractiveUI; // not including menuIcon & menu UI

    public enum TextItemCategory
    {
        TextBox,
        TextBoxText,
        TextBoxOk,
        TextInteractiveUI
    }
    public enum SizingType
    {
        Neutral, // set to to neutral, when not using the font manager: for safety
        Height, 
        Width,
        PosY,
        FontSize
    }

    [Range(0.9f, 1.1f)]
    public float sizingFactor;
    public TextItemCategory textItemCategory;
    public SizingType sizingType;

    public void SizeProcedure()
    {
        switch (textItemCategory)
        {
            case TextItemCategory.TextBox:
                ApplySizing(textbox);
                break;
            case TextItemCategory.TextBoxText:
                ApplySizing(textboxText);
                break;
            case TextItemCategory.TextBoxOk:
                ApplySizing(textboxOk);
                break;
            case TextItemCategory.TextInteractiveUI:
                ApplySizing(textOtherInteractiveUI);
                break;
        }
    }

    private void ApplySizing(GameObject[] go)
    {
        switch (sizingType)
        {
            case SizingType.Height:
                for (int i = 0; i < go.Length; i++)
                {
                    float height = go[i].GetComponent<RectTransform>().rect.height;
                    go[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * sizingFactor);
                }
                break;
            case SizingType.Width:
                for (int i = 0; i < go.Length; i++)
                {
                    float width = go[i].GetComponent<RectTransform>().rect.width;
                    go[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * sizingFactor);
                }
                break;
            case SizingType.PosY:
                for (int i = 0; i < go.Length; i++)
                {
                    RectTransform rectTransform = go[i].GetComponent<RectTransform>();
                    Quaternion rot = rectTransform.rotation;
                    Vector3 pos = rectTransform.position;
                    go[i].GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(pos.x, pos.y * sizingFactor, pos.z), rot);
                }
                break;
            case SizingType.FontSize:
                for (int i = 0; i < go.Length; i++)
                {
                    go[i].GetComponent<TextMeshProUGUI>().fontSize *= sizingFactor;
                }
                break;
        }
    }

   



    public void ApplySelectedFont() // call from color manager using the arrays of textObjects and textOkObjects
    {
        if (colorManager ==  null)
            colorManager = GetComponent<ColorManager>();
        for (int i = 0; i < colorManager.textUIObjects.Length; i++)
        {
            colorManager.textUIObjects[i].GetComponent<TextMeshProUGUI>().font = fonts[fontIndex];
        }
        for (int i = 0; i < colorManager.textUIObjectsOK.Length; i++)
        {
            colorManager.textUIObjectsOK[i].GetComponent<TextMeshProUGUI>().font = fonts[fontIndex];
        }
        fontName = fonts[fontIndex].name;
    }

    public void ChooseRandomFont()
    {
        fontIndex = Random.Range(0, fonts.Length);
        ApplySelectedFont();
    }
    public void ChooseNextFont()
    {
        fontIndex++;
        ApplySelectedFont();
    }
    public void ChoosePreviousFont()
    {
        fontIndex--;
        ApplySelectedFont();
    }
}
