using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometWallHits : MonoBehaviour
{
    public ParticleSystem edgehitLeftOrRight;
    public ParticleSystem edgehitTopOrBottom;
    ParticleSystem.MainModule psmain_lr;
    ParticleSystem.MainModule psmain_tb;
    public GameObject comet;

    public Color edgehitWin, edgehitDefeat;

    private void Start()
    {
        psmain_lr = edgehitLeftOrRight.main;
        psmain_tb = edgehitTopOrBottom.main;
        SetEdgehitColor(true);
    }

    public void ToggleEdgehitParticles(bool active)
    {
        edgehitLeftOrRight.gameObject.SetActive(active);
        edgehitTopOrBottom.gameObject.SetActive(active);
    }
    public void SetEdgehitColor(bool win)
    {
        if (win || PauseMenu.exitingOrbit)
        {
            psmain_lr.startColor = edgehitWin;
            psmain_tb.startColor = edgehitWin;
        }
        else
        {
            psmain_lr.startColor = edgehitDefeat;
            psmain_tb.startColor = edgehitDefeat;
        }
    }

    public void EdgeHitTop()
    {
        edgehitTopOrBottom.transform.position = comet.transform.position;
        edgehitTopOrBottom.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        edgehitTopOrBottom.Play();
    }
    public void EdgeHitBottom()
    {
        edgehitTopOrBottom.transform.position = comet.transform.position;
        edgehitTopOrBottom.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        edgehitTopOrBottom.Play();
    }
    public void EdgeHitLeft()
    {
        edgehitLeftOrRight.transform.position = comet.transform.position;
        edgehitLeftOrRight.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        edgehitLeftOrRight.Play();
    }
    public void EdgeHitRight()
    {
        edgehitLeftOrRight.transform.position = comet.transform.position;
        edgehitLeftOrRight.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        edgehitLeftOrRight.Play();
    }
}
