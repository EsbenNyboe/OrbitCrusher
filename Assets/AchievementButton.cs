using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AchievementButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Animator overlayAnimation;
    public int levelNumber;

    public GameObject panel;

    public bool levelInfoDisplay;
    [HideInInspector]
    public bool inFocusNewAchievement;
    [HideInInspector]
    public bool inFocusSelect;

    private void Awake()
    {
        achievementStar = GetComponentInParent<AchievementStar>();
    }
    private void Update()
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        inFocusSelect = true;
        SetFocus();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inFocusSelect = false;
        SetFocus();
    }
    AchievementStar achievementStar;
    public void OnPointerDown(PointerEventData eventData)
    {
        Achievements.buttonDownHasHappened[achievementStar.levelNumber - 1] = true;
        //print("button down.lvlnum:" + achievementStar.levelNumber + ". " + transform.parent.gameObject.name);
        if (!levelInfoDisplay)
            overlayAnimation.SetBool("MousePressed", true);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Achievements.buttonDownHasHappened[achievementStar.levelNumber - 1] = false;
        //print("button up.lvlnum:" + achievementStar.levelNumber + ". " + transform.parent.gameObject.name);
        if (!levelInfoDisplay)
            overlayAnimation.SetBool("MousePressed", false);
        ToggleLevelInfo();


    }
    public static bool levelMenuOpen;
    public void ToggleLevelInfo()
    {
        levelInfoDisplay = !levelInfoDisplay;
        levelMenuOpen = levelInfoDisplay;
        panel.SetActive(levelInfoDisplay);
        SetFocus();
    }
    public void SetFocus()
    {
        if (levelInfoDisplay)
        {
            overlayAnimation.SetBool("InFocus", true);
        }
        else if (inFocusNewAchievement)
        {
            //print("focus new achievement");
            //print(levelNumber + "focus new achievement");
            overlayAnimation.SetBool("InFocus", true);
        }
        else if (inFocusSelect)
        {
            //print(levelNumber + "focus select");
            overlayAnimation.SetBool("InFocus", true);
        }
        else
        {
            //print(levelNumber + " not focus");
            overlayAnimation.SetBool("InFocus", false);
        }
    }
}
