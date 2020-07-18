using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // 1
using UnityEngine.UI;

public class MenuIcon : MonoBehaviour
{
    public PauseMenu pauseMenu;
    SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Color colorNormal;
    [HideInInspector]
    public Color colorHover;
    public static bool inTransition;

    //void Awake()
    //{
    //    //pauseMenu = FindObjectOfType<PauseMenu>();
    //    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    //    colorNormal = spriteRenderer.color;
    //}
    //public void OnMouseEnter()
    //{
    //    pauseMenu.HoverUI();
    //    spriteRenderer.color = colorHover;
    //}
    //public void OnMouseExit()
    //{
    //    spriteRenderer.color = colorNormal;
    //}
    //public void OnMouseDown()
    //{
    //    ClickMenuIcon();
    //}

    public void ClickMenuIcon()
    {
        if (!inTransition && Time.timeScale != 0)
        {
            PauseMenu.ClickedOnUI();
            pauseMenu.ClickUI();
            //spriteRenderer.color = colorNormal;
            if (GameManager.betweenLevels)
            {
                pauseMenu.ToggleMenu();
            }
            else if (GameManager.inTutorial)
            {
                pauseMenu.ToggleMenu();
            }
            else
            {
                pauseMenu.EnterDialogue_ExitOrbit();
            }
        }
    }
}