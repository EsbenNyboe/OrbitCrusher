using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEnterOrbit : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public AchievementButton achievementButton;
    public AchievementStar achievementStar;
    public HoverGraphicText hoverGraphicText;

    private void Awake()
    {
        //achievementStar = GetComponentInParent<AchievementStar>();
    }
    private void Update()
    {
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Achievements.buttonDownHasHappened[achievementStar.levelNumber - 1] = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Achievements.buttonDownHasHappened[achievementStar.levelNumber - 1] = false;
        //Debug.Log("pointerup", transform.parent.parent.gameObject);
        achievementStar.HideLevelInfo();
        hoverGraphicText.TextClicked();
        GetComponentInParent<Achievements>().EnterOrbit(achievementStar.levelNumber - 1);
    }
}
