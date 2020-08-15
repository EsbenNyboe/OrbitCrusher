using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    [HideInInspector]
    public MeshRenderer meshRenderer;
    Material material;

    [Tooltip("the animator sets this to 0-1")]
    [Range(0,1)]
    public float aColorComponent;

    public GameObject[] starContainers;
    public BgStarBehavior[] bgStarBehavior;

    public float starAnimFreqLvl;
    public float starAnimFreqBetweenLvls;
    float starAnimPlayTiming;
    public int[] starsRecentlyPlayed;
    public int[] starsNotRecentlyPlayed;
    public int noRepeatLength;

    public bool useBgStars;

    public GameObject bgStarsParentGo;
    public GameObject disabledBgStars;

    float[] bgStarStartTime;
    bool[] bgStarIsDisabled;
    public float disableTime;


    private void Start()
    {
        if (!useBgStars)
            Destroy(bgStarsParentGo);
        else
            Destroy(disabledBgStars);
        if (useBgStars)
        {
            bgStarsParentGo.SetActive(true);

            starIndex = Random.Range(0, bgStarBehavior.Length - 1);

            starsRecentlyPlayed = new int[noRepeatLength];
            starsNotRecentlyPlayed = new int[bgStarBehavior.Length - noRepeatLength];
            bgStarStartTime = new float[bgStarBehavior.Length];
            bgStarIsDisabled = new bool[bgStarBehavior.Length];

            for (int i = 0; i < starsRecentlyPlayed.Length; i++)
            {
                starsRecentlyPlayed[i] = i;
            }
            for (int i = 0; i < starsNotRecentlyPlayed.Length; i++)
            {
                starsNotRecentlyPlayed[i] = i + noRepeatLength;
            }
            for (int i = 0; i < bgStarBehavior.Length; i++)
            {
                starContainers[i].SetActive(false);
                //bgStarBehavior[i].gameObject.SetActive(false);
            }

            //StartCoroutine(DisableUnusedComponents());
        }
    }
    //IEnumerator DisableUnusedComponents()
    //{
    //    yield return new WaitForSeconds(bgStarFadeinTime);
    //    for (int i = 0; i < bgStarBehavior.Length; i++)
    //    {
    //        bgStarBehavior[i].starIdleAnim.enabled = false;
    //    }
    //}

    private void Update()
    {
        if (GameManager.betweenLevels)
            CheckIfStarAnimShouldPlay(starAnimFreqBetweenLvls);
        else
            CheckIfStarAnimShouldPlay(starAnimFreqLvl);
    }

    private void CheckIfStarAnimShouldPlay(float threshold)
    {
        if (Time.time > starAnimPlayTiming + threshold)
        {
            if (useBgStars)
                PlayRandomStarAnim();
            starAnimPlayTiming = Time.time;
        }
    }

    public void LoadStarContainers()
    {
        bgStarBehavior = new BgStarBehavior[starContainers.Length];
        for (int i = 0; i < starContainers.Length; i++)
        {
            bgStarBehavior[i] = starContainers[i].GetComponentInChildren<BgStarBehavior>();
        }
    }
    public void LoadMaterial(Color startColor)
    {
        material = meshRenderer.material;

        if (useBgStars)
        {
            //for (int i = 0; i < bgStarBehavior.Length; i++)
            //{
            //    Color c = startColor;
            //    bgStarBehavior[i].starIdleSprite.color = new Color(c.r, c.g, c.b, 0);
            //}
        }
    }
    public void ColorChangeNew(Color start, Color end, float transitionOutput)
    {
        Color col;
        col = Color.Lerp(start, end, transitionOutput);
        col = new Color(col.r, col.g, col.b, aColorComponent);
        material.color = col;

        if (useBgStars)
        {
            for (int i = 0; i < bgStarBehavior.Length; i++)
            {
                //bgStarBehavior[i].ApplyColorToIdle(col);
                //if (bgStarBehavior[i].starAnimAnim.enabled)
                bgStarBehavior[i].ApplyColorToAnim(col);
            }
        }
    }



    int forLoopCount;


    int starIndex;
    int indexRecentStars;
    int indexNotRecentStars;

    private void PlayRandomStarAnim()
    {
        indexNotRecentStars = Random.Range(0, starsNotRecentlyPlayed.Length);
        int starBecomingNotRecent = starsRecentlyPlayed[indexRecentStars];
        starIndex = starsRecentlyPlayed[indexRecentStars] = starsNotRecentlyPlayed[indexNotRecentStars];
        starsNotRecentlyPlayed[indexNotRecentStars] = starBecomingNotRecent;

        //starContainers[starsNotRecentlyPlayed[indexNotRecentStars]].SetActive(false);
        for (int i = 0; i < bgStarBehavior.Length; i++)
        {
            if (!bgStarIsDisabled[i] && Time.time > bgStarStartTime[i] + disableTime)
            {
                starContainers[i].SetActive(false);
                bgStarIsDisabled[i] = true;
            }
        }

        bgStarStartTime[starIndex] = Time.time;
        bgStarIsDisabled[starIndex] = false;
        starContainers[starIndex].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        starContainers[starIndex].SetActive(true);

        indexRecentStars++;
        if (indexRecentStars >= starsRecentlyPlayed.Length)
            indexRecentStars = 0;
    }
}
