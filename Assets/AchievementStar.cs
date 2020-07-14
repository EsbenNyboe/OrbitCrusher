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
    public Image starSilver;
    public Image starGold;
    public Image starDiamond;
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
        Silver,
        Gold,
        Diamond
    }
    private void Awake()
    {
        shinyStarAnimation.enabled = false;
        panel.SetActive(false);
    }
    private void Start()
    {
        ButtonTextPlayOtherLevel();
    }
    private void Update()
    {
        starSilver.color = SetAlpha(starSilver.color);
        starGold.color = SetAlpha(starGold.color);
        starDiamond.color = SetAlpha(starDiamond.color);
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
    public void SetLevelStatus(bool lvlUnlocked, bool lvlWon, bool fullHp, bool noDmg)
    {
        if (lvlUnlocked)
        {
            if (lvlWon)
            {
                if (fullHp || noDmg)
                {
                    if (fullHp)
                    {
                        lvlStatus = LevelStatus.Gold;
                    }
                    if (noDmg)
                    {
                        lvlStatus = LevelStatus.Diamond;
                    }
                }
                else
                {
                    lvlStatus = LevelStatus.Silver;
                }
            }
            else
            {
                lvlStatus = LevelStatus.Unlocked;
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


    public void NewAchievement(float t)
    {
        StartCoroutine(AnimationDelay(t));
    }
    IEnumerator AnimationDelay(float t)
    {
        yield return new WaitForSeconds(t);
        HighlightLevelStar();
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
    public string silverTxtTitle, goldTxtTitle, diamondTxtTitle;
    public string lockedTxt, unlockedTxt, silverTxt, goldTxt, diamondTxt;
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
                DisplayAchievement(false, false, false);
                break;
            case LevelStatus.Unlocked:
                textAchievementTitle.text = " ";
                SetTextAchievementDescription(unlockedTxt);
                txtLvlNum.color = txtUnlocked;
                DisplayAchievement(false, false, false);
                break;
            case LevelStatus.Silver:
                textAchievementTitle.text = silverTxtTitle;
                SetTextAchievementDescription(silverTxt);
                txtLvlNum.color = txtCompleted;
                DisplayAchievement(true, false, false);
                break;
            case LevelStatus.Gold:
                textAchievementTitle.text = goldTxtTitle;
                SetTextAchievementDescription(goldTxt);
                txtLvlNum.color = txtCompleted;
                DisplayAchievement(false, true, false);
                break;
            case LevelStatus.Diamond:
                textAchievementTitle.text = diamondTxtTitle;
                SetTextAchievementDescription(diamondTxt);
                txtLvlNum.color = txtCompleted;
                DisplayAchievement(false, false, true);
                break;
        }
    }

    private void DisplayAchievement(bool silver, bool gold, bool diamond)
    {
        starSilver.enabled = silver;
        starGold.enabled = gold;
        starDiamond.enabled = diamond;
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
