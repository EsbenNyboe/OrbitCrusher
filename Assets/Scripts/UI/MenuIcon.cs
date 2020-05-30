using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // 1
using UnityEngine.UI;

public class MenuIcon : MonoBehaviour
{
    public PauseMenu pauseMenu;
    SpriteRenderer spriteRenderer;
    Color colorNormal;
    public Color colorHover;

    void Awake()
    {
        //pauseMenu = FindObjectOfType<PauseMenu>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorNormal = spriteRenderer.color;
    }
    public void OnMouseEnter()
    {
        pauseMenu.HoverUI();
        spriteRenderer.color = colorHover;
    }
    public void OnMouseExit()
    {
        spriteRenderer.color = colorNormal;
    }
    public void OnMouseDown()
    {
        pauseMenu.ClickUI();
        spriteRenderer.color = colorNormal;
        pauseMenu.ToggleMenu();
    }
}