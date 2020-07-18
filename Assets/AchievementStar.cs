using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AchievementStar : MonoBehaviour
{
    public Animator overlayAnimation;
    public Animator shinyStarAnimation;
    public Image starBg;
    public Image starWood, starBronze, starSilver, starGold;
    public TextMeshProUGUI txtLvlNum;
    public LevelStatus lvlStatus;
    public int levelNumber;
    public Color txtLocked, txtUnlocked, txtCompleted;

    [Range(0, 1)]
    public float overlayFadeStatus;

    public GameObject panel;

    public AchievementButton achievementButton;

    public bool fullHp;
    public bool noDmg;

    public TextMeshProUGUI textAchievementDescription;
    public TextMeshProUGUI textAchievementTitle;

    public enum LevelStatus
    {
        Locked,
        Unlocked,
        Wood,
        Bronze,
        Silver,
        Gold
    }
    public bool alphaTime;

    private void Awake()
    {
        shinyStarAnimation.enabled = false;
        panel.SetActive(false);
    }
    private void Start()
    {
        if (lvlStatus == LevelStatus.Wood || lvlStatus == LevelStatus.Bronze || lvlStatus == LevelStatus.Silver || lvlStatus == LevelStatus.Gold)
            starBg.enabled = true;
        else
            starBg.enabled = false;

        ButtonTextPlayOtherLevel();
    }
    private void Update()
    {
        if (alphaTime)
            SetAlphaStuff();
    }

    private void SetAlphaStuff()
    {
        starWood.color = SetAlpha(starWood.color);
        starBronze.color = SetAlpha(starBronze.color);
        starSilver.color = SetAlpha(starSilver.color);
        starGold.color = SetAlpha(starGold.color);
        txtLvlNum.color = SetAlpha(txtLvlNum.color);
    }

    public void SetTextAchievementDescription(string text)
    {
        textAchievementDescription.text = text;
    }





    private Color SetAlpha(Color c)
    {
        c = new Color(c.r, c.g, c.b, overlayFadeStatus);
        return c;
    }

    public void ApplyNumberToTextbox(int number)
    {
        levelNumber = number;
        txtLvlNum.text = levelNumber.ToString();
    }
    public void SetLevelStatus(bool lvlUnlocked, bool lvlWonEasy, bool lvlWon, bool fullHp, bool noDmg)
    {
        if (lvlUnlocked)
        {
            if (lvlWonEasy)
            {
                lvlStatus = LevelStatus.Wood;
            }
            else
            {
                lvlStatus = LevelStatus.Unlocked;
            }
            if (lvlWon)
            {
                if (fullHp || noDmg)
                {
                    if (fullHp)
                    {
                        lvlStatus = LevelStatus.Silver;
                    }
                    if (noDmg)
                    {
                        lvlStatus = LevelStatus.Gold;
                    }
                }
                else
                {
                    lvlStatus = LevelStatus.Bronze;
                }
            }
        }
        else
        {
            lvlStatus = LevelStatus.Locked;
        }
        ApplyLevelStatusVisualSettings();
    }
    public void HighlightLevelStar()
    {
        achievementButton.inFocusNewAchievement = true;
        achievementButton.SetFocus();
    }


    public float achievementDelay;
    public void NewAchievement()
    {
        StartCoroutine(AnimationDelay(achievementDelay));
    }
    IEnumerator AnimationDelay(float t)
    {
        HighlightLevelStar();
        yield return new WaitForSeconds(t);
        shinyStarAnimation.enabled = true;
        shinyStarAnimation.Play(0);
    }

    public void HideLevelInfo()
    {
        panel.SetActive(false);
        achievementButton.levelInfoDisplay = false;
        RemoveFocusLevelInfoDisplay();
        StartCoroutine(CustomDelay());
    }
    IEnumerator CustomDelay()
    {
        yield return new WaitForSeconds(0.4f);
        AchievementButton.levelMenuOpen = false;
    }

    public void RemoveFocus()
    {
        achievementButton.inFocusNewAchievement = false;
        achievementButton.SetFocus();
    }
    public void RemoveFocusLevelInfoDisplay()
    {
        achievementButton.inFocusSelect = false;
        achievementButton.SetFocus();
    }


    public void ButtonTextPlaySameLevel()
    {
        buttonEnterOrbit.GetComponentInChildren<TextMeshProUGUI>().text = txtSameLvl;
    }
    public void ButtonTextPlayOtherLevel()
    {
        buttonEnterOrbit.GetComponentInChildren<TextMeshProUGUI>().text = txtOtherLvl;
    }

    public string txtSameLvl;
    public string txtOtherLvl;


    public GameObject buttonEnterOrbit;
    public string woodTxtTitle, silverTxtTitle, goldTxtTitle, diamondTxtTitle;
    public string lockedTxt, unlockedTxt, woodTxt, silverTxt, goldTxt, diamondTxt;
    private void ApplyLevelStatusVisualSettings()
    {
        if (lvlStatus != LevelStatus.Locked)
            buttonEnterOrbit.SetActive(true);
        switch (lvlStatus)
        {
            case LevelStatus.Locked:
                textAchievementTitle.text = " ";
                buttonEnterOrbit.SetActive(false);
                SetTextAchievementDescription(lockedTxt);
                txtLvlNum.color = txtLocked;
                DisplayAchievement(false, false, false, false);
                break;
            case LevelStatus.Unlocked:
                textAchievementTitle.text = " ";
                SetTextAchievementDescription(unlockedTxt);
                txtLvlNum.color = txtUnlocked;
                DisplayAchievement(false, false, false, false);
                break;
            case LevelStatus.Wood:
                textAchievementTitle.text = woodTxtTitle;
                SetTextAchievementDescription(woodTxt);
                txtLvlNum.color = txtUnlocked;
                DisplayAchievement(true, false, false, false);
                break;
            case LevelStatus.Bronze:
                textAchievementTitle.text = silverTxtTitle;
                SetTextAchievementDescription(silverTxt);
                txtLvlNum.color = txtCompleted;
                DisplayAchievement(false, true, false, false);
                break;
            case LevelStatus.Silver:
                textAchievementTitle.text = goldTxtTitle;
                SetTextAchievementDescription(goldTxt);
                txtLvlNum.color = txtCompleted;
                DisplayAchievement(false, false, true, false);
                break;
            case LevelStatus.Gold:
                textAchievementTitle.text = diamondTxtTitle;
                SetTextAchievementDescription(diamondTxt);
                txtLvlNum.color = txtCompleted;
                DisplayAchievement(false, false, false, true);
                break;
        }
    }

    private void DisplayAchievement(bool wood, bool bronze, bool silver, bool gold)
    {
        if (wood || bronze || silver || gold)
            starBg.enabled = true;
        starWood.enabled = wood;
        starBronze.enabled = bronze;
        starSilver.enabled = silver;
        starGold.enabled = gold;
    }


    public void SetPanelPositionX(float xPosition)
    {
        Vector3 pos = panel.GetComponent<RectTransform>().localPosition;
        panel.GetComponent<RectTransform>().localPosition = new Vector3(xPosition, pos.y, pos.z);
    }
    //public void SetPanelPositionY(float yPosition)
    //{
    //    Vector3 pos = panel.GetComponent<RectTransform>().position;
    //    panel.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, yPosition, pos.z);
    //}
}
