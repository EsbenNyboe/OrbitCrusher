using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public NodeBehavior nodeBehavior;
    public EnergySphereBehavior energySphereBehavior;
    public CometColor cometColor;
    public HealthBar healthBar;
    public MenuIcon menuIcon;
    public BackgroundColorManager backgroundColorManager;

                            // materials and static particles
    public ColorIndex nodeNotTarget, nodeTarget, nodeCompleted, nodeNotTargetParticle;
    public ColorIndex orbGhost, orbAlive, orbPickup, orbGlued;
    public ColorIndex cometInOrbit, cometOutOfOrbit, cometEnraged;
    //public ColorIndex cometBetweenOrbits, cometOutOfOrbit; // not used yet
    public ColorIndex hpLow, hpMedium, hpHigh, hpDrain, hpCharge;

                            // particle events
    public ColorIndex orbCollNodeNotTarget, orbCollNodeTarget;
    //public ColorIndex orbCollTargetCompleted; // not used yet
    //public ColorIndex cometCollNodeCompleted, cometCollNodeFailed, cometCollNodeActivateObjective; // not used yet, because not unique: same as node mat colors
    public ColorIndex cometCollNodeNotTarget;

    // skipped these, because they use complex ColorByLifetime methods (can be taken up again later if needed)
    //public ColorIndex nodeSpawn, nodeExplode; // not used yet: as for now, set colors in NodeSpawnA & NodeSpawnB
    //public ColorIndex orbSpawnGhost, orbSpawnAlive; // not used yet: as for now, set colors in OrbSpawnA & OrbSpawnB
    //public ColorIndex orbCollComet; // not used yet: as for now, set colors in OrbCollBad & OrbCollGood

                            // text
    public ColorIndex textUI, textUIHover, textUIOK, clickBox, clickBoxHover;
    //    public ColorIndex text3DFail, text3DWin, text3DTitle;

    public ColorIndex menuIconColor, menuIconColorHover;

    public ColorIndex bgColorA, bgColorB, bgColorC;


    public enum ColorIndex
    {
        // these names can be changed (only the int-index is serialized), but changing their order messes up the settings
        // size can be increased, but decreasing the size can mess up the settings
        colorGray,
        colorYellow,
        colorGreen,
        colorRed,
        colorCyan,
        colorMagenta,
        colorBlue,
        colorPurple,
        colorLightBlue,
        colorLightRed,
        colorLightGreen,
        colorWhite,
        colorLightLightRed
    }

    public GameObject[] textUIObjects;
    public GameObject[] textUIObjectsOK;
    public GameObject[] clickableBoxes;
    public GameObject[] text3D;

    public Color[] colorPalette;   

    public ColorIndex nodeLightBad, nodeLightGood, nodeLightPulse;
    public GameObject nodeLight;

    public void ApplyColors()
    {
        Color nodeLightBadC = colorPalette[(int)nodeLightBad];
        Color nodeLightGoodC = colorPalette[(int)nodeLightGood];
        Color nodeLightPulseC = colorPalette[(int)nodeLightPulse];
        nodeLight.GetComponentInChildren<NodeCometHaloAnimation>().ApplyColors(nodeLightBadC, nodeLightGoodC, nodeLightPulseC);

        Color bgA = colorPalette[(int)bgColorA];
        Color bgB = colorPalette[(int)bgColorB]; 
        Color bgC = colorPalette[(int)bgColorC];
        backgroundColorManager.ApplyColors(bgA, bgB, bgC);


        menuIcon.GetComponent<SpriteRenderer>().color = colorPalette[(int)menuIconColor];
        menuIcon.colorNormal = colorPalette[(int)menuIconColor];
        menuIcon.colorHover = colorPalette[(int)menuIconColorHover];

        //HoverGraphicText.colorNormal = colorPalette[(int)textUI];
        //HoverGraphicText.colorHover = colorPalette[(int)textUIHover];
        //for (int i = 0; i < textUIObjects.Length; i++)
        //{
        //    textUIObjects[i].GetComponent<TextMeshProUGUI>().color = colorPalette[(int)textUI];
        //}
        //for (int i = 0; i < textUIObjectsOK.Length; i++)
        //{
        //    textUIObjectsOK[i].GetComponent<TextMeshProUGUI>().color = colorPalette[(int)textUIOK];
        //}

        HoverGraphicImage.colorNormal = colorPalette[(int)clickBox];
        HoverGraphicImage.colorHover = colorPalette[(int)clickBoxHover];
        for (int i = 0; i < clickableBoxes.Length; i++)
        {
            clickableBoxes[i].GetComponent<Image>().color = colorPalette[(int)clickBox];
        }

        Color cOrbNotT = colorPalette[(int)orbCollNodeNotTarget];
        Color cOrbT = colorPalette[(int)orbCollNodeTarget];
        Color cCometNotTarget = colorPalette[(int)cometCollNodeNotTarget];
        nodeBehavior.ApplyColorsParticleEvents(cOrbNotT, cOrbT, cCometNotTarget);

        Color cHpLow = colorPalette[(int)hpLow];
        Color cHpMedium = colorPalette[(int)hpMedium];
        Color cHpHigh = colorPalette[(int)hpHigh];
        Color cHpDrain = colorPalette[(int)hpDrain];
        Color cHpCharge = colorPalette[(int)hpCharge];
        healthBar.ApplyColors(cHpLow, cHpMedium, cHpHigh, cHpDrain, cHpCharge);

        Color cCometInOrbit = colorPalette[(int)cometInOrbit];
        Color cCometOutOfOrbit = colorPalette[(int)cometOutOfOrbit];
        Color cCometEnraged = colorPalette[(int)cometEnraged];
        cometColor.ApplyColors(cCometInOrbit, cCometOutOfOrbit, cCometEnraged);

        Color cGhost = colorPalette[(int)orbGhost];
        Color cAlive = colorPalette[(int)orbAlive];
        Color cPickup = colorPalette[(int)orbPickup];
        Color cGlued = colorPalette[(int)orbGlued];
        energySphereBehavior.ApplyColors(cGhost, cAlive, cPickup, cGlued);

        Color cNodeNotTarget = colorPalette[(int)nodeNotTarget];
        Color cNodeTarget = colorPalette[(int)nodeTarget];
        Color cNodeCompleted = colorPalette[(int)nodeCompleted];
        Color cNodeNotTargetParticle = colorPalette[(int)nodeNotTargetParticle];
        nodeBehavior.ApplyColors(cNodeNotTarget, cNodeTarget, cNodeCompleted, cNodeNotTargetParticle);
    }
}
